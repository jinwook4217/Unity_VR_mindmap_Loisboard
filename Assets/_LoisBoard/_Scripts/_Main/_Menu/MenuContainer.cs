using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class MenuContainer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IGvrPointerHoverHandler
    {
        public float gazeTime = 1f;
        public float dampTime = 0.2f;

        public GameObject cubeCam;

        private int imageWidth = 4096;

        private const string HOME = "Home";
        private const string UNDO = "Undo";
        private const string DELETE = "Delete";
        private const string SAVE = "Save";
        private const string EXPORT = "Export";

        private const string EDIT_PAGE = "EditPage";
        private const string TUTORIAL = "Tutorial";
        private const string EXIT = "Exit";

        private Transform _transform;
        private Transform _cameraTransform;
        private GameObject _currentHoverButton;
        private EventTrigger _eventTrigger;
        private Image _progressImg;

        private float _desiredAngle;
        private float _moveVelocity;

        private float _timer;
        private bool _shouldGazeAgain;
        private Transform _menuAxis;

        private void Awake()
        {
            _eventTrigger = GetComponent<EventTrigger>();
            _transform = transform;
            _cameraTransform = Camera.main.transform;
            _menuAxis = transform.parent;
        }

        private void Update()
        {
            float eulerAngleX = _cameraTransform.eulerAngles.x;
            // Debug.Log(eulerAngleX);
            //  50도 아래로 바라보면 마지막위치에 고정
            if (eulerAngleX > 50f && eulerAngleX < 180f)
            {
                return;
            }

            MenuAxisRotate();
        }

        private void MenuAxisRotate()
        {
            Vector3 cameraForward = _cameraTransform.forward;
            cameraForward.y = 0f;
            cameraForward.Normalize();

            float angle = Vector3.Angle(Vector3.forward, cameraForward);
            angle = Vector3.Dot(Vector3.right, cameraForward) > 0f ? angle : -angle;
            _desiredAngle = angle;

            float yAngle = Mathf.SmoothDampAngle(_menuAxis.eulerAngles.y, _desiredAngle, ref _moveVelocity, dampTime);
            _menuAxis.rotation = Quaternion.Euler(0f, yAngle, 0f);

            _transform.LookAt(_cameraTransform);
            _transform.forward = -_transform.forward;
        }

        public void OnPointerEnter(PointerEventData ped)
        {
            _eventTrigger.enabled = true;
            _progressImg = ped.pointerEnter.GetComponent<Image>();
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
            _shouldGazeAgain = false;
            _progressImg.fillAmount = 1f;
            iTween.ScaleTo(_currentHoverButton, Vector3.one, 1f);
        }

        public void OnGvrPointerHover(PointerEventData ped)
        {
            if (_shouldGazeAgain)
            {
                return;
            }

            _timer += Time.deltaTime;
            _progressImg.fillAmount = 1f - (_timer / gazeTime);


            if (_timer > gazeTime)
            {
                ExecuteByButtonName(_currentHoverButton.name);
                ResetTimer();
                _eventTrigger.enabled = false;
                _progressImg.fillAmount = 1f;
                iTween.ScaleTo(_currentHoverButton, Vector3.one, 1f);
                _shouldGazeAgain = true;
            }
        }

        private void ExecuteByButtonName(string buttonName)
        {
            switch (buttonName)
            {
                case HOME:
                    {
                        // _cameraTransform.LookAt(Vector3.forward);
                        GameObject.FindObjectOfType<NodeManager>().enabled = false;
                        SceneLoader.Instance.LoadLobyScene();
                        if (TutorialManager.Instance.isFirst) TutorialManager.Instance.DoMission(13);
                        AudioManager.Instance.PlaySound(1);
                    }
                    break;
                case SAVE:
                    {
                        RedisManager.Instance.Save();

                        string popupMessage = "Saved Successfully !";
                        PopupTextManager.Instance.OnPopupText(popupMessage, 2f);
                        AudioManager.Instance.PlaySound(13);

                        if (TutorialManager.Instance.isFirst) TutorialManager.Instance.DoMission(10);
                    }
                    break;
                case DELETE:
                    {
                        DataManager.Instance.Remove(true);
                        string popupMessage = "Deleted Successfully !";
                        PopupTextManager.Instance.OnPopupText(popupMessage, 2f);
                        AudioManager.Instance.PlaySound(8);
                    }
                    break;
                case EXPORT:
                    {
                        Instantiate(cubeCam, Camera.main.transform.position, Quaternion.identity);
                        GameObject[] allNodes = GameObject.FindGameObjectsWithTag("Node");
                        for (int i = 0; i < allNodes.Length; i++)
                        {
                            allNodes[i].GetComponentInChildren<NodePannel>().ToAllTextMode();
                        }
                        StartCoroutine(I360Render.Instance.Capture(imageWidth));
                        StartCoroutine(I360Render.Instance.SaveShare());
                        string popupMessage = EXPORT + " Progressing...";
                        PopupTextManager.Instance.OnPopupText(popupMessage, 4f);
                        if (TutorialManager.Instance.isFirst) TutorialManager.Instance.DoMission(11);
                        AudioManager.Instance.PlaySound(11);
                    }
                    break;
                case UNDO:
                    {
                        DataManager.Instance.Undo();
                        string popupMessage = UNDO + " Successfully !";
                        PopupTextManager.Instance.OnPopupText(popupMessage, 2f);
                        AudioManager.Instance.PlaySound(14);
                    }
                    break;
                case EDIT_PAGE:
                    {
                        SavedPagesScroll savedPagesScroll = GameObject.FindObjectOfType<SavedPagesScroll>();
                        if (savedPagesScroll.enabled)
                        {
                            savedPagesScroll.IsDeleteMode = !savedPagesScroll.IsDeleteMode;
                            string popupMessage;
                            if (savedPagesScroll.IsDeleteMode)
                            {
                                _currentHoverButton.GetComponentInChildren<Text>().text = "EDIT CANCLE";
                                _currentHoverButton.GetComponent<Image>().color = new Color32(255, 82, 119, 255);
                                popupMessage = "Edit mode!";
                            }
                            else
                            {
                                _currentHoverButton.GetComponentInChildren<Text>().text = "EDIT PAGE";
                                _currentHoverButton.GetComponent<Image>().color = new Color32(0, 245, 155, 255);
                                popupMessage = "Edit completed!";
                            }
                            PopupTextManager.Instance.OnPopupText(popupMessage, 3f);
                        }
                        else
                        {
                            string popupMessage = "there are no space!";
                            PopupTextManager.Instance.OnPopupText(popupMessage, 3f);
                        }
                    }
                    break;
                case TUTORIAL:
                    {
                        TutorialManager.Instance.StartTutorial();
                    }
                    break;
                case EXIT:
                    {
                        Application.Quit();
                    }
                    break;
                default:
                    {
                        Debug.Log("Error At Current Hover Button Name: " + buttonName);
                    }
                    break;
            }
        }

    }
}
