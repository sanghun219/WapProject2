using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DialogueTrigger : MonoBehaviour
{
   
    public Dialgoue dialogue;
    //Dialogue이면 UI 대화창 형식으로 나옴
    //System이면 잠시 깜빡이고 사라짐.
    public enum TRIGGER_TYPE
    {
        DIALOGUE,
        SYSTEM,
    }
    public enum SYSTEM_TYPE
    {
        PORTAL,
        BOSS,
    }
   
    //Dialogue는 UI animation이 추가된 것(보스방 대화용)
    public void TriggerDialogue()
    {
        FindObjectOfType<Dialoguemanager>().StartDialogue(dialogue);
    }
    //시스템 알림처럼 text만 active되는 것 ('몬스터를 다 잡으시오' 같은 문구에 이용)
    public void TriggerSystemText()
    {
        FindObjectOfType<EventScript>().StartDialogue(dialogue);
    }

    
   
}
