using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Helpers
{
    public class FadeHelper
    {
        private readonly Image Image;
        private readonly int Seconds;
        private readonly System.Drawing.Color Color;

        private float CurrentFade;

        public FadeHelper(Image image, int seconds, System.Drawing.Color color)
        {
            Image = image;
            Seconds = seconds;
            Color = color;
            CurrentFade = 0;
        }

        public void ShowImage()
        {
            CurrentFade = 1;
        }

        // Modified solution from ryanmillerca @ https://forum.unity.com/threads/simple-ui-animation-fade-in-fade-out-c.439825/#:~:text=139-,You%20can%20use%20coroutines%20to%20animate%20a%20fade%20in%20code%2C%20eg%3A,-Code%20(CSharp)%3A
        public IEnumerator FadeImageOut()
        {
            for (CurrentFade = 1; CurrentFade >= 0; CurrentFade -= Time.deltaTime / Seconds)
            {
                Image.color = new Color(Color.R, Color.G, Color.B, CurrentFade);
                yield return null;
            }
        }

        public IEnumerator FadeImageIn()
        {
            for (CurrentFade = 0; CurrentFade < 1; CurrentFade += Time.deltaTime / Seconds)
            {
                Image.color = new Color(Color.R, Color.G, Color.B, CurrentFade);
                yield return null;
            }
        }
    }
}
