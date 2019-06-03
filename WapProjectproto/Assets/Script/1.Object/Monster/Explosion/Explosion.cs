using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : Monster
{
    #region override_Monster
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

    public override IEnumerator PATROL()
    {
        do
        {
            yield return null;
            if (isNewState) break;
            Vector3 moveVelocity = Vector3.zero;

            if (MovingFlag == 1)
            {
                moveVelocity = Vector3.right;
                transform.localScale = new Vector3(-1, 1, 1);
                anim.SetBool("isAttack", true);

            }
            else if (MovingFlag == 2)
            {
                moveVelocity = Vector3.left;
                transform.localScale = new Vector3(1, 1, 1);
                anim.SetBool("isAttack", true);
            }
            else
            {
                anim.SetBool("isAttack", false);
            }

            transform.position += moveVelocity * Speed * Time.deltaTime;



        } while (!isNewState);
    }
    #endregion

    #region BoomberFunction

 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            if (isAttacked)
            {
                ChangeMonsterState(MONSTER_STATUS.CHASE);
                anim.SetBool("isAttack", true);
                Speed = Mathf.Clamp(Speed + 5, 1, 15);
            }
        }

    }

    private void Bomb()
    {

        target.GetComponent<Player>().PlayerDamaged(damage);
        Dead();
    }


    public override IEnumerator CHASE()
    {
        do
        {
            yield return null;
            if (isNewState) break;
           
            if (Vector2.Distance(transform.position, target.position) < 1.0f)
            {
                Bomb();
                break;
            }

            if (Vector2.Distance(transform.position, target.position) > DetectRadius)
            {
                ChangeMonsterState(MONSTER_STATUS.PATROL);
             
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



    #endregion

}
