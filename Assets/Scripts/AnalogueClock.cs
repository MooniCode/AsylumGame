using System;
using Unity.VisualScripting;
using UnityEngine;

public class AnalogueClock : MonoBehaviour
{
    [Header("Clock Hands")]
    [SerializeField] private Transform HourHand;
    [SerializeField] private Transform MinuteHand;

    [Header("Clock Settings")]
    [SerializeField] private float timeSpeed = 1f;

    private DateTime gameTime;
    private float timer;

    private void Start()
    {
        // Initialize game time
        gameTime = new DateTime(2024, 1, 1, 8, 0, 0);
        UpdateClockHands();
    }

    private void Update()
    {
        timer += timeSpeed * Time.deltaTime;

        if (timer >= 1f)
        {
            gameTime = gameTime.AddSeconds(1);
            timer = 0f;
            UpdateClockHands();
        }
    }

    private void UpdateClockHands()
    {
        // Calculate angles for the hands
        float hourAngle = CalculateHourAngle();
        float minuteAngle = CalculateMinuteAngle();

        // Apply rotation to the hands
        if (HourHand != null)
        {
            HourHand.rotation = Quaternion.Euler(hourAngle, 90, 0);
        }

        if (MinuteHand != null)
        {
            MinuteHand.rotation = Quaternion.Euler(minuteAngle, 90, 0);
        }
    }

    private float CalculateMinuteAngle()
    {
        float minuteAngle = (gameTime.Minute / 60f) * 360f;
        return minuteAngle;
    }

    private float CalculateHourAngle()
    {
        float totalMinutes = gameTime.Hour * 60 + gameTime.Minute;
        float hourAngle = (totalMinutes / 720f) * 360f;
        return hourAngle;
    }
}
