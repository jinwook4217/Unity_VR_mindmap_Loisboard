using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
     * 이 스크립트는 노드의 판낼캔버스에 붙어있는 스크립트이다.
     * 판낼캔버스는 노드의 자식이다.
     * 노드의 위쪽에 위치하고, 바라봤을 때 모든 텍스트를 보여준다.
*/
namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class NodePannel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IGvrPointerHoverHandler
    {
        //  Text의 Height와 Pannel Canvas의 Height가 Text의 크기에 맞춰 동시에 늘어난다.
        public float basicPannelWidth = 3200.0f;
        public float basicPannelHeight = 750.0f;
        public float basicTextWidth = 600.0f;
        public float basicTextHeight = 120.0f;
        public float textScale = 5.0f;
        public float showSpeed = 3.0f;
        public float gazeTime = 0.0f;

        [HideInInspector]
        public bool isShowAllMode;
        private float _timer;

        //  pannel canvas
        private RectTransform _rectTransform;
        private GameObject _gameObject;
        private Vector3 _initLocalScale;
        private Transform _cameraTransform;
        //  Text
        private Text _text;
        private RectTransform _textRectTrans;

        //  현재 실행되고 있는 ToShowAllText 코르틴
        private Coroutine _currentToShowAllTextCoroutine;

        private void Awake()
        {
            //  node pannel canvas
            _gameObject = gameObject;
            _cameraTransform = Camera.main.transform;
            _rectTransform = (RectTransform)transform;
            _initLocalScale = _rectTransform.localScale;

            //  text
            _text = GetComponentInChildren<Text>();
            _textRectTrans = _text.GetComponent<RectTransform>();

            isShowAllMode = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (VoiceRecognitionManager.Instance.IsNowRecording || VoiceRecognitionManager.Instance.IsNowProgressing)
            {
                return;
            }
            ResetTimer();
            iTween.ScaleTo(_gameObject, _initLocalScale * 1.2f, 2f);
        }

        private void ResetTimer()
        {
            _timer = 0f;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (VoiceRecognitionManager.Instance.IsNowRecording || VoiceRecognitionManager.Instance.IsNowProgressing)
            {
                return;
            }

            if (isShowAllMode)
            {
                ToBasicTextMode();
            }
            iTween.ScaleTo(_gameObject, _initLocalScale, 2f);
        }

        public void OnGvrPointerHover(PointerEventData eventData)
        {
            if (VoiceRecognitionManager.Instance.IsNowRecording || VoiceRecognitionManager.Instance.IsNowProgressing)
            {
                return;
            }

            _timer += Time.deltaTime;

            if (_timer > gazeTime && !isShowAllMode)
            {
                ToAllTextMode();
                ResetTimer();
            }
        }

        //  Text를 0.5초간 바라봤을 때 실행
        public void ToAllTextMode()
        {
            isShowAllMode = true;
            _currentToShowAllTextCoroutine = StartCoroutine(ToShowAllText());
        }

        public IEnumerator ToShowAllText()
        {
            //  전체 text에 알맞는 높이
            float preferredTextHeight = _text.preferredHeight;

            while (basicTextHeight > preferredTextHeight)
            {
                yield return null;
            }

            //  text에 알맞는 높이를 고려한 전체 Canvas 높이 ( Text의 Scale이 10배, 1000 = 녹음버튼의 높이 )
            float preferredPannelHeight = ((preferredTextHeight - basicTextHeight) * textScale) + basicPannelHeight;
            float t = 0.0f;
            float f = 0.0f;

            while (f <= 1f)
            {
                t += Time.deltaTime;
                f = t * showSpeed;
                _rectTransform.sizeDelta = new Vector2(basicPannelWidth, Mathf.Lerp(basicPannelHeight, preferredPannelHeight, f));
                _textRectTrans.sizeDelta = new Vector2(basicTextWidth, Mathf.Lerp(basicTextHeight, preferredTextHeight, f));
                yield return null;
            }
        }

        //  Pannel 에서 포인터가 벗어났을 때 실행
        public void ToBasicTextMode()
        {
            isShowAllMode = false;
            //  기존에 올라가던 pannel 코르틴 stop
            StopCoroutine(_currentToShowAllTextCoroutine);
            StartCoroutine(ToBasicText());
        }

        public IEnumerator ToBasicText()
        {
            //  포인터가 나간 순간 text 높이
            float currentTextHeight = _textRectTrans.sizeDelta.y;

            //  포인터가 나간 순간 canvas 높이
            float currentCanvasHeight = _rectTransform.sizeDelta.y;
            float t = 0.0f;
            float f = 0.0f;

            while (f <= 1f)
            {
                t += Time.deltaTime;
                f = t * showSpeed;
                _rectTransform.sizeDelta = new Vector2(basicPannelWidth, Mathf.Lerp(currentCanvasHeight, basicPannelHeight, f));
                _textRectTrans.sizeDelta = new Vector2(basicTextWidth, Mathf.Lerp(currentTextHeight, basicTextHeight, f));
                yield return null;
            }
        }

    }
}