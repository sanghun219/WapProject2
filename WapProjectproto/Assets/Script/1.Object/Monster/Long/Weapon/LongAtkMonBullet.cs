using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongAtkMonBullet : MonoBehaviour
{
    public float bulletSpeed =6f;
    public int damage;
    [SerializeField]
    private Vector3 targetPos;
    
    public void Start()
    {
        targetPos = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).transform.position;
    }
    void FixedUpdate()
    {
        transform.position += Vector3.Normalize(targetPos - transform.position) * bulletSpeed * Time.fixedDeltaTime;
        
        if (Vector3.Distance(transform.position,targetPos) <0.3f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("Player"))
        {
            if (collision.CompareTag("Player"))
            {
                collision.GetComponent<Player>().PlayerDamaged(damage);
            }
            Destroy(gameObject);
            
        }
    }
}
