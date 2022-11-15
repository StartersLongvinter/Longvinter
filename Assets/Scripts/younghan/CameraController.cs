using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 targetPosition;
    public Vector3 offset;


    public PlayerController playerController;

    void Start()
    {
        offset = transform.position;
    }

    void FixedUpdate()
    {
        //if (playerController.isAiming)
        //{
        //    Vector3 dir = (playerController.lookPoint - target.position).normalized;
        //    dir = new Vector3(dir.x, 0, dir.z);

        //    float distance = Mathf.Clamp();

        //    targetPosition = dir * distance;
        //}
        //else
        //{
        //    targetPosition = target.position;
        //}

        //float dist = Vector3.Distance(target.position, transform.position);

        //transform.position = targetPosition + offset;
        transform.position = target.position + offset;
    }

}
