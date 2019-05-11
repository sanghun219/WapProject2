using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum GUNTYPE
    {
        NORMAL,
        LASER,
        WHITE_LASER,
    }

   
    [SerializeField]
    GameObject bullet;
    [SerializeField]
    public GUNTYPE GunType;
   

   public void UpdateGunShoot(Player player)
    {
        
        if (player.ShootTime <=player.shoot)
        {
            if (player.transform.localScale.x > 0)
            {
                Bullet _bullet = Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<Bullet>();
                _bullet.isRight = true;
                _bullet.SetGunType(this.GunType);
            }
            else
            {
                Bullet _bullet = Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<Bullet>();
                _bullet.isRight = false;
                _bullet.SetGunType(this.GunType);
            }
            player.shoot = 0;
        }
        
    }
}
