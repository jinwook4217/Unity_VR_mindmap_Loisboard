using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    public bool willLoad;
    public string deviceId;
    public int mapId;
    public int themeId;
    public static bool isRun;
    public bool isTempData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        isRun = false;

        DontDestroyOnLoad(this);

    }
    void Start()
    {
        DontDestroyOnLoad(this);
        deviceId = SystemInfo.deviceUniqueIdentifier;
    }

    public void LoadMainScene()
    {
        StartCoroutine(CRLoadMainScene(1));
    }
    public void LoadLobyScene()
    {
        StartCoroutine(CRLoadMainScene(0));
    }

    IEnumerator CRLoadMainScene(int scene)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(scene);
        while (!async.isDone)
        {
            isRun = true;
            // Debug.Log("loading");
            yield return null;
        }
        isRun = false;
        if (scene == 0)
        {
            GameObject.FindObjectOfType<RedisManager>().isLoaded = false;
        }
        GvrViewer.Instance.VRModeEnabled = VRModeToggle.Instance.VRMode;
        GameObject.FindObjectOfType<RedisManager>().isNotStart = true;
        if (TutorialManager.Instance.isFirst && scene == 1) TutorialManager.Instance.DoMission(2);
        AudioManager.Instance.PlaySound(0);
    }


}
