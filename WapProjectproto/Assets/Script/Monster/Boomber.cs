using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomber : Monster
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
                transform.localScale = new Vector3(1, 1, 1);

            }
            else if (MovingFlag == 2)
            {
                moveVelocity = Vector3.left;
                transform.localScale = new Vector3(-1, 1, 1);
            }

            transform.position += moveVelocity * Speed * Time.deltaTime;



        } while (!isNewState);
    }
    #endregion

    #region BoomberFunction

    //나중에 업데이트 함수로 바꿔서 room update에 넣어주자!
    public void FixedUpdate()
    {
       
        
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

    private void Bomb()
    {
        anim.SetTrigger("Bomb");
    }


    public IEnumerator CHASE()
    {
        do
        {
            yield return null;
            if (isNewState) break;

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
