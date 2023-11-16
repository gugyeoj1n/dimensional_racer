using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnMouseWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject window;

    public string name;
    public float speed;
    public float accel;
    public float fuel;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse entered the object!");
        if (window != null)
        {
            window.SetActive(true);
            SetText();
        }
    }

    public void SetText()
    {
        window.transform.GetChild(1).GetComponent<TMP_Text>().text = name;
        window.transform.GetChild(3).GetComponent<TMP_Text>().text = GetSpeedTier(speed);
        window.transform.GetChild(5).GetComponent<TMP_Text>().text = GetAccelTier(accel);
        window.transform.GetChild(7).GetComponent<TMP_Text>().text = GetFuelTier(fuel);
    }

    private string GetSpeedTier(float speed)
    {
        if (speed <= 5000) return "평범함";
        else if (speed is > 5000 and <= 6000) return "훌륭함";
        else return "압도적임";
    }

    private string GetAccelTier(float accel)
    {
        if (accel <= 100) return "평범함";
        else if (accel is > 100 and <= 150) return "훌륭함";
        else return "압도적임";
    }
    
    private string GetFuelTier(float fuel)
    {
        if (fuel <= 900) return "평범함";
        else if (fuel is > 900 and <= 1100) return "훌륭함";
        else return "압도적임";
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