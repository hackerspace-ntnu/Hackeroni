using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
using TMPro;

public class SimpleBootstrap : MonoBehaviour
{
    public TMP_Text hackeroniText;

    private bool gameLaunched = false;
    private int launchedGameId = 0;
    
    private int hackeronisBeforeMinigameIsLaunched = 0;

    void Start()
    {
        hackeroniText.text = PlayerPrefs.GetInt("Hackeronis", 0).ToString();
    }

    void OnSceneEnd()
    {
        gameLaunched = false;
        Screen.orientation = ScreenOrientation.Portrait;
        StartCoroutine(HackeroniEarnedAnimation());
    }

    // Update is called once per frame
    public void OnButtonPress(int buttonIndex)
    {
        if (!gameLaunched)
        {
            gameLaunched = true;
            launchedGameId = buttonIndex;
            hackeronisBeforeMinigameIsLaunched = PlayerPrefs.GetInt("Hackeronis", 0);

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
                case 3:
                    MinigameScene.LoadMinigameScene("TiltyHacker", OnSceneEnd); 
                    break;
                // TODO Add new games here as they are made
                case 4:
                    MinigameScene.LoadMinigameScene("Soup_falls", OnSceneEnd); 
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
    
    public IEnumerator HackeroniEarnedAnimation()
    {
        var previousHackeronis = hackeronisBeforeMinigameIsLaunched;
        var currentHackeronis = PlayerPrefs.GetInt("Hackeronis", 0);

        if(previousHackeronis >= currentHackeronis) {
            //Special case maybe?
            hackeroniText.text = currentHackeronis.ToString(); 
            yield break;
        }

        float animationTime = 1.5f;
        float timer = 0;

        while(timer <= animationTime)
        {
            int intermediateValue = Mathf.RoundToInt((currentHackeronis - previousHackeronis) * timer / animationTime + previousHackeronis);
            hackeroniText.text = intermediateValue.ToString(); 
            timer += Time.deltaTime;
            yield return null;
        }

        hackeroniText.text = currentHackeronis.ToString(); 
        hackeronisBeforeMinigameIsLaunched = previousHackeronis;
    }
}
