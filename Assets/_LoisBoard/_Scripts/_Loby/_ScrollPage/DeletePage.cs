using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeletePage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IGvrPointerHoverHandler
{
	public float gazeTime = 1f;
	private float _timer = 0f;


	public RectTransform OwnRectTransform
	{
		get
		{
			if (cashedRect != null)
			{
				return cashedRect;
			}

			cashedRect = GetComponent<RectTransform>();
			return cashedRect;
		}
	}
	private RectTransform cashedRect;

	public void OnActiveDeleteButton(GameObject page)
	{
		OwnRectTransform.localScale = Vector3.one * 0.003f;
		page.transform.Rotate(Vector3.forward * -2f);
		iTween.RotateTo(page, iTween.Hash(
			"rotation", Vector3.forward * 2f,
			"easetype", iTween.EaseType.linear,
			"looptype", iTween.LoopType.pingPong,
			"time", 0.1f
		));
	}

	public void OffActiveDeleteButton(GameObject page)
	{
		OwnRectTransform.localScale = Vector3.zero;
		iTween.RotateTo(page, Vector3.forward * -2f, 0.1f);
	}

	public void OnPointerEnter(PointerEventData ped)
	{
		_timer = 0f;
	}

	public void OnPointerExit(PointerEventData ped)
	{
		_timer = 0f;
	}

	public void OnGvrPointerHover(PointerEventData ped)
	{
		_timer += Time.deltaTime;

		if (_timer > gazeTime)
		{
			SavedPagesScroll tempSavedPagesScroll = GameObject.FindObjectOfType<SavedPagesScroll>();
			// if (tempSavedPagesScroll.PageCount == -1 || tempSavedPagesScroll.PageCount == 0)
			// {
			// 	return;
			// }
			StartCoroutine(DeleteAnim(tempSavedPagesScroll));
			RedisManager.Instance.deleteMap(transform.parent.GetComponentInChildren<JumpToPage>().mapId);
			_timer = 0f;
		}
	}

	private IEnumerator DeleteAnim(SavedPagesScroll tempSavedPagesScroll)
	{
		int ActivePageIndex = tempSavedPagesScroll.ActivePageIndex;

		iTween.ScaleTo(tempSavedPagesScroll.ActivePage.gameObject, iTween.Hash(
            "scale", Vector3.zero,
            "easetype", iTween.EaseType.easeInBack,
            "time", 0.5f
        ));
		yield return new WaitForSeconds(1f);

		tempSavedPagesScroll.RemovePage(tempSavedPagesScroll.ActivePage);
		RedisManager.Instance._savedSpaces.RemoveAt(ActivePageIndex);

		if (tempSavedPagesScroll.PageCount == -1 || tempSavedPagesScroll.PageCount == 0)
		{
			iTween.ScaleTo(RedisManager.Instance.EnterArrow, Vector3.zero, 0.5f);
			Transform[] children = tempSavedPagesScroll.GetComponentsInChildren<Transform>();
			for (int i = 1; i < children.Length; ++i)
			{
				Destroy(children[i].gameObject);
			}
			yield break;
		}
		
		tempSavedPagesScroll.Reset();
	}
}
