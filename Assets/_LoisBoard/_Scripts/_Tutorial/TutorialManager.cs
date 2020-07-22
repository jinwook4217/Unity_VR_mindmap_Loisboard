using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    public GameObject missionCanvas;


    public GameObject _currentMissionCanvas;
    public GameObject warning;
    public int currentMissionNum = 0;
    public GameObject tutorialCanvasPositionSet;
    public GameObject tutorialImageSet;

    public RectTransform[] tutorialCanvasPositions;
    private Image[] _tutorialImages;
    public bool isFirst;

    private Transform _newNodePos;

    void Awake()
    {
        if (Instance == null) Instance = this;
        DontDestroyOnLoad(this);

        tutorialCanvasPositions = tutorialCanvasPositionSet.GetComponentsInChildren<RectTransform>();
        _tutorialImages = tutorialImageSet.GetComponentsInChildren<Image>();

        //test
        if (PlayerPrefs.GetInt("isFirst", 1) == 1)
        {
            isFirst = true;
            PlayerPrefs.SetInt("isFirst", 0);
            PlayerPrefs.Save();
        }
        else
        {
            isFirst = false;
            //test용 
            //PlayerPrefs.SetInt("isFirst", 1);
            //PlayerPrefs.Save();
            //
        }

        //isFirst = true;

    }
    void Start()
    {
        if (isFirst) DoMission(0);
    }

    public void StartTutorial()
    {
        if (currentMissionNum != 14)
        {
            isFirst = true;
            currentMissionNum = 0;
            DoMission(0);
        }
    }

    public void DoMission(int missionNum)
    {
        //if user has complete current tutorial mission, load next tutorial canvas
        if (currentMissionNum == missionNum) StartCoroutine(CRDoMission(missionNum));
        if(currentMissionNum == 1 && missionNum == 2) 
        {
            currentMissionNum ++;
             StartCoroutine(CRDoMission(missionNum));
        }
    }
    public IEnumerator NetworkWarning()
    {
        //if(warning != null) Destroy(warning.gameObject);
        //yield return new WaitForSeconds(1f);
        if(warning == null)
        {
            warning = Instantiate(missionCanvas, Vector3.zero, Quaternion.identity);
            GameObject temp = new GameObject("temp");
            temp.transform.position = Camera.main.transform.position;
            temp.transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.y * 120f, 0f);

            warning.transform.parent = temp.transform;
            warning.transform.localPosition = Vector3.forward * 10f;
            //_curruntMissionCanvas.transform.localRotation = tutorialCanvasPositions[curruntMissionNum].rotation;
            warning.transform.parent = Camera.main.transform.parent;
            warning.transform.LookAt(Camera.main.transform);
            warning.transform.Rotate(0f, 180f, 0f, Space.Self);
            warning.GetComponentInChildren<Image>().sprite = _tutorialImages[0].sprite;
            AudioManager.Instance.PlaySound(12);
            yield return new WaitForSeconds(2f);
            for (int i = 0; i < 100; i++)
            {
                warning.GetComponentInChildren<Image>().color = new Color(1f, 1f, 1f, 1f - 0.01f * i);
                yield return null;
            }

            Destroy(warning.gameObject);
        }
    }
    IEnumerator CRDoMission(int missionNum)
    {
        //after finish load scene
        while (SceneLoader.isRun)
        {
            yield return null;
        }

        //delete previous canvas
        if (_currentMissionCanvas != null) Destroy(_currentMissionCanvas);

        //for recenter tutorial// fake camra rotation
        // if(currentMissionNum == 1) Camera.main.transform.parent.transform.rotation = Quaternion.Euler(0f, 60f, 0f);
        // else if (currentMissionNum == 2) Camera.main.transform.parent.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        currentMissionNum++;

        if (currentMissionNum < tutorialCanvasPositions.Length)
        {
            //get next tutorial position
            _currentMissionCanvas = Instantiate(missionCanvas, tutorialCanvasPositions[currentMissionNum].position, tutorialCanvasPositions[currentMissionNum].rotation);

            if (currentMissionNum == 1 || (currentMissionNum > 9 && currentMissionNum < 14))//relative
            {

                GameObject temp = new GameObject("temp");
                temp.transform.position = Camera.main.transform.position;
                temp.transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.y * 120f, 0f);

                _currentMissionCanvas.transform.parent = temp.transform;
                _currentMissionCanvas.transform.localPosition = tutorialCanvasPositions[currentMissionNum].position;
                //_curruntMissionCanvas.transform.localRotation = tutorialCanvasPositions[curruntMissionNum].rotation;
                _currentMissionCanvas.transform.parent = Camera.main.transform.parent;


            }
            else if (currentMissionNum == 8 || currentMissionNum == 9)//new node
            {
                //Debug.Log("th");
                //Debug.Log(_newNodePos);
                if (_newNodePos == null)
                {
                    _newNodePos = new GameObject("Temp").transform;
                    _newNodePos.position = Camera.main.transform.position + Camera.main.transform.forward * 10f;

                }
                _currentMissionCanvas.transform.localPosition = tutorialCanvasPositions[currentMissionNum].position + new Vector3(_newNodePos.position.x * 0.5f, _newNodePos.position.y * 0.5f, (-13f + _newNodePos.position.z) * 0.5f);
                //_curruntMissionCanvas.transform.localRotation =   _newNodePos.rotation;
            }
            //get next tutorial image
            _currentMissionCanvas.transform.LookAt(Camera.main.transform);
            _currentMissionCanvas.transform.Rotate(0f, 180f, 0f, Space.Self);
            _currentMissionCanvas.GetComponentInChildren<Image>().sprite = _tutorialImages[currentMissionNum].sprite;
            _currentMissionCanvas.GetComponentInChildren<Image>().SetNativeSize();
            if (currentMissionNum == 14)
            {
                StartCoroutine(EndTutorial());
            }

        }
        else
        {

        }
    }

    public IEnumerator NextTutorial(int num, float time)
    {
        yield return new WaitForSeconds(time);
        DoMission(num);
    }

    IEnumerator EndTutorial()
    {

        yield return new WaitForSeconds(5f);
        for (int i = 0; i < 100; i++)
        {
            if (_currentMissionCanvas == null)
            {
                ArrowProgress.Instance.directionCanvas.transform.position = Vector3.zero;
                yield break;
            }
            _currentMissionCanvas.GetComponentInChildren<Image>().color = new Color(1f, 1f, 1f, 1f - 0.01f * i);
            yield return null;
        }
        PlayerPrefs.SetInt("isFirst", 0);
        PlayerPrefs.Save();
        isFirst = false;
        currentMissionNum = 0;
        ArrowProgress.Instance.directionCanvas.transform.position = Vector3.zero;
        Destroy(_currentMissionCanvas.gameObject);

    }
    public void GetNewNodePosition(Transform pos)
    {
        _newNodePos = pos;
    }
}
