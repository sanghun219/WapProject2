using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class BattleZone : MonoBehaviour
{
    private bool isFirstBattle = false;
    [SerializeField]
    private DialogueTrigger[] dialogueTrigger;
    [SerializeField]
    private Cinemachine.CinemachineVirtualCamera mainCamera;
    [SerializeField]
    private Dialoguemanager dialogueManager;

    //나중에 Init으로 바꾸자
    void Awake()
    {
        dialogueTrigger = GameObject.Find("Boss").GetComponents<DialogueTrigger>();
        dialogueManager.OnEndDialogue += EndBattleZone;
    }
    public void StartBattleZone()
    {
        //Pause는 EndDialogue에서 false가 된다.
        EventManager.GetInst().IsPause = true;
        mainCamera.Follow = GameObject.FindGameObjectWithTag("Boss").transform;
        DialogueTrigger d1 = Array.Find(dialogueTrigger, d => d.dialogue.name == "Start");
        d1.TriggerDialogue();

       
       
    }


    public System.Action OnEndBattleZone;

    //DialogueManager.EndDialogue()에 Listner로 묶임
    public void EndBattleZone()
    {
        if (isFirstBattle) return;
        if (OnEndBattleZone != null) OnEndBattleZone();

       
        EventManager.GetInst().IsPause = false;
        isFirstBattle = true;

        //배틀존 종료후 몬스터에게 AI제어권을 넘겨주자
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (isFirstBattle) return;
        
        if (collision.transform.CompareTag("Player"))
        {
           
            StartBattleZone();
            isFirstBattle = false;
        }
    }
}
