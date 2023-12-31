using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using UnityEngine.SceneManagement;

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
    public GameObject ItemIcon1;
    public GameObject ItemIcon2;
    public GameObject Fuel;

    public GameObject settlePanel;
    public GameObject[] sequencePanels;
    public TMP_Text winnerNameText;
    public TMP_Text winnerTimeText;
    public TMP_Text coinAddText;
    public TMP_Text ratingText;
    
    private Color startColor = Color.green;
    private Color endColor = Color.red;
    
    public bool isShifting = false;
    public float currentFillAmount;

    public GameObject loadingPanel;
    
    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        gameManager = FindObjectOfType<GameManager>();
        airplaneController = FindObjectOfType<AirplaneController>();
        item1 = ItemIcon1.GetComponent<Image>();
        item2 = ItemIcon2.GetComponent<Image>();
        fuelImage = Fuel.GetComponent<Image>();
    }

    public void SetLoadingPanel(bool target)
    {
        loadingPanel.SetActive(target);
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
    
    public void SetSettlePanel(string winnerName, string winnerTime, int coin, int rating)
    {
        settlePanel.SetActive(true);
        winnerNameText.text = winnerName;
        winnerTimeText.text = winnerTime;
        coinAddText.text = string.Format("+ {0} Coins", coin);
        ratingText.text = string.Format("{0} {1} Rank Points", (rating > 0) ? "+" : "-", Mathf.Abs(rating));
    }

    public void SetSequencePanel(List<int> sequence, Dictionary<PlayerProperty, int> players)
    {
        int i = 0;
        foreach (int id in sequence)
        {
            Debug.Log("정산 ID : " + id);
            var sequenceObject = sequencePanels[i];
            string status = "";
            foreach (KeyValuePair<PlayerProperty, int> kvp in players)
            {
                if (kvp.Value == id)
                {
                    sequenceObject.transform.GetChild(1).GetComponent<TMP_Text>().text =
                        string.Format("{0} ({1})", kvp.Key.name, kvp.Key.client);
                }
            }

            i++;
        }
    }

    void Update()
    {
        if (!gameManager.isStarted) return;

        currentSpeedText.text = CalculateSpeed(airplaneController.speed).ToString();

        if (airplaneController.boosterAmount == 1)
        {
            item1.enabled = true;
            item2.enabled = false;
        }
        else if (airplaneController.boosterAmount == 2)
        {
            item1.enabled = true;
            item2.enabled = true;
        }
        else
        {
            item1.enabled = false;
            item2.enabled = false;
        }

        if (gameManager.isStarted) // true -> gameManager.isStarted
        {
            int minutes = Mathf.FloorToInt(gameManager.time / 60);
            int seconds = Mathf.FloorToInt(gameManager.time % 60);
            float millisecondsFloat = (gameManager.time * 1000) % 1000;
            string millisecondsString = millisecondsFloat.ToString("000");
            timeText.text = "Time / " + minutes + " : " + seconds + " : " + millisecondsString;
        }
        
        fuelImage.fillAmount = playerManager.fuel / playerManager.maxFuel;
        fuelImage.color = Color.Lerp(endColor, startColor, fuelImage.fillAmount);

    }

    public void BackToLobby()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(2);
    }

    private int CalculateSpeed(float speed)
    {
        return (int)(speed * 0.0375f - 75f);
    }
}