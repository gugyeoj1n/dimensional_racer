using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public float masterVolume;
    public bool isWindowed;
    
    void Start()
    {
        LoadSettings();
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        PlayerPrefs.SetFloat("masterVolume", value);
        
        AudioSource[] audios = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in audios)
        {
            audio.volume = masterVolume;
        }
    }
    
    public void SetWindowed(bool value)
    {
        isWindowed = value;
        PlayerPrefs.SetString("isWindowed", (value) ? "true" : "false");
    }

    public void SaveSettings(float master, bool window)
    {
        SetMasterVolume(master);
        SetWindowed(window);

        Screen.fullScreen = !window;
    }

    public void LoadSettings()
    {
        masterVolume = (PlayerPrefs.HasKey("masterVolume")) ? PlayerPrefs.GetFloat("masterVolume") : 1f;
        SetMasterVolume(masterVolume);

        if (PlayerPrefs.HasKey("isWindowed"))
            isWindowed = PlayerPrefs.GetString("isWindowed") == "true" ? true : false;
        else
        {
            isWindowed = false;
            SetWindowed(false);
        }
    }
}
