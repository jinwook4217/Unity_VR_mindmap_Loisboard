using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class Cube : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        //  gaze in 됬을 때 변하는 색 + 녹음중 깜박거리는 색
        public Color recodeColor;
        //  최대 글자수
        public int maxTextNum = 60;
        //  녹음중 깜박거리는 시간
        public float blinkDuration = 0.3f;
        //  Y축으로 움직이는 노드와의 최소 거리
        public float minYdistanceToNode = 10f;

        private Transform _transform;
        private Renderer _renderer;
        private GameObject _subMenuContainer;

        [HideInInspector]
        public AddDeleteMenu addDeleteMenu;
        [HideInInspector]
        public RecordMenu recordMenu;
        private Text _pannelText;

        private Transform _cameraTransform;

        private bool _isShowingSubMenus = false;

        private void Awake()
        {
            _transform = transform;
            _renderer = GetComponent<Renderer>();
            _subMenuContainer = _transform.parent.GetChild(1).gameObject;
            addDeleteMenu = _subMenuContainer.GetComponentInChildren<AddDeleteMenu>();
            recordMenu = _subMenuContainer.GetComponentInChildren<RecordMenu>();
            _pannelText = _transform.parent.GetChild(2).GetComponentInChildren<Text>();
            _cameraTransform = Camera.main.transform;

            SetCubeScaleByTextLength();
        }

        //  cube scale을 현재 글자수를 최대 글자수(60)에 비례해서 0.3 ~ 0.7 사이로 정한다
        public void SetCubeScaleByTextLength()
        {
            Vector3 cubeScale = Vector3.one;
            float ratioByText = (float)_pannelText.text.Length / maxTextNum;
            ratioByText = (Mathf.Clamp01(ratioByText) * 0.4f) + 0.3f;
            cubeScale *= ratioByText;
            _transform.localScale = cubeScale;
        }

        //  + / x 버튼 show
        public void ShowSubContainer()
        {
            if (!_isShowingSubMenus)
            {
                AudioManager.Instance.PlaySound(3);
                _isShowingSubMenus = true;
                iTween.ScaleTo(_subMenuContainer, Vector3.one * 0.01f, 0.5f);
            }
            
        }

        //  + / x 버튼 hide
        public void HideSubContainer()
        {
            if (addDeleteMenu.progressImg)
            {
                addDeleteMenu.progressImg.fillAmount = 1f;
            }

            if (recordMenu.progressImg)
            {
                recordMenu.progressImg.fillAmount = 1f;
            }

            if (VoiceRecognitionManager.Instance.IsNowRecording)
            {
                return;
            }

            _isShowingSubMenus = false;
            //AudioManager.Instance.PlaySound(4);
            iTween.ScaleTo(_subMenuContainer, Vector3.zero, 0.5f);
        }

        //  gaze in
        public void OnPointerEnter(PointerEventData ped)
        {
            //  녹음중이면 아무것도 실행하지 않는다	
            if (VoiceRecognitionManager.Instance.IsNowRecording || VoiceRecognitionManager.Instance.IsNowProgressing)
            {
                return;
            }


            ShowSubContainer();
            CheckDistanceToNode();
            if(TutorialManager.Instance.isFirst) TutorialManager.Instance.DoMission(3);
        }

        //  바라본 노드와의 Y축 거리가 최소 Y축 거리보다 크면 바라본 노드로 이동
        private void CheckDistanceToNode()
        {
            float distance = Mathf.Abs(_transform.position.y - _cameraTransform.position.y);
            if (distance > minYdistanceToNode)
            {
                GameObject player = _cameraTransform.parent.gameObject;
                Vector3 moveToY = new Vector3(0f, _transform.position.y);
                iTween.MoveTo(player, iTween.Hash("position", moveToY, "easetype", iTween.EaseType.easeInCubic, "time", 3f));
                AudioManager.Instance.PlaySound(9);
            }
        }

        public void OnPointerExit(PointerEventData ped)
        {
            if (!IsSubContainer(ped.pointerCurrentRaycast.gameObject))
            {
                HideSubContainer();
            }
        }

        private bool IsSubContainer(GameObject currentHover)
        {
            if (!currentHover)
            {
                return false;
            }
            else
            {
                if (!currentHover.CompareTag("SubButtons"))
                {
                    return false;
                }
                return true;
            }
        }
        private bool IsCube(GameObject currentHover)
        {
            if (!currentHover)
            {
                return false;
            }
            else
            {
                if (!currentHover.CompareTag("Cube"))
                {
                    return false;
                }
                return true;
            }
        }


        //  수정 필요
        public IEnumerator Blink()
        {
            Color tempColor = new Color(0.1f, 0.1f, 0.1f);
            bool isWhite = true;
            while (VoiceRecognitionManager.Instance.IsNowRecording)
            {
                // float lerp = Mathf.PingPong(Time.time, blinkDuration) / blinkDuration;
                // _renderer.material.color = Color.Lerp(Color.white, recodeColor, lerp);

                _renderer.material.SetColor("_EmissionColor", tempColor);
                _renderer.material.color = isWhite ? recodeColor : Color.white;
                isWhite = !isWhite;
                yield return new WaitForSeconds(blinkDuration);
            }

            tempColor = new Color(0.5f, 0.5f, 0.5f);
            _renderer.material.color = Color.white;
            _renderer.material.SetColor("_EmissionColor", tempColor);
        }
    }
}
