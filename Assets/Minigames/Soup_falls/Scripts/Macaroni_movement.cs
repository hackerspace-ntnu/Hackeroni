using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Macaroni_movement : MonoBehaviour
{
    public float rotationSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        var cam = Camera.main;
        var left = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth*0.1f, 0, 0)).x;
        var right = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth*0.9f, 0, 0)).x;

        gameObject.GetComponent<Transform>().position = new Vector3(Random.Range(left, right),5.5f,0);
    }

    // Update is called once per frame
    void Update()
    {
        //gameObject.GetComponent<Transform>().position += new Vector3(1,1,0)*Time.deltaTime;
        gameObject.GetComponent<Transform>().Rotate(0,0,rotationSpeed);
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


