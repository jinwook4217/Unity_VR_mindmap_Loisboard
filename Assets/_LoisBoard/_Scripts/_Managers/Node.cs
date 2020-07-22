using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
  public GameObject player;
  private float _currentTime;

  private Transform _transform;

  //부모 노드에 대하여 이 노드의 위치 번호
  public int posNum;
  //부모노드
  public GameObject parentNode;
  //자신의 고유 키
  public string nodeKey;
  //부모의 고유 키
  public string parentNodeKey;
  //각도
  public float angle;
  //자신의 자식 위치 번호에 자식이 존재하는지
  public GameObject[] children;
  //자식이 있는지
  public bool hasChild;
  //라인렌더러
  private LineRenderer _lineRenderer;
  //라인 두께
  public float lineWidth;
  //노드 생성시 노드가 최대로 커질때의 노드 크기
  public float maxScale;
  //라인 생성 속도(노드 움직이는 속도랑 일치)
  private float _lineSpeed;
  //노드 이동시에 라인을 다시 그려주는지 여부
  public bool shouldDraw = false;
  //노드가 커지고있는가
  private bool _isBeingLarger = true;
  //노드가 작아지고있는가
  private bool _isBeingSmaller;
  //노드 스케일 조정용 시간 기록 함수
  private float _scaleTime;
  //노드 스케일 조정 속도
  public float scaleSpeed;
  //노드 세대
  public int gth;

  public bool willDestroy;
  public Node parentNodeInfo;
  private bool _isAnimPlayed;
  public bool isFirstDeleteNode;

  //겹쳤을때 자리를 찾기위한 변수들
  private Vector3 _originPosition;
  private float _nodeRadius = 1.5f;
  private bool _isOrgPosChecked;
  private float currentAngleTime;
  private bool _isNodePiled;
  private float currentChangingRadius;
  private float ChangingRadius;
  private float currentRadiusTime;

  public bool shouldNodeCheckPiled;
  //노드 겹침 판별 레이케스트힛
  public RaycastHit hitInfo;
  public Vector3 firstPos;
  private void Start()
  {
    _transform = transform;
    player = Camera.main.transform.parent.gameObject;

    //player = GameObject.Find("Player");
    //라인 스피드를 노드 노드 이동 스피드와 동기화한다.
    _lineSpeed = NodeManager.Instance.nodeMoveSpeed;

    //자식 위치 번호 리스트를 false로 초기화한다

    children = new GameObject[NodeManager.Instance.posNumLength];

    _lineRenderer = GetComponent<LineRenderer>();

    //부모노드가 없다면(=자신이 마더노드라면)
    if (parentNode == null)
    {
      //라인렌더러를 끈다
      _lineRenderer.enabled = false;
    }
    else
    {
      parentNodeInfo = parentNode.GetComponent<Node>();
      //라인 그리기 코루틴을 실행
      StartCoroutine(LineDraw());

    }
    //시작시 스케일을 0으로 초기화
    _transform.localScale = Vector3.zero;
  }

  private void Update()
  {
    if (parentNode != null && parentNodeInfo == null)
    {
      parentNodeInfo = parentNode.GetComponent<Node>();
    }
    //부모가 있고, shouldDraw가 true이면(= 라인을 새로 그려야 하면) 라인을 그린다.
    if (parentNode != null && shouldDraw)
    {
      _lineRenderer.enabled = true;
      _lineRenderer.startWidth = lineWidth;
      _lineRenderer.endWidth = lineWidth;
      _lineRenderer.SetPosition(0, _transform.position);
      _lineRenderer.SetPosition(1, parentNode.transform.position);
      shouldDraw = false;
    }

    //노드 생성시 노드가 커졌다가 작아지는 것을 구현
    if (_isBeingLarger)
    {
      _scaleTime += Time.deltaTime * scaleSpeed;
      _transform.localScale = Vector3.one * Mathf.Sqrt(_scaleTime) * maxScale;
      if (_scaleTime > 1f)
      {
        //transform.localScale=new Vector3(1,1,1);
        _isBeingLarger = false;
        _isBeingSmaller = true;
        _scaleTime *= maxScale;
      }
    }
    if (_isBeingSmaller)
    {
      _scaleTime -= Time.deltaTime * scaleSpeed;
      _transform.localScale = Vector3.one * _scaleTime;
      if (_scaleTime < 1f)
      {
        _transform.localScale = Vector3.one;
        _isBeingSmaller = false;
      }
    }

    _currentTime += Time.deltaTime;
    if (willDestroy)
    {

      for (int i = 0; i < children.Length; i++)
      {
        if (children[i])
        {
          children[i].GetComponent<Node>().willDestroy = true;
        }
      }
      if (isFirstDeleteNode)
      {
        if (parentNode == null)
        {
          willDestroy = false;
          isFirstDeleteNode = false;
          hasChild = false;
          children = new GameObject[NodeManager.Instance.posNumLength];
          return;
        }
        parentNodeInfo.children[posNum] = null;
        for (int i = 0; i < parentNodeInfo.children.Length; i++)
        {
          if (parentNodeInfo.children[i] != null)
          {
            break;
          }
          else if (i == parentNodeInfo.children.Length - 1)
          {
            parentNodeInfo.hasChild = false;
            if(parentNodeInfo.parentNode!=null){
              StartCoroutine(NodeManager.Instance.NodeMove(parentNode, parentNodeInfo.firstPos, true));
            }
          }
        }
      }
      _lineRenderer.enabled=false;
      StartCoroutine(deleteAnim());
    }

    //노드 겹칠때 피하는 알고리즘

    //Debug.DrawLine(transform.position,player.transform.position,Color.red);

    // if (shouldNodeCheckPiled)
    // {
    //   if (Physics.SphereCast(2 * transform.position - transform.position, _nodeRadius, player.transform.position - transform.position, out hitInfo, 10))
    //   {
    //     // print(_nodeRadius);
    //     _isNodePiled = true;
    //     shouldDraw=true;
    //     for(int i=0;i<children.Length;i++){
    //         children[i].GetComponent<Node>().shouldDraw=true;
    //     }
    //   }
    //   else
    //   {
    //     _isNodePiled = false;
    //     _isOrgPosChecked = false;
    //     currentChangingRadius = 0;
    //     shouldNodeCheckPiled = false;
    //     shouldDraw=true;
    //   }

    //   if (_isNodePiled)
    //   {
    //     if (!hitInfo.collider.name.Equals(player.name))
    //     {
    //       if (!_isOrgPosChecked)
    //       {
    //         _originPosition = transform.position;
    //         _isOrgPosChecked = true;
    //         ChangingRadius = 0.2f;
    //         currentRadiusTime = 0;
    //         currentAngleTime = 0;
    //       }
    //       else
    //       {

    //         if (currentChangingRadius != ChangingRadius)
    //         {
    //           currentChangingRadius += Time.deltaTime * 0.1f;
    //           transform.position = Vector3.Lerp(_originPosition, _originPosition + Vector3.up * ChangingRadius, currentChangingRadius / ChangingRadius + currentRadiusTime);
    //           if (currentChangingRadius >= ChangingRadius)
    //           {
    //             currentChangingRadius = ChangingRadius;
    //           }
    //         }
    //         else
    //         {
    //           currentAngleTime += Time.deltaTime;
    //           transform.RotateAround(_originPosition, Vector3.back, currentAngleTime * Mathf.PI);
    //           if (currentAngleTime > 2.5)
    //           {
    //             print(ChangingRadius + "!!" + currentChangingRadius);
    //             currentAngleTime = 0;
    //             ChangingRadius += 0.2f;
    //             currentRadiusTime = 0;
    //           }
    //         }

    //       }
    //     }
    //   }
    // }

  }

  //이벤트 트리거 pointEnter로 실행하기 위해 작성한 함수
  //자신을 nodemanager의 selectednode로 선택하여 생성할 함수의 부모로 삼기위한 준비
  public void selectMe()
  {
    NodeManager.Instance.selectedNode = gameObject;
  }

  //라인 그리기 함수
  //러프를 사용하여 라인 시작 포지션을 리턴한다.
  public IEnumerator LineDraw()
  {
    float drawTime = 0f;
    while (drawTime < 1f)
    {
      drawTime += Time.deltaTime * _lineSpeed;
      _lineRenderer.startWidth = lineWidth;
      _lineRenderer.endWidth = lineWidth;
      _lineRenderer.SetPosition(0, Vector3.Lerp(parentNode.transform.position, _transform.position, drawTime));
      _lineRenderer.SetPosition(1, parentNode.transform.position);
      yield return null;
    }
    shouldDraw = true;
  }

  IEnumerator deleteAnim()
  {
    //i=1
    //i가 0보다 작아지기 전까지 i에 0.1을 빼면서 투명도를 줄여라
    //for (float i = 1f; i >= 0; i -= Time.deltaTime * 0.5f)
    //{
    //    Color color = new Vector4(1, 1, 1, i);
    //    _transform.GetChild(0).GetComponent<MeshRenderer>().material.color = color;
    //    yield return null;
    //}

    bool _isBeingLargerDestroy = true;
    bool _isBeingSmallerDestroy = false;
    float _scaleTime = 1.0f;
    while (_isBeingLargerDestroy || _isBeingSmallerDestroy)
    {
      if (_isBeingLargerDestroy)
      {
        _scaleTime += Time.deltaTime * scaleSpeed;
        _transform.localScale = Vector3.one * _scaleTime;
        if (_scaleTime > maxScale)
        {
          _transform.localScale = Vector3.one;
          _isBeingLargerDestroy = false;
          _isBeingSmallerDestroy = true;
          _scaleTime = 1.0f;
        }
      }
      if (_isBeingSmallerDestroy)
      {
        _scaleTime -= Time.deltaTime * scaleSpeed;
        if (_transform.localScale.x > 0.05f)
        {
          _transform.localScale = Vector3.one * Mathf.Sqrt(_scaleTime) * maxScale;
        }

        if (_scaleTime <= 0.2f)
        {
          //transform.localScale=new Vector3(1,1,1);

          _isBeingSmallerDestroy = false;
        }
      }
      yield return null;
    }
    Destroy(gameObject);
  }
}