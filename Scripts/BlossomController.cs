using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlossomController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    [SerializeField] public RectTransform ones;
    [SerializeField] public RectTransform tens;
    [SerializeField] public RectTransform hundreds;

    [SerializeField] public GameObject flowerPowerPrefab;
    [SerializeField] public GameObject flowerPowerParent;

    private bool dragging = false;
    private int bucket = 0;
    private List<GameObject> flowerPowers = new List<GameObject>();

    public int GetBucket() {
        return bucket;
    }

    public void OnPointerDown(PointerEventData eventData) {
        dragging = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        dragging = false;
    }

    public void ClearFlowerPower() {
        flowerPowers = new List<GameObject>();
    }

    public AbstractGoTween MakeFlowerPower() {
        if (bucket == 100) {
            GoTweenFlow makeFlow = new GoTweenFlow();

            for (int i = 0; i < bucket; i++) {
                float radiansPerOrb = (2 * Mathf.PI) / 20;
                float distPerOrb = 2;

                float angle = radiansPerOrb * i;

                GameObject flowerPower = Instantiate(flowerPowerPrefab, gameObject.transform);
                flowerPower.transform.localScale = Vector3.zero;
                flowerPower.transform.localPosition = Vector3.zero;
                flowerPower.transform.SetParent(flowerPowerParent.transform, true);
                flowerPowers.Add(flowerPower);

                float dist = distPerOrb * i;
                makeFlow.insert(.02f * i, Go.to(flowerPower.transform, 1, new GoTweenConfig()
                    .scale(1)
                    .position(new Vector3(dist * Mathf.Cos(angle), dist * Mathf.Sin(angle)), true)
                    .setEaseType(GoEaseType.BackOut))
                );
            }

            return makeFlow;
        } else if (bucket == 10) {
            GoTweenFlow makeFlow = new GoTweenFlow();

            for (int i = 0; i < bucket; i++) {
                float angle = (2 * Mathf.PI * i) / bucket;

                GameObject flowerPower = Instantiate(flowerPowerPrefab, gameObject.transform);
                flowerPower.transform.localScale = Vector3.zero;
                flowerPower.transform.localPosition = Vector3.zero;
                flowerPower.transform.SetParent(flowerPowerParent.transform, true);
                flowerPowers.Add(flowerPower);

                makeFlow.insert(.1f * i, Go.to(flowerPower.transform, 1, new GoTweenConfig()
                    .scale(1)
                    .position(new Vector3(100 * Mathf.Cos(angle), 100 * Mathf.Sin(angle)), true)
                    .setEaseType(GoEaseType.BackOut))
                );
            }

            return makeFlow;
        } else {
            GameObject flowerPower = Instantiate(flowerPowerPrefab, gameObject.transform);
            flowerPower.transform.localScale = Vector3.zero;
            flowerPower.transform.localPosition = Vector3.zero;
            flowerPower.transform.SetParent(flowerPowerParent.transform, true);
            flowerPowers.Add(flowerPower);

            return Go.to(flowerPower.transform, 1, new GoTweenConfig()
                .scale(1)
                .setEaseType(GoEaseType.BackOut)
            );
        }
    }

    public AbstractGoTween SendFlowerPower(Transform target) {
        GoTweenFlow sendFlow = new GoTweenFlow();

        for (int i = 0; i < flowerPowers.Count; i++) {
            GameObject flowerPower = flowerPowers[i];

            GoTweenChain chain = new GoTweenChain();

            //chain.append(Go.to(flowerPower.transform, 1, new GoTweenConfig()
            //    .position(new Vector3(0, 100), true)
            //    .setEaseType(GoEaseType.CubicOut)
            //));

            chain.append(Go.to(flowerPower.transform, 1.25f, new GoTweenConfig()
                .position(target.position)
                .sizeDelta(Vector2.zero)
                .setEaseType(GoEaseType.BackIn)
            ));

            sendFlow.insert(.02f * i, chain);
        }

        return sendFlow;
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (dragging) {
            transform.position = FlattenPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            if (BucketContainsMouse(ones)) {
                bucket = 1;
            } else if (BucketContainsMouse(tens)) {
                bucket = 10;
            } else if (BucketContainsMouse(hundreds)) {
                bucket = 100;
            } else {
                bucket = 0;
            }
        }

        Vector2 relativePoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(ones, Input.mousePosition, Camera.main, out relativePoint);
    }

    private Vector3 FlattenPoint(Vector3 point) {
        return new Vector3(point.x, point.y, 0);
    }

    private bool BucketContainsMouse(RectTransform bucket) {
        Vector2 relativePoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(bucket, Input.mousePosition, Camera.main, out relativePoint);
        return bucket.rect.Contains(relativePoint);
    }

}
