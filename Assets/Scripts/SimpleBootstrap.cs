using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using TMPro;

public class SimpleBootstrap : MonoBehaviour
{
    [SerializeField]
    int counter = 0;

    public TMP_Text textObject;
    string[] buttonNames = {"harmless button", "harmles buton", "hamless baton 😀", "harmlest button 😅", "hamless butt on? 😆", "start? 😂"};
    void Start()
    {
        textObject.text = buttonNames[counter];
    }

    void OnSceneEnd(MinigameScene.Outcome outcome)
    {
        Debug.LogFormat("Your highscore was: {0}", outcome.highscore);
    }

    // Update is called once per frame
    public void OnButtonPress()
    {
        counter++;
        if (counter > 5)
        {
            counter = 0;
            MinigameScene.LoadMinigameScene("MatchTwo", OnSceneEnd); 
        }
        textObject.text = buttonNames[counter];
    }
    public void OnButtonPress2()
    {
        MinigameScene.LoadMinigameScene("Rythmic_fisk", OnSceneEnd); 
    }
}
