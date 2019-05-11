using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    float bulletSpeed = 3.0f;
    public bool isRight;
    [SerializeField]
    GameObject DestroyBulletMotion;
    private Gun.GUNTYPE gunType;
   

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isRight)
        {
            transform.Translate(Vector3.right * Time.deltaTime * bulletSpeed);
        }
        else
        {
            transform.Translate(Vector3.left * Time.deltaTime * bulletSpeed);
        }

        Destroy(gameObject,2.5f);
    }

    public void SetGunType(Gun.GUNTYPE _gunType)
    {
        this.gunType = _gunType;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("Monster"))
        {

            if (collision.CompareTag("Monster"))
            {
                switch (this.gunType)
                {
                    case Gun.GUNTYPE.LASER:
                        break;
                    case Gun.GUNTYPE.NORMAL:
                        collision.GetComponent<Monster>().DamagedByPlayerBullet(1);
                        break;
                    case Gun.GUNTYPE.WHITE_LASER:
                        break;
                    default:
                        break;
                }
                
            }

            Destroy(gameObject);
            Destroy(Instantiate(DestroyBulletMotion, transform.position, Quaternion.identity), 0.3f);
        }
    }
}
