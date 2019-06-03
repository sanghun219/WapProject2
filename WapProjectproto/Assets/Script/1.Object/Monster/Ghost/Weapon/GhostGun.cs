using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostGun : MonoBehaviour
{
    public int damage;
    [SerializeField]
    private GameObject Bullet;
    public void UpdateLongAtkMonsterGunShoot(Ghost mon)
    {

        if (mon.ShootTime <= mon.shoot)
        {
            if (mon.transform.localScale.x > 0)
            {
                GhostBullet _bullet = Instantiate(Bullet, transform.position, Quaternion.identity).GetComponent<GhostBullet>();
                _bullet.damage = this.damage;
                _bullet.transform.localScale = new Vector3(1, 1, 1);
                _bullet.bulletSpeed = mon.AttackSpeed;
            }
            else
            {
                GhostBullet _bullet = Instantiate(Bullet, transform.position, Quaternion.identity).GetComponent<GhostBullet>();
                _bullet.damage = this.damage;
                _bullet.transform.localScale = new Vector3(-1, 1, 1);
                _bullet.bulletSpeed = mon.AttackSpeed;
            }
            mon.shoot = 0;
        }
    }
}
