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
            Logger logger = Logger.GetLogger(); // If aditional extension methods created, reconsider if should be global and lazy singleton.

            try
            {
                logger.LogDebug($"Attempting to play audio. | AudioSourceName: {audioSource.name}");
                if (audioSource != null)
                {
                    audioSource.Play();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while attempting to play audio. | AudioSourceName: {audioSource.name}");
                wasPlayed = false;
            }

            return wasPlayed;
        }
    }
}