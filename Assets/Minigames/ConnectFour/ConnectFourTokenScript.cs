using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectFourTokenScript : MonoBehaviour
{
    public int x;
    public int y;
    public int Player;

    public List<GameObject> ParticleSystems = new List<GameObject>();

    private AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>(); 
    }


    // Update is called once per frame
    void Update()
    {
        CalculatePosition();
    }

    public void CalculatePosition()
    {
        x = Mathf.RoundToInt(transform.localPosition.x);
        y = Mathf.RoundToInt(transform.localPosition.y);
    }

    public void BreakParticles()
    {
        GameObject Particles = null;

        if(Player == 1)
        {
            Particles = Instantiate(ParticleSystems[0]);
        }
        else if(Player == 2)
        {
            Particles = Instantiate(ParticleSystems[1]);
        }

        Particles.transform.position = transform.position;
    }

    public void BreakPointParticles()
    {
        GameObject Particles = null;

        if (Player == 1)
        {
            Particles = Instantiate(ParticleSystems[2]);
            Particles.transform.position = transform.position;
            //Vector3 pos = Camera.main.ViewportToWorldPoint(GameObject.Find("Engine").GetComponent<ConnectFourEngine>().PlayerPointsText.transform.position);
            Particles.GetComponent<ConnectFourParticleScript>().MoveToStart(new Vector3(-1.685027f, 17.84167f, -2.855676f), 0.5f);
        }
        else if (Player == 2)
        {
            Particles = Instantiate(ParticleSystems[3]);
            Particles.transform.position = transform.position;
            //Vector3 pos = Camera.main.ViewportToWorldPoint(GameObject.Find("Engine").GetComponent<ConnectFourEngine>().OpponentPointsText.transform.position);
            Particles.GetComponent<ConnectFourParticleScript>().MoveToStart(new Vector3(2.178409f, 17.84167f, -2.855647f), 0.5f);
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        var soundEffectVolume = PlayerPrefs.GetFloat("settings/soundEffectVolume/ConnectFour", 0.5f);
        audioSource.volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / 10f * soundEffectVolume);
        audioSource.Play();
    }
}
