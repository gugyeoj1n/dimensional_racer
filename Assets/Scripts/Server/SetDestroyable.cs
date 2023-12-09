using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetDestroyable : MonoBehaviour
{
    public GameObject[] objects;

    void Awake()
    {
        foreach (GameObject target in objects)
            DontDestroyOnLoad(target);
        SceneManager.LoadScene(1);
    }
}
