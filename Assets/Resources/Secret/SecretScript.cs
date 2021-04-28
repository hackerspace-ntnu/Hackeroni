using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SecretScript : MonoBehaviour
{
    void Awake()
    {
        var hat = PlayerPrefs.GetString("CurrentHats", null);
        var color = PlayerPrefs.GetString("CurrentColors", null);
        var skin = PlayerPrefs.GetString("CurrentSkins", null);
        var wallpaper = PlayerPrefs.GetString("CurrentWallpapers", null);
        
        if (!hat.Contains("Forgotten_Spark") || color != "Chili" || !skin.Contains("Forgotten_Form") || !wallpaper.Contains("Forgotten_Place"))
        {
            var menu = transform.parent.parent.parent.GetComponent<MenuSwipeScript>();
            transform.SetParent(null);
            menu.OnRectTransformDimensionsChange();
            Destroy(gameObject);
        }
    }

    public void OnClick()
    {
        Screen.orientation = ScreenOrientation.Landscape;
        SceneManager.LoadSceneAsync("SecretEnding");
        if (SimpleBootstrap.theImmortalMusicSource != null)
            SimpleBootstrap.theImmortalMusicSource?.Pause();
    }
}
