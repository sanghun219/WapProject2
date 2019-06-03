using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameObject SettingCanvas;
    private GameObject MenuCanvas;
    private GameObject LoadingCanvas;

    [SerializeField]
    private Toggle MusicToggle;
    [SerializeField]
    private Toggle SoundToggle;

    private int isOnMusicToggle;
    private int isOnSoundToggle;
    
    private void Awake()
    {
        SettingCanvas = GameObject.Find("Setting");
        MenuCanvas = GameObject.Find("Menu");
        LoadingCanvas = GameObject.Find("Loading");


        if (SettingCanvas.activeInHierarchy)
            SettingCanvas.SetActive(false);
        if (LoadingCanvas.activeInHierarchy)
            LoadingCanvas.SetActive(false);
        if (!MenuCanvas.activeInHierarchy)
            MenuCanvas.SetActive(true);


        if (!PlayerPrefs.HasKey("Music") || !PlayerPrefs.HasKey("Sound"))
        {
            return;
        }

        if (PlayerPrefs.GetInt("Music") == 1)
        {
            MusicToggle.isOn = true;
        }
        else
        {
            MusicToggle.isOn = false;
            
        }

        if (PlayerPrefs.GetInt("Sound") == 1)
        {
            SoundToggle.isOn = true;
        }
        else
        {
            SoundToggle.isOn = false;
           
        }

       
    }

    public void ChangeSettingAndMenuCanvas()
    {
        if (MenuCanvas.activeInHierarchy)
        {
            MenuCanvas.SetActive(false);
            if (!SettingCanvas.activeInHierarchy)
                SettingCanvas.SetActive(true);
        }
        else
        {
            MenuCanvas.SetActive(true);
            if (SettingCanvas.activeInHierarchy)
                SettingCanvas.SetActive(false);
        }
    }

    public void ChangeTurnMusic()
    {
        SoundManager.GetInst().ChaingingMusic();
        if (MusicToggle.isOn)
        {      
            isOnMusicToggle = 1;
        }
        else
        {            
            isOnMusicToggle = 0;
           
        }

        PlayerPrefs.SetInt("Music", isOnMusicToggle);

    }
    public void ChangeTurnSound()
    {
        SoundManager.GetInst().ChaingingSound();
        if (SoundToggle.isOn)
        {       
            isOnSoundToggle = 1;
        }
        else
        {        
            isOnSoundToggle = 0;
           
        }

        PlayerPrefs.SetInt("Sound", isOnSoundToggle);
    }

    public void ClickButtonSound(int NumofSound)
    {
        switch (NumofSound)
        {
            case 1:
                SoundManager.GetInst().PlaySound("click");
                break;
            case 2:
                SoundManager.GetInst().PlaySound("exit");
                break;
            case 3:
                SoundManager.GetInst().PlaySound("about");
                break;
        }
    }
    public void ExitGameGiveDelay()
    {
        StartCoroutine(ExitApplicationGiveDelay());
    }
    public IEnumerator ExitApplicationGiveDelay()
    {
        float delay = 1.0f;
        while (delay > 0)
        {
            yield return null;
            delay -= 0.1f;          
        }
        EventManager.GetInst().ExitApplication();
    }
}
