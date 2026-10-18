using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private GameObject target;       
    public Vector3 offset;
    public float speed;

    void LateUpdate()
    {
        transform.position = target.transform.position + offset;
    }
}
