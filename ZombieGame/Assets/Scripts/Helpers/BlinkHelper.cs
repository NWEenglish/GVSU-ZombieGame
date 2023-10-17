using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Helpers
{
    public class BlinkHelper
    {
        private Image Image;
        private Color Color;
        private double LengthOfTime;
        private DateTime StartShowing;

        public BlinkHelper(Image image, Color color, double lengthOfTime)
        {
            Image = image;
            Color = color;
            LengthOfTime = lengthOfTime;
        }

        public void Start()
        {
            StartShowing = DateTime.Now;
            Image.color = new Color(Color.r, Color.g, Color.b, 1);
        }

        public void TryEnd()
        {
            if (StartShowing.AddSeconds(LengthOfTime) < DateTime.Now)
            {
                Image.color = new Color(Color.r, Color.g, Color.b, 0);
            }
        }
    }
}
