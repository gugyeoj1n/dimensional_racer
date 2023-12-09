using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STrainspawn : MonoBehaviour
{
    public GameObject SciTrainObject;
    public GameObject SciTrainStartPosition;
  
    private void Start()
    {
        StartCoroutine(SciTrainSpawn_coroutine());
    }

    IEnumerator SciTrainSpawn_coroutine()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(10.0f);

             GameObject nowTrain = Instantiate(SciTrainObject);
             nowTrain.transform.position = SciTrainStartPosition.transform.position;
        }
        
        
    }
}