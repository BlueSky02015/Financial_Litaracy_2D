using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    private float elapsedTime;
    [SerializeField] private TMP_Text clockText;
    [SerializeField] private TMP_Text dateText;
    private float Timescale = 1000f;
    private float timeinaDay = 86400f; // 24 hours in seconds

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
}
