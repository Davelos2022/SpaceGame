using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Audio Data", menuName = "Audio")]
public class AudioData: ScriptableObject
{
    public Sound[] Audio;
}


[Serializable]
public class Sound
{
    public string SoundName;
    public AudioClip Clip;
    [Range(0f, 1f)] public float Volume = 0.7f;
    [Range(0.5f, 1.5f)] public float Pitch = 1f;
    public bool Loop = false;
}