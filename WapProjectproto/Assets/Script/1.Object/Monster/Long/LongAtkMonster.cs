using System.Collections;
using UnityEngine;

public class LongAtkMonster : Monster
{
    public double ShootTime;
    public double shoot;
    public LongAtkMonsterGun Gun;

    public override void InitMonster()
    {
        ShootTime = 1.0f;
        shoot = 0;
        Gun = transform.GetChild(0).GetComponent<LongAtkMonsterGun>();
        Gun.damage = this.damage;
    }

    public override void UpdateMonster()
    {
        shoot += Time.deltaTime;
        Detect += Time.deltaTime;
    }

    public void Shoot()
    {
        
        Gun.UpdateLongAtkMonsterGunShoot(this);
    }

    public override void ChangeMonsterState(MONSTER_STATUS status)
    {
        base.ChangeMonsterState(status);
    }

    public override IEnumerator MonsterFSM()
    {
        
        return base.MonsterFSM();
    }

    public override IEnumerator Move()
    {
        MovingFlag = Random.Range(0, 3);
        yield return new WaitForSeconds(3f);
        StartCoroutine("Move");
    }


    public override IEnumerator ATTACK()
    {
        do
        {
            yield return null;
            if (isNewState) break;

                //인지 범위내에서 벗어나면 Patrol로 바꿈
                if (Vector2.Distance(transform.position, target.position) > DetectRadius)
                {
            
                    ChangeMonsterState(MONSTER_STATUS.PATROL);
                    break;
                }
                //공격 범위에서 벗어났을 떄 인지 시간이 지나면 patrol로 바꿈 아닐시에는 계속 attack상태
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

    public override IEnumerator CHASE()
    {
        do
        {
            yield return null;
            if (isNewState) break;

           
                if (Vector2.Distance(transform.position, target.position) < AttackRadius)
                {
                    ChangeMonsterState(MONSTER_STATUS.ATTACK);
                  
                    break;
                }
            

            if (Vector2.Distance(transform.position, target.position) > DetectRadius)
            {
                ChangeMonsterState(MONSTER_STATUS.PATROL);
                break;
            }
            

            Vector3 moveVelocity = Vector3.zero;

            
            if (target.position.x < transform.position.x)
            {
                moveVelocity = Vector3.left;
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (target.position.x >= transform.position.x)
            {
                moveVelocity = Vector3.right;
                transform.localScale = new Vector3(-1, 1, 1);
            }


            transform.position += moveVelocity * Speed * Time.deltaTime;



        } while (!isNewState);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            if (isAttacked)
            {
                ChangeMonsterState(MONSTER_STATUS.CHASE);
            }
           
        }

    }
}
