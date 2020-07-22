using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RecenterMiniMap : MonoBehaviour, IGvrPointerHoverHandler, IPointerEnterHandler, IPointerExitHandler
{
	// public GameObject preMotherNode;
	// public GameObject nextMotherNode;
	public Transform preMotherTrans;
	public Transform nextMotherTrans;
	public GameObject recenterCanvas;
	public GameObject railCanvas;
	public float gazeTime = 1f;
	public float dampTime = 0.2f;

	private Transform _nextMotherNodeAxis;
	// private Transform _recenterCanvas;
	private Transform _camTrans;

	// private Renderer _preMotherNodeRen;
	// private Renderer _nextMotherNodeRen;

	private CanvasGroup _preMotherCanvasGroup;
	private CanvasGroup _nextMotherCanvasGroup;

	private float _desiredAngle;
    private float _moveVelocity;

	private float _timer;
	private bool _shouldGazeAgain;

	private void Start()
	{
		_camTrans = Camera.main.transform;
		_nextMotherNodeAxis = transform.GetChild(1);
		// _recenterCanvas = _nextMotherNodeAxis.GetChild(1);

		// _preMotherNodeRen = preMotherNode.GetComponent<Renderer>();
		// _nextMotherNodeRen = nextMotherNode.GetComponent<Renderer>();

		_preMotherCanvasGroup = preMotherTrans.GetComponent<CanvasGroup>();
		_nextMotherCanvasGroup = nextMotherTrans.GetComponent<CanvasGroup>();

		_timer = 0f;
		_shouldGazeAgain = false;
		ResetPinTransparent();
	}

	private void Update()
	{
		NextMotherNodeAxisRotate();
		CanvasLookAtPlayer();
	}

	private void NextMotherNodeAxisRotate()
	{
		Vector3 camForward = _camTrans.forward;
		camForward.y = 0f;
		camForward.Normalize();

		float angle = Vector3.Angle(Vector3.forward, camForward);
        angle = Vector3.Dot(Vector3.right, camForward) > 0f ? angle : - angle;
		_desiredAngle = angle;

		float yAngle = Mathf.SmoothDampAngle(_nextMotherNodeAxis.eulerAngles.y, _desiredAngle, ref _moveVelocity, dampTime);
		_nextMotherNodeAxis.rotation = Quaternion.Euler(0f, yAngle, 0f);
	}

	private void CanvasLookAtPlayer()
	{
		Transform[] lookAtplayerTransforms = {recenterCanvas.transform, railCanvas.transform, nextMotherTrans, preMotherTrans};

		for (int i = 0; i < lookAtplayerTransforms.Length; i++) 
		{
			lookAtplayerTransforms[i].LookAt(_camTrans);
			lookAtplayerTransforms[i].forward = - lookAtplayerTransforms[i].forward;
		}
	}

	public void OnPointerEnter(PointerEventData ped)
	{
		_timer = 0f;
		AudioManager.Instance.PlaySound(2);
		iTween.ScaleTo(recenterCanvas, Vector3.one * 0.003f * 1.2f, 2f);
	}
	public void OnPointerExit(PointerEventData ped)
	{
		_timer = 0f;
		_shouldGazeAgain = false;
		ResetPinTransparent();
		iTween.ScaleTo(recenterCanvas, Vector3.one * 0.003f, 2f);
	}

	private void ResetPinTransparent()
	{
		// _preMotherNodeRen.material.color = new Color(1f, 1f, 1f, 1f);
		// _nextMotherNodeRen.material.color = new Color(1f, 1f, 1f, 0f);

		_preMotherCanvasGroup.alpha = 1f;
		_nextMotherCanvasGroup.alpha = 0f;
	}

	public void OnGvrPointerHover(PointerEventData ped)
	{
		if (_shouldGazeAgain)
		{
			return;
		}

		_timer += Time.deltaTime;
		FadeInAndOutMinimapPin();

		if (_timer > gazeTime)
		{
			_timer = 0f;
			_shouldGazeAgain = true;
			Recenter();
			if(TutorialManager.Instance.isFirst) TutorialManager.Instance.DoMission(1);
			AudioManager.Instance.PlaySound(10);
		}
	}

	private void FadeInAndOutMinimapPin()
	{
		float temp1 = easeOutCubic(0f, 1f, _timer/gazeTime);
		float temp2 = easeInQuad(0f, 1f, _timer/gazeTime);
		// _preMotherNodeRen.material.color = new Color(1f, 1f, 1f, 1f - temp1);
		// _nextMotherNodeRen.material.color = new Color(1f, 1f, 1f, temp2);

		_preMotherCanvasGroup.alpha = 1f - temp1;
		_nextMotherCanvasGroup.alpha = temp2;
	}

	private float easeInQuad(float start, float end, float value)
	{
		end -= start;
		return end * value * value + start;
	}

	private float easeOutCubic(float start, float end, float value){
		value--;
		end -= start;
		return end * (value * value * value + 1) + start;
	}

	private void Recenter()
	{
		GvrViewer viewer = FindObjectOfType<GvrViewer>();
        if (viewer == null)
        {
            return;
        }
        
        GameObject player = Camera.main.transform.parent.gameObject;
        iTween.MoveTo(player, iTween.Hash("position", Vector3.zero, "easetype", iTween.EaseType.easeInCubic, "time", 3f));

        viewer.Recenter();
	}
}
