using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Unity.VisualScripting;

public class GameManager : MonoBehaviourPunCallbacks
{
    // 게임 씬에서 필요한 기능
    // 위치 재설정, 포톤 동기화, 플레이팹 카탈로그에서 자동차 아이템 가져오기, 게임 종료 후 점수 정산
    
    // 방에 다 들어오면 (캐릭터들 다 생성되면) 3 2 1 하고 시작해
    // 1등이 들어오면 10초 세고 끝 못들어오면 리타이어
    // 정산하고 로비로 복귀

    public bool isStarted = false;
    public float time = 300f;

    public GameObject playerPrefab;
    
    public void Start()
    {
        Debug.Log("현재 룸 인원수 : " + PhotonNetwork.CurrentRoom.PlayerCount);

        Vector3 startPos = Vector3.up + Vector3.right * PhotonNetwork.CurrentRoom.PlayerCount;

        PhotonNetwork.Instantiate(playerPrefab.name, startPos, Quaternion.identity, 0);
    }

    public void StartGame()
    {
        isStarted = true;
        StartCoroutine(CountStart());
    }

    IEnumerator CountStart()
    {
        Debug.Log("3");
        yield return new WaitForSeconds(1f);
        Debug.Log("2");
        yield return new WaitForSeconds(1f);
        Debug.Log("1");
        yield return new WaitForSeconds(1f);
        Debug.Log("GAME START");
        UnlockInput();
    }

    private void UnlockInput()
    {
        PlayerManager[] players = FindObjectsOfType<PlayerManager>();
        foreach (PlayerManager player in players)
        {
            if (player.GameObject().GetPhotonView().IsMine)
                player.GameObject().GetComponent<AirplaneController>().enabled = true;
        }
    }

    void Update()
    {
        //CountTime();
    }

    public void CountTime()
    {
        if(time >= 0f)
        {
            time -= Time.deltaTime;
            Debug.Log(time);
            // 여기는 UI Manager로 넘기기
            //timeText.text = Mathf.Floor(time / 60) + ":" + Mathf.Floor(time % 60);
        }
    }
    
    IEnumerator FinishCount()
    {
        int cnt = 10;
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log(cnt);
            cnt--;
        }
        yield return new WaitForSeconds(1f);
        Debug.Log("FINISHED");
    }
}
