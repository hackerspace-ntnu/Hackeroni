using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallpaperScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var image = gameObject.GetComponentInChildren<Image>();
        image.sprite = PlayerPrefManager.GetCurrentWallpaperSprite();
    }
}
