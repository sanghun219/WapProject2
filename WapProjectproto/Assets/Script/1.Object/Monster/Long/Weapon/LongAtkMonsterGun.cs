using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongAtkMonsterGun : MonoBehaviour
{
    public int damage;
    [SerializeField]
    private GameObject Bullet;
    public void UpdateLongAtkMonsterGunShoot(LongAtkMonster mon)
    {

        if (mon.ShootTime <= mon.shoot)
        {
            if (mon.transform.localScale.x > 0)
            {
                LongAtkMonBullet _bullet = Instantiate(Bullet, transform.position, Quaternion.identity).GetComponent<LongAtkMonBullet>();
                _bullet.damage = this.damage;
                _bullet.transform.localScale = new Vector3(1, 1, 1);
                _bullet.bulletSpeed = mon.AttackSpeed;
            }
            else
            {
                LongAtkMonBullet _bullet = Instantiate(Bullet, transform.position, Quaternion.identity).GetComponent<LongAtkMonBullet>();
                _bullet.damage = this.damage;
                _bullet.transform.localScale = new Vector3(-1, 1, 1);
                _bullet.bulletSpeed = mon.AttackSpeed;
            }
            mon.shoot = 0;
        }
    }
}
