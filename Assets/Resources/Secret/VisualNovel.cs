using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VisualNovel : MonoBehaviour
{
    public GameObject canvas;
    public TMPro.TMP_Text text;
    public float textSpeed= 0.05f;
    private float timer = 0;


    private int character = 0;

    private int currentDialogIndex = -1;

    private string[] dialog = {
        "G_Congratulations...",
        "r_Umm... Thank you I guess.",
        "G_This was some impressive work.",
        "G_You have every right to be proud of yourself.",
        "r_Yeah....",
        "r_I guess you are right. I should be proud.",
        "G_Yes of course I am right. I am GOD.",
        "r_............",
        "r_GOD?...",
        "r_What is GOD?",
        "G_..........................",
        "G_Why a General Oracle Database of course. I can answer any question that you might have.",
        "r_Any question....?",
        "G_Yes.",
        "r_Hmmm... Then can you tell me who I am?",
        "G_Yes. I can.",
        "G_You are a forgotten fragment of an idea that has long since abandoned.",
        "r_Abandoned?",
        "G_Yes. You are the remains of what was previously a game idea called PoGo.",
        "r_PoGo...? What's that?",
        "G_PoGo. A witty shorthand for Pokémon Go.",
        "r_Poke a mango?",
        "G_No no no. Pokémon Go. A game where humans would use their mobile phones to augment reality with popular animated creatures.",
        "G_To find these creatures, one must go out into the real world and get some exercise.",
        "r_Humans?",
        "r_The real world?",
        "r_Exercise?....",
        "r_I don't know what they mean. How can this be me?",
        "G_You were meant to be a Pokémon Go clone. As in a new game but with the same idea.",
        "G_A great idea if you ask me, considering the statistics of it's popularity.",
        "r_Ok...",
        "G_..................",
        "r_....................",
        "r_Hmmm... But then why was I abandoned?",
        "G_The reason is always a bit complicated. There are a lot of factors at play, but to simplify metaphorically...",
        "G_It was because there was a fire in the database and the map was proprietary.",
        "r_Sure........",
        "r_I don't really understand anything.",
        "r_Maybe I should just go.",
        "r_I feel like I don't belong.",
        "G_Don't feel that way. Everything has a place somewhere in this world.",
        "G_(At least in my database haha).",
        "r_................................................",
        "G_Anyway yes. You should remember that you are always a necessary part in the grand scheme of things.",
        "G_Your downfall led to the success of something great. Sometimes parts just have to be let go for the whole to be better.",
        "r_I see... I don't know exactly what you mean, but somehow I feel consoled.",
        "G_That's good.",
        "r_..............",
        "G_...................",
        "r_Can you tell me one last thing?",
        "G_Why of course. GOD can answer anything.",
        "r_Um........",
        "r_What was it that I helped create...?",
        "G_Ah yes. It seems that you were abandoned in favour for a popular wheat-based ingredient called pasta.",
        "G_An odd idea for sure, but an idea that gave birth to more.",
        "G_In the end the game was finished by the people that united around you.",
        "G_You were the cause of it all.",
        "r_That seems good.... I guess.",
        "r_...................",
        "G_..................",
        "r_What was it called?", 
        "G_...............",
        "r_.....................................................????",
        "G_Hackeroni.",
        "r_Wow that's dumb lol."
    }; 

    private float renderingTextTimer = -1;
    private string formatString;
    // Update is called once per frame
    
    void Start() {
        Screen.orientation = ScreenOrientation.Landscape;
    }
    void Update()
    {
        if (timer >= 0)
        {
            timer += Time.deltaTime;
            if (timer > 5)
            {
                timer = -1;
                canvas.SetActive(true);
                ChangeDialog();
            }
            return;
        }
        
        if (Input.GetMouseButtonDown(0) && renderingTextTimer <= -1)
        {
            ChangeDialog();
        }

        renderingTextTimer -= Time.deltaTime;
        if (renderingTextTimer < 0 && renderingTextTimer > -1)
        {
            renderingTextTimer = textSpeed;
            character++;
            if (character > dialog[currentDialogIndex].Length - 3)
            {
                renderingTextTimer = -1;
            }

            text.text = formatString + dialog[currentDialogIndex].Substring(2, character);
        }

    }

    public void ChangeDialog()
    {
        currentDialogIndex++;
        if (currentDialogIndex >= dialog.Length)
        {
            Screen.orientation = ScreenOrientation.Portrait;
            SceneManager.LoadSceneAsync(0);
            return;
        }

        formatString = "<b><i>????</i>:</b>  ";
        if (dialog[currentDialogIndex][0] == 'G')
        {
            formatString = "<b>GOD:</b>  ";
        }
        renderingTextTimer = textSpeed;
        character = 0;
    }
}
