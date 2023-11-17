using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
// using System.Numerics;
using UnityEngine;

public class TestAirplaneController : MonoBehaviourPunCallbacks
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
    public TestPlayerManager testPlayerManager;

    void MoveAircraft()
    {
        Vector3 lerpVector = new Vector3(pitchValue * pitchAmount, yawValue * yawAmount, -rollValue * rollAmount);
        rotateValue = Vector3.Lerp(rotateValue, lerpVector, lerpAmount * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotateValue * Time.fixedDeltaTime));

        rb.velocity = transform.forward * speed * Time.fixedDeltaTime;
    }

    void Start()
    {
        testPlayerManager = FindObjectOfType<TestPlayerManager>();
        rb = GetComponent<Rigidbody>();
        previousPosition = transform.position;
        previousRotation = transform.rotation;
        minSpeed = speed;
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
                speed += testPlayerManager.acceleration * 15f * Time.deltaTime;
            }
            boosterAmount--;
        }
    }
    
}