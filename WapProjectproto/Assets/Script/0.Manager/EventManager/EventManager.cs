using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    #region Singleton
    private static EventManager m_Inst = null;
    public static EventManager GetInst()
    {
        
        if (m_Inst == null)
        {
            m_Inst = FindObjectOfType(typeof(EventManager)) as EventManager;
            if (m_Inst == null)
                Debug.Log("EventMgr is'nt Exist");
        }
            
        return m_Inst;
    }
    #endregion
    public bool IsGameOver = false;
    public GameObject gameOverUI;
    private Fade fader;
    private Text GameOverText;
    private Button GameOverButton;
    public Player player;
    public GameObject victoryUI;
    public Text SuccessText;
    public Text TimerText;
    public Button ExitButton;
    public Button ReturnButton;
    public bool IsPause;
    public bool IsClear;

    public checkPlayTime playTime;
    //게임오버시 fade효과 후 ActivateGameOverUI 이벤트를 실행시킴.
    public void GameOver()
    {
        IsGameOver = true;              
        // 게임오버시 처리할 것들 ...
        fader.FadeOut(ActivateGameOverUI);
        
    }
    //게임클리어시 fade효과 후 ActivateClearUI 이벤트를 실행시킴.
    public void ClearGame()
    {
        IsClear = true;
        playTime.SavePlayTime();
        fader.FadeOut(ActivateClearUI);

    }
    //비활성화 상태인 UI를 활성화시킴.
    public void ActivateClearUI()
    {
        TimerText.text = playTime.finalTime;
        SuccessText.gameObject.SetActive(true);
        TimerText.gameObject.SetActive(true);
        ReturnButton.gameObject.SetActive(true);
        ExitButton.gameObject.SetActive(true);
        SoundManager.GetInst().AllStopMusic();
        SoundManager.GetInst().PlaySound("Clear");
    }
    //어플리케이션 종료
    public void ExitApplication()
    {

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
    //비활성화된 UI를 활성화 시킴.
    public void ActivateGameOverUI()
    {
        if (!GameOverButton.gameObject.activeInHierarchy)
        {
            GameOverButton.gameObject.SetActive(true);
        }
        if (!GameOverText.gameObject.activeInHierarchy)
            GameOverText.gameObject.SetActive(true);
    }

    //메뉴로 돌아가는 버튼 클릭이벤트에 바인딩
    public void ReturnMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    
    public void EventInit()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).GetComponent<Player>();
        gameOverUI = GameObject.Find("EventUI").transform.Find("GameOver").gameObject;
        fader = GameObject.Find("EventUI").transform.Find("Fade").GetComponent<Fade>();
        GameOverText = gameOverUI.transform.Find("GameOverText").GetComponent<Text>();
        GameOverButton = gameOverUI.transform.Find("Return").GetComponent<Button>();

        victoryUI = GameObject.Find("EventUI").transform.Find("Victory").gameObject;
        SuccessText = victoryUI.transform.Find("SuccessText").GetComponent<Text>();
        TimerText = victoryUI.transform.Find("PlayTimeText").GetComponent<Text>();
        ReturnButton = victoryUI.transform.Find("Return").GetComponent<Button>();
        ExitButton = victoryUI.transform.Find("Exit").GetComponent<Button>();

   

        //플레이어가 죽었을 때 바로 GameOver 함수를 실행시키기 위함.
        player.OnDeath += GameOver;


       
    }

}
