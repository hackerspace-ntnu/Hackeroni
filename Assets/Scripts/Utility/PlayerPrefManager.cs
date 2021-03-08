using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefManager 
{
    //Returns:
    // bool: Whether or not this was a new highscore
    // int:  The current highscore (can be the new score)
    public static Tuple<bool, int> GetAndOrUpdateHighscore(string minigameId, int newScore)
    {
        bool isNewHighscore = false;
        var key = "Highscore/" + minigameId;
        var oldHighscore = PlayerPrefs.GetInt(key, -1);
        if (oldHighscore == -1 || newScore > oldHighscore)
        {
            isNewHighscore = true;
            oldHighscore = newScore;
            PlayerPrefs.SetInt(key, newScore);
        }
        return new Tuple<bool, int>(isNewHighscore, oldHighscore);
    }
    

    public static void AddEarnedHackeronis(int hackeronisEarned)
    {
        var previousHackeronis = PlayerPrefs.GetInt("Hackeronis", 0);
        PlayerPrefs.SetInt("Hackeronis", previousHackeronis + hackeronisEarned);
    }
    
    public static Sprite GetCurrentWallpaperSprite()
    {
        var path = PlayerPrefs.GetString("CurrentWallpapers", "Wallpapers/Default-0");
        return Resources.Load<Sprite>(path);
    }

    public static Sprite GetCurrentSkinSprite()
    {
        var path = PlayerPrefs.GetString("CurrentSkins", "Skins/Default-0");
        return Resources.Load<Sprite>(path);
    }

    public static Color GetCurrentColor()
    {
        var ColorName = PlayerPrefs.GetString("CurrentColors", null);
        foreach (var color in ShopEngineScript.ColorList)
        {
            if (color.Name == ColorName)
            {
                return color.Color; 
            }
        }
        return ShopEngineScript.ColorList[0].Color;
    }
    public static Sprite GetCurrentHatSprite()
    {
        var path = PlayerPrefs.GetString("CurrentHats", null);
        if (path == null) return null;
        return Resources.Load<Sprite>(path);
    }
}
