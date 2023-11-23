using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCylinder : MonoBehaviour
{
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = new Vector3(target.transform.position.x, target.transform.position.y + 20, target.transform.position.z);
        transform.position = position;
    }
}
