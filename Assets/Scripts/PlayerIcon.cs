using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIcon : MonoBehaviour
{
    private AccountManager accountManager;
    
    void Start()
    {
        accountManager = FindObjectOfType<AccountManager>();
    }

    public void SetPlayerIcon()
    {
        accountManager.UpdatePlayerIcon(GetSpriteFileName(GetComponent<Image>().sprite));
    }
    
    string GetSpriteFileName(Sprite sprite)
    {
        string path = UnityEditor.AssetDatabase.GetAssetPath(sprite.texture);
        return System.IO.Path.GetFileNameWithoutExtension(path);
    }
}
