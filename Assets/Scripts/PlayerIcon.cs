using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class PlayerIcon : MonoBehaviour
{
    private AccountManager accountManager;
    
    void Start()
    {
        accountManager = FindObjectOfType<AccountManager>();
    }

    public void SetPlayerIcon()
    {
        accountManager.UpdatePlayerIcon(GetComponent<Image>().sprite.name);
    }
}
