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
    public TextMeshProUGUI Right;
    public TextMeshProUGUI Wrong;
    public TextMeshProUGUI Accuracy;
    public Button RealButton;
    public Button FakeButton;
    public Button NextButton;
    private bool TweetIsReal;
    private int RightAmount;
    private int WrongAmount;

    // Start is called before the first frame update
    void Start()
    {
        //CreateTweetListPrefabs();
        LoadTweets();
        NewTweet();

        NextButton.onClick.AddListener(NextButtonOnClick);
        RealButton.onClick.AddListener(RealButtonOnClick);
        FakeButton.onClick.AddListener(FakeButtonOnClick);
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
    }

    void NextButtonOnClick()
    {
        NewTweet();
    }

    void RealButtonOnClick()
    {
        if(TweetIsReal == true) {
            RightAmount += 1;
        }
        else {
            WrongAmount += 1;
        }

        AnswerCompleted();
    }

    void FakeButtonOnClick()
    {
        if(TweetIsReal == false) {
            RightAmount += 1;
        }
        else {
            WrongAmount += 1;
        }

        AnswerCompleted();
    }

    void AnswerCompleted()
    {
        if(TweetIsReal == true) {
            Verified.gameObject.SetActive(true);
        }
        else {
            ProfileTag.text = "@fakeDonaldTrump";
            ProfilePicture.sprite = Resources.Load<Sprite>("Minigames/TrickyTrumpTest/Textures/Bot");
        }

        int AccuracyInt = Mathf.RoundToInt(((float)RightAmount/(float)(WrongAmount + RightAmount)) * 100);
        Right.text = "You got " + RightAmount + " right";
        Wrong.text = "You got " + WrongAmount + " wrong";
        Accuracy.text = AccuracyInt + "% Accuracy";

        NextButton.gameObject.SetActive(true);
        RealButton.gameObject.SetActive(false);
        FakeButton.gameObject.SetActive(false);
    }
    
    public void ReturnToMainMenu()
    {
        GetComponent<Assets.Scripts.MinigameScene>().EndScene(new Assets.Scripts.MinigameScene.Outcome(){ highscore = -1});
    }
}
