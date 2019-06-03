using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Monster
{

    public double ShootTime;
    public double shoot;
    public GhostGun Gun;
    public int UpDownMovingFlag;
    public void Shoot()
    {

        Gun.UpdateLongAtkMonsterGunShoot(this);
    }
    public override IEnumerator ATTACK()
    {
        do
        {
            yield return null;
            if (isNewState) break;

           
                if (Vector3.Distance(transform.position, target.position) > AttackRadius)
                {

                  
                    ChangeMonsterState(MONSTER_STATUS.CHASE);
                    break;
                }

            if (Vector2.Distance(transform.position, target.position) > AttackRadius)
            {
                if (Detect > DetectTime)
                {
                    ChangeMonsterState(MONSTER_STATUS.PATROL);
                    Detect = 0;
                }
                break;
            }


            if (target.position.x < transform.position.x)
            {

                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (target.position.x >= transform.position.x)
            {

                transform.localScale = new Vector3(-1, 1, 1);
            }
            if (shoot > ShootTime)
                Shoot();


        } while (!isNewState);
    }

    public override void ChangeMonsterState(MONSTER_STATUS status)
    {
        base.ChangeMonsterState(status);
    }

    public override IEnumerator CHASE()
    {
        do
        {
            yield return null;
            if (isNewState) break;

            
                if (Vector3.Distance(transform.position, target.position) < AttackRadius)
                {
                    ChangeMonsterState(MONSTER_STATUS.ATTACK);                
                    break;
                }
            

            if (Vector3.Distance(transform.position, target.position) > DetectRadius)
            {
                ChangeMonsterState(MONSTER_STATUS.PATROL);
                break;
            }


            Vector3 moveVelocity = Vector3.zero;
            moveVelocity = Vector3.Normalize(target.position-transform.position);

            if (target.position.x < transform.position.x)
            {
                
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (target.position.x >= transform.position.x)
            {
                
                transform.localScale = new Vector3(-1, 1, 1);
            }


            transform.position += moveVelocity * Speed * Time.deltaTime;



        } while (!isNewState);
    }

    public override void DamagedByPlayerBullet(int damage)
    {
        base.DamagedByPlayerBullet(damage);
    }

    public override void Dead()
    {
        base.Dead();
    }

    public override void InitMonster()
    {
        ShootTime = 1.0f;
        shoot = 0;
        Gun = transform.GetChild(0).GetComponent<GhostGun>();
        Gun.damage = this.damage;
    }

    public override IEnumerator MonsterFSM()
    {
        return base.MonsterFSM();
    }

    public override IEnumerator Move()
    {
        MovingFlag = Random.Range(0, 3);
        UpDownMovingFlag = Random.Range(0, 3);
        yield return new WaitForSeconds(3f);
        StartCoroutine("Move");
    }

    public override IEnumerator PATROL()
    {
        do
        {
            yield return null;
            if (isNewState) break;
            Vector3 moveVelocity = Vector3.zero;

            if (MovingFlag == 1)
            {
                moveVelocity = Vector3.left;
                transform.localScale = new Vector3(1, 1, 1);

            }
            else if (MovingFlag == 2)
            {
                moveVelocity = Vector3.right;
                transform.localScale = new Vector3(-1, 1, 1);
            }

            if (UpDownMovingFlag == 1)
            {
                moveVelocity += Vector3.up;
            }
            else if (UpDownMovingFlag == 2)
            {
                moveVelocity += Vector3.down;
            }


            transform.position += moveVelocity * Speed * Time.deltaTime;



        } while (!isNewState);
    }

    public override void UpdateMonster()
    {
        shoot += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            if (isAttacked)
            {
                ChangeMonsterState(MONSTER_STATUS.CHASE);
                isAttacked = false;
            }

        }

    }
}
