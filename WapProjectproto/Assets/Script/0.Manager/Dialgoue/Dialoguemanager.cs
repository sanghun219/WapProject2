using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Dialoguemanager : MonoBehaviour
{


    public Queue<string> sentences;
    public Text dialogueText;
    public Animator animator;
    public string currentDialogueName;
   

    private void Awake()
    {
        sentences = new Queue<string>(); 
    }
   
    public void StartDialogue(Dialgoue dialogue)
    {
        currentDialogueName = dialogue.name;
        animator.SetBool("IsOpen", true);
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public System.Action OnEndDialogue;

    public void EndDialogue()
    {
       
        if (OnEndDialogue != null) OnEndDialogue();
      
        animator.SetBool("IsOpen", false);
        EventManager.GetInst().IsPause = false;
    }


    //특정 조건에 이벤트를 추가시킴 - 이름을 key로 해당 Dialogue인것을 확인하고 event발생
    public void AddEventOnEndDialogue(string name, System.Action _event =null)
    {
        if (currentDialogueName == name)
        {
            OnEndDialogue += _event;          
        }
       
    }
}
