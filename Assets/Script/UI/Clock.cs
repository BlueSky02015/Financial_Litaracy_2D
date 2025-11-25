using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public static Clock instance;
    public float elapsedTime { get; private set; }
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private float Timescale = 1000f;
    private float timeinaDay = 86400f; // 24 hours in seconds
    private int currentDay = 1;

    [Header("UI")]
    [SerializeField] private TMP_Text clockText;
    [SerializeField] private TMP_Text dateText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        MorningTime();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale > 0f)
        {
            elapsedTime += Time.deltaTime * Timescale;
            float timeOfDay = elapsedTime % timeinaDay;
            ClockTextUI(timeOfDay);
            DateTextUI();
        }
    }

    void ClockTextUI(float timeOfDay)
    {
        if (clockText != null)
        {
            int totalSeconds = Mathf.FloorToInt(timeOfDay);
            int hours = (totalSeconds / 3600) % 24;
            int minutes = (totalSeconds / 60) % 60;
            int seconds = totalSeconds % 60;

            clockText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
    }

    void DateTextUI()
    {
        if (dateText != null)
        {
            // after evry 24 real-time minutes, the date increases by one day
            int daysPassed = Mathf.FloorToInt(elapsedTime / timeinaDay);
            currentDay = daysPassed;
            dateText.text = "Day " + currentDay.ToString();
        }
    }

    public void MorningTime()
    {
        int morning1 = 6 * 3600; // 6 AM 
        int morning2 = 8 * 3600; // 8 AM 
        elapsedTime = Random.Range(morning1, morning2);
    }

    // FOR Sleeping OR Time Skip
    public void JumpToMorning()
    {
        // Choose random time between 6 AM and 8 AM
        int morningStart = 6 * 3600;
        int morningEnd = 8 * 3600;
        int randomMorningSeconds = Random.Range(morningStart, morningEnd);

        // Set time to: start of next day + morning time
        // (so if it's 10 PM, you wake up next morning)
        elapsedTime = (currentDay + 1) * timeinaDay + randomMorningSeconds;

        // Update UI immediately
        float timeOfDay = elapsedTime % timeinaDay;
        ClockTextUI(timeOfDay);
        DateTextUI();
    }

    public void AddTimeHours(float hours)
    {
        if (hours <= 0) return;

        float secondsToAdd = hours * 3600f;
        elapsedTime += secondsToAdd;

        if (playerStats != null)
        {
            playerStats.UpdateStatsPerHour(hours);
        }

        // Update UI immediately
        float timeOfDay = elapsedTime % timeinaDay;
        ClockTextUI(timeOfDay);
        DateTextUI();
    }

    public void InitializeClockUI()
    {
        float timeOfDay = elapsedTime % timeinaDay;
        ClockTextUI(timeOfDay);
        DateTextUI();
    }

    public void SetElapsedTime(float time)
    {
        elapsedTime = time;
        InitializeClockUI();
    }

    public void SetCurrentDay(int day)
    {
        currentDay = day;
    }
    public int GetCurrentDay() => currentDay;
}
