using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


// 이 클래스는 외부 자료 그대로를 사용함
public class LoadingSceneManager : MonoBehaviour
{
    public Slider slider;
    public Text slider_Percent;
    public int percent;
    public float maxTime = 1;
    bool IsDone = false;
    float fTime = 0f;
    AsyncOperation async_operation;
    public GameObject LoadingCanvas;

    //다음 신으로 넘어가는 함수. UI버튼들에 바인딩 됨.
    public void LoadingNextScene(string sceneName)
    {
        if (!LoadingCanvas.activeInHierarchy)
            LoadingCanvas.SetActive(true);
        StartCoroutine(UpdateTime());
        StartCoroutine(StartLoad(sceneName));
        
    }

    //메뉴->게임 로딩시 화면 전환용 함수
    IEnumerator UpdateTime()
    {
        while (true)
        {
            fTime += Time.deltaTime;
            slider.value = fTime;
            percent = (int)(100f*(slider.value));
            slider_Percent.text = percent.ToString() + "%";
            if (fTime >= maxTime)
            {
                async_operation.allowSceneActivation = true;
                break;
            }
          
            yield return null;
        }
        
       
    }

    public IEnumerator StartLoad(string strSceneName)
    {
        async_operation = SceneManager.LoadSceneAsync(strSceneName);
        async_operation.allowSceneActivation = false;

        if (IsDone == false)
        {
            IsDone = true;

            while (async_operation.progress < 0.9f)
            {
                slider.value = async_operation.progress;

                yield return true;
            }
        }
    }


   
   
}
