using System;
using TMPro;

namespace Assets.Scripts.HUD
{
    public class TimerHUD
    {
        private readonly TextMeshProUGUI HUDText;

        public TimerHUD(TextMeshProUGUI pointsHUD)
        {
            HUDText = pointsHUD;
        }

        public void UpdateHUD(float timerMs)
        {
            TimeSpan timer = TimeSpan.FromMilliseconds(timerMs);

            int minutes = (int)timer.TotalMinutes;
            int seconds = (int)timer.Subtract(new TimeSpan(0, minutes, 0)).TotalSeconds;
            int millisecs = (int)timer.Subtract(new TimeSpan(0, minutes, seconds)).TotalMilliseconds;

            string text;
            if (minutes >= 1)
            {
                text = $"{minutes}:{seconds.ToString("D2")}";
            }
            else if (millisecs >= 0)
            {
                text = $"{seconds}:{millisecs.ToString("D3")}";
                HUDText.color = UnityEngine.Color.red;
            }
            else
            {
                text = $"0:000";
                HUDText.color = UnityEngine.Color.black;
            }

            if (HUDText.text != text)
            {
                HUDText.text = text;
            }
        }
    }
}
