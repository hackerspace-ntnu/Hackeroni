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
                AnswerCompleted(false);
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
    }

    void NextButtonOnClick()
    {
        NewTweet();
    }

    void RealButtonOnClick()
    {
        AnswerCompleted(TweetIsReal == true);
    }

    void FakeButtonOnClick()
    {
        AnswerCompleted(TweetIsReal == false);
    }

    void AnswerCompleted(bool correct)
    {
        if(correct) {
            RightAmount += 1;
            CorrectText.gameObject.SetActive(true);
        }
        else {
            WrongAmount += 1;
            IncorrectText.gameObject.SetActive(true);

            var fadeTime = 0.7f;
            if (WrongAmount == 1) {
                StartCoroutine(FadeOutCoroutine(Heart1, fadeTime));
            }
            if (WrongAmount == 2) {
                StartCoroutine(FadeOutCoroutine(Heart2, fadeTime));
            }
            if (WrongAmount == 3) {
                StartCoroutine(FadeOutCoroutine(Heart3, fadeTime));
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
            StartCoroutine(ReturnToMainMenuCoroutine());
        }

        isPaused = true;
    }
    
    public void ReturnToMainMenu()
    {
        GetComponent<Assets.Scripts.MinigameScene>().EndScene(new Assets.Scripts.MinigameScene.Outcome(){ highscore = RightAmount});
    }
    
    public IEnumerator ReturnToMainMenuCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        ReturnToMainMenu();
    }

    public IEnumerator FadeOutCoroutine(Image img, float fade_time)
    {
        var tick = 0f;
        while (tick <= fade_time)
        {
            tick += Time.deltaTime;

            var color = img.color;
            color.a = Mathf.Lerp(1, 0, Mathf.Pow(tick/fade_time, 1.4f));
            img.color = color;
            yield return null;
        }
    }
}
