using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Extensions;
using Assets.Scripts.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MainMenu
{
    // No longer being used
    public class MainMenuLogic : MonoBehaviour
    {
        private const int FadeTime = 30;

        private bool IsFirstFadeCall;
        private FadeHelper Fader;
        private List<AudioSource> AudioSources;

        private void Start()
        {
            Image backgroundImage = GameObject.Find(ObjectNames.Background).GetComponent<Image>();
            Fader = new FadeHelper(backgroundImage, FadeTime, System.Drawing.Color.White);
            Fader.ShowImage();
            IsFirstFadeCall = true;

            AudioSources = gameObject.GetComponents<AudioSource>().ToList();
            AudioSources[0].TryPlay();
        }

        private void Update()
        {
            if (!AudioSources[0].isPlaying)
            {
                if (IsFirstFadeCall)
                {
                    StartCoroutine(Fader.FadeImageOut());
                    IsFirstFadeCall = false;
                }

                if (!AudioSources[1].isPlaying)
                {
                    AudioSources[1].TryPlay();
                    AudioSources[1].loop = true;
                }
            }
        }
    }
}
