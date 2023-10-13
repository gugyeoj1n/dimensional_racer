using System;
using System.Collections;
using System.Collections.Generic;
// using System.Numerics;
using UnityEngine;

public class AirplaneController : MonoBehaviour
{
    public float rollAmount;
    public float pitchAmount;
    public float yawAmount;
    public float speed;
    public float lerpAmount;

    Vector3 rotateValue;
    Rigidbody rb;

    float pitchValue;
    float yawValue;
    float rollValue;

    void MoveAircraft()
    {
        // Vector3 lerpVector = new Vector3(pitchValue * pitchAmount, yawValue * rollAmount, rollValue * rollAmount);
        Vector3 lerpVector = new Vector3(pitchValue * pitchAmount, yawValue * yawAmount, rollValue * rollAmount);
        rotateValue = Vector3.Lerp(rotateValue, lerpVector, lerpAmount * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotateValue * Time.fixedDeltaTime));

        rb.velocity = transform.forward * speed * Time.fixedDeltaTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() 
    {
        if (Input.GetButton("Pitch"))
            pitchValue = Input.GetAxisRaw("Pitch");
        else
            pitchValue = 0;

        if (Input.GetButton("Roll"))
            rollValue = Input.GetAxisRaw("Roll");
        else
            rollValue = 0;

        if (Input.GetButton("Yaw"))
            yawValue = Input.GetAxisRaw("Yaw");
        else
            yawValue = 0;

        MoveAircraft();

        // transform.Rotate(Vector3.up * Time.deltaTime * 3);
    }

}
