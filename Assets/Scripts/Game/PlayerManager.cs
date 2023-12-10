using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    private AirplaneController airplaneController;
    public string playerId;

    public float maxFuel;
    public float fuel;
    public float acceleration;

    public AudioClip[] audios;
    private AudioSource audio;

    public ParticleSystem[] particles;

    void Start()
    {
        airplaneController = GetComponent<AirplaneController>();
        particles = GetComponentsInChildren<ParticleSystem>();
        audio = GetComponent<AudioSource>();

        fuel = maxFuel;
    }

    public void InitParticles()
    {
        foreach (ParticleSystem particle in particles)
            particle.Play();

        audio.clip = audios[0];
        audio.Play();
    }

    void Update()
    {
        if (!photonView.IsMine) return;
        
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

        fuel -= acceleration * 5f * Time.deltaTime;
        airplaneController.speed += acceleration * 10f * Time.deltaTime;
        SetParticles(true);
    }

    public void SetParticles(bool up)
    {
        foreach (ParticleSystem particle in particles)
        {
            ParticleSystem.MainModule main = particle.main;
            main.startSpeed = new ParticleSystem.MinMaxCurve(up ? 40f : 20f);
        }
    }

    private void Decelerate()
    {
        if (airplaneController.speed <= airplaneController.minSpeed)
            return;

        if (fuel <= maxFuel) fuel += acceleration * 5f * Time.deltaTime;
        airplaneController.speed -= 500f * Time.deltaTime;
        SetParticles(false);
    }
}