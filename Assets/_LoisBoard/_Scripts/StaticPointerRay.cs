using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// 이 스크립트는 Player 로 부터 쏜 Ray를 이용해야 할 때 Ray를 사용할 수 있도록
// Ray를 반환한다.
//
public class StaticPointerRay : MonoBehaviour
{
    public static StaticPointerRay Instance { get; private set; }
    private Transform _pointerTransform;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _pointerTransform = Camera.main.transform;
    }

    public Ray PointerRay
    {
        get
        {
            Ray pointerRay = new Ray(_pointerTransform.position, _pointerTransform.forward);
            return pointerRay;
        }
    }
}
