using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubPortal : MonoBehaviour
{
    [SerializeField]
    GameObject JunctionPortal;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetKeyDown(KeyCode.UpArrow))
        {
            collision.transform.position = JunctionPortal.transform.position;
        }
    }
}
