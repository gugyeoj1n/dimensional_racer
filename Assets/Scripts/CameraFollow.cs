using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class CameraFollow : MonoBehaviour
{
    private bool isFollowing = false;
    private Transform camTransform;

    private Vector3 centerOffset = Vector3.zero;
    private Vector3 cameraOffset = Vector3.zero;

    [SerializeField]
    private float distance;
    [SerializeField]
    private float height;

    [SerializeField]
    private float smoothSpeed;

    private void LateUpdate()
    {
        if (camTransform == null && isFollowing)
        {
            StartFollowing();
        }

        if (isFollowing)
        {
            Follow();
        }
    }

    public void StartFollowing()
    {
        camTransform = Camera.main.transform;
        isFollowing = true;
        Cut();
    }

    private void Follow()
    {
        cameraOffset.z = -distance;
        cameraOffset.y = height;
        camTransform.position = Vector3.Lerp(camTransform.position,
            this.transform.position + this.transform.TransformVector(cameraOffset), smoothSpeed * Time.deltaTime);
        camTransform.LookAt(this.transform.position + centerOffset);
    }

    private void Cut()
    {
        cameraOffset.z = -distance;
        cameraOffset.y = height;
        camTransform.position = this.transform.position + this.transform.TransformVector(cameraOffset);
        camTransform.LookAt(this.transform.position + centerOffset);
    }
}
