using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnsavedSpaceEnter : MonoBehaviour, IPointerEnterHandler, IGvrPointerHoverHandler, IPointerExitHandler
{
    public float gazeTime = 2f;

    private float _timer = 0f;
    private bool _shouldGazeAtAgain = false;

	private void Start()
	{
		if (PlayerPrefs.GetInt("isExistTempData") == 1)
		{
			transform.parent.localScale = Vector3.one * 0.0007f;
		}
		else
		{
			transform.parent.localScale = Vector3.zero;
		}
	}

    public void OnPointerEnter(PointerEventData ped)
    {
        _timer = 0f;

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
            _timer = 0f;
            _shouldGazeAtAgain = true;

            GameObject.FindObjectOfType<ThemeManager>().OnGetRandomThemeByDayTime();
            GameObject.FindObjectOfType<RedisManager>().enabled=true;
            GameObject.FindObjectOfType<NodeManager>().enabled=true;
			GameObject.FindObjectOfType<SceneLoader>().isTempData = true;
            
            SceneLoader.Instance.LoadMainScene();
        }
    }
}
