using System;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Logger = Assets.Scripts.Singletons.Logger;

namespace Assets.Scripts.Intro
{
    public class IntroLogic : MonoBehaviour
    {
        private const int StartFadeTime = 12;
        private const int EndFadeTime = 5;
        private const int StartFadingOut = 60;
        private readonly DateTime IntroStart = DateTime.Now;
        private readonly Logger _logger = Logger.GetLogger();

        private bool StartedFadeOut = false;
        private AudioSource IntroAudio;
        private FadeHelper StartFader;
        private FadeHelper EndFader;

        private bool PlayerHitEsc => Input.GetKeyDown(KeyCode.Escape);

        private void Start()
        {
            IntroAudio = gameObject.GetComponent<AudioSource>();

            var image = GameObject.Find(ObjectNames.Background).GetComponent<Image>();
            StartFader = new FadeHelper(image, StartFadeTime, System.Drawing.Color.White);
            EndFader = new FadeHelper(image, EndFadeTime, System.Drawing.Color.White);

            StartCoroutine(StartFader.FadeImageIn());
        }

        private void Update()
        {
            if (IntroStart.AddSeconds(StartFadingOut) <= DateTime.Now && !StartedFadeOut)
            {
                StartCoroutine(EndFader.FadeImageOut());
                StartedFadeOut = true;
            }
            else if (!IntroAudio.isPlaying || PlayerHitEsc)
            {
                _logger.LogDebug($"Intro sequence over. Going to main menu. | AudioOver: {!IntroAudio.isPlaying} | PlayerHitEsc: {PlayerHitEsc}");
                SceneManager.LoadScene(SceneNames.MainMenu);
            }
        }
    }
}
