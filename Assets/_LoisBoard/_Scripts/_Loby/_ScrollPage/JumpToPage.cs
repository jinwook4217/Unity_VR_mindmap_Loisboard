using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JumpToPage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IGvrPointerHoverHandler
{
    [Tooltip("Destination page.")]
    public Transform page;

    [Tooltip("Time that should gaze in time to get in space.")]
    public float gazeTimeToGetIn = 2f;

    private bool _isGazeAt = false;
    private float _timer = 0f;
    private bool _isActiveDeleteButton = false;
    private bool _shouldGazeAgain = false;

    public int themeIndex;
    public int mapId;
    public SavedPagesScroll SavedOwnPageScroll
    {
        get
        {
            if (cashedPagedScroll != null)
            {
                return cashedPagedScroll;
            }

            if (page != null)
            {
                cashedPagedScroll = page.GetComponentInParent<SavedPagesScroll>();
            }

            return cashedPagedScroll;
        }
    }
    private SavedPagesScroll cashedPagedScroll;

    public bool CanClick
    {
        get
        {
            if (SavedOwnPageScroll != null)
            {
                bool isActivePage = SavedOwnPageScroll.ActivePage == page;
                return !isActivePage && !SavedOwnPageScroll.IsSnapping;
            }

            return false;
        }
    }

    public bool IsActivePage
    {
        get
        {
            if (SavedOwnPageScroll != null)
            {
                bool isActivePage = SavedOwnPageScroll.ActivePage == page;
                return isActivePage;
            }

            return false;
        }
    }

    private void Awake()
    {
        if (page == null)
        {
            page = transform.parent;
        }
    }

    public void OnPointerEnter(PointerEventData ped)
    {
        _isGazeAt = true;
        _timer = 0f;
        if (CanClick)
        {
            SavedOwnPageScroll.SnapToVisiblePage(page);
        }
    }
    public void OnPointerExit(PointerEventData ped)
    {
        _isGazeAt = false;
        _timer = 0f;
    }
    public void OnGvrPointerHover(PointerEventData ped)
    {
        if (_isGazeAt && !_shouldGazeAgain)
        {
            _timer += Time.deltaTime;

            if (_timer > gazeTimeToGetIn)
            {
                if (!CanClick)
                {
                    // ThemeManager.MainTheme = ThemeManager.Instance.GetThemeByInherenceIndex(themeIndex);

                    // GameObject.FindObjectOfType<SceneLoader>().mapId = mapId;
                    // GameObject.FindObjectOfType<SceneLoader>().willLoad = true;
                    // GameObject.FindObjectOfType<NodeManager>().enabled = true;
                    // // GameObject.Find("SceneManager").GetComponent<SceneLoader>().mapId=mapId;
                    // // GameObject.Find("SceneManager").GetComponent<SceneLoader>().willLoad=true;
                    // // GameObject.Find("ManagerContainer").GetComponent<RedisManager>().enabled=true;
                    // SceneLoader.Instance.LoadMainScene();
                    // if (TutorialManager.Instance.isFirst) TutorialManager.Instance.DoMission(15);

                    StartCoroutine(LoadAfterEnterAnim());
                }

                _timer = 0f;
                _isGazeAt = false;
                

            }
        }
    }

    private IEnumerator LoadAfterEnterAnim()
    {
        _shouldGazeAgain = true;
        GameObject player = Camera.main.transform.gameObject;
        Camera.main.transform.GetChild(0).gameObject.SetActive(false);
        iTween.MoveTo(player, iTween.Hash("path", iTweenPath.GetPath("EnterSpace")
                        , "time", 5f
                        , "easetype", iTween.EaseType.easeInOutCubic
                        ));

        yield return new WaitForSeconds(5f);

        ThemeManager.MainTheme = ThemeManager.Instance.GetThemeByInherenceIndex(themeIndex);

        GameObject.FindObjectOfType<SceneLoader>().mapId = mapId;
        GameObject.FindObjectOfType<SceneLoader>().willLoad = true;
        GameObject.FindObjectOfType<NodeManager>().enabled = true;
        // GameObject.Find("SceneManager").GetComponent<SceneLoader>().mapId=mapId;
        // GameObject.Find("SceneManager").GetComponent<SceneLoader>().willLoad=true;
        // GameObject.Find("ManagerContainer").GetComponent<RedisManager>().enabled=true;
        SceneLoader.Instance.LoadMainScene();
        if (TutorialManager.Instance.isFirst) TutorialManager.Instance.DoMission(15);
        _shouldGazeAgain = false;
    }

    private void Update()
    {
        if (SavedOwnPageScroll == null)
        {
            return;
        }

        if (!SavedOwnPageScroll.IsDeleteMode && _isActiveDeleteButton)
        {
            DeletePage deletePage = page.GetComponentInChildren<DeletePage>();
            deletePage.OffActiveDeleteButton(page.gameObject);
            _isActiveDeleteButton = false;
        }
        if (SavedOwnPageScroll.IsDeleteMode && !IsActivePage && _isActiveDeleteButton)
        {
            DeletePage deletePage = page.GetComponentInChildren<DeletePage>();
            deletePage.OffActiveDeleteButton(page.gameObject);
            _isActiveDeleteButton = false;
        }
        if (SavedOwnPageScroll.IsDeleteMode && IsActivePage && !_isActiveDeleteButton)
        {
            DeletePage deletePage = page.GetComponentInChildren<DeletePage>();
            deletePage.OnActiveDeleteButton(page.gameObject);
            _isActiveDeleteButton = true;
        }
    }
}
