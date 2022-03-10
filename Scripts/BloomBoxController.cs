using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BloomBoxController : MonoBehaviour {

    [SerializeField] List<BlossomController> blossoms = new List<BlossomController>();

    [SerializeField] Text onesLabel;
    [SerializeField] Text tensLabel;
    [SerializeField] Text hundredsLabel;
    [SerializeField] Text sumLabel;

    public void StartMachine() {
        sumLabel.text = "0";
        blossoms.ForEach(blossom => blossom.ClearFlowerPower());

        GoTweenChain tweenChain = new GoTweenChain();
        tweenChain.autoRemoveOnComplete = true;

        List<BlossomController> activeBlossoms = blossoms.Where(blossom => blossom.GetBucket() > 0).OrderBy(blossom => blossom.GetBucket()).ToList();

        GoTweenFlow makeFlow = new GoTweenFlow();
        activeBlossoms.ForEach(blossom => makeFlow.insert(0, blossom.MakeFlowerPower()));
        tweenChain.append(makeFlow);

        GoTweenFlow moveFlow = new GoTweenFlow();
        for (int i = 0; i < activeBlossoms.Count; i++) {
            BlossomController blossom = activeBlossoms[i];
            AbstractGoTween sendTween = blossom.SendFlowerPower(sumLabel.gameObject.transform);
            moveFlow.insert(i * 0.5f, sendTween);
            sendTween.setOnCompleteHandler(agt => sumLabel.text = (int.Parse(sumLabel.text) + blossom.GetBucket()).ToString());
        }
        tweenChain.append(moveFlow);

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
    }
}
