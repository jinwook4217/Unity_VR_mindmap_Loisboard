using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class MySpeech : MonoBehaviour
    {
        //private Toggle _isRuntimeDetectionToggle;
        private GCSpeechRecognition _speechRecognition;

        //public Button _startRecordButton, _stopRecordButton;
        public Text _speechRecognitionResult;

        public AudioSource bgm;

        private Cube _nodeCube;

        //public static MySpeech Instance { get; private set; }

        void Awake()
        {
            //if (Instance == null)
            //{
            //    Instance = this;
            //}
            _speechRecognition = GCSpeechRecognition.Instance;
            _speechRecognition.RecognitionSuccessEvent += SpeechRecognizedSuccessEventHandler;
            _speechRecognition.RecognitionFailedEvent += SpeechRecognizedFailedEventHandler;

            //_startRecordButton.onClick.AddListener(StartRecordButtonOnClickHandler);
            //_stopRecordButton.onClick.AddListener(StopRecordButtonOnClickHandler);

            //_speechRecognitionState.color = Color.white;
            //_startRecordButton.interactable = true;
            //_stopRecordButton.interactable = false;

            LanguageTran(Application.systemLanguage);
            bgm =  GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
        }

 
        public void StartSpeech(Text text)
        {
            _speechRecognitionResult = text;
            
            //Debug.Log(_speechRecognition);
            _speechRecognition.StartRecord(false);  //_speechRecognition.StartRecord(_isRuntimeDetectionToggle.isOn)
            //_stopRecordButton.interactable = true;
            //_startRecordButton.interactable = false;
            //_speechRecognition.SetLanguage((Enumerators.LanguageCode)value++);
            //Debug.Log(value);
        }

        public void StopSpeech(Cube nodeCube)
        {
            _nodeCube = nodeCube;
            //Debug.Log("SpeechStop");
            _speechRecognition.StopRecord();
            bgm.volume = 1f;
            _speechRecognitionResult.text = "Progressing...";
            VoiceRecognitionManager.Instance.IsNowProgressing = true;
        }

        private void SpeechRecognizedFailedEventHandler(string obj, long requestIndex)
        {
            _speechRecognitionResult.text = "No word detected !";
            //StartCoroutine(TutorialManager.Instance.NetworkWarning());
            VoiceRecognitionManager.Instance.IsNowProgressing = false;
        }

        private void SpeechRecognizedSuccessEventHandler(RecognitionResponse obj, long requestIndex)
        {
            VoiceRecognitionManager.Instance.IsNowProgressing = false;
            if (obj != null && obj.results.Length > 0)
            {
                _speechRecognitionResult.text =  obj.results[0].alternatives[0].transcript;
                DataManager.Instance.ChageState();

                _nodeCube.SetCubeScaleByTextLength();
            }
            else
            {
                _speechRecognitionResult.text = "Speech Recognition succeeded! Words are no detected.";
            }
            if(TutorialManager.Instance.isFirst) TutorialManager.Instance.DoMission(5);
			    AudioManager.Instance.PlaySound(6);
            
            if (TutorialManager.Instance.isFirst) StartCoroutine(TutorialManager.Instance.NextTutorial(6, 5));
        }

        void LanguageTran(SystemLanguage lang)
        {
            switch (lang)
            {
                case SystemLanguage.Korean:
                    _speechRecognition.SetLanguage(Enumerators.LanguageCode.KO);
                    break;
                case SystemLanguage.English:
                    _speechRecognition.SetLanguage(Enumerators.LanguageCode.EN_US);
                    break;
                case SystemLanguage.Japanese:
                    _speechRecognition.SetLanguage(Enumerators.LanguageCode.JA);
                    break;
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    _speechRecognition.SetLanguage(Enumerators.LanguageCode.ZH_TW);
                    break;


                default:
                    _speechRecognition.SetLanguage(Enumerators.LanguageCode.EN_US);
                    break;
            }
        }
    }
   
}
