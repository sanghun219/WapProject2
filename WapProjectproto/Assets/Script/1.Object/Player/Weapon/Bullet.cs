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
    private int damage;
    void GunShot()
    {
     
        if (isRight)
        {
            transform.Translate(Vector3.right * Time.deltaTime * bulletSpeed);
        }
        else
        {
            transform.Translate(Vector3.left * Time.deltaTime * bulletSpeed);
        }
    }

  

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (gunType)
        {
            case Gun.GUNTYPE.NORMAL:
                GunShot();
                break;
            case Gun.GUNTYPE.LASER:
                GunShot();
                break;
            case Gun.GUNTYPE.WHITE_LASER:
                break;
            default:
                break;
        }
      
        Destroy(gameObject, 2.5f);
    }

    public void SetGunType(Gun.GUNTYPE _gunType,int damage =1, double bulletSpeed =10)
    {
        this.gunType = _gunType;
        this.damage = damage;
        this.bulletSpeed = (float)bulletSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("Monster")||collision.CompareTag("Flying") || collision.CompareTag("Boss"))
        {

            if (collision.CompareTag("Monster") || collision.CompareTag("Flying") || collision.CompareTag("Boss"))
            {
                collision.GetComponent<Monster>().DamagedByPlayerBullet(this.damage);

            }

            Destroy(gameObject);
            Destroy(Instantiate(DestroyBulletMotion, transform.position, Quaternion.identity), 0.3f);
        }
    }
}
