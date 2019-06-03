
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound 
{
    public enum SOUND_TYPE
    {
        MUSIC,
        SOUND,
        END,
    }
    public SOUND_TYPE soundType;
    public string name;
    public bool loop;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;

    [Range(.1f, 3f)]
    public float pitch;

   // [HideInInspector]
    public AudioSource source;
}
