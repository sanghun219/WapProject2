using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventScript : MonoBehaviour
{

    public Queue<string> sentences;

    public Text dialogueText;
    public Animator animator;


    private void Awake()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialgoue dialogue)
    {
       
        animator.SetBool("isActive", true);
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
        dialogueText.text = sentence;
        Invoke("EndDialogue", 2f);
    }

   

    void EndDialogue()
    {
        animator.SetBool("isActive", false);
    }


}
