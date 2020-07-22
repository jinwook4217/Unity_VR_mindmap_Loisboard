using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
  public static NodeManager Instance;                //nodeManager는 싱글톤으로 구현
  [HideInInspector]
  public GameObject selectedNode;                                            //현재 선택된 노드
  private Node _selectedNodeInfo;                                            //선택된 노드의 node스크립트
  private Node _newNodeInfo;                                                 //새로운 노드의 node스크립트
                                                                             //노드 프리펩
  public GameObject newNodePrefab;
  public GameObject motherNodePrefab;
  //새로운 노드와 부모노드와의 거리
  private float _newNodeDis;
  //새로운 노드의 각도(y축 기준)
  private float _newNodeAngle;
  //새로운 노드가 들어갈 자리(각도)의 기준점
  private List<float> _newNodeAngles;
  //자리 번호 중 0번째 자리의 각도
  public float firstAngle;
  //각도에 불규칙성을 부가하기위해 랜덤범위를 지정
  public float _angleRange;
  //새로운 노드와 부모노드와의 거리 최소값
  public float minDis;
  //새로운 노드와 부모노드와의 거리 최대값
  public float maxDis;
  //자리번호 개수
  [HideInInspector]
  public int posNumLength;
  //뎁스의 불규칙성을 위한 범위
  public float radiusRange;
  //절대 반지름(마더노드와 카메라(플레이어)의 거리
  private float _ebsoluteRadius;
  //현재 선택된 노드와의 거리
  private float _currentRadius;
  //회전각
  private float _yAngle;
  //y좌표 증감값
  private float _yDis;
  //마더노드
  public GameObject motherNode;
  //노드 이동 속도
  public float nodeMoveSpeed;
  //노드에 노드가 달릴떄 이동할 거리
  public float extraDis;
  public float gthWeight;
  public bool isSix;
  public bool noRandom;
  bool isFirstNode;
  void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else
    {
      Destroy(gameObject);
    }
    if (isSix)
    {
      posNumLength = 6;
    }
    else
    {
      posNumLength = 10;
    }
    //posNumLength와 firstAngle을 기준으로 newNodeAngles를 초기화
    _newNodeAngles = new List<float>();
    float anglePerIndex = 360 / posNumLength;
    _angleRange=anglePerIndex/2;
    _newNodeAngles.Add(firstAngle);
    for (int i = 1; i < posNumLength; i++)
    {
      _newNodeAngles.Add(_newNodeAngles[i - 1] + anglePerIndex);
    }
  }
  void Start()
  {
    if(!isSix){
      _angleRange/=2;
    }
    if (noRandom)
    {
      minDis = (maxDis + minDis) / 2;
      maxDis = minDis + 0.01f;
      radiusRange = 0;
      _angleRange = 0;
      firstAngle = 0;
    }
  }
  public void CreateNode()
  {

    _selectedNodeInfo = selectedNode.GetComponent<Node>();

    //포지션 번호를 랜덤으로 돌려주는 함수 실행
    int temp = SelectPosNumber(_selectedNodeInfo.posNum);
    //생성할수 있는 포지션 번호가 없을 때(=SelectPosNumber()의 리턴값이 -1일때) 노드 생성 x
    if (temp == -1)
    {
      print("can not create node");
      return;
    }

    GameObject newNode = Instantiate(newNodePrefab);
    _newNodeInfo = newNode.GetComponent<Node>();
    _newNodeInfo.posNum = temp;
    //부모 지정
    _newNodeInfo.parentNode = selectedNode;
    _newNodeInfo.parentNodeKey = _selectedNodeInfo.nodeKey;
    _newNodeInfo.gth = _selectedNodeInfo.gth + 1;
    //새로 생성된 노드의 고유번호 생성
    _newNodeInfo.nodeKey = _selectedNodeInfo.nodeKey + _newNodeInfo.posNum;
    //부모와의 거리 결정
    _newNodeDis = Random.Range(minDis, maxDis);
    //_newNodeDis = Random.Range(minDis, maxDis)+gthWeight*_newNodeInfo.gth;

    //부모노드의 자식 자리번호에 자신이 있다는걸 알림
    _selectedNodeInfo.children[_newNodeInfo.posNum] = newNode;
    //자리번호에 해당하는 기준각(newNodeAngles)을 기준으로 증감값을 랜덤으로 더함
    _newNodeAngle = _newNodeAngles[_newNodeInfo.posNum] + Random.Range(-_angleRange, _angleRange);
    _newNodeInfo.angle = _newNodeAngle;

    //노드의 위치를 위치 계산 함수로 지정
    newNode.transform.position = calculatePos(selectedNode.transform.position, _newNodeDis, _newNodeAngle);
    TutorialManager.Instance.GetNewNodePosition(newNode.transform);
    //지금 생성되는 노드가 부모노드의 첫번째 자식일때 nodeMove함수로 노드 이동
    if (_selectedNodeInfo.parentNode != null && !_selectedNodeInfo.hasChild)
    {
      //Vector3 tempPos= selectedNode.transform.position+Vector3.up*0;
      StartCoroutine(NodeMove(selectedNode, calculatePos(selectedNode.transform.position, extraDis, _selectedNodeInfo.angle), true));
      StartCoroutine(NodeMove(newNode, calculatePos(newNode.transform.position, extraDis, _selectedNodeInfo.angle), false));
    }
    _selectedNodeInfo.hasChild = true;
    _newNodeInfo.firstPos=newNode.transform.position;
    DataManager.Instance.SaveState(DataManager.work.CREATE);
  }

  //노드 번호 생성 함수
  int SelectPosNumber(int parentPosNum)
  {
    int selectedPosNumber;
    
    if(!_selectedNodeInfo.hasChild){
      isFirstNode=true;
    }else{
      isFirstNode=false;
    }
    List<int> canBeSelectedNum = new List<int>();

    for (int i = 0; i < posNumLength; i++)
    {
      if (_selectedNodeInfo.parentNode != null)
      {

        if (positionCheck(i, parentPosNum))
        {//여기
          if (!_selectedNodeInfo.children[i])
          {
            canBeSelectedNum.Add(i);
          }
        }
      }
      else
      {//여기
        if (!_selectedNodeInfo.children[i])
        {
          canBeSelectedNum.Add(i);
        }
      }

    }
    if (canBeSelectedNum.Count != 0)
    {
      selectedPosNumber = canBeSelectedNum[Random.Range(0, canBeSelectedNum.Count)];
    }
    else
    {
      selectedPosNumber = -1;
    }
    return selectedPosNumber;
  }

  //노드 이동 함수
  public IEnumerator NodeMove(GameObject nodeToMove, Vector3 endPos, bool shouldDraw)
  {
    float currentTime = 0;
    Vector3 startPos = nodeToMove.transform.position;

    while (currentTime < 1.0f)
    {
      currentTime += Time.deltaTime * nodeMoveSpeed;

      nodeToMove.transform.position = Vector3.Lerp(startPos, endPos, currentTime);
      nodeToMove.GetComponent<Node>().shouldDraw = shouldDraw;
      yield return null;
    }
  }

  //위치값 계산 함수(극좌표 이용)
  public Vector3 calculatePos(Vector3 lastNodePos, float dis, float angle)
  {
    transform.forward = new Vector3(lastNodePos.x, 0, lastNodePos.z);
    _currentRadius = Vector3.Distance(new Vector3(lastNodePos.x, 0, lastNodePos.z), transform.position);
    _yAngle = Mathf.Asin(Mathf.Sin(angle * Mathf.Deg2Rad) * dis / (_currentRadius * 2)) * 2;
    _yDis = Mathf.Cos(angle * Mathf.Deg2Rad) * dis;
    transform.Rotate(new Vector3(0, _yAngle * Mathf.Rad2Deg, 0), Space.World);
    return transform.position + transform.forward * (_ebsoluteRadius + Random.Range(-radiusRange, radiusRange)) + Vector3.up * (lastNodePos.y - 1 + _yDis);
  }

  public bool positionCheck(int i, int parentPosNum)
  {
    if (isSix)
    {
      if ((i == parentPosNum
              || i == (parentPosNum + 1) % posNumLength
              || i == (parentPosNum + posNumLength - 1) % posNumLength
              ))
      {
      }
      else
      {
        return false;
      }
    }
    else
    {
      if ((i == parentPosNum
              || i == (parentPosNum + 1) % posNumLength
              || i == (parentPosNum + posNumLength - 1) % posNumLength
              || i == (parentPosNum + 2) % posNumLength
              || i == (parentPosNum + posNumLength - 2) % posNumLength
              ))
      {
      }
      else
      {
        return false;
      }
    }

    Vector3 selectedPos = selectedNode.transform.position;
    Vector3 newPos = calculatePos(selectedNode.transform.position, maxDis, _newNodeAngles[i]);
    Vector3 direction = newPos - selectedPos;

    RaycastHit hitInfo;
    if(isFirstNode){
      selectedPos=calculatePos(selectedNode.transform.position, extraDis, _selectedNodeInfo.angle)-direction.normalized;
    }else{
      selectedPos=selectedPos+direction.normalized;
    }
    if (Physics.SphereCast(selectedPos,
      maxDis * Mathf.Sin(180.0f / posNumLength * Mathf.Deg2Rad), direction, out hitInfo, maxDis))
    {
      return false;
    }
    else
    {
      return true;
    }

  }


  public void ReStart()
  {
    motherNode = Instantiate(motherNodePrefab);
    motherNode.transform.position = new Vector3(0, 0, 13);
    motherNode.GetComponent<Node>().nodeKey = "m";
    _ebsoluteRadius = Vector3.Distance(motherNode.transform.position, transform.position);
    _currentRadius = _ebsoluteRadius;
    DataManager.Instance.SaveState(DataManager.work.CREATE);
  }
}