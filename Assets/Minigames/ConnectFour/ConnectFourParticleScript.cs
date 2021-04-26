using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectFourParticleScript : MonoBehaviour
{
    float Timer;
    float TotalTime;
    Vector3 From;
    Vector3 To;
    bool Move;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Move == true)
        {
            MoveToUpdate();
        }
    }

    public void MoveToStart(Vector3 To, float Time)
    {
        Move = true;
        this.To = To;
        TotalTime = Time;
        From = transform.position;
        Timer = 0f;

        print(To);
    }

    void MoveToUpdate()
    {
        Timer += Time.deltaTime;

        if(Timer >= TotalTime)
        {
            Timer = TotalTime;

            Move = false;
            GameObject.Find("Engine").GetComponent<ConnectFourEngine>().UpdateScoreText();
        }

        Vector3 NextPos = From + ((To - From) * (Timer/TotalTime));
        transform.position = NextPos;
    }
}
