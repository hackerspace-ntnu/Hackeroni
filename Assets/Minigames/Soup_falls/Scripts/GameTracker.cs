using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameTracker : MonoBehaviour
{   
    private bool gameIsOn;
    public static double globalTimer = 0;
    private int score = 0;
    private int lives;
    private float mainCountdown = 1;
    public GameObject mainCamera;
    public TMP_Text score_text;
    public TMP_Text life_text;
    public TMP_Text message;
    public GameObject greenMacaroniPref;
    public GameObject redMacaroniPref;
    public GameObject goldMacaroniPref;
    public GameObject power_big;
    public GameObject power_speed;
    public GameObject power_macBurst;
    public GameObject life;
    public GameObject failure;
    private Color32 defaultBackgroundColor = new Color32(49,77,121,255);
    public double messageTimer;

    public GameObject end_screen_canvas;
    public TMP_Text final_score_txt;
    public TMP_Text highscore_txt;
    public TMP_Text hackeronies_earned_txt;

    private bool powerIsBIG = false;
    private double powerBIGCountdown = 0;

    private bool powerIsSPEED = false;
    private double powerSPEEDCountdown = 0;

    private double colorchangeCountdown = 0;

    private ArrayList fallingObjects = new ArrayList();
    private int trigger;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        lives = 3;
        gameIsOn = true;
        end_screen_canvas.SetActive(false);
        message.gameObject.SetActive(false);

        fallingObjects.Add(greenMacaroniPref);
        fallingObjects.Add(redMacaroniPref);
        fallingObjects.Add(goldMacaroniPref);
        fallingObjects.Add(power_big);
        fallingObjects.Add(power_speed);
        fallingObjects.Add(power_macBurst);
        fallingObjects.Add(life);
        fallingObjects.Add(failure);
    }

    // Update is called once per frame
    void Update()
    {   
        if (gameIsOn){
            if (mainCountdown>0){
                mainCountdown -= Time.deltaTime;
            }
            else{
                instantiator();
                mainCountdown = Random.Range(0.5f, 1.5f);
            }
            backgroundColorManager();
            globalTimer+= Time.deltaTime;
        }

        if (lives == 0){
            score_text.gameObject.SetActive(false);
            life_text.gameObject.SetActive(false);
            final_score_txt.text = "Your score: " + score;

            var highscore = PlayerPrefManager.GetAndOrUpdateHighscore("Soup_falls", score).Item2;
            highscore_txt.text = "Highscore: " + highscore; 
            
            var hackeronis = Mathf.RoundToInt(score/20f);
            PlayerPrefManager.AddEarnedHackeronis(hackeronis);
            hackeronies_earned_txt.text = hackeronis.ToString(); 

            GameObject.Find("Music").GetComponent<AudioSource>().Stop();

            end_screen_canvas.SetActive(true);
            gameIsOn = false;
            Time.timeScale = 0;
            lives = -1;
        }

        if (globalTimer > 30 && trigger == 0){
            trigger = 1;
            foreach (GameObject Object in fallingObjects)
            {
                Object.GetComponent<Rigidbody2D>().gravityScale += 0.5f;
            }   
        }else if (globalTimer > 60 && trigger == 1){
            trigger = 2;
            foreach (GameObject Object in fallingObjects)
            {
                Object.GetComponent<Rigidbody2D>().gravityScale += 0.5f;
            }
        }else if (globalTimer > 120 && trigger == 2){
            trigger = 3;
            foreach (GameObject Object in fallingObjects)
            {
                Object.GetComponent<Rigidbody2D>().gravityScale += 0.5f;
            }
        }

    }
    //TODO utdyp algoritme for utregning av hackaronies
    private int konvertToHackeronies(int score){
        // Do the algorithm here
        return 25;
    }
        

    private void backgroundColorManager(){
        if (colorchangeCountdown>0){
            colorchangeCountdown -= Time.deltaTime;
        }
        else if (mainCamera.GetComponent<Camera>().backgroundColor != defaultBackgroundColor){
            changeBackgroundColor(defaultBackgroundColor);
        }
    }

    /*
    * Instansierer ulike objekter avhengig av tilveldig valgt tall. Noen objekter har annen rarity enn andre.
    */
    public void instantiator(){
        float randomSpawnNumber = Random.Range(0.0f,20.0f);
        GameObject fallingObject = null;
        if (randomSpawnNumber < 5){
            fallingObject = Instantiate(greenMacaroniPref);
        }
        else if (randomSpawnNumber < 8){
            fallingObject = Instantiate(redMacaroniPref);
        }
        else if (randomSpawnNumber < 10){
            fallingObject = Instantiate(goldMacaroniPref);
        }
        else if (randomSpawnNumber < 10.3){
            fallingObject = Instantiate(life);
        }
        else if (randomSpawnNumber < 12){
            fallingObject = Instantiate(power_big);
        }
         else if (randomSpawnNumber < 13){
            fallingObject = Instantiate(power_speed);
        }
         else if (randomSpawnNumber < 14){
            fallingObject = Instantiate(power_macBurst);
        }
         else if (randomSpawnNumber < 20){
            fallingObject = Instantiate(failure);
        }
        fallingObject.SetActive(true);
    }

    public void addPoints(int points)
    {
        this.score += points;
        score_text.text = "Score: " + this.score;
    }

    public int getScore()
    {
        return score;
    }

    public void adjustLives(int lives)
    {
        this.lives += lives;
        life_text.text = "Life: "+this.lives;
    }

    public int getLives()
    {
        return lives;
    }

    public bool getPowerIsBIG(){
        return powerIsBIG;
    }

    public void setPowerIsBIG(bool value){
        powerIsBIG = value;
    }

    public double getPowerBIGCountdown(){
        return powerBIGCountdown;
    }

    public void setPowerBIGCountdown(double value){
        if (value > 0){
            powerBIGCountdown = value;
        }
    }

    public void adjustPowerBIGCountdown(double value){
        if (value > 0 && powerIsBIG){
            powerBIGCountdown -= value;
        }
    }

    public bool getPowerIsSPEED(){
        return powerIsSPEED;
    }

    public void setPowerIsSPEED(bool value){
        powerIsSPEED = value;
    }

    public double getPowerSPEEDCountdown(){
        return powerSPEEDCountdown;
    }

    public void setPowerSPEEDCountdown(int value){
        if (value > 0){
            powerSPEEDCountdown = value;
        }
    }

    public void adjustPowerSPEEDCountdown(double value){
        if (value > 0 && powerIsSPEED){
            powerSPEEDCountdown -= value;
        }
    }
    public void macBURST(){
        GameObject fallingObject = null;
        for(int i = 0; i < 5; i++){
            fallingObject = Instantiate(greenMacaroniPref);
            fallingObject.SetActive(true);
            fallingObject = Instantiate(redMacaroniPref);
            fallingObject.SetActive(true);
        } 
        for (int i = 0; i < 3; i++){
            fallingObject = Instantiate(goldMacaroniPref);
            fallingObject.SetActive(true);
        }
    }

    //Bruk denne til å skifte farge på bakgrunn når du fanger en failure.
    public void changeBackgroundColor(int red, int green, int blue){
        mainCamera.GetComponent<Camera>().backgroundColor = new Color32((byte) red, (byte) green, (byte) blue, 255);
    }
    public void changeBackgroundColor(Color32 color){
        mainCamera.GetComponent<Camera>().backgroundColor = color;
    }

    public void setColorChangeTime(float seconds){
        this.colorchangeCountdown = seconds;
    }

    public void gamePopUpMessage(string messageText, Color32 color){
        message.text = messageText;
        message.color = color;
        messageTimer = 1;
        message.gameObject.SetActive(true);
        
    }
    
    public void adjustPopUpMessageCountdown(double value){
        if (value > 0 && message.gameObject.activeSelf){
            messageTimer -= value;
        }
    }
}
