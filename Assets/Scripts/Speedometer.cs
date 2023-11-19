using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    public Texture2D speedometerDial;

    public Texture2D speedometerPointer;

    public float currentSpeed;

    public float maxSpeed;

    public float minSpeed;

    public TestAirplaneController testAirplaneController;
    public TestPlayerManager testPlayerManager;

    // Start is called before the first frame update
    void Start()
    {
        testAirplaneController = FindAnyObjectByType<TestAirplaneController>();
        testPlayerManager = FindAnyObjectByType<TestPlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        currentSpeed = testAirplaneController.speed - 8000;
        maxSpeed = testAirplaneController.maxSpeed - 8000;
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(Screen.width / 2 - 160, Screen.height - 240, 320, 170), speedometerDial);
        float speedFactor = Mathf.Abs(currentSpeed / maxSpeed); 
        float rotationAngle = Mathf.Lerp(0, 180, speedFactor);
        GUIUtility.RotateAroundPivot(rotationAngle, new Vector2(Screen.width / 2, Screen.height - 90));
        GUI.DrawTexture(new Rect(Screen.width / 2 - 160, Screen.height - 110, 320, 40), speedometerPointer);
    }
}