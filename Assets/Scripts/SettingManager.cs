using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public float masterVolume;
    public float backgroundVolume;
    public float effectVolume;
    public bool isWindowed;
    
    void Start()
    {
        LoadSettings();
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        PlayerPrefs.SetFloat("masterVolume", value);
    }
    
    public void SetBackgroundVolume(float value)
    {
        backgroundVolume = value;
        PlayerPrefs.SetFloat("backgroundVolume", value);
    }
    
    public void SetEffectVolume(float value)
    {
        effectVolume = value;
        PlayerPrefs.SetFloat("effectVolume", value);
    }
    
    public void SetWindowed(bool value)
    {
        isWindowed = value;
        PlayerPrefs.SetString("isWindowed", (value) ? "true" : "false");
    }

    public void SaveSettings(float master, float back, float effect, bool window)
    {
        SetMasterVolume(master);
        SetBackgroundVolume(back);
        SetEffectVolume(effect);
        SetWindowed(window);

        Screen.fullScreen = !window;
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("masterVolume"))
            masterVolume = PlayerPrefs.GetFloat("masterVolume");
        else
        {
            masterVolume = 1f;
            SetMasterVolume(masterVolume);
        }
        
        if (PlayerPrefs.HasKey("backgroundVolume"))
            backgroundVolume = PlayerPrefs.GetFloat("backgroundVolume");
        else
        {
            backgroundVolume = 1f;
            SetBackgroundVolume(backgroundVolume);
        }
        
        if (PlayerPrefs.HasKey("effectVolume"))
            effectVolume = PlayerPrefs.GetFloat("effectVolume");
        else
        {
            effectVolume = 1f;
            SetEffectVolume(effectVolume);
        }
        
        if (PlayerPrefs.HasKey("masterVolume"))
            isWindowed = PlayerPrefs.GetString("isWindowed") == "true" ? true : false;
        else
        {
            isWindowed = false;
            SetWindowed(false);
        }
    }
}
