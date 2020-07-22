using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//
// 이 스크립트는 음성인식이 실행될 때 사용자에게 음성인식 중이라는 
// UI를 표시해주는 기능을 한다.
//
namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class VoiceRecognitionManager : MonoBehaviour
    {
        public static VoiceRecognitionManager Instance { get; private set; }

        public float recordTime = 7f;
        public GameObject voiceRecognition;
        public Image progressImg;

        private bool _isNowRecording;
        private Cube _nodeCube;
        private GameObject _addDeleteMenu;
        private RecordMenu _recodeMenu;
        private NodePannel _nodePannel;

        public MySpeech speech;
        
        //  현재 Recoding 중인가
        public bool IsNowRecording
        {
            get
            {
                return _isNowRecording;
            }
            set
            {
                _isNowRecording = value;
            }
        }

        public bool IsNowProgressing { get; set; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            speech = Camera.main.GetComponent<MySpeech>();
            _isNowRecording = false;
            voiceRecognition.SetActive(false);
        }

        //  음성인식 시작
        public void StartVoiceRecognition(Text pannelText, Cube cube)
        {
            _isNowRecording = true;
            _nodeCube = cube;
            _addDeleteMenu = cube.addDeleteMenu.gameObject;
            _recodeMenu = cube.recordMenu;
            _nodePannel = pannelText.GetComponentInParent<NodePannel>();
            voiceRecognition.SetActive(true);
            iTween.ScaleTo(_addDeleteMenu, Vector3.zero, 1f);
            _recodeMenu.OnSpriteToggle();
            StartCoroutine(StartImageProgress());
            StartCoroutine(_nodeCube.Blink());

            speech.StartSpeech(pannelText);
        }

        private IEnumerator StartImageProgress()
        {
            progressImg.fillAmount = 1f;
            float timer = 0f;

            while (_isNowRecording)
            {
                timer += Time.deltaTime;
                progressImg.fillAmount = 1f - (timer / recordTime);
                
                if (timer > recordTime)
                {
                    StopVoiceRecognition();
                    timer = 0f;
                }
                yield return null;
            }
        }

        //  음성인식 종료
        public void StopVoiceRecognition()
        {
            _isNowRecording = false;

            speech.StopSpeech(_nodeCube);
            // StartCoroutine(ShowTextOneTime());
            iTween.ScaleTo(_addDeleteMenu, Vector3.one, 0f);
            iTween.ScaleTo(_nodePannel.gameObject, Vector3.one * 0.001f, 2f);
            _nodeCube.HideSubContainer();
            _recodeMenu.OnSpriteToggle();
            voiceRecognition.SetActive(false);
            // _nodeCube.SetCubeScaleByTextLength();
        }
    }
}
