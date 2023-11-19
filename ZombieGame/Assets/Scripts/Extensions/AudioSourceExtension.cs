using System;
using UnityEngine;
using Logger = Assets.Scripts.Singletons.Logger;

namespace Assets.Scripts.Extensions
{
    public static class AudioSourceExtension
    {
        private static readonly Logger _logger = Logger.GetLogger();

        public static bool TryPlay(this AudioSource audioSource)
        {
            bool wasPlayed = false;

            try
            {
                _logger.LogDebug($"Attempting to play audio. | AudioSourceName: {audioSource.name}");
                if (audioSource != null)
                {
                    audioSource.Play();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while attempting to play audio. | AudioSourceName: {audioSource.name}");
                wasPlayed = false;
            }

            return wasPlayed;
        }
    }
}