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
    public TextMeshProUGUI gyroTMP;
    public GameObject enemyPrefab;
    public GameObject powerupPrefab;
    public GameObject beamPrefab;
    public GameObject bombPrefab;
    public GameObject explosionPrefab;
    public GameObject friendPrefab;
    public GameObject friendExplosionPrefab;
    public GameObject bombExplosionPrefab;
    public GameObject HP;
    public Sprite angryTomat;
    public Sprite sickTomat;
    public SpriteRenderer hatSprite;
    public SpriteRenderer playerSprite;
    public float vibrationKoeffisient;
    public float vibrationDistanse;
    public float ronaChance;
    public float friendRadius;
    public float friendSpeed;
    public int spawnRate;
    public float sensitivity; // Between deadzone and infinity
    public float deadzone; // Between 0 and sensitivity
    public float maxSpeed;
    public float powerupSpawnTime;
    public float[] powerupDurations;
    public float enemyMaxSpeedAge;
    public float explosionDistance;
    public float beamFlipRate;
    public float safeSpawnRadius;
    
    public AudioClip tomatoDeadSound;
    public AudioClip powerupSound;
    public AudioClip ronaSound;
    public AudioClip spinSound;
    public AudioClip beamSound;
    public AudioClip friendSound;
    public AudioClip bombSound;
    public AudioClip bombExplosionSound;

    public AudioSource powerupAudioSource;
    private AudioSource audioSource;

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
    private int kills;
    private int hackeronies;
    private Quaternion cal = Quaternion.identity;
    private bool passedGyroTest = false;

    // Testing variables:
    // private bool trigger = false;

    void Awake() // Things that need to start early
    {
        Input.gyro.enabled = true;
        Screen.orientation = ScreenOrientation.Landscape;
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerSprite.sprite = PlayerPrefManager.GetCurrentSkinSprite();
        playerSprite.color = PlayerPrefManager.GetCurrentColor();
        hatSprite.sprite = PlayerPrefManager.GetCurrentHatSprite();

        Bounds playerBounds = playerSprite.sprite.bounds;
        Bounds hatBounds = hatSprite.sprite.bounds;

        float playerXFactor = 1/playerBounds.size.x;
        float playerYFactor = 1/playerBounds.size.y;

        float hatXFactor = 1/hatBounds.size.x/2;
        float hatYFactor = 1/hatBounds.size.y/2;

        transform.GetChild(0).localScale = new Vector3(playerXFactor, playerYFactor, 0);
        transform.GetChild(1).localScale = new Vector3(hatXFactor, hatYFactor, 0);

        rb = GetComponent<Rigidbody2D>();

        maxHP = HP.transform.childCount;

        infectChance = 1-Mathf.Pow(1-ronaChance, Time.fixedDeltaTime);

        cam = Camera.main;

        bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        topRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, cam.nearClipPlane));
        Vector2 topLeft = new Vector2(bottomLeft.x, topRight.y);
        Vector2 bottomRight = new Vector2(topRight.x, bottomLeft.y);
        ec.points = new Vector2[5]{bottomLeft,topLeft,topRight,bottomRight,bottomLeft};

        enemyPool = new PoolAllocator<enemyInfo>(enemyPrefab, 100);

        powerups = new List<GameObject>();
        powerupObjects = new GameObject[1];

        if(Application.isEditor == true)
        {
            passedGyroTest = true;
            gyroTMP.gameObject.SetActive(false);
        }

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
        if((Vector2) cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)) != bottomLeft)
        {
            bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
            topRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, cam.nearClipPlane));
            Vector2 topLeft = new Vector2(bottomLeft.x, topRight.y);
            Vector2 bottomRight = new Vector2(topRight.x, bottomLeft.y);
            ec.points = new Vector2[5]{bottomLeft,topLeft,topRight,bottomRight,bottomLeft};
        }
        if(passedGyroTest)
        {
            gyroTMP.gameObject.SetActive(false);
        }

    }

    void FixedUpdate() // Physics calculations
    {
        if(passedGyroTest)
        {
            move();

            if(!dead)
            {
                passedTime += Time.fixedDeltaTime;

                moveEnemies();
                powerupHandler();
                SpawnEnemy();
                spawnPowerup();
            }
        }
        else if(qLength(Gyro()) > 0.5f)
        {
            passedGyroTest = true;
            gyroTMP.gameObject.SetActive(false);
            transform.position = new Vector3(0, 0, 0);
            calibrate();
        }
        else
        {
            rb.AddForce((Vector2.right * Random.Range(-1f, 1f) + Vector2.up * Random.Range(-1f, 1f)) * maxSpeed);
        }
    }

    // void OnGUI()
    // {
    //     Vector3 point = cal*Gyro()*Vector3.forward;
    //     Vector2 mov = -Proj(point);
    //     float magnitude = mov.magnitude;
        
    //     GUI.skin.label.fontSize = Screen.width / 40;

    //     GUILayout.Label("Gyro: " + Gyro());
    //     GUILayout.Label("Movementvektor: " + mov);
    //     GUILayout.Label("Magnitude: " + magnitude);
    //     GUILayout.Label("Enabled: " + Input.gyro.enabled);
    //     GUILayout.Label("In Editor? " + Application.isEditor);
    //     GUILayout.Label("Triggered? " + trigger);
    // }

    private void superStart() // Things that need to be setafter every restart
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
        kills = 0;
        dead = false;
        activePowerup = -1;
        powerupTime = 0f;
        passedTime = 0f;
        lastPowerupSpawnTime = 0f;

        transform.position = new Vector3(0, 0, 0);
    }
    
    private Quaternion Gyro() // Transform Gyroscope quaternion from right hand side to left hand side
    {
        Quaternion q = Input.gyro.attitude;
        return (new Quaternion(q.x, q.y, -q.z, -q.w));
    }

    private Vector2 Proj(Vector3 p) // Simple stereographic projection
    {
        return new Vector2(p.x/p.z, p.y/p.z);
    }

    public void calibrate() // Calibrate gyro controls
    {
        if(Input.gyro.enabled && passedGyroTest)
        {
            cal = Quaternion.Inverse(Gyro());
        }
    }
    private float qLength(Quaternion q) // Simple 1 norm of a quaternion
    {
        return q.x + q.y + q.z + q.w;
    }

    private void move() // Movement by tilt controls
    {
        float lr = 0f;
        float ud = 0f;
        if(Application.isEditor)
        {
            lr = Input.GetAxis("Horizontal");
            ud = Input.GetAxis("Vertical");
        }
        else
        {
            // After a month of working on tis shit, I've finally made something that works.
            Vector3 point = cal*Gyro()*Vector3.forward;
            if(point.z >= 0)
            {
                Vector2 mov = -Proj(point);
                float magnitude = mov.magnitude;
                mov = mov/magnitude;

                if(magnitude > deadzone)
                {
                    if(magnitude < sensitivity)
                    {
                        mov = mov*(magnitude/(sensitivity-deadzone)-deadzone/(sensitivity-deadzone));
                    }
                }
                else
                {
                    mov = new Vector2(0, 0);
                }
                lr = mov.x;
                ud = mov.y;
            }
        }
        rb.AddForce((Vector2.right * lr + Vector2.up * ud) * maxSpeed);
    }

    private void spawnPowerup() // Spawn powerupss based on powerupSpawnTime
    {
        if(powerups.Count == 0 && activePowerup == -1)
        {
            if(lastPowerupSpawnTime > powerupSpawnTime)
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

    private void moveEnemies() // Enemy moving code
    {
        List<Vector2> posList = new List<Vector2>();
        for(int i=0; i < enemyPool.active_object_count; i++)
        {
            enemyPool.metadata[i].lifetime += Time.deltaTime;
            posList = moveTowards(enemyPool.gameObjects[i], posList, enemyPool.metadata[i].lifetime);
        }
    }

    private void died() // Run when dead
    {
        powerupAudioSource.Stop();
        int score = kills - 10;
        hackeronies = score; // Should be optimized.
        PlayerPrefManager.AddEarnedHackeronis(score);

        dead = true;
        activePowerup = -1;

        for(int k = enemyPool.active_object_count-1; k >= 0; k --)
        {
            if(enemyPool.metadata[k].hasRona == true)
            {
                enemyPool.gameObjects[k].GetComponent<SpriteRenderer>().sprite = angryTomat;
                enemyPool.gameObjects[k].transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        var high = PlayerPrefManager.GetAndOrUpdateHighscore("FuckingGyroskop", score);

        if(!high.Item1)
        {
            finalScoreTMP.text = "Final Score: " + score.ToString() + "\nHighscore: " + high.Item2;
        }
        else
        {
            finalScoreTMP.text = "New highscore: " + high.Item2 + "!!";
        }
        
        hackeronisTMP.text = "Hackeronies acquired: " + hackeronies.ToString();

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

    private void SpawnEnemy() // Enemyspawning
    {
        int goal = Mathf.FloorToInt(passedTime)*spawnRate+1;

        for(int i=spawned; i < goal; i++)
        {
            GameObject obj = enemyPool.CreateInstance(new enemyInfo() {lifetime = 0f, hasRona = false});
            obj.transform.position = SafeSpawn();
        }
        spawned = goal;
    }

    private List<Vector2> moveTowards(GameObject obj, List<Vector2> poss, float age) // Determines how enemies move
    {
        float speed = age/enemyMaxSpeedAge < 1 ? age/enemyMaxSpeedAge : 1;
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

    private Vector2 SafeSpawn(float radius = 0f) // Determine a safe spawning position for powerups and enemies
    {
        while(true)
        {
            Vector2 vector = new Vector2(Random.Range(bottomLeft.x + radius,topRight.x - radius),Random.Range(bottomLeft.y + radius,topRight.y - radius));
            if(Vector2.Distance(transform.position, vector) > safeSpawnRadius)
            {
                return vector;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col) // When the player hits things
    {
        if(col.tag == "Enemy")
        {
            enemyHit(col.gameObject);
        }
        else
        {
            powerups.Remove(col.gameObject);
            Destroy(col.gameObject);
            
            audioSource.PlayOneShot(powerupSound);

            if(col.tag == "Powerup0") // Spin2Win
            {
                activePowerup = 0;
                powerupTime = powerupDurations[0];
                powerupAudioSource.clip = spinSound;    
                powerupAudioSource.loop = false;
            }
            else if(col.tag == "Powerup1") // Blaster
            {
                activePowerup = 1;
                powerupTime = powerupDurations[1];
                powerupObjects[0] = GameObject.Instantiate(beamPrefab, transform);
                powerupAudioSource.clip = beamSound;    
                powerupAudioSource.loop = true;
            }
            else if(col.tag == "Powerup2") // Bomb
            {
                activePowerup = 2;
                powerupTime = powerupDurations[2];
                powerupObjects[0] = GameObject.Instantiate(bombPrefab, transform.position, Quaternion.identity);
                powerupAudioSource.clip = bombSound;    
                powerupAudioSource.loop = true;
            }
            else if(col.tag == "Powerup3") // Corona
            {
                activePowerup = 3;
                powerupTime = powerupDurations[3];
                int randint = Random.Range(0, enemyPool.active_object_count+1);
                enemyPool.metadata[randint].hasRona = true;
                enemyPool.gameObjects[randint].GetComponent<SpriteRenderer>().sprite = sickTomat;
                enemyPool.gameObjects[randint].transform.GetChild(0).gameObject.SetActive(true);

                powerupAudioSource.clip = ronaSound;    
                powerupAudioSource.loop = true;
            }
            else if(col.tag == "Powerup4") // Friend
            {
                activePowerup = 4;
                powerupTime = powerupDurations[4];
                powerupObjects[0] =  GameObject.Instantiate(friendPrefab, SafeSpawn(1f), Quaternion.identity);
                int t = Random.Range(0,360);
                friendDirection = new Vector3(Mathf.Cos(t), Mathf.Sin(t), 0);
                powerupAudioSource.clip = friendSound;    
                powerupAudioSource.loop = true;
            }
            powerupAudioSource.Play();
        }
    }

    public void enemyHit(GameObject obj, bool player=true) // Hitting an enemy
    {
        if(player)
        {
            switch(activePowerup)
            {
                case 0:
                    enemyKill(obj);
                    break;
                case 3:
                    int index = enemyPool.GetIndex(obj);
                    enemyPool.metadata[index].hasRona = false;
                    enemyPool.gameObjects[index].GetComponent<SpriteRenderer>().sprite = angryTomat;
                    enemyPool.gameObjects[index].transform.GetChild(0).gameObject.SetActive(false);
                    goto default;
                default:
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
                    break;
            }
        }
        else
        {
            enemyKill(obj);
        }
    }

    private string precRound(float number, int prec) // Helper function for nice rounding
    {
        float num = Mathf.Floor(number*Mathf.Pow(10, prec))/Mathf.Pow(10, prec);
        if(num%1 == 0)
        {
            return num + ".0";
        }
        return num.ToString();
    }

    private void rotatePowerups() // How powerups rotate
    {
        foreach(GameObject obj in powerups)
        {
            obj.transform.Rotate(new Vector3(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f)));
        }
    }

    private void updateTimers() // Updating timers
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

    private void updateHP() // Updating HP
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

    public void enemyKill(GameObject obj) // Killing an enemy
    {
        GameObject.Instantiate(explosionPrefab, obj.transform.position, Quaternion.identity);
        enemyPool.DisableInstance(obj);
        
        audioSource.PlayOneShot(tomatoDeadSound);

        kills ++;
    }

    private void powerupHandler() // How powerups act upon the world
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
                    SpriteRenderer lol =  powerupObjects[0].GetComponent<SpriteRenderer>();
                    if(powerupTime % beamFlipRate >= beamFlipRate/2)
                    {
                        lol.flipX = true;
                        lol.flipY = true;
                    }
                    else
                    {
                        lol.flipX = false;
                        lol.flipY = false;
                    }
                    break;
                case 2:
                    powerupObjects[0].GetComponentInChildren<TextMeshPro>().text = Mathf.FloorToInt(powerupTime).ToString();
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
                            enemyPool.gameObjects[i].GetComponent<SpriteRenderer>().sprite = sickTomat;
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
                powerupAudioSource.Stop();
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
                        audioSource.PlayOneShot(bombExplosionSound);
                        GameObject.Instantiate(bombExplosionPrefab, powerupObjects[0].transform.position, Quaternion.identity);
                        for(int i=0; i < enemyPool.active_object_count; i++)
                        {
                            if(Vector2.Distance(enemyPool.gameObjects[i].transform.position, powerupObjects[0].transform.position) < explosionDistance)
                            {
                                enemyKill(enemyPool.gameObjects[i]);
                            }
                        }
                        Destroy(powerupObjects[0]);
                        break;
                    case 3:
                        for(int k = enemyPool.active_object_count-1; k >= 0; k --)
                        {
                            if(enemyPool.metadata[k].hasRona == true)
                            {
                                GameObject.Instantiate(explosionPrefab, enemyPool.gameObjects[k].transform.position, Quaternion.identity);
                                enemyPool.gameObjects[k].GetComponent<SpriteRenderer>().sprite = angryTomat;
                                enemyPool.gameObjects[k].transform.GetChild(0).gameObject.SetActive(false);
                                enemyPool.DisableInstance(k); // Kan disabla enemies og øydelegga indekseringo?
                                kills++;
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

public class enemyInfo // Helper struct for storing information in enemies.
{
    public float lifetime;
    public bool hasRona;

} 