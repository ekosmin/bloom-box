using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BloomBoxController : MonoBehaviour {

    [SerializeField] List<BlossomController> blossoms = new List<BlossomController>();

    [SerializeField] Text onesLabel;
    [SerializeField] Text tensLabel;
    [SerializeField] Text hundredsLabel;
    [SerializeField] Text sumLabel;

    [SerializeField] CanvasGroup previewsCanvasGroup;

    [SerializeField] Transform onesPreview;
    [SerializeField] Transform tensPreview;
    [SerializeField] Transform hundredsPreview;

    [SerializeField] GameObject flowerPowerPrefab;
    [SerializeField] GameObject tensPrefab;
    [SerializeField] GameObject hundredsPrefab;

    public void StartMachine() {
        sumLabel.text = "0";
        blossoms.ForEach(blossom => blossom.ClearFlowerPower());

        GoTweenChain tweenChain = new GoTweenChain();
        tweenChain.autoRemoveOnComplete = true;

        List<BlossomController> activeBlossoms = blossoms.Where(blossom => blossom.GetBucket() > 0).OrderBy(blossom => blossom.GetBucket()).ToList();

        tweenChain.append(Go.to(previewsCanvasGroup, 1, new GoTweenConfig().floatProp("alpha", 0)));

        GoTweenFlow makeFlow = new GoTweenFlow();
        activeBlossoms.ForEach(blossom => makeFlow.insert(0, blossom.MakeFlowerPower()));
        tweenChain.append(makeFlow);

        GoTweenFlow moveFlow = new GoTweenFlow();
        for (int i = 0; i < activeBlossoms.Count; i++) {
            BlossomController blossom = activeBlossoms[i];
            AbstractGoTween sendTween = blossom.SendFlowerPower(sumLabel.gameObject.transform);
            moveFlow.insert(i * 0.5f, sendTween);
            sendTween.setOnCompleteHandler(agt => {
                sumLabel.gameObject.SetActive(true);
                sumLabel.text = (int.Parse(sumLabel.text) + blossom.GetBucket()).ToString();
            });
        }
        tweenChain.append(moveFlow);

        tweenChain.appendDelay(2);

        tweenChain.setOnCompleteHandler(agt => {
            previewsCanvasGroup.alpha = 1;
            sumLabel.gameObject.SetActive(false);
        });

        tweenChain.play();
    }

    private void Start() {
        sumLabel.text = "0";
    }

    private void Update() {
        int onesSum = blossoms.Count(blossom => blossom.GetBucket() == 1);
        int tensSum = blossoms.Count(blossom => blossom.GetBucket() == 10) * 10;
        int hundredsSum = blossoms.Count(blossom => blossom.GetBucket() == 100) * 100;

        onesLabel.text = onesSum.ToString();
        tensLabel.text = tensSum.ToString();
        hundredsLabel.text = hundredsSum.ToString();

        bool doRescale = false;
        doRescale |= UpdatePreview(onesPreview, flowerPowerPrefab, onesSum);
        doRescale |= UpdatePreview(tensPreview, tensPrefab, tensSum / 10);
        doRescale |= UpdatePreview(hundredsPreview, hundredsPrefab, hundredsSum / 100);

        if (doRescale) {
            if (hundredsSum > 0) {
                onesPreview.gameObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(25, 25);
                tensPreview.gameObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(30, 45);
            } else if (tensSum > 0) {
                onesPreview.gameObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(50, 50);
                tensPreview.gameObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(85, 120);
            } else {
                onesPreview.gameObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(100, 100);
            }

        }
    }

    private bool UpdatePreview(Transform target, GameObject prefab, int count) {
        if (count != target.transform.childCount) {
            foreach (Transform child in target) { Destroy(child.gameObject); }
            for (int i = 0; i < count; i++) {
                Instantiate(prefab, target);
            }
            return true;
        }
        return false;
    }

}
