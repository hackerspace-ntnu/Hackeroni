using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animTimer : MonoBehaviour
{
    public float time = 5f;
    private float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timer >= 0) {
            timer += Time.deltaTime;
        }   
        else return; 
        if (timer > time) {
            timer = -1;
            
            GetComponent<Animator>().SetBool("Running", false);
        }
    }
}
