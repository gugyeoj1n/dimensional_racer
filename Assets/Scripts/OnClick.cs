using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClick : MonoBehaviour
{
    public GameObject scroll;

    public void SetActiveSelf()
    {
        scroll.SetActive(!scroll.active);
    }
    
}
