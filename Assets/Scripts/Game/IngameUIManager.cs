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
    private float curTime = 0f;

    public TMP_Text countText;

    public Image item1;
    public Image item2;

    public AirplaneController airplaneController;
    public PlayerManager playerManager;

    private GameManager gameManager;
    private TestAirplaneController testAirplaneController;
    public GameObject ItemIcon1;
    public GameObject ItemIcon2;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        testAirplaneController = FindObjectOfType<TestAirplaneController>();
        item1 = ItemIcon1.GetComponent<Image>();
        item2 = ItemIcon2.GetComponent<Image>();
    }

    void Update()
    {
        //if (!gameManager.isStarted) return;

        //currentSpeedText.text = CalculateSpeed(airplaneController.speed).ToString();

        if (testAirplaneController.boosterAmount == 1)
        {
            item1.enabled = true;
            item2.enabled = false;
        }
        else if (testAirplaneController.boosterAmount == 2)
        {
            item1.enabled = true;
            item2.enabled = true;
        }
        else
        {
            item1.enabled = false;
            item2.enabled = false;
        }

        if (true) // true -> gameManager.isStarted
        {
            curTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(curTime / 60);
            int seconds = Mathf.FloorToInt(curTime % 60);
            float millisecondsFloat = (curTime * 1000) % 1000;
            string millisecondsString = millisecondsFloat.ToString("000");
            timeText.text = "Time / " + minutes + " : " + seconds + " : " + millisecondsString;
        }

    }


    private int CalculateSpeed(float speed)
    {
        return (int)(speed * 0.0375f - 75f);
    }
}
