using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    //  Delete / Add Button Container
    public class AddDeleteMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IGvrPointerHoverHandler
    {
        public float gazeTime = 0.5f;

        [HideInInspector]
        public Image progressImg;

        private const string DELETE = "Delete";
        private const string ADD = "Add";

        private Node _node;
        private Cube _cube;
        private GameObject _currentHoverButton;

        private float _timer;

        private void Awake()
        {
            _node = transform.parent.parent.GetComponent<Node>();
            _cube = transform.parent.parent.GetComponentInChildren<Cube>();
        }

        public void OnPointerEnter(PointerEventData ped)
        {
            _node.selectMe();
            progressImg = ped.pointerEnter.GetComponent<Image>();
            progressImg.fillAmount = 1f;
            _currentHoverButton = ped.pointerEnter.transform.parent.gameObject;
            ResetTimer();
            iTween.ScaleTo(_currentHoverButton, Vector3.one * 1.2f, 1f);
            AudioManager.Instance.PlaySound(2);
        }

        private void ResetTimer()
        {
            _timer = 0f;
        }

        public void OnPointerExit(PointerEventData ped)
        {
            progressImg.fillAmount = 1f;
            ResetTimer();
            iTween.ScaleTo(_currentHoverButton, Vector3.one, 1f);
        }

        public void OnGvrPointerHover(PointerEventData ped)
        {
            _timer += Time.deltaTime;
            progressImg.fillAmount = 1f - (_timer / gazeTime);
            if (TutorialManager.Instance.isFirst && _currentHoverButton.name == ADD) TutorialManager.Instance.DoMission(6);
            

            if (_timer > gazeTime)
            {
                ExecuteByButtonName(_currentHoverButton.name);
                ResetTimer();
                progressImg.fillAmount = 1f;
                iTween.ScaleTo(_currentHoverButton, Vector3.one, 1f);
                _cube.HideSubContainer();
            }
        }

        private void ExecuteByButtonName(string buttonName)
        {
            switch (buttonName)
            {
                case ADD:
                    {
                        NodeManager.Instance.CreateNode();
                        if (TutorialManager.Instance.isFirst) TutorialManager.Instance.DoMission(7);
                        AudioManager.Instance.PlaySound(7);
                        StartCoroutine(NextTutorial());
                    }
                    break;
                case DELETE:
                    {
                        DataManager.Instance.RemoveOne(_node.nodeKey, true);
                        if(TutorialManager.Instance.isFirst) TutorialManager.Instance.DoMission(9);
                        AudioManager.Instance.PlaySound(8);
                    }
                    break;
                default:
                    {
                        Debug.Log("there is something wrong at button name: " + buttonName);
                    }
                    break;
            }
        }
        IEnumerator NextTutorial()
        {
            yield return new WaitForSeconds(5f);
            if(TutorialManager.Instance.isFirst) TutorialManager.Instance.DoMission(8);
			    
        }
    }
}