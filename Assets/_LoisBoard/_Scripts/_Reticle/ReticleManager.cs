using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleManager : MonoBehaviour
{
    public static ReticleManager Instance;
    public static bool Rotating;
    //레티클 렌더링 업데이트 제어하는 변수
    public static bool isDrawn = true;
    public float oneShotreticleSize;

    GvrReticlePointer reticlePointer;

    //레티클 성장속도 변수
    public float reticleGrowthSpeed;
    //레티클 도형 
    public int reticleSegment;
    //코루틴 정지문
    public static bool isLarger = true;
    //레티클 오브젝트 트랜스폼
    private Transform _reticleTransform;


    //레티클 회전 스위치
    public static bool isRotation;

    //레티클 회전 각도 변수
    // public Vector3 ReticleRotationAxis;
    //레티클 회전 속도 변수
    public float reticleRotationSpeed;

    //무한회전모드
    public bool isInfiniteMode = true;

    //레티클 회전 횟수
    public int reticleRotationNumber;
    //레티클 회전 시간 간격
    public float reticleRotationTime = 1.0f;
    //레티클 회전 각도
    public int reticleRotationAngle = 1;

    //레티클 => 후에 겟셋으로 바꾸는거 연습해보기.
    public bool isInfiniteRotation;


    //레티클


    void Awake()
    {
        if (Instance == null) Instance = this;
        Rotating = false;
    }

    void Start()
    {
        reticlePointer = Camera.main.GetComponentInChildren<GvrReticlePointer>();
        // ControlReticle();
        //레티클 회전 받아오기
        //1. 레티클 오브젝트에 접근해서
        //2. 트랜스폼에 접근해서
        //3. 정해진 시간별로 회전 되게 만든다.

        //이벤트가 발생할 때만 회전하게 만든다=> 0.5초에 큰 각도로

        _reticleTransform = GameObject.FindGameObjectWithTag("Reticle").transform;

    }


    public void ControlReticle()
    {
        //엄데이트 중지
        isDrawn = false;
    }




    IEnumerator MakeReticle()
    {
        isDrawn = false;
        while (isLarger)
        {
            MakeOnce();
            yield return null;
        }
    }

    IEnumerator RotateReticle()
    {
        //레티클 무한모드가 켜져 있으면 코루틴을 계속 실행시킨다 엑시트 할때 까지.
        WaitForSeconds wfs = new WaitForSeconds(reticleRotationTime);

        if(Rotating) yield return null;
        else if (isInfiniteMode == false)
        {
            Rotating = true;
            for (int i = 0; i < reticleRotationNumber; i++)
            {
                _reticleTransform.Rotate(Vector3.forward * reticleRotationAngle);
                yield return wfs;
            }
            Rotating = false;

        }
        else
        {
            Rotating = true;
            while (isInfiniteRotation)
            {
                _reticleTransform.Rotate(Vector3.forward * Time.deltaTime * reticleRotationSpeed);
                yield return null;
            }
            Rotating = false;
        }
    }

    //레티클을 한번 마음대로 크게 쓰는 함수
    public void MakeOnce()
    {

        reticlePointer.ConnectFunction1(oneShotreticleSize);
        // reticlePointer.ConnectFunction1();
    }

    //수동으로 레티클을 작게만드는 함수
    public void RecoverReticle()
    {
        isDrawn = true;
        reticlePointer.ConnectFunction2();
        isLarger = true;
    }

    public void ChangeCircle()
    {

    }

    // public void RotateReticle()
    // {
    //     _reticleTransform.Rotate(Vector3.forward * Time.deltaTime * reticleRotationSpeed);

    public void TriggerCoroutine()
    {
        StartCoroutine("RotateReticle");
        // isRotation = false;
    }






}