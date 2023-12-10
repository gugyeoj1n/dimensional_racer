using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform TargetTransform;
    
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Teleport(other.gameObject);
        }
    }

    public void Teleport(GameObject target)
    {
        target.transform.position = TargetTransform.position;
    }
}
