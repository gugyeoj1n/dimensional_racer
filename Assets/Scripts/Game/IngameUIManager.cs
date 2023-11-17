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

    public TMP_Text countText;

    public Image item1;
    public Image item2;

    public AirplaneController airplaneController;
    public PlayerManager playerManager;

    private GameManager gameManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (!gameManager.isStarted) return;

        currentSpeedText.text = CalculateSpeed(airplaneController.speed).ToString();
    }


    private int CalculateSpeed(float speed)
    {
        return (int)(speed * 0.0375f - 75f);
    }
}
