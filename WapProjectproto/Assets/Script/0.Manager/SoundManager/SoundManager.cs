using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;

    #region SINGLETON
    private static SoundManager m_Inst;
    public static SoundManager GetInst()
    {
        if (m_Inst == null)
        {
            m_Inst = FindObjectOfType(typeof(SoundManager)) as SoundManager;

            if (m_Inst == null)
                Debug.Log("SoundManager isn't exist");
        }

        return m_Inst;
    }
    #endregion

    public void InitSoundinAwake()
    {
       
        foreach (Sound s in sounds)
        {          
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume; 
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    public void AllStopMusic()
    {
        foreach (Sound other in sounds)
        {
            other.source.Stop();
        }
    }
    public void PlayMusic(string name)
    {
        if (PlayerPrefs.GetInt("Music") == 0)
            return;

        AllStopMusic();
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        s.soundType = Sound.SOUND_TYPE.MUSIC;
        s.source.mute = false;
       


        s.source.Play();
        
    }

    public void PlaySound(string name)
    {
        if (PlayerPrefs.GetInt("Sound") == 0)
            return;
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        s.soundType = Sound.SOUND_TYPE.SOUND;
        s.source.mute = false;
        s.source.Play();
        
    }

    public void ChaingingSound()
    {
        foreach (Sound s in sounds)
        {
            if (s.source == null) return;
            if (s.soundType == Sound.SOUND_TYPE.SOUND)
            {
                if (s.source.mute == false)
                {
                    s.source.mute = true;
                }
                else
                {
                    s.source.mute = false;
                }
            }
              
        }
    }

    public void ChaingingMusic()
    {
       
        foreach (Sound s in sounds)
        {
            if (s.source == null) return;
            if (s.soundType == Sound.SOUND_TYPE.MUSIC)
            {
                if (s.source.mute == false)
                {
                    s.source.mute = true;
                }
                else
                {
                    s.source.mute = false;
                }
            }
        }
    }

}
