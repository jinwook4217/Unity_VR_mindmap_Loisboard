using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CreateNewSpace : MonoBehaviour, IPointerEnterHandler, IGvrPointerHoverHandler, IPointerExitHandler
{
    public float gazeTime = 1f;

    private float _timer = 0f;
    private bool _shouldGazeAtAgain = false;

    private void Start()
    {
        // _themeContainer = FindObjectOfType<ThemesContainer>().gameObject;
        // _themeContainer.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData ped)
    {
        _timer = 0f;

        //  세부 사항 변경
        // iTween.ScaleTo(createSphere, iTween.Hash("scale", Vector3.one * 0.8f, "easetype", iTween.EaseType.easeOutBounce, "time", gazeTime));

        iTween.ScaleTo(gameObject, iTween.Hash(
            "scale", Vector3.one * 1.2f,
            "easetype", iTween.EaseType.easeOutElastic,
            "looptype", iTween.LoopType.pingPong,
            "time", 0.5f
        ));

        AudioManager.Instance.PlaySound(2);
    }

    public void OnPointerExit(PointerEventData ped)
    {
        iTween.ScaleTo(gameObject, Vector3.one, 1f);
        //  세부 사항 변경 필요
        // iTween.ScaleTo(createSphere, iTween.Hash("scale", Vector3.zero, "easetype", iTween.EaseType.easeInOutBack, "time", gazeTime));
    }

    public void OnGvrPointerHover(PointerEventData ped)
    {
        if (_shouldGazeAtAgain)
        {
            return;
        }

        _timer += Time.deltaTime;

        if (_timer > gazeTime)
        {
            // MoveToSelectThemePosition();
            _timer = 0f;
            _shouldGazeAtAgain = true;

            GameObject.FindObjectOfType<ThemeManager>().OnGetRandomThemeByDayTime();
            GameObject.FindObjectOfType<RedisManager>().enabled=true;
            GameObject.FindObjectOfType<NodeManager>().enabled=true;
            
            SceneLoader.Instance.LoadMainScene();
        }
    }

    // private void MoveToSelectThemePosition()
    // {
    //     GameObject player = Camera.main.transform.parent.gameObject;
    //     _themeContainer.SetActive(true);
    //     iTween.MoveTo(player, iTween.Hash(
    //         "path", iTweenPath.GetPath("AfterCreateComplete"),
    //         "easetype", iTween.EaseType.easeInOutSine,
    //         "time", 5f
    //     ));
    // }
}
