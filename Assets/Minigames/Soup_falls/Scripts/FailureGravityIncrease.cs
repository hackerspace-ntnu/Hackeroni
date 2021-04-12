using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailureGravityIncrease: MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        if (GameTracker.globalTimer > 120){
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 1.75f;
        } 
        else  if (GameTracker.globalTimer > 90){
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 1.75f;
        }
        else if (GameTracker.globalTimer > 60){
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 1.5f;
        }
        else if (GameTracker.globalTimer > 20){
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 1.25f;
        }
    }
}
