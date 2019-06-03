using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Monster
{

    public enum BOSS_PHASE
    {
        ONE,
        TWO,
        END,
    }
    public BOSS_PHASE bossPhase;
    public BattleZone StartBattleZone;
    public Dialoguemanager dialoguemanager;
    public Transform movingActionZone;
    public bool isEndBossFirstAction = false;
    public float attackDepth = 3.0f;
    public float ChaseTime = 1.5f;
    private BoxCollider2D myCollider2D;
    public DialogueTrigger[] dialogueTriggers;
    public void BossFirstAction()
    {
        //대화종료시 보스 액션 ㄱ
        SoundManager.GetInst().PlayMusic("Boss");
        StartCoroutine(MovingAndAttackMotion());



    }

    public override IEnumerator DIE()
    {
        anim.SetTrigger("onDeath");
        isDead = true;
        if (target.position.x < transform.position.x)
        {

            transform.localScale = new Vector3(-5, 5, 5);
        }
        else if (target.position.x >= transform.position.x)
        {

            transform.localScale = new Vector3(5, 5, 5);
        }
        do
        {
            yield return null;
            if (isNewState) break;
            



        } while (!isNewState);
    }

    private IEnumerator MovingAndAttackMotion()
    {
        Vector3 moveVelocity = Vector3.zero;
        moveVelocity = Vector3.left;
        //기본 이미지가 너무작아서 크기를 조금 키움 인스펙터창 확인
        transform.localScale = new Vector3(-5, 5, 5);
        anim.SetBool("isMoving", true);
        while (Vector2.Distance(transform.position,movingActionZone.position)>3f)
        {
            transform.position += moveVelocity * Speed * Time.deltaTime;
            yield return null;
        }
        GameObject.Find("CM vcam1").GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).transform;
        isEndBossFirstAction = true;
        anim.SetBool("isMoving", false);
        ChangeMonsterState(MONSTER_STATUS.CHASE);
    }


   



  
    

    public override IEnumerator ATTACK()
    {
        //공격 애니메이션
        anim.SetTrigger("isAttack");
        do
        {
            yield return null;
            if (isNewState) break;



            if (attackTime >= attack)
            {
                attackTime = 0;
                anim.SetTrigger("isAttack");
                target.GetComponent<Player>().PlayerDamaged(damage);

            }
           
            if (Vector2.Distance(transform.position, target.position) >= myCollider2D.size.x * 2.7f
              && EndAnimationDone("Boss_Attack"))
            {
                anim.SetBool("isMoving", true);
                ChangeMonsterState(MONSTER_STATUS.CHASE);
                
            }
            





        } while (!isNewState);
    }

    public override void ChangeMonsterState(MONSTER_STATUS status)
    {
        base.ChangeMonsterState(status);
    }

    public override IEnumerator CHASE()
    {
        anim.SetBool("isMoving", true);
        do
        {
            yield return null;
            if (isNewState) break;

            if (Vector2.Distance(transform.position, target.position) >myCollider2D.size.x*2.7f)
            {
                Vector3 moveVelocity = Vector3.zero;
                moveVelocity = Vector3.Normalize(new Vector3(target.position.x - transform.position.x,0,0));

                if (target.position.x < transform.position.x)
                {

                    transform.localScale = new Vector3(-5, 5, 5);
                }
                else if (target.position.x >= transform.position.x)
                {

                    transform.localScale = new Vector3(5, 5, 5);
                }


                transform.position += new Vector3(moveVelocity.x,0,0) * Speed * Time.deltaTime;

            }
            else
            {
                anim.SetBool("isMoving", false);
                ChangeMonsterState(MONSTER_STATUS.ATTACK);
            }




        } while (!isNewState);
    }

    public override void DamagedByPlayerBullet(int damage)
    {
        base.DamagedByPlayerBullet(damage);
    }

    public override void Dead()
    {
        ChangeMonsterState(MONSTER_STATUS.DIE);
        OffThePhysicsComponent();
        //게임 클리어 이벤트 or 다음 페이즈 처리
        Invoke("DeathEvent", 1.0f);
        
    }

    public void DeathEvent()
    {
        DialogueTrigger d1 = System.Array.Find(dialogueTriggers, d => d.dialogue.name == "Death");
        d1.TriggerDialogue();    
        dialoguemanager.AddEventOnEndDialogue("Death", RealDeath);
    }
    public System.Action onDeath;
    public void RealDeath()
    {
        if (onDeath != null) onDeath();
        StartCoroutine(FadeOutOnDeath());
    }

    public IEnumerator FadeOutOnDeath()
    {
       
        for (float i = 1f; i >= 0; i -= 0.1f)
        {
            Color color = new Vector4(1, 1, 1, i);
            transform.GetComponent<SpriteRenderer>().color = color;
            yield return 0;
        }
        isDead = true;
        Destroy(gameObject, anim.GetCurrentAnimatorStateInfo(0).length);
        EventManager.GetInst().ClearGame();
    }

    public override IEnumerator MonsterFSM()
    {
        return base.MonsterFSM();
    }

    public void Awake()
    {
        StartBattleZone = GameObject.Find("BattleZone").GetComponent<BattleZone>();
        dialogueTriggers = GetComponents<DialogueTrigger>();
        myCollider2D = GetComponent<BoxCollider2D>();
       
        StartBattleZone.OnEndBattleZone += BossFirstAction;
        this.MaxHp = 30;
        this.Hp = this.MaxHp;
        attack = 1f;
    }

    //나중에 Update에 옮기자
    private void FixedUpdate()
    {
        attackTime += Time.deltaTime;
        UIinformationMgr.GetInst().SetBossHP(this.Hp,this.MaxHp);
    }
  

    private void OnCollisionEnter2D(Collision2D collision)
    {
       
    }
}
