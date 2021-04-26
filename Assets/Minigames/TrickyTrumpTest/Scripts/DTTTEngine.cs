using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DTTTEngine : MonoBehaviour
{
    private List<string> FakeTweets = new List<string>();
    private List<string> RealTweets = new List<string>();
    public TextMeshProUGUI TweetText;
    public TextMeshProUGUI ProfileTag;
    public Image ProfilePicture;
    
    public Image Verified; 
    public Image Heart1;
    public Image Heart2;
    public Image Heart3;
    public TextMeshProUGUI CorrectText;
    public TextMeshProUGUI IncorrectText;
    public TextMeshProUGUI Right;
    public Scrollbar Timer;
    public Button RealButton;
    public Button FakeButton;
    public Button NextButton;
    public GameObject EndGameScreenCanvas;
    public AudioSource tickTockAudioSource;
    public AudioClip IncorrectSound;
    public AudioClip CorrectSound;
    public AudioClip TimeoutSound;
    private AudioSource audioSource;
    public float MinNumberSecondsToAnswer = 5;
    public float MaxNumberSecondsToAnswer = 20;

    private bool TweetIsReal;
    private int RightAmount;
    private int WrongAmount;
    private float time = 1;
    private bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //CreateTweetListPrefabs();
        LoadTweets();
        NewTweet();

        NextButton.onClick.AddListener(NextButtonOnClick);
        RealButton.onClick.AddListener(RealButtonOnClick);
        FakeButton.onClick.AddListener(FakeButtonOnClick);
        
        CorrectText.gameObject.SetActive(false);
        IncorrectText.gameObject.SetActive(false);
    }
    
    void Update()
    {
        if (isPaused == false) 
        {
            Timer.size = time;
            if (time < 0)
            {
                AnswerCompleted(false, true);
            }

            time -= Time.deltaTime / (Mathf.Lerp(MinNumberSecondsToAnswer, MaxNumberSecondsToAnswer, Mathf.Clamp01(TweetText.text.Length / 400f)));
        }
        
    }

    void CreateTweetListPrefabs() {
        FakeTweets = new List<string>();
        RealTweets = new List<string>();

        string[] FakeTweetsArray = System.IO.File.ReadAllLines(Application.dataPath + "/Resources/Minigames/TrickyTrumpTest/FakeTrumpTweets.txt");
        FakeTweets.AddRange(FakeTweetsArray);

        string[] RealTweetsArray = System.IO.File.ReadAllLines(Application.dataPath + "/Resources/Minigames/TrickyTrumpTest/RealTrumpTweets.txt");
        RealTweets.AddRange(RealTweetsArray);

        GameObject.Find("FakeTrumpTweetsList").GetComponent<ListScript>().List = FakeTweets;
        GameObject.Find("RealTrumpTweetsList").GetComponent<ListScript>().List = RealTweets;
    }

    void LoadTweets()
    {
        FakeTweets = Resources.Load<GameObject>("Minigames/TrickyTrumpTest/FakeTrumpTweetsList").GetComponent<ListScript>().List;
        RealTweets = Resources.Load<GameObject>("Minigames/TrickyTrumpTest/FakeTrumpTweetsList").GetComponent<ListScript>().List;
    }
    
    void NewTweet()
    {
        if(Random.Range(0, 2) == 0) {
            TweetIsReal = true;
            
            string TweetString = RealTweets[Random.Range(0, RealTweets.Count)];
            TweetText.text = TweetString;
            RealTweets.Remove(TweetString);
        }
        else
        {
            TweetIsReal = false;

            string TweetString = FakeTweets[Random.Range(0, FakeTweets.Count)];
            TweetText.text = TweetString;
            FakeTweets.Remove(TweetString);
        }

        ProfileTag.text = "@realDonaldTrump";
        Verified.gameObject.SetActive(false);
        ProfilePicture.sprite = Resources.Load<Sprite>("Minigames/TrickyTrumpTest/Textures/RealTrump");

        NextButton.gameObject.SetActive(false);
        RealButton.gameObject.SetActive(true);
        FakeButton.gameObject.SetActive(true);

        CorrectText.gameObject.SetActive(false);
        IncorrectText.gameObject.SetActive(false);

        time = 1;
        isPaused = false;
        tickTockAudioSource.UnPause();
    }

    void NextButtonOnClick()
    {
        NewTweet();
    }

    void RealButtonOnClick()
    {
        AnswerCompleted(TweetIsReal == true, false);
    }

    void FakeButtonOnClick()
    {
        AnswerCompleted(TweetIsReal == false, false);
    }

    void AnswerCompleted(bool correct, bool timeout)
    {
        tickTockAudioSource.Pause();

        if(correct) {
            RightAmount += 1;
            CorrectText.gameObject.SetActive(true);
            audioSource.PlayOneShot(CorrectSound);
        }
        else {
            WrongAmount += 1;
            IncorrectText.gameObject.SetActive(true);
            if (timeout) {
                IncorrectText.text = "Time's up";
                audioSource.PlayOneShot(TimeoutSound);
            } else {
                IncorrectText.text = "Incorrect";
                audioSource.PlayOneShot(IncorrectSound);
            }

            var fadeTime = 0.7f;
            if (WrongAmount == 1) {
                StartCoroutine(FadeCoroutine(Heart1, 1, 0, fadeTime));
            }
            if (WrongAmount == 2) {
                StartCoroutine(FadeCoroutine(Heart2, 1, 0, fadeTime));
            }
            if (WrongAmount == 3) {
                StartCoroutine(FadeCoroutine(Heart3, 1, 0, fadeTime));
            }
        }

        if(TweetIsReal == true) {
            Verified.gameObject.SetActive(true);
        }
        else {
            ProfileTag.text = "@fakeDonaldTrump";
            ProfilePicture.sprite = Resources.Load<Sprite>("Minigames/TrickyTrumpTest/Textures/Bot");
        }

        //int AccuracyInt = Mathf.RoundToInt(((float)RightAmount/(float)(WrongAmount + RightAmount)) * 100);
        Right.text = "Score: " + RightAmount;

        NextButton.gameObject.SetActive(true);
        RealButton.gameObject.SetActive(false);
        FakeButton.gameObject.SetActive(false);


        if (WrongAmount >= 3) {
            //Game over
            StartCoroutine(GameOverCoroutine());
        }

        isPaused = true;
    }
    
    public void ReturnToMainMenu()
    {
        GetComponent<Assets.Scripts.MinigameScene>().EndScene();
    }
    
    public IEnumerator GameOverCoroutine()
    {
        NextButton.gameObject.SetActive(false);
        EndGameScreenCanvas.SetActive(true);

        var result = PlayerPrefManager.GetAndOrUpdateHighscore("TrickyTrumpTest", RightAmount);
        
        var adjustedValue = Mathf.Max(RightAmount-2, 0);
        var hackeronisEarned = adjustedValue * adjustedValue + (result.Item1 ? 10 : 0);
        PlayerPrefManager.AddEarnedHackeronis(hackeronisEarned);


        EndGameScreenCanvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text 
                = string.Format("{0}\n\n{1}\n\n{2}", RightAmount, result.Item2, hackeronisEarned);

        EndGameScreenCanvas.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text 
                = string.Format("Score\n\nHighscore {0}\n\nHackeronis", result.Item1 ? "(NEW!)" : "");

        var buttons = EndGameScreenCanvas.GetComponentsInChildren<Button>();
        foreach(var button in buttons)
        {
            button.interactable = false;
        }


        var canvasGroup = EndGameScreenCanvas.GetComponent<CanvasGroup>(); 
        const float fade_time = 0.5f;
        var tick = 0f;
        while (tick <= fade_time)
        {
            tick += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, Mathf.Pow(tick/fade_time, 1.5f));
            yield return null;
        }
        foreach(var button in buttons)
        {
            button.interactable = true;
        }
    }

    public IEnumerator FadeCoroutine(Image img, float startAlpha, float endAlpha, float fade_time)
    {
        var tick = 0f;
        while (tick <= fade_time)
        {
            tick += Time.deltaTime;

            var color = img.color;
            color.a = Mathf.Lerp(startAlpha, endAlpha, Mathf.Pow(tick/fade_time, 1.4f));
            img.color = color;
            yield return null;
        }
    }
}
