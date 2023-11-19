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

    public Transform playersContent;
    public GameObject playerStatus;
    public List<GameObject> playerStatusList;

    public Image item1;
    public Image item2;

    public AirplaneController airplaneController;
    public PlayerManager playerManager;

    private GameManager gameManager;
    private TestAirplaneController testAirplaneController;
    private TestPlayerManager testPlayerManager;
    public GameObject ItemIcon1;
    public GameObject ItemIcon2;
    public GameObject Fuel;
    
    private Color startColor = Color.green;
    private Color endColor = Color.red;
    private Color currentColor;
    
    public bool isShifting = false;
    public float currentFillAmount;
    
    void Start()
    {
        //testPlayerManager = FindObjectOfType<TestPlayerManager>();
        gameManager = FindObjectOfType<GameManager>();
        //testAirplaneController = FindObjectOfType<TestAirplaneController>();
        //item1 = ItemIcon1.GetComponent<Image>();
        //item2 = ItemIcon2.GetComponent<Image>();
        //fuelImage = Fuel.GetComponent<Image>();
    }

    public void InitPlayers(Dictionary<PlayerProperty, int>.KeyCollection target)
    {
        playerStatusList = new List<GameObject>();
        foreach (PlayerProperty player in target)
        {
            GameObject playerInst = Instantiate(playerStatus, playersContent);
            playerInst.transform.GetChild(0).GetComponent<TMP_Text>().text = player.name;
            playerStatusList.Add(playerInst);
        }
    }

    void Update()
    {
        if (!gameManager.isStarted) return;

        currentSpeedText.text = CalculateSpeed(airplaneController.speed).ToString();

        /*if (testAirplaneController.boosterAmount == 1)
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
        }*/

        if (gameManager.isStarted) // true -> gameManager.isStarted
        {
            int minutes = Mathf.FloorToInt(gameManager.time / 60);
            int seconds = Mathf.FloorToInt(gameManager.time % 60);
            float millisecondsFloat = (gameManager.time * 1000) % 1000;
            string millisecondsString = millisecondsFloat.ToString("000");
            timeText.text = "Time / " + minutes + " : " + seconds + " : " + millisecondsString;
        }
        
        //fuelImage.fillAmount = testPlayerManager.fuel / testPlayerManager.maxFuel;
        //fuelImage.color = Color.Lerp(endColor, startColor, fuelImage.fillAmount);

    }
    
    private int CalculateSpeed(float speed)
    {
        return (int)(speed * 0.0375f - 75f);
    }
}