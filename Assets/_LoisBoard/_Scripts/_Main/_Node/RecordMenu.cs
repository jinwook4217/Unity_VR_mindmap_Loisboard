using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class RecordMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IGvrPointerHoverHandler
    {
        public float gazeTime = 0.5f;
        public Sprite recodeSprite;
        public Sprite stopSprite;
        public Image recordImg;
        public Image recordProgressImg;
        public AudioSource bgm;

        [HideInInspector]
        public Image progressImg;
        private GameObject _currentHoverButton;
		private Cube _cube;
        private NodePannel _nodePannel;
        private Text _pannelText;

        private float _timer;

        private void Awake()
        {
            _nodePannel = transform.parent.parent.GetComponentInChildren<NodePannel>();
            _pannelText = _nodePannel.GetComponentInChildren<Text>();
			_cube = transform.parent.parent.GetComponentInChildren<Cube>();
            bgm = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
        }

        public void OnPointerEnter(PointerEventData ped)
        {
            progressImg = ped.pointerEnter.GetComponent<Image>();
            progressImg.fillAmount = 1f;
            _currentHoverButton = ped.pointerEnter.transform.parent.gameObject;

            if (!(VoiceRecognitionManager.Instance.IsNowRecording || VoiceRecognitionManager.Instance.IsNowProgressing))
            {
                iTween.ScaleTo(_nodePannel.gameObject, Vector3.one * 0.001f * 1.2f, 2f);
                _nodePannel.ToAllTextMode();
            }

            ResetTimer();
            AudioManager.Instance.PlaySound(2);
            iTween.ScaleTo(_currentHoverButton, Vector3.one * 1.2f, 1f);
        }

        private void ResetTimer()
        {
            _timer = 0f;
        }

        public void OnPointerExit(PointerEventData ped)
        {
            progressImg.fillAmount = 1f;

            //  녹음 중이 아니고 text all mode이면 다시 원상복귀
            if (!VoiceRecognitionManager.Instance.IsNowRecording && _nodePannel.isShowAllMode)
            {
                iTween.ScaleTo(_nodePannel.gameObject, Vector3.one * 0.001f, 2f);
                _nodePannel.ToBasicTextMode();
            }

            ResetTimer();
            iTween.ScaleTo(_currentHoverButton, Vector3.one, 1f);
        }

        public void OnGvrPointerHover(PointerEventData ped)
        {
            _timer += Time.deltaTime;
            progressImg.fillAmount = 1f - (_timer / gazeTime);

            if (_timer > gazeTime)
            {
				OnRecodeORStop();
                ResetTimer();
                progressImg.fillAmount = 1f;
                iTween.ScaleTo(_currentHoverButton, Vector3.one, 1f);
            }
        }

        private void OnRecodeORStop()
        {
            if (VoiceRecognitionManager.Instance.IsNowRecording)
            {
                //  녹음 정지알고리즘
                VoiceRecognitionManager.Instance.StopVoiceRecognition();
                
            }
            else
            {
                //  녹음 시작알고리즘
                VoiceRecognitionManager.Instance.StartVoiceRecognition(_pannelText, _cube);
                bgm.volume = 0.2f;
                if(TutorialManager.Instance.isFirst) TutorialManager.Instance.DoMission(4);
			    AudioManager.Instance.PlaySound(5);
            }
        }

        //  녹음 버튼의 모양을 바꾼다
		public void OnSpriteToggle()
		{
            RectTransform rect = _currentHoverButton.GetComponent<RectTransform>();
			bool recording = VoiceRecognitionManager.Instance.IsNowRecording;
            recordImg.overrideSprite = recording ? stopSprite : recodeSprite;
            recordProgressImg.overrideSprite = recording ? stopSprite : recodeSprite;

            //  정지 버튼 위치 바꾼다
            rect.anchorMax = recording ? new Vector2(0.5f, 0f) : new Vector2(0.5f, 1f);
            rect.anchorMin = recording ? new Vector2(0.5f, 0f) : new Vector2(0.5f, 1f);
            rect.anchoredPosition = recording ? new Vector2(0f, 30f) : new Vector2(0f, -20f);
		}
    }
}
