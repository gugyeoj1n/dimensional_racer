using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class horse : MonoBehaviour
{
    public float destructionDelay = 30.0f;

    void Start()
    {
        Destroy(gameObject, destructionDelay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
