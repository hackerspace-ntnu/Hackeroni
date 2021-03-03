using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TiltyHackerEngine : MonoBehaviour
{
    // Define public global varaiables
    public EdgeCollider2D ec;
    public TextMeshProUGUI elapsedTimeTMP;
    public TextMeshProUGUI powerupTimeTMP;
    public TextMeshProUGUI gameOverTMP;
    public TextMeshProUGUI finalScoreTMP;
    public TextMeshProUGUI hackeronisTMP;
    public TextMeshProUGUI restartTMP;
    public GameObject enemyPrefab;
    public GameObject powerupPrefab;
    public GameObject beamPrefab;
    public GameObject bombPrefab;
    public GameObject explosionPrefab;
    public GameObject friendPrefab;
    public GameObject friendExplosionPrefab;
    public GameObject HP;
    public float vibrationKoeffisient;
    public float vibrationDistanse;
    public float ronaChance;
    public float friendRadius;
    public float friendSpeed;
    public int spawnRate;
    
    // Define private global variables
    private Rigidbody2D rb;
    private Camera cam;
    private PoolAllocator<enemyInfo> enemyPool;
    private List<GameObject> powerups;
    private GameObject[] powerupObjects;
    private Vector2 bottomLeft;
    private Vector2 topRight;
    private float passedTime;
    private int spawned;
    private float lastPowerupSpawnTime;
    private float powerupTime; 
    private string[] powerupNames = new string[5]{"Spin", "Beam", "Bomb", "Rona", "Friend"};
    private int activePowerup; // 0-spin, 1-beam, 2-bomb, 3-corona, 4-friend
    private float infectChance;
    private Vector3 friendDirection;
    private int health;
    private int maxHP;
    private bool dead = false;
    private int score;

   // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.Landscape;

        rb = GetComponent<Rigidbody2D>();

        maxHP = HP.transform.childCount;

        infectChance = 1-Mathf.Pow(1-ronaChance, Time.fixedDeltaTime);

        cam = Camera.main;

        bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        topRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, cam.nearClipPlane));
        Vector2 topLeft = new Vector2(bottomLeft.x, topRight.y);
        Vector2 bottomRight = new Vector2(topRight.x, bottomLeft.y);
        ec.points = new Vector2[5]{bottomLeft,topLeft,topRight,bottomRight,bottomLeft};

        enemyPool = new PoolAllocator<enemyInfo>(enemyPrefab, 25);

        powerups = new List<GameObject>();
        powerupObjects = new GameObject[1];

        superStart();
    }

    void Update() // Rydd!!
    {
        updateHP();
        rotatePowerups();
        updateTimers();

        if(dead && (Input.touchCount > 0 || Input.GetMouseButtonDown(0)))
        {
            superStart();
        }

    }

    void FixedUpdate()
    {
        float lr = Input.GetAxis("Horizontal");
        float ud = Input.GetAxis("Vertical");
        rb.AddForce((Vector2.right * lr + Vector2.up * ud) * 5f);

        if(!dead)
        {
            passedTime += Time.fixedDeltaTime;

            moveEnemies();
            powerupHandler();
            SpawnEnemy();
            spawnPowerup();
        }
    }

    private void superStart()
    {
        for(int k = enemyPool.active_object_count-1; k >= 0; k --)
        {
            enemyPool.DisableInstance(k);
        }
        
        restartTMP.gameObject.SetActive(false);
        hackeronisTMP.gameObject.SetActive(false);
        gameOverTMP.gameObject.SetActive(false);
        finalScoreTMP.gameObject.SetActive(false);

        spawned = 0;
        health = maxHP;
        score = 0;
        dead = false;
        activePowerup = -1;
        powerupTime = 0f;
        passedTime = 0f;
    }

    // 5 sek itte powerupen er ferdig
    private void spawnPowerup()
    {
        if(powerups.Count == 0 && activePowerup == -1)
        {
            if(lastPowerupSpawnTime > 5)
            {
                Vector2 spawnPos = SafeSpawn();
                GameObject obj = GameObject.Instantiate(powerupPrefab);
                obj.transform.position = spawnPos;
                obj.tag = "Powerup"+Random.Range(0, 5); // 0, 5
                powerups.Add(obj);
                lastPowerupSpawnTime = 0;
            }
            lastPowerupSpawnTime += Time.fixedDeltaTime;
        }
    }

    private void moveEnemies()
    {
        List<Vector2> posList = new List<Vector2>();
        for(int i=0; i < enemyPool.active_object_count; i++)
        {
            enemyPool.metadata[i].lifetime += Time.deltaTime;
            posList = moveTowards(enemyPool.gameObjects[i], posList, enemyPool.metadata[i].lifetime);
        }
    }

    private void died()
    {
        dead = true;
        activePowerup = -1;

        for(int k = enemyPool.active_object_count-1; k >= 0; k --)
        {
            if(enemyPool.metadata[k].hasRona == true)
            {
                enemyPool.gameObjects[k].GetComponent<SpriteRenderer>().color = Color.red;
                enemyPool.gameObjects[k].transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        finalScoreTMP.text = "Final Score: "+ score.ToString();
        hackeronisTMP.text = "Hackeronies acquired: 0";

        restartTMP.gameObject.SetActive(true);
        gameOverTMP.gameObject.SetActive(true);
        finalScoreTMP.gameObject.SetActive(true);
        hackeronisTMP.gameObject.SetActive(true);

        foreach (var i in powerups)
        {
            Destroy(i);
        }
        powerups.Clear();

        foreach (var i in powerupObjects)
        {
            Destroy(i);
        }
        
        transform.rotation = Quaternion.identity;

        // Save Hackeronies and stuff
    }
    private void SpawnEnemy()
    {
        int goal = Mathf.FloorToInt(passedTime)*spawnRate+1;

        for(int i=spawned; i < goal; i++)
        {
            GameObject obj = enemyPool.CreateInstance(new enemyInfo() {lifetime = 0f, hasRona = false});
            obj.transform.position = SafeSpawn();
        }
        spawned = goal;
    }
    private List<Vector2> moveTowards(GameObject obj, List<Vector2> poss, float age) //Tek posisjonen til alle tidlegare enemieso
    {
        float speed = age/600 < 0.1f ? age/600 : 0.1f;
        Vector2 position = (transform.position-obj.transform.position).normalized*speed+obj.transform.position;

        foreach(Vector2 pos in poss)
        {
            if(Vector2.Distance(position, pos) < vibrationDistanse)
            {
                position += new Vector2(Random.Range(-vibrationKoeffisient, vibrationKoeffisient), Random.Range(-vibrationKoeffisient, vibrationKoeffisient));
            }
        }

        obj.transform.position = position;
        poss.Add(position);
        return poss;
    }

    private Vector2 SafeSpawn(float radius = 0f)
    {
        while(true)
        {
            Vector2 vector = new Vector2(Random.Range(bottomLeft.x + radius,topRight.x - radius),Random.Range(bottomLeft.y + radius,topRight.y - radius));
            if(Vector2.Distance(transform.position, vector) > 3)
            {
                return vector;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Enemy")
        {
            enemyHit(col.gameObject);
        }
        else
        {
            powerups.Remove(col.gameObject);
            Destroy(col.gameObject);

            if(col.tag == "Powerup0") // Spin2Win
            {
                activePowerup = 0;
                powerupTime = 10f;
            }
            else if(col.tag == "Powerup1") // Blaster
            {
                activePowerup = 1;
                powerupTime = 10f;
                powerupObjects[0] = GameObject.Instantiate(beamPrefab, transform);
            }
            else if(col.tag == "Powerup2") // Bomb
            {
                activePowerup = 2;
                powerupTime = 5.1f;
                powerupObjects[0] = GameObject.Instantiate(bombPrefab, transform.position, Quaternion.identity);
            }
            else if(col.tag == "Powerup3") // Corona
            {
                activePowerup = 3;
                powerupTime = 10f;
                int randint = Random.Range(0, enemyPool.active_object_count+1);
                enemyPool.metadata[randint].hasRona = true;
                enemyPool.gameObjects[randint].GetComponent<SpriteRenderer>().color = Color.green;
                enemyPool.gameObjects[randint].transform.GetChild(0).gameObject.SetActive(true);

            }
            else if(col.tag == "Powerup4") // Friend
            {
                activePowerup = 4;
                powerupTime = 10f;
                powerupObjects[0] =  GameObject.Instantiate(friendPrefab, SafeSpawn(1f), Quaternion.identity);
                int t = Random.Range(0,360);
                friendDirection = new Vector3(Mathf.Cos(t), Mathf.Sin(t), 0);
            }
        }
    }
    public void enemyHit(GameObject obj, bool damagePlayer = true)
    {
        if(activePowerup == 0 || damagePlayer == false)
        {
            enemyKill(obj);
        }
        else
        {
            enemyKill(obj);
            if(health > 1)
            {
                health --;
            }
            else if(health == 1)
            {
                health --;
                died();
            }
        }
    }

    private string precRound(float number, int prec)
    {
        float num = Mathf.Floor(number*Mathf.Pow(10, prec))/Mathf.Pow(10, prec);
        if(num%1 == 0)
        {
            return num + ".0";
        }
        return num.ToString();
    }

    private void rotatePowerups()
    {
        foreach(GameObject obj in powerups)
        {
            obj.transform.Rotate(new Vector3(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f)));
        }
    }
    private void updateTimers()
    {
        elapsedTimeTMP.text = "Time elapsed: "+precRound(passedTime, 1);
        if(activePowerup != -1)
        {
            powerupTimeTMP.text = powerupNames[activePowerup]+": "+precRound(powerupTime, 1);
        }
        else
        {
            powerupTimeTMP.text = "";
        }
    }
    private void updateHP()
    {
        int hp = Mathf.Max(0,health);

        for(int i = maxHP-1; i >= 0; i --)
        {
            if(i > hp-1)
            {
                HP.transform.GetChild(i).gameObject.SetActive(false);
            }
            else if(i <= hp-1)
            {
                HP.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    public void enemyKill(GameObject obj)
    {
        GameObject.Instantiate(explosionPrefab, obj.transform.position, Quaternion.identity);
        enemyPool.DisableInstance(obj);

        score ++;
    }

    // Korleis vil eg ha powerups??
    // Noken powerups komboe.
    // Plukke opp same powerup ++ tida.
    // maks 2 powerups om gongen.
    // maks 1 powerup item on screen.
    // Går for fyrst 1 powerup om gongen, kan legga til 2 om gongen med komboar.
    private void powerupHandler()
    {
        if(activePowerup >= 0)
        {
            if(powerupTime > 0f)
            {
                switch(activePowerup)
                {
                case 0:
                    transform.Rotate(Vector3.forward*360*Time.fixedDeltaTime);
                    break;
                case 1:
                    transform.Rotate(Vector3.forward*60*Time.fixedDeltaTime);
                    break;
                case 2:
                    if(powerupTime > 0.1f)
                    {
                        powerupObjects[0].GetComponentInChildren<TextMeshPro>().text = ""+Mathf.FloorToInt(powerupTime);
                    }
                    else
                    {
                        powerupObjects[0].transform.GetChild(0).gameObject.SetActive(true);
                        powerupObjects[0].GetComponentInChildren<TextMeshPro>().text = "";
                        powerupObjects[0].transform.GetChild(0).localScale = new Vector3(1,1,1)*(1/(powerupTime*10f+0.1f));
                    }
                    break;
                case 3:
                    bool[] infected = new bool[enemyPool.active_object_count];
                    for(int i=0; i < enemyPool.active_object_count; i++)
                    {
                        if(enemyPool.metadata[i].hasRona == true)
                        {
                            for(int k=0; k < enemyPool.active_object_count; k++)
                            {
                                if(Vector2.Distance(enemyPool.gameObjects[i].transform.position, enemyPool.gameObjects[k].transform.position) < vibrationDistanse)
                                {
                                    if(Random.Range(0f,1f) < infectChance)
                                    {
                                        infected[k] = true;
                                    }
                                }
                            }
                        }
                    }
                    for(int i=0; i  < infected.Length; i++)
                    {
                        if(infected[i] == true)
                        {
                            enemyPool.metadata[i].hasRona = true;
                            enemyPool.gameObjects[i].GetComponent<SpriteRenderer>().color = Color.green;
                            enemyPool.gameObjects[i].transform.GetChild(0).gameObject.SetActive(true);
                        }
                    }
                    break;
                case 4:
                    powerupObjects[0].transform.position = powerupObjects[0].transform.position + friendDirection*friendSpeed*Time.fixedDeltaTime;
                    if(powerupObjects[0].transform.position.x < bottomLeft.x + friendRadius || powerupObjects[0].transform.position.x > topRight.x - friendRadius)
                    {
                        friendDirection.x = -friendDirection.x;
                    }
                    else if(powerupObjects[0].transform.position.y < bottomLeft.y + friendRadius || powerupObjects[0].transform.position.y > topRight.y - friendRadius)
                    {
                         friendDirection.y = -friendDirection.y;   
                    }
                    break;
                }
            }
            else
            {
                switch(activePowerup)
                {
                    case 0:
                        transform.rotation = Quaternion.identity;
                        break;
                    case 1:
                        Destroy(powerupObjects[0]);
                        transform.rotation = Quaternion.identity;
                        break;
                    case 2:
                        Destroy(powerupObjects[0]);
                        break;
                    case 3:
                        for(int k = enemyPool.active_object_count-1; k >= 0; k --)
                        {
                            if(enemyPool.metadata[k].hasRona == true)
                            {
                                GameObject.Instantiate(explosionPrefab, enemyPool.gameObjects[k].transform.position, Quaternion.identity);
                                enemyPool.gameObjects[k].GetComponent<SpriteRenderer>().color = Color.red;
                                enemyPool.gameObjects[k].transform.GetChild(0).gameObject.SetActive(false);
                                enemyPool.DisableInstance(k); // Kan disabla enemies og øydelegga indekseringo?
                            }
                        }
                        break;
                    case 4:
                        GameObject.Instantiate(friendExplosionPrefab, powerupObjects[0].transform.position, Quaternion.identity);
                        Destroy(powerupObjects[0]);
                        break;
                }
                activePowerup = -1;
            }
            powerupTime -= Time.fixedDeltaTime;
        }
    }
}

public class enemyInfo
{
    public float lifetime;
    public bool hasRona;

} 