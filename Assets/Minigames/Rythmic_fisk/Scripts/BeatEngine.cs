using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts;
using UnityEngine.EventSystems;

public class BeatEngine : MonoBehaviour, IPointerDownHandler
{
    public GameObject macaroniPrefab;
    public GameObject successEffectPrefab;
    public AudioSource musicSource;
    public AudioSource failSoundEffect;
    public AudioSource successSoundEffect;
    public AudioSource hooraySoundEffect;
    public TextMeshProUGUI scoreTracker;
    public TextMeshProUGUI scoreMultiplierTracker;
    public TextMeshProUGUI failText;
    public TextMeshProUGUI comboText;
    public Canvas endScreenCanvas;
    public TextMeshProUGUI gameoverText;
    public TextMeshProUGUI congratulationsText;
    public TextMeshProUGUI statisticsText;
    public float macaroniTravelDuration = 2f; 
    public float clickMargin = 0.05f;
    public float destroyTime = 0;
    public float minSpawnRadius = 10;
    public float maxSpawnRadius = 20;
    public int baseScoreIncrement = 10;
    float timer = 0;
    const int beats_per_minute = 120;
    int beat_index = 0;
    List<float> beat_times; 

    private PoolAllocator<MacaroniData> macaroniPool;
    private PoolAllocator<float> successEffectPool;

    private float macaroniTouchRadiusSqr;
    public float beatRadius;

    private int numberFailures = 0;
    private int comboCount = 0;
    private int score = 0;

    private int startUpCount = 15;
    private float musicLength = 0;
    void Start()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Minigames/Rythmic_fisk/rhythm_BeatSouls");
        string text = textAsset.text;
        bool in_bar = false;
        int bar_number = 0;
        int parsed_number = 0;
        musicLength = musicSource.clip.length;

        beat_times = new List<float>(100); 
        macaroniPool = new PoolAllocator<MacaroniData>(macaroniPrefab, 16);
        successEffectPool = new PoolAllocator<float>(successEffectPrefab, 2);

        var circle = macaroniPool.gameObjects[0].GetComponent<CircleCollider2D>();
        var macaroniTouchRadius = circle.radius * circle.transform.localScale.x;
        macaroniTouchRadiusSqr = macaroniTouchRadius * macaroniTouchRadius;

        float seconds_per_beat = 60f / beats_per_minute;
        float seconds_per_32_bar = seconds_per_beat / 8f;

        for(int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (c == '{') {
                in_bar = true;
                continue;
            }
            if (c == '}') {
                in_bar = false;
                bar_number++;
                continue;
            }
            if (in_bar) {
                if (c >= '0' && c <= '9')
                {
                    parsed_number *= 10;
                    parsed_number += c - '0';
                }
                if (c == ',')
                {
                    beat_times.Add((2*parsed_number + bar_number * 32) * seconds_per_32_bar);
                    parsed_number = 0;
                }
                if (c == '.')
                {
                    i+=2; //skip .5,
                    beat_times.Add((2*parsed_number + 1 + bar_number * 32) * seconds_per_32_bar);
                    parsed_number = 0;
                }
            }
        }
        for(int i=0; i < macaroniPool.active_object_count; i++)
        {
            macaroniPool.gameObjects[i].SetActive(false);
        }
        timer = -0.05f;
        beat_index = 0;
        macaroniPool.active_object_count = 0;
        score = 0;
        numberFailures = 0;
        scoreTracker.text = "Score: 0";
        failText.text = "";
        endScreenCanvas.enabled = false;
        ResetFailure();
        ResetCombo();
    }

    public void ResetState()
    {
        timer = -0.05f;
        beat_index = 0;
        for(int i=0; i < macaroniPool.active_object_count; i++)
        {
            macaroniPool.gameObjects[i].SetActive(false);
        }
        macaroniPool.active_object_count = 0;
        score = 0;
        numberFailures = 0;
        scoreTracker.text = "Score: 0";
        failText.text = "";

        ResetFailure();
        ResetCombo();

        endScreenCanvas.enabled = false;

        musicSource.Play();
    }


    private void CreateMacaroni(Vector3 initialPosition, Vector3 targetPosition, float rotationSpeed, float desiredTimeWhenReachedTarget)
    {
        var data = new MacaroniData();
        data.initialPosition = initialPosition;
        data.targetPosition = targetPosition;
        data.rotationSpeed = rotationSpeed;
        data.desiredTimeWhenReachedTarget = desiredTimeWhenReachedTarget;

        var obj = macaroniPool.CreateInstance(data);
        obj.GetComponent<SpriteRenderer>().color = Color.white;
        obj.transform.position = initialPosition;
    }

    void Update()
    {
        if (startUpCount > 0)
        {
            //This gives unity a little time to load everything, 
            //so that the music comes in sync with everything else.
            startUpCount--;
            if (startUpCount == 0)
                ResetState();
        }
        if (endScreenCanvas.enabled)
            return;

        float deltaTime = Time.deltaTime;

        while ( beat_index < beat_times.Count && timer + macaroniTravelDuration >= beat_times[beat_index])
        {
            float rotationSpeed = 2*(Mathf.Round(Random.value) - 0.5f) * Random.Range(10f, 600f);

            float angle = Random.Range(0, Mathf.PI * 2);
            Vector2 circlePoint = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector3 targetPoint = circlePoint * beatRadius;
            Vector3 startPoint = circlePoint * (Random.Range(minSpawnRadius, maxSpawnRadius));
            CreateMacaroni(startPoint, targetPoint, rotationSpeed, beat_times[beat_index]);
            beat_index++;
        }

        for(int i = 0; i < macaroniPool.active_object_count; i++)
        {
            var data = macaroniPool.metadata[i];
            var obj = macaroniPool.gameObjects[i];
            float progress = 1 - (data.desiredTimeWhenReachedTarget - timer) / macaroniTravelDuration;
            obj.transform.position = Vector3.LerpUnclamped(data.initialPosition, data.targetPosition, progress);
            obj.transform.Rotate(0, 0, data.rotationSpeed * deltaTime, Space.Self);

            if (progress > 1 && destroyTime != 0)
            {
                obj.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, (timer - data.desiredTimeWhenReachedTarget)/destroyTime);
            }

            var time_diff = Mathf.Abs(timer - data.desiredTimeWhenReachedTarget);
            if(time_diff < clickMargin)
            {
                obj.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }

        for(int i = macaroniPool.active_object_count - 1; i >= 0; i--)
        {
            if (macaroniPool.metadata[i].desiredTimeWhenReachedTarget < timer - destroyTime)
            {
                macaroniPool.DisableInstance(i);
                AddFailure();
            }
        }
        for(int i = successEffectPool.active_object_count - 1; i >= 0; i--)
        {
            successEffectPool.metadata[i] -= deltaTime;
            if (successEffectPool.metadata[i] <= 0)
            {
                successEffectPool.DisableInstance(i);
            }
        }

        timer += deltaTime;

        if(timer >= musicLength)
        {
            CongratulatePlayer();
        }
    }

    private void CongratulatePlayer()
    {
        hooraySoundEffect.Play();
        endScreenCanvas.enabled = true;
        gameoverText.enabled = false;
        congratulationsText.enabled = true;
        statisticsText.transform.parent.gameObject.SetActive(true);
        statisticsText.enabled = true;
        var result = PlayerPrefManager.GetAndOrUpdateHighscore("BeatPasta", score);
        var hackeronis = Mathf.RoundToInt(score/10);
        PlayerPrefManager.AddEarnedHackeronis(hackeronis);

        statisticsText.text = string.Format("{0}\n{1} {2}\n{3}", score, result.Item1 ? "(NEW!)" : null, result.Item2, hackeronis);
    }

    public void OnPointerDown (PointerEventData pointerData)
    {
        if (Time.timeScale == 0) {
            return;
        }
        bool hitSomeone = false;
        for (int i = macaroniPool.active_object_count - 1; i >= 0; i--)
        {
            var data = macaroniPool.metadata[i];
            var obj = macaroniPool.gameObjects[i];
            var diff = Camera.main.ScreenToWorldPoint(pointerData.position)- obj.transform.position;
            diff.z = 0;

            var time_diff = Mathf.Abs(timer - data.desiredTimeWhenReachedTarget);
            if(diff.sqrMagnitude < macaroniTouchRadiusSqr && time_diff < clickMargin)
            {
                macaroniPool.DisableInstance(i);
                if (numberFailures > 0)
                    ResetFailure();

                IncrementScore(obj);
                hitSomeone = true;
                break;
            }
        }
        if (hitSomeone == false)
        {
            ResetCombo();
        }
    }

    private void AddFailure()
    {
       numberFailures++; 
       failText.text = "Fail" + new string('!', numberFailures);
       failSoundEffect?.Play();
       if (numberFailures > 4)
       {
           endScreenCanvas.enabled = true;
           gameoverText.enabled = true;
           congratulationsText.enabled = false;
           statisticsText.transform.parent.gameObject.SetActive(false);
           musicSource.Stop();
       }
       ResetCombo();
    }
    private void ResetFailure()
    {
        numberFailures = 0;
        failText.text = "";
    }

    private void IncrementScore(GameObject obj)
    {
        successSoundEffect?.Play();
        successEffectPool.CreateInstance(0.5f).transform.position = obj.transform.position;
        comboCount++;
        score += baseScoreIncrement * calculateComboMultiplier();
        scoreTracker.text = "Score: " + score;
        comboText.text = "Combo: "+ comboCount;
        scoreMultiplierTracker.text = "x" + calculateComboMultiplier().ToString();
    }

    private void ResetCombo()
    {
        comboCount = 0;
        comboText.text = "Combo: "+ comboCount;
        scoreMultiplierTracker.text = "x" + calculateComboMultiplier().ToString();
    }

    private int calculateComboMultiplier()
    {
        if(comboCount < 5)
            return 1;
        else if (comboCount < 10)
            return 2;
        else if (comboCount < 18)
            return 4;
        else if (comboCount < 30)
            return 8;
        else return 16;
    }

    public void EndGame()
    {
        macaroniPool.Destroy();
        successEffectPool.Destroy();
        GetComponent<MinigameScene>().EndScene();
    }
}
public struct MacaroniData
{
    public Vector3 initialPosition;
    public Vector3 targetPosition;
    public float rotationSpeed;
    public float desiredTimeWhenReachedTarget;
}