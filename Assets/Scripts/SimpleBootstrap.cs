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
    void Start()
    {
    }

    void OnSceneEnd(MinigameScene.Outcome outcome)
    {
        gameLaunched = false;
        Debug.LogFormat("Your highscore was: {0}", outcome.highscore);
    }

    // Update is called once per frame
    public void OnButtonPress(int buttonIndex)
    {
        if (!gameLaunched)
        {
            gameLaunched = true;

            switch (buttonIndex)
            {
                case 0:
                    MinigameScene.LoadMinigameScene("MatchTwo", OnSceneEnd); 
                    break;
                case 1:
                    MinigameScene.LoadMinigameScene("Rythmic_fisk", OnSceneEnd); 
                    break;
                // TODO Add new games here as they are made
                case 2:
                    // MinigameScene.LoadMinigameScene("MatchTwo", OnSceneEnd); 
                    break;
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
                    break;
            }           
        }
    }
}
