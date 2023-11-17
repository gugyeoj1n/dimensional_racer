using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        Destroy(gameObject);
        if (other.gameObject.GetComponent<TestAirplaneController>().boosterAmount < 2) other.gameObject.GetComponent<TestAirplaneController>().boosterAmount++;
    }
}
