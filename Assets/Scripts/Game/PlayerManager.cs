using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    private AirplaneController airplaneController;
    private GameManager gameManager;
    public string playerId;

    public float maxFuel;
    public float fuel;
    public float acceleration;
    
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        airplaneController = GetComponent<AirplaneController>();

        fuel = maxFuel;
    }

    void Update()
    {
        if (!photonView.IsMine || !gameManager.isStarted) return;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Accelerate();
        }
        else
        {
            Decelerate();
        }
    }

    public void StartCamera()
    {
        CameraFollow cam = GetComponent<CameraFollow>();
        cam.StartFollowing();
    }

    private void Accelerate()
    {
        if (fuel <= 0 || airplaneController.speed >= airplaneController.maxSpeed)
            return;

        fuel -= acceleration * 0.5f * Time.deltaTime;
        airplaneController.speed += acceleration * 10f * Time.deltaTime;
    }

    private void Decelerate()
    {
        if (airplaneController.speed <= airplaneController.minSpeed)
            return;

        airplaneController.speed -= 100f * Time.deltaTime;
    }
}
