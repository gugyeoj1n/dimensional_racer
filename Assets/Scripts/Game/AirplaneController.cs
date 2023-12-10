using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
// using System.Numerics;
using UnityEngine;

public class AirplaneController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private float rollAmount;
    [SerializeField]
    private float pitchAmount;
    [SerializeField]
    private float yawAmount;
    public float speed;
    public float minSpeed;
    public float maxSpeed;
    public int boosterAmount;
    [SerializeField]
    private float lerpAmount;

    private Vector3 rotateValue;
    private Rigidbody rb;

    private float pitchValue;
    private float yawValue;
    private float rollValue;

    public float currentTime;
    public float previousTime;
    public Vector3 previousPosition;
    public Quaternion previousRotation;
    public PlayerManager playerManager;
    
    public bool isOnWall;

    public AudioClip[] audios;
    private AudioSource audio;

    void MoveAircraft()
    {
        Vector3 lerpVector = new Vector3(pitchValue * pitchAmount, yawValue * yawAmount, -rollValue * rollAmount);
        rotateValue = Vector3.Lerp(rotateValue, lerpVector, lerpAmount * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotateValue * Time.fixedDeltaTime));

        rb.velocity = transform.forward * speed * Time.fixedDeltaTime;
    }

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        rb = GetComponent<Rigidbody>();
        previousPosition = transform.position;
        previousRotation = transform.rotation;
        minSpeed = speed;

        audio = transform.GetChild(0).GetComponent<AudioSource>();
    }

    public void Back()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();

        transform.position = previousPosition;
        transform.rotation = previousRotation;

        rb.WakeUp();
    }
    
    public void OnWall(){
        speed = 2000f;
    }
    
    public void LeftWall(){
        speed = minSpeed;
    }

    void Update()
    {
        
        currentTime = Time.time;
        if (currentTime - previousTime >= 10f)
        {
            previousPosition = transform.position;
            previousRotation = transform.rotation;
            previousTime = currentTime;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Back();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Boost();
            audio.clip = audios[0];
            audio.Play();
        } else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            audio.Stop();
        }
    }


    void FixedUpdate()
    {
        pitchValue = Input.GetAxisRaw("Vertical");
        rollValue = Input.GetAxisRaw("Roll");
        yawValue = Input.GetAxisRaw("Horizontal");

        MoveAircraft();
    }

    void Boost()
    {
        float curSpeed = speed;
        if (boosterAmount > 0)
        {
            while (speed < maxSpeed)
            {
                speed += playerManager.acceleration * 15f * Time.deltaTime;
            }
            boosterAmount--;
        }
    }

}