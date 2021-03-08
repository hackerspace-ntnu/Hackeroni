using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailureMovement : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        gameObject.GetComponent<Transform>().position = new Vector3(Random.Range(-3.0f,3.0f),5.5f,0);
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

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<Transform>().Rotate(0,0,5);
        if (gameObject.transform.position.y <= Camera.main.ScreenToWorldPoint(Vector3.zero).y)
        {
            //print("Fffffff");
            Destroy(gameObject);
        }
    }
}
