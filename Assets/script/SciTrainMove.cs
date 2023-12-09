using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SciTrainMove : MonoBehaviour
{
   public float destructionDelay = 30.0f;

    void Start()
    {
        Destroy(gameObject, destructionDelay);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.right * 150 * Time.deltaTime;
    }
}
