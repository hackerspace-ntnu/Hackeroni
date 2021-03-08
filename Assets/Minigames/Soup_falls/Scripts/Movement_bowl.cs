using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_bowl : MonoBehaviour
{
    Vector3 bowlPos = new Vector3();
    private Vector3 LastMousePosition;
    private int MovementSpeed;
    public Camera Camera;


    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Transform>().position = new Vector3(0, -4, 0);
        LastMousePosition = new Vector3(0, 0, 0);
        SetMovementSpeed(5);
    }

    // Update is called once per frame
    void Update()
    {
        bowlPos = gameObject.GetComponent<Transform>().position;
        if (Input.GetMouseButton(0))
        {
            //print(LastMousePosition);
            LastMousePosition = Input.mousePosition;
            Vector3 newPositionInWorld = Camera.main.ScreenToWorldPoint(LastMousePosition);
            transform.position = Vector2.MoveTowards(transform.position, newPositionInWorld, MovementSpeed * Time.deltaTime);

        }
    }

    //Ideer for power-ups:
    //S P E E D
    //Wider bowl
    //Macaroni zone
    //Lazer


    //Screenwrapping hadde vært kult

    public void SetMovementSpeed(int speed)
    {
        if (speed > 0 && speed <= 100)
        {
            MovementSpeed = speed;
        }
    }
}
