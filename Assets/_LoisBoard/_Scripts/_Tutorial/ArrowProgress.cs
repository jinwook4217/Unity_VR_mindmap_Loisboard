using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowProgress : MonoBehaviour
{
    public static ArrowProgress Instance;

    public GameObject directionCanvas;
    private Image _arrow;
    private CanvasGroup _arrowCG;
    public Vector3 arrowPosition;
    //public GameObject arrow;

    public float directionSofterValue = 0.5f;
    public float clockingValue = 0.2f;
    public float clockingDistance = 8f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        _arrow = directionCanvas.GetComponentInChildren<Image>();
        _arrowCG = directionCanvas.GetComponent<CanvasGroup>();
    }
    void Update()
    {
        if (TutorialManager.Instance._currentMissionCanvas != null)
        {
            //2d
            arrowPosition = Camera.main.transform.position + Camera.main.transform.forward * 10f;
            this.transform.position = arrowPosition;
            Vector3 relativePos = TutorialManager.Instance._currentMissionCanvas.transform.position - arrowPosition;

            //new Vector3(arrowPosition.transform.position.x * 2f, arrowPosition.transform.position.y * 2f, arrowPosition.transform.position.y * 2f);
            //Transform target = TutorialManager.Instance.tutorialCanvasPositions[TutorialManager.Instance.curruntMissionNum];
            Vector3 target = Camera.main.transform.InverseTransformPoint(TutorialManager.Instance._currentMissionCanvas.transform.position);
            float AngleRad = Mathf.Atan2(target.y * directionSofterValue, target.x);
            float AngleDeg = (180 / Mathf.PI) * AngleRad;
            this.transform.localRotation = Camera.main.transform.rotation; //Quaternion.Euler(Camera.main.transform.rotation.x * 180f, Camera.main.transform.rotation.y * 180f, Camera.main.transform.rotation.z * 180f );
            _arrow.transform.localRotation = Quaternion.Euler(0, 0, AngleDeg);





            //Debug.Log(relativePos.magnitude);
            _arrow.transform.localPosition = new Vector2(target.x, target.y * directionSofterValue).normalized * relativePos.magnitude * 30f;
            // _arrow.GetComponent<Image>().color = new Color(1f, 1f, 1f, (relativePos.magnitude - clockingDistance) * clockingValue);
            _arrowCG.alpha = (relativePos.magnitude-clockingDistance) * clockingValue;

        }
        else _arrowCG.alpha = 0f;
        // else _arrow.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);

    }
}
