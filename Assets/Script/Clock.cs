using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public static Clock instance;
    public float elapsedTime {get; private set; }
    [SerializeField] private TMP_Text clockText;
    [SerializeField] private TMP_Text dateText;
    private float Timescale = 1000f;
    private float timeinaDay = 86400f; // 24 hours in seconds
    
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
        elapsedTime += Time.deltaTime * Timescale;
        float timeOfDay = elapsedTime % timeinaDay;
        ClockTextUI(timeOfDay);
        DateTextUI();
    }

    void ClockTextUI(float timeOfDay)
    {
        int totalSeconds = Mathf.FloorToInt(timeOfDay);
        int hours = (totalSeconds / 3600) % 24;
        int minutes = (totalSeconds / 60) % 60;
        int seconds = totalSeconds % 60;

        clockText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }

    void DateTextUI()
    {
        // after evry 24 real-time minutes, the date increases by one day
        int daysPassed = Mathf.FloorToInt(elapsedTime / timeinaDay);
        int currentDay = daysPassed + 1; // Start from Day 1
        dateText.text = "Day " + currentDay.ToString();
    }

    void MorningTime()
    {
        // Initialize elapsed time between 6 am & 8 AM
        int morning1 = 6 * 3600; // 6 AM in seconds
        int morning2 = 8 * 3600; // 8 AM in seconds 
        elapsedTime = Random.Range(morning1, morning2);
    }

    public void JumpToMorning()
{
    // Get current day (before jump)
    int currentDay = Mathf.FloorToInt(elapsedTime / timeinaDay);

    // Choose random time between 6 AM and 8 AM
    int morningStart = 6 * 3600; // 6 AM
    int morningEnd = 8 * 3600;   // 8 AM
    int randomMorningSeconds = Random.Range(morningStart, morningEnd);

    // Set time to: start of next day + morning time
    // (so if it's 10 PM, you wake up next morning)
    elapsedTime = (currentDay + 1) * timeinaDay + randomMorningSeconds;

    // Update UI immediately
    float timeOfDay = elapsedTime % timeinaDay;
    ClockTextUI(timeOfDay);
    DateTextUI();
}
}
