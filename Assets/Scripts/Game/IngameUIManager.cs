using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon;
using Photon.Pun;

public class IngameUIManager : MonoBehaviourPunCallbacks
{
    public TMP_Text currentSpeedText;
    public TMP_Text currentStageText;
    public TMP_Text maxStageText;
    public Image fuelImage;
    public TMP_Text timeText;

    public Image item1;
    public Image item2;

    public AirplaneController airplaneController;
    public PlayerManager playerManager;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
