using System;
using System.Collections;
using GameManager;
using Misc;
using Unity.Mathematics;
using UnityEngine;
using Utilities;

namespace Sounds
{
    [DisallowMultipleComponent]
    public class SoundEffectManager : SingletonMonobehaviour<SoundEffectManager>
    {
        public int soundsVolume = 8;

        private void Start()
        {
            if (PlayerPrefs.HasKey("soundsVolume"))
            {
                soundsVolume = PlayerPrefs.GetInt("soundsVolume");
            }
            SetSoundsVolume(soundsVolume);
        }

        private void OnDisable()
        {
            PlayerPrefs.SetInt("soundsVolume", soundsVolume);
        }

        public void PlaySoundEffect(SoundEffectSO soundEffect)
        {
            SoundEffect sound =
                (SoundEffect) PoolManager.PoolManager.Instance.ReuseComponent(soundEffect.soundPrefab, Vector3.zero,
                    Quaternion.identity);
            sound.SetSound(soundEffect);
            sound.gameObject.SetActive(true);
            StartCoroutine(DisableSound(sound, soundEffect.soundEffectClip.length));
        }

        private IEnumerator DisableSound(SoundEffect sound, float length)
        {
            yield return new WaitForSeconds(length);
            sound.gameObject.SetActive(false);
        }

        public void IncreaseSoundsVolume()
        {
            int maxSoundsVolume = 20;
            if (soundsVolume >= maxSoundsVolume)
            {
                return;
            }

            soundsVolume++;
            SetSoundsVolume(soundsVolume);
        }
        
        public void DecreaseSoundsVolume()
        {
            if (soundsVolume == 0)
            {
                return;
            }

            soundsVolume--;
            SetSoundsVolume(soundsVolume);
        }

        private void SetSoundsVolume(int soundsVolume)
        {
            float muteDecibels = -80f;

            if (soundsVolume == 0f)
            {
                GameResources.Instance.soundsMasterMixerGroup.audioMixer.SetFloat("soundsVolume", muteDecibels);
            }
            else
            {
                GameResources.Instance.soundsMasterMixerGroup.audioMixer.SetFloat("soundsVolume",
                    HelperUtilities.LinearToDecibels(soundsVolume));
            }
        }
    }
}