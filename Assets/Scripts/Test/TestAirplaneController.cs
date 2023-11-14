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
    [SerializeField]
    private float lerpAmount;

    private Vector3 rotateValue;
    private Rigidbody rb;

    private float pitchValue;
    private float yawValue;
    private float rollValue;

    void MoveAircraft()
    {
        Vector3 lerpVector = new Vector3(pitchValue * pitchAmount, yawValue * yawAmount, -rollValue * rollAmount);
        rotateValue = Vector3.Lerp(rotateValue, lerpVector, lerpAmount * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotateValue * Time.fixedDeltaTime));

        rb.velocity = transform.forward * speed * Time.fixedDeltaTime;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        minSpeed = speed;
    }

    void FixedUpdate()
    {
        
        pitchValue = Input.GetAxisRaw("Vertical");
        rollValue = Input.GetAxisRaw("Roll");
        yawValue = Input.GetAxisRaw("Horizontal");
        
        MoveAircraft();
    }

}