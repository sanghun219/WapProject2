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
    GameObject[] bullets;
    [SerializeField]
    public GUNTYPE GunType = GUNTYPE.NORMAL;
    public double bulletSpeed;

   public void UpdateGunShoot(Player player)
    {
        
        if (player.ShootTime <=player.shoot)
        {
            if (player.transform.localScale.x > 0)
            {
                Bullet _bullet = Instantiate(bullets[(int)this.GunType], transform.position, Quaternion.identity).GetComponent<Bullet>();
                switch (this.GunType)
                {
                    case GUNTYPE.LASER:
                        _bullet.SetGunType(this.GunType, 3,20);
                        break;
                    case GUNTYPE.NORMAL:
                        _bullet.SetGunType(this.GunType, 1, 10);
                        break;
                    case GUNTYPE.WHITE_LASER:
                        break;
                }
                _bullet.isRight = true;
              
            }
            else
            {
                Bullet _bullet = Instantiate(bullets[(int)this.GunType], transform.position, Quaternion.identity).GetComponent<Bullet>();
                _bullet.transform.localScale = new Vector3(-_bullet.transform.localScale.x, _bullet.transform.localScale.y, _bullet.transform.localScale.z);
                switch (this.GunType)
                {
                    case GUNTYPE.LASER:
                        _bullet.SetGunType(this.GunType, 3, 20);
                        break;
                    case GUNTYPE.NORMAL:
                        _bullet.SetGunType(this.GunType, 1, 10);
                        break;
                    case GUNTYPE.WHITE_LASER:
                        break;
                }

                _bullet.isRight = false;

            }
            player.shoot = 0;
        }
        
    }
}
