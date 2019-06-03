using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    private Image fader;
    
    public float animTime = 2f;

    private float start = 0f;
    private float end = 1f;
    private float time = 0f;

    private bool isPlaying = false;

    private void Awake()
    {
        fader = gameObject.GetComponent<Image>();
    }

    public void FadeOut(System.Action nextEvent =null)
    {
        
        if (isPlaying == true) return;
        StartCoroutine(PlayFadeOut(nextEvent));
    }

   
    public IEnumerator PlayFadeOut(System.Action nextEvent =null)
    {
        isPlaying = true;

        Color color = fader.color;
        time = 0f;
        color.a = Mathf.Lerp(start, end, time);

        while (color.a < 1f)
        {
            time += Time.deltaTime / animTime;
            color.a = Mathf.Lerp(start, end, time);
            fader.color = color;

            yield return null;
        }

        isPlaying = false;
        if (nextEvent != null) nextEvent();
    }

    


}
