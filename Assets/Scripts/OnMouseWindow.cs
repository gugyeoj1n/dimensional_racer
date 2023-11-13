using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnMouseWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject window;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse entered the object!");
        if (window != null)
        {
            window.SetActive(true);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse exited the object!");
        if (window != null)
        {
            window.SetActive(false);
        }
    }
}