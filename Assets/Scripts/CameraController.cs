using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private Vector3 _offset;

    // Update is called once per frame
    void Update()
    {
        transform.position = _target.position + _offset;   
    }
}