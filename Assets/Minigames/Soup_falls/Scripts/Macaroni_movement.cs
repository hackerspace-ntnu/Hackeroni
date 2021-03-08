using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Macaroni_movement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Transform>().position = new Vector3(Random.Range(-3.0f,3.0f),5.5f,0);
    }

    // Update is called once per frame
    void Update()
    {
        //gameObject.GetComponent<Transform>().position += new Vector3(1,1,0)*Time.deltaTime;
        gameObject.GetComponent<Transform>().Rotate(0,0,5);
        if (gameObject.transform.position.y <= Camera.main.ScreenToWorldPoint(Vector3.zero).y)
        {
            //print("Ggggggg");
            Destroy(gameObject); //When stuff hits floor, it disappears
        }
        //print(gameObject.transform.position.y);
        //print(Camera.main.ScreenToWorldPoint(Vector3.zero).y);
        //Make it rain
    }
}


