using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCylinder : MonoBehaviour
{
    public Transform target;
    private Quaternion targetRotation;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Quaternion relativeRotation = Quaternion.Inverse(target.rotation) * transform.rotation;
        Quaternion negativeRelativeRotation = Quaternion.Euler(-relativeRotation.eulerAngles);
        transform.rotation = target.rotation * negativeRelativeRotation;
    }
}
