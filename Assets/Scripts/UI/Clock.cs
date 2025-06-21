using System;
using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    private TMP_Text clockText;
    [SerializeField] private bool use24HoursFormat = true;
    [SerializeField] private float timeSpeed = 1f;

    private DateTime gameTime;
    private float timer;

    private void Start()
    {
        if (clockText == null)
            clockText = GetComponent<TMP_Text>();

        gameTime = new DateTime(2024, 1, 1, 8, 0, 0);
        UpdateClockDisplay();
    }

    private void Update()
    {
        timer += timeSpeed * Time.deltaTime;

        if (timer >= 1f)
        {
            gameTime = gameTime.AddSeconds(1);
            timer = 0f;

            if (gameTime.Minute % 15 == 0)
            {
                UpdateClockDisplay();
            }
        }
    }

    private void UpdateClockDisplay()
    {
        if (use24HoursFormat)
        {
            clockText.text = gameTime.ToString("HH:mm");
        }
        else
        {
            clockText.text = gameTime.ToString("hh:mm tt");
        }
    }

    public void SetTime(int hour, int minute, int second = 0)
    {
        gameTime = new DateTime(gameTime.Year, gameTime.Month, gameTime.Day, hour, minute, second);
        UpdateClockDisplay();
    }

    public void PauseClock()
    {
        timeSpeed = 0f;
    }

    public void ResumeClock()
    {
        timeSpeed = 1f;
    }

    public DateTime GetCurrentDateTime()
    {
        return gameTime;
    }
}
