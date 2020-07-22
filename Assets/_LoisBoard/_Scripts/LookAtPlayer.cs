using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform _player;
    private Transform _transform;

    private IEnumerator Start()
    {
        _transform = transform;
        _player = Camera.main.transform;

        while (true)
        {
            _transform.LookAt(_player);
            _transform.forward = - _transform.forward;
            yield return null;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
