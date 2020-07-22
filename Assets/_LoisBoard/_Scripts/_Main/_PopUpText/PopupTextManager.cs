using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupTextManager : MonoBehaviour
{
	public static PopupTextManager Instance { get; private set; }

	private Text _text;
	private CanvasGroup _canvasGroup;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		_text = GetComponentInChildren<Text>();
		_canvasGroup = GetComponent<CanvasGroup>();
		_canvasGroup.alpha = 0f;
	}

	private void OffPopupText()
	{
		Vector3 initPos = new Vector3(0f, 0f, 0.747f);
		transform.localPosition = initPos;
		_canvasGroup.alpha = 0f;
	}

	/// <summary>
    /// show popup message front of player
    /// </summary>
    /// <param name="popupMessage"> The text of popup message.</param>
    /// <param name="popupTime"> The show time of popup message</param>
	public void OnPopupText(string popupMessage, float popupTime)
	{
		_text.text = popupMessage;
		StartCoroutine(FadeInAndOut(popupTime));
		Vector3 moveToY = Vector3.up * 0.1f;
		iTween.MoveAdd(gameObject, iTween.Hash("amount", moveToY, "easeType", iTween.EaseType.easeOutExpo, "time", 2f));
		Invoke("OffPopupText", popupTime);
	}

	private IEnumerator FadeInAndOut(float popupTime)
	{ 
		float timer = 0f;
		while (timer <= popupTime)
		{
			timer += Time.deltaTime * 2;
			float lerp = Mathf.PingPong(timer, popupTime) / popupTime;
			_canvasGroup.alpha = Mathf.Lerp(0f, 1f, lerp);
			yield return null;
		}
	}
}
