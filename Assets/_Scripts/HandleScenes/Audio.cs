using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets._Scripts.HandleScenes
{
    public class Audio : MonoBehaviour
    {
        private AudioSource _AS;

        private void Start()
        {
            _AS = GetComponent<AudioSource>();
            StartCoroutine(FadeInSound(_AS, 0.4f));
            StartCoroutine(FadeOutSound(_AS, 0.3f));
        }

        private IEnumerator FadeInSound(AudioSource audioSource, float fadeDuration, float targetVolume = 1.0f)
        {
            audioSource.volume = 0f;
            audioSource.Play();

            while (audioSource.volume < targetVolume)
            {
                audioSource.volume += targetVolume * Time.deltaTime / fadeDuration;
                yield return null;
            }
        }
        private IEnumerator FadeOutSound(AudioSource audioSource, float fadeDuration)
        {
            float startVolume = audioSource.volume;

            while (audioSource.volume > 0)
            {
                audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
                yield return null;
            }

            audioSource.Stop();
            audioSource.volume = startVolume;
        }
    }
}
