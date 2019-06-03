using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class checkPlayTime : MonoBehaviour
{
    [SerializeField]
    Text playTimeText;
    private float timer;
    public string finalTime;
    private void Awake()
    {
        timer = 0;
    }
    private void Update()
    {
        timer += Time.deltaTime;
       
    }

    public void SavePlayTime()
    {
        finalTime = "" + timer.ToString("00.00");
        finalTime = finalTime.Replace(".", ":");
        /*string.Format("0:0.0",Mathf.Ceil(timer).ToString());*/
        timer = 0;
    }
}
