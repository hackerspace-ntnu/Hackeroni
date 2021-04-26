using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitregMessager : MonoBehaviour
{
    private TiltyHackerEngine engine;
    public bool player = false;

    void Start()
    {
        engine = GameObject.Find("Player").GetComponent<TiltyHackerEngine>();
    }
    private void OnTriggerEnter2D(Collider2D col) // Av og til blir det samma objektet calla to gonga?
    {
        engine.enemyHit(col.gameObject, player);
    }
}
