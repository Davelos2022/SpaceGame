using UnityEngine;
using System.Collections.Generic;
using Zenject;

namespace SpaceGame.General
{
    public class AudioManager : MonoBehaviour
    {
        private AudioData _audioData;
        private Dictionary<string, AudioSource> _soundsMap;

        [Inject]
        public void Construct(AudioData audioData)
        {
            _audioData = audioData;
        }
        private void Awake()
        {
            Initialize();
        }

        private void OnEnable()
        {
            Play(AudioClipsName.Ost);
        }

        private void Initialize()
        {
            _soundsMap = new Dictionary<string, AudioSource>();

            foreach (var sound in _audioData.Audio)
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.clip = sound.Clip;
                source.volume = sound.Volume;
                source.pitch = sound.Pitch;
                source.loop = sound.Loop;
                _soundsMap[sound.SoundName] = source;
            }
        }
        public void Play(string name)
        {
            if (_soundsMap.TryGetValue(name, out AudioSource source))
            {
                source.Play();
            }
            else
            {
                ShowMessageNotFFound(name);
            }
        }

        public void Stop(string name)
        {
            if (_soundsMap.TryGetValue(name, out AudioSource source))
            {
                source.Stop();
            }
            else
            {
                ShowMessageNotFFound(name);
            }
        }

        private void ShowMessageNotFFound(string name)
        {
            Debug.LogWarning($"{name} - sound name not found, please check name in AudioClipsName or Audio Data");
        }
    }
}