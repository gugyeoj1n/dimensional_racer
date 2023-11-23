using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCylinder : MonoBehaviour
{
    public Transform target;
    private Quaternion targetRotation;

    void Update()
    {
        Quaternion relativeRotation = Quaternion.Inverse(target.rotation) * transform.rotation;
        Quaternion negativeRelativeRotation = Quaternion.Euler(-relativeRotation.eulerAngles);
        transform.rotation = target.rotation * negativeRelativeRotation;
    }
}