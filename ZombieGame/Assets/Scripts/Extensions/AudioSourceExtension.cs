using System;
using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class AudioSourceExtension
    {
        public static bool TryPlay(this AudioSource audioSource)
        {
            bool wasPlayed = false;

            try
            {
                if (audioSource != null)
                {
                    audioSource.Play();
                }
            }
            catch (Exception ex)
            {
                // TODO - Logging
                wasPlayed = false;
            }

            return wasPlayed;
        }
    }
}