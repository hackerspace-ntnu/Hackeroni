using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
using TMPro;

public class SimpleBootstrap : MonoBehaviour
{
    public TMP_Text textObject;

    private bool gameLaunched = false;
    private int launchedGameId = 0;
    void Start()
    {
    }

    void OnSceneEnd(MinigameScene.Outcome outcome)
    {
        gameLaunched = false;
        var key = "highscore"+launchedGameId;
        int prevHighScore = PlayerPrefs.GetInt(key, 0);
        PlayerPrefs.SetInt(key, Mathf.Max(outcome.highscore, prevHighScore));
        if (outcome.highscore > prevHighScore)
        {
            Debug.LogFormat("You got a new highscore: {0}", outcome.highscore);
        }
    }

    // Update is called once per frame
    public void OnButtonPress(int buttonIndex)
    {
        if (!gameLaunched)
        {
            gameLaunched = true;
            launchedGameId = buttonIndex;

            switch (buttonIndex)
            {
                case 0:
                    MinigameScene.LoadMinigameScene("MatchTwo", OnSceneEnd); 
                    break;
                case 1:
                    MinigameScene.LoadMinigameScene("Rythmic_fisk", OnSceneEnd); 
                    break;
                case 2:
                    MinigameScene.LoadMinigameScene("TrickyTrumpTest", OnSceneEnd); 
                    break;
                // TODO Add new games here as they are made
                case 3:
                    // MinigameScene.LoadMinigameScene("Rythmic_fisk", OnSceneEnd); 
                    break;
                case 4:
                    // MinigameScene.LoadMinigameScene("MatchTwo", OnSceneEnd); 
                    break;
                case 5:
                    // MinigameScene.LoadMinigameScene("Rythmic_fisk", OnSceneEnd); 
                    break;
                default:
                    gameLaunched = false;
                    launchedGameId = 0;
                    break;
            }           
        }
    }
}
