﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts;

public class Engine : MonoBehaviour
{
    public GameObject Board;
    public GameObject ButtonPrefab;
    public List<GameObject> BackgroundBits = new List<GameObject>();
    public GameObject EndScreen;
    public GameObject Particles;
    public GameObject Initiate;
    
    public AudioClip InitialClickSound;
    public AudioClip SuccessClickSound;
    public AudioClip FailClickSound;
    public AudioClip PastaClickSound;
    public AudioClip CompletedSound;
    
    private AudioSource audioSource;

    private List<GameObject> Buttons = new List<GameObject>();

    private float BoardAnimationTimer;
    private Vector3 BoardRotationOld;
    private Vector3 BoardRotationNew;

    private GameObject Guess1;
    private GameObject Guess2;

    private float WrongTime;
    private float RevealTime;
    private float RevealSpace;

    private float StartGameTimer;

    private List<Color32> BoardColors = new List<Color32>();

    private int Fails;
    private Vector2 Direction;

    private bool ButtonsEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();   
        EndScreen.SetActive(false);
        Initiate.GetComponent<Button>().onClick.AddListener(InitiateOnClick);

        Direction = new Vector2(-1, -1);
    }

    void InitiateOnClick()
    {
        GameStart();
        Initiate.SetActive(false);
    }

    void GameStart()
    {
        RevealSpace = 1f;

        GameObject CenterButton = null;
        
        for (int i = 0; i < 25; i++)
        {
            GameObject ButtonPref = Instantiate(ButtonPrefab);
            ButtonPref.transform.SetParent(Board.transform);
            ButtonPref.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            ButtonPref.GetComponent<RectTransform>().anchoredPosition = new Vector2(((i % 5) * 120) + 80, -(Mathf.FloorToInt(i / 5) * 120) - 80);
            ButtonPref.GetComponent<ButtonScript>().setCoordinates(i % 5, (4 - Mathf.FloorToInt(i / 5)));
            Buttons.Add(ButtonPref);

            if(i == 12)
            {
                CenterButton = ButtonPref;
            }
        }

        List<GameObject> MatchingList = new List<GameObject>(Buttons);
        int MatchNum = 1;

        while(MatchingList.Count > 0)
        {
            GameObject Match1 = MatchingList[Random.Range(0, MatchingList.Count)];
            if(MatchNum == 1)
            {
                Match1 = CenterButton;
            }
            MatchingList.Remove(Match1);
              

            if (MatchingList.Count > 0)
            {

                GameObject Match2 = MatchingList[Random.Range(0, MatchingList.Count)];
                MatchingList.Remove(Match2);

                Match1.GetComponent<ButtonScript>().setMatch(Match2);
                Match2.GetComponent<ButtonScript>().setMatch(Match1);
                Match1.GetComponent<ButtonScript>().setMatchNumber(MatchNum);
                Match2.GetComponent<ButtonScript>().setMatchNumber(MatchNum);
                Match1.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Minigames/MatchTwo/" + MatchNum.ToString());
                Match2.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Minigames/MatchTwo/" + MatchNum.ToString());

                MatchNum += 1;
            }
            else
            {
                //Instant kill button (pasta bomb)
                var pastaSprite = Match1.transform.GetChild(0).GetComponent<Image>();
                pastaSprite.sprite = PlayerPrefManager.GetCurrentSkinSprite();
                pastaSprite.color = PlayerPrefManager.GetCurrentColor();
                Match1.GetComponent<ButtonScript>().IsPastaBomb = true;
                Match1.GetComponent<Image>().color = new Color(1,1,1,0);
            }
        }

        for(int i = 0; i < 3; i++)
        {
            bool Repeat = false;
            do
            {
                Repeat = false;
                List<int> StartColor = new List<int>(new int[3] { 255, 0, Random.Range(0, 256)});
                List<int> JumbleColor = new List<int>();

                while(StartColor.Count > 0)
                {
                    int RandomIndex = Random.Range(0, StartColor.Count);
                    JumbleColor.Add(StartColor[RandomIndex]);
                    StartColor.RemoveAt(RandomIndex);
                }

                Color32 newColor = new Color32((byte)JumbleColor[0], (byte)JumbleColor[1], (byte)JumbleColor[2], 255);

                foreach(Color32 Color in BoardColors)
                {
                    if(Vector3.Angle(new Vector3(Color.r, Color.g, Color.b), new Vector3(newColor.r, newColor.g, newColor.b)) < 45)
                    {
                        Repeat = true;
                    }
                }

                if(Repeat == false)
                {
                    BoardColors.Add(newColor);
                }
            }
            while (Repeat == true);
        }

        BackgroundBits[0].GetComponent<Image>().color = ColorCalculator(0.2f, 0, 1, 255);
        BackgroundBits[1].GetComponent<Image>().color = ColorCalculator(0.3f, 0, 1, 255);
        BackgroundBits[2].GetComponent<Image>().color = ColorCalculator(0.4f, 0, 1, 255);
        BackgroundBits[3].GetComponent<Image>().color = ColorCalculator(0.9f, 0, 1, 255);
        BackgroundBits[4].GetComponent<Image>().color = ColorCalculator(0.7f, 0, 1, 255);

        BackgroundBits[5].GetComponent<Image>().color = ColorCalculator(1f, 0, 1, 100);
        BackgroundBits[6].GetComponent<Image>().color = ColorCalculator(0.9f, 0, 1, 100);
        BackgroundBits[7].GetComponent<Image>().color = ColorCalculator(0.1f, 0, 1, 100);
        BackgroundBits[8].GetComponent<Image>().color = ColorCalculator(0f, 0, 1, 100);

        BackgroundBits[9].GetComponent<Image>().color = ColorCalculator(1f, 0, 1, 150);
        BackgroundBits[10].GetComponent<Image>().color = ColorCalculator(0f, 0, 1, 150);

        var HSparticlesCOL = Particles.transform.GetChild(0).GetComponent<ParticleSystem>().colorOverLifetime;
        Gradient newGradient1 = new Gradient();
        Gradient newGradient2 = new Gradient();
        newGradient1.SetKeys(new GradientColorKey[] { new GradientColorKey(BoardColors[0], 0f) }, new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 0.4f), new GradientAlphaKey(1f, 0.6f), new GradientAlphaKey(0f, 1f) });
        newGradient2.SetKeys(new GradientColorKey[] { new GradientColorKey(BoardColors[1], 0f) }, new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 0.4f), new GradientAlphaKey(1f, 0.6f), new GradientAlphaKey(0f, 1f) });

        HSparticlesCOL.color = new ParticleSystem.MinMaxGradient(newGradient1, newGradient2);


        foreach (GameObject Button in Buttons)
        {
            if(Button.GetComponent<ButtonScript>().IsPastaBomb)
            {
                continue;
            }
            Button.transform.GetChild(0).GetComponent<Image>().color = ColorCalculator((float)Button.GetComponent<ButtonScript>().getMatchNumber()/12f, Button.GetComponent<ButtonScript>().getMatchNumber() % 2, 2, 255);
        }

        EnableButtons(false);
        StartGameTimer = 3f;
    }

    public Color32 ColorCalculator(float Percentage, int Color1, int Color2, int Alpha)
    {
        Color32 Color = new Color32((byte)(BoardColors[Color1].r + ((BoardColors[Color2].r - BoardColors[Color1].r) * Percentage)),
                                    (byte)(BoardColors[Color1].g + ((BoardColors[Color2].g - BoardColors[Color1].g) * Percentage)),
                                    (byte)(BoardColors[Color1].b + ((BoardColors[Color2].b - BoardColors[Color1].b) * Percentage)), (byte)Alpha);

        return Color;
    }

    void ChangeBoard()
    {
        int ChangeTypeChance = Random.Range(0, 5);

        BoardAnimationTimer = 1f;
        EnableButtons(false);

        BoardRotationOld = Board.GetComponent<RectTransform>().eulerAngles;
        if (ChangeTypeChance <= 2)
        {
            BoardRotationNew = Board.GetComponent<RectTransform>().eulerAngles + new Vector3(0f, 0f, 90f);
        }
        else if (ChangeTypeChance == 3)
        {
            BoardRotationNew = Board.GetComponent<RectTransform>().eulerAngles + new Vector3(0f, 180f, 0f);
        }
        else if (ChangeTypeChance == 4)
        {
            BoardRotationNew = Board.GetComponent<RectTransform>().eulerAngles + new Vector3(180f, 0f, 0f);
        }
    }

    void ChangeBoardAnimation()
    {
        if(BoardAnimationTimer > 0)
        {
            BoardAnimationTimer -= (Time.deltaTime * 2);
            Board.GetComponent<RectTransform>().eulerAngles = BoardRotationOld + ((BoardRotationNew - BoardRotationOld) * (1f - BoardAnimationTimer));

            foreach (GameObject Button in Buttons)
            {
                Button.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 0);
            }
        }
        else if(BoardAnimationTimer < 0)
        {
            Board.GetComponent<RectTransform>().eulerAngles = new Vector3(
                Mathf.RoundToInt(BoardRotationNew.x / 90) * 90,
                Mathf.RoundToInt(BoardRotationNew.y / 90) * 90,
                Mathf.RoundToInt(BoardRotationNew.z / 90) * 90);

            foreach (GameObject Button in Buttons)
            {
                Button.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 0);
            }

            BoardAnimationTimer = 0;

            EnableButtons(true);
        }
    }

    public void OnClick(GameObject Button)
    {
        if (ButtonsEnabled == false)
        {
            return;
        }
        if (Button.GetComponent<ButtonScript>().IsPastaBomb) {
            //Is macaroni kill button
            audioSource.PlayOneShot(PastaClickSound);
            ButtonsEnabled = false;
            StartCoroutine(StartGameOverSequence()); 
            return;
        }
        if(Guess1 == null)
        {
            audioSource.PlayOneShot(InitialClickSound);
            Guess1 = Button;
            Button.GetComponent<ButtonScript>().Reveal(true);
        }
        else if(Guess2 == null && Button != Guess1)
        {
            Guess2 = Button;
            Button.GetComponent<ButtonScript>().Reveal(true);

            if (Guess1.GetComponent<ButtonScript>().getMatchNumber() ==
               Guess2.GetComponent<ButtonScript>().getMatchNumber())
            {
                audioSource.PlayOneShot(SuccessClickSound);
                ChangeBoard();

                Guess1.GetComponent<ButtonScript>().setComplete(true);
                Guess2.GetComponent<ButtonScript>().setComplete(true);
                
                Guess1 = null;
                Guess2 = null;

                CheckCompletion();
            }
            else
            {
                audioSource.PlayOneShot(FailClickSound);
                WrongTime = 0.5f;
                Fails += 1;
                EnableButtons(false);
            }
        }
    }

    void WrongTimer()
    {
        if(WrongTime > 0)
        {
            WrongTime -= Time.deltaTime;
        }
        else if(WrongTime < 0)
        {
            WrongTime = 0;

            RevealTime = 1f + RevealSpace;

            EnableButtons(false);
        }
    }

    void EnableButtons(bool Bool)
    {
        ButtonsEnabled = Bool;
    }

    void RevealBoardAnimation()
    {
        if(RevealTime > -RevealSpace)
        {
            RevealTime -= Time.deltaTime;

            for(int i = 0; i < Buttons.Count; i++)
            {
                if((float)i / (float)Buttons.Count > (RevealTime / 1f) - RevealSpace &&
                   (float)i / (float)Buttons.Count < (RevealTime / 1f) + RevealSpace)
                {
                    Buttons[i].GetComponent<ButtonScript>().Reveal(true);
                }
                else if(Buttons[i].GetComponent<ButtonScript>().getComplete() == false && Buttons[i] != Guess1 && Buttons[i] != Guess2)
                {
                    Buttons[i].GetComponent<ButtonScript>().Reveal(false);
                } 
                else
                {
                    Buttons[i].GetComponent<ButtonScript>().Reveal(true);
                }
            }
        }
        else if(RevealTime < -RevealSpace)
        {
            RevealTime = -RevealSpace;

            Guess1 = null;
            Guess2 = null;
            EnableButtons(true);

            for (int i = 0; i < Buttons.Count; i++)
            {
                if(Buttons[i].GetComponent<ButtonScript>().getComplete() == false)
                    Buttons[i].GetComponent<ButtonScript>().Reveal(false);
            }
        }
    }

    void CheckCompletion()
    {
        bool Completed = true;

        foreach(GameObject Button in Buttons)
        {
            var bscript = Button.GetComponent<ButtonScript>();
            if(bscript.getComplete() == false && bscript.IsPastaBomb == false)
            {
                Completed = false;
                break;
            }
        }

        if(Completed == true)
        {
            foreach (GameObject Button in Buttons)
            {
                Button.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 120);
                Button.GetComponent<ButtonScript>().setComplete(true);
                Button.GetComponent<ButtonScript>().Reveal(true);
            }
            audioSource.PlayOneShot(CompletedSound);
            EndScreen.SetActive(true);
            EndScreen.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GetScore().ToString();
            EndScreen.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Fails: " + Fails.ToString();
            
            var result = PlayerPrefManager.GetAndOrUpdateHighscore("MatchTwo", GetScore());
            EndScreen.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = result.Item1 ? "New highscore!" : ("Highscore: " + result.Item2); 
            
            var hackeronis = 10 + 19 * Mathf.Max(10 - Fails, 0);
            PlayerPrefManager.AddEarnedHackeronis(hackeronis);
            EndScreen.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Hackeronis: " + hackeronis; 
        }
    }
    
    IEnumerator StartGameOverSequence()
    {
        foreach (GameObject Button in Buttons)
        {
            Button.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 120);
            var bscript = Button.GetComponent<ButtonScript>();
            bscript.setComplete( !bscript.IsPastaBomb );
            bscript.Reveal(true);
        }
        yield return new WaitForSeconds(0.8f);
        
        
        EndScreen.SetActive(true);
        EndScreen.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Game Over"; 
        EndScreen.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = ""; 
        EndScreen.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "(don't touch the pasta)";
        EndScreen.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = ""; 
        EndScreen.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = ""; 
    }

    public void ReturnFromMinigame()
    {
        //Debug.Log("Exiting");
        GetComponent<MinigameScene>().EndScene();
    }

    public void RestartMinigame()
    {
        GetComponent<MinigameScene>().RestartScene();
    }

    public int GetScore()
    {
        int Score = 0;

        if(Fails >= 15)
        {
            Score = 10;
        }
        else if(Fails >= 5) // (5): 90,  (14): 18
        {
            Score = 135 - (Fails * 8);
        }
        else // (0): 100, (4): 92
        {
            Score = 100 - (Fails * 2);
        }

        return Score;
    }

    // Update is called once per frame
    void Update()
    {
        if (Debug.isDebugBuild)
        {
            if(Input.GetKeyDown("r"))
            {
                ChangeBoard();
            }
            if(Input.GetKeyDown("s") )
            {
                ReturnFromMinigame();
            }
        }

        if (StartGameTimer == 0)
        {
            ChangeBoardAnimation();
            RevealBoardAnimation();
            WrongTimer();
        }
        else if(StartGameTimer > 0)
        {
            StartGameTimer -= Time.deltaTime;
            if(StartGameTimer <= 0)
            {
                StartGameTimer = 0;
                EnableButtons(true);
            }
        }
    }
}