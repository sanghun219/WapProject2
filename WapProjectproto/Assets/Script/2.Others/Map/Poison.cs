using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : MonoBehaviour
{
    //독의 데미지
    [SerializeField]
    private int PoisonDamage = 1;
  
    [SerializeField]
    private double poisonDelay = 6;

   

 

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {

            if (!collision.transform.GetComponent<Player>().isPoisoned)
            {
               
                collision.transform.GetComponent<Player>().StartPoisonedCorutine(PoisonDamage, poisonDelay);

            }

        }
    }
}
