using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    public GameTracker gameTracker;
    public AudioSource failSoundEffect;
    public AudioSource successSoundEffect;
    private Vector3 smallBowl = new Vector3(2.5f,2.5f,1f);
    private Vector3 bigBowl = new Vector3(3.0f,3.0f,1f);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameTracker.getPowerIsBIG()){
            gameTracker.adjustPowerBIGCountdown(Time.deltaTime);
        }
        if (gameTracker.getPowerBIGCountdown() <= 0 && gameTracker.getPowerIsBIG()){
            gameTracker.setPowerIsBIG(false);
            //print(gameTracker.getPowerIsBIG());
            gameObject.transform.localScale = smallBowl;
        }
        
        if (gameTracker.getPowerIsSPEED()){
            gameTracker.adjustPowerSPEEDCountdown(Time.deltaTime);
        }
        if (gameTracker.getPowerSPEEDCountdown() <= 0 && gameTracker.getPowerIsSPEED()){
            gameTracker.setPowerIsSPEED(false);
            //print(gameTracker.getPowerIsSPEED());
            gameObject.GetComponent<Movement_bowl>().SetMovementSpeed(5);
        }

        if (gameTracker.message.gameObject.activeSelf){
            gameTracker.adjustPopUpMessageCountdown(Time.deltaTime);
        }
        if (gameTracker.message.gameObject.activeSelf && gameTracker.messageTimer <=0 ){
            gameTracker.message.gameObject.SetActive(false);
        }

    }
    
    // Stuff that happens when you hit the falling stuffs
    private void OnTriggerEnter2D(Collider2D other)
    {
        var soundEffect = successSoundEffect;

        if (other.tag == "Macaroni_green"){
            gameTracker.addPoints(50);
            Destroy(other.gameObject);
            gameTracker.gamePopUpMessage("+50", new Color32((byte) 0, (byte) 255, (byte) 0, 255));
        }
        else if (other.tag == "Macaroni_red"){
            gameTracker.addPoints(100);
            Destroy(other.gameObject);
            gameTracker.gamePopUpMessage("+100", new Color32((byte) 0, (byte) 255, (byte) 0, 255));
        }
        else if (other.tag == "Macaroni_gold"){
            gameTracker.addPoints(200);
            Destroy(other.gameObject);
            gameTracker.gamePopUpMessage("+200", new Color32((byte) 0, (byte) 255, (byte) 0, 255));
        }
        else if (other.tag == "Power_BIG"){
            Destroy(other.gameObject);
            gameTracker.setPowerIsBIG(true);
            gameTracker.setPowerBIGCountdown(5);
            gameObject.transform.localScale = bigBowl;
            gameTracker.gamePopUpMessage("Powerup: Big!", new Color32((byte) 255, (byte) 255, (byte) 0, 255));
        }
        else if (other.tag == "Power_SPEED"){
            Destroy(other.gameObject);
            gameTracker.setPowerIsSPEED(true);
            gameTracker.setPowerSPEEDCountdown(5);
            gameObject.GetComponent<Movement_bowl>().SetMovementSpeed(10);
            gameTracker.gamePopUpMessage("Powerup: Speed!", new Color32((byte) 255, (byte) 255, (byte) 0, 255));
        }
        else if (other.tag == "Power_macBURST"){
            Destroy(other.gameObject);
            gameTracker.macBURST();
            gameTracker.gamePopUpMessage("Macaroni burst!", new Color32((byte) 255, (byte) 255, (byte) 0, 255));
        }
        else if (other.tag == "Life"){
            gameTracker.adjustLives(1);
            Destroy(other.gameObject);
            print(gameTracker.getLives());
            gameTracker.gamePopUpMessage("Obtained: Life!", new Color32((byte) 255, (byte) 255, (byte) 255, 255));
        }
        else if (other.tag == "Failure"){
            gameTracker.setColorChangeTime(0.5f);
            gameTracker.changeBackgroundColor(255,99,71);
            gameTracker.adjustLives(-1);
            soundEffect = failSoundEffect;
            Destroy(other.gameObject);
            gameTracker.gamePopUpMessage("- 1 life", new Color32((byte) 255, (byte) 255, (byte) 255, 255));
        }
        soundEffect.Play();
    }
}
