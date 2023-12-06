using System;
using UnityEngine;
using Logger = Assets.Scripts.Singletons.Logger;

namespace Assets.Scripts.Extensions
{
    public static class AudioSourceExtension
    {
        public static bool TryPlay(this AudioSource audioSource)
        {
            bool wasPlayed = false;

            // If aditional extension methods are created, reconsider if should be global or still be a lazy singleton.
            Logger logger = Logger.GetLogger();

            try
            {
                logger.LogDebug($"Attempting to play audio. | AudioSourceName: {audioSource?.name}");
                if (audioSource != null)
                {
                    audioSource.Play();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while attempting to play audio. | AudioSourceName: {audioSource?.name}");
                wasPlayed = false;
            }

            return wasPlayed;
        }
    }
}