﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Monster : MonoBehaviour
{
    public enum MONSTER_TYPE
    {
        SHORT_ATK,
        BOOM_ATK,
        LONG_ATK,
        SHORT_SPEED_ATK,
        MIDDLE_BOSS,
        BOSS,
        END,

    }
    public enum MONSTER_STATUS
    {
        
        PATROL,
        CHASE,
        ATTACK,
        DIE,
        END,
    }
    
    public MONSTER_TYPE monsterType;
    public MONSTER_STATUS monsterStatus;
    public float Speed; 
    public float AttackSpeed;
    public float attack;
    public float attackTime;
    public int Hp;
    public int MaxHp;
    public int MovingFlag;
    public int damage;
    public bool isDead;
    public bool isAttacked;
    public double DetectRadius;
    public double AttackRadius;
    public double Detect;
    public double DetectTime;
    public bool isAtacking;
    public bool isTracing;
    public Transform target;
    public IEnumerator currentState;
    public bool isNewState;
    public Animator anim;
   

    //나중에 함수로 만들어서 Room에서 실행, Test용
    public void OnEnable()
    {
        StartCoroutine("MonsterFSM");
        StartCoroutine("Move");
        monsterStatus = MONSTER_STATUS.PATROL;
        target = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).transform;
        anim = gameObject.GetComponent<Animator>();
    }
    //이건 자식 클래스에서 그대로 사용하자, 모든 몬스터가 동일
    public virtual void InitMonster()
    {
        
       
    }

    public virtual void UpdateMonster()
    {
        
    }

    //상태가 변화하면 bool값을 바꿔서 진행중이던 모션을 멈추고 새로운 모션을 실행한다.
    //이건 자식 클래스에서 그대로 사용하자, 모든 몬스터가 동일
    virtual public void ChangeMonsterState(MONSTER_STATUS status)
    {
        isNewState = true;
        monsterStatus = status;
    }

    //현재상태유지
   virtual public IEnumerator MonsterFSM()
    {
        while (!isDead)
        {
            isNewState = false;
            yield return StartCoroutine(monsterStatus.ToString());
        }
    }

    public virtual IEnumerator CHASE()
    {
        yield return null;
    }


    virtual public IEnumerator Move()
    {
        yield return null;
    }

    virtual public IEnumerator PATROL()
    {

        yield return null;
       
    }

    virtual public IEnumerator ATTACK()
    {
        yield return null;
    }

    virtual public IEnumerator DIE()
    {
        yield return null;
    }

    virtual public void DamagedByPlayerBullet(int damage)
    {
        isAttacked = true;
        Hp -= damage;
        if (Hp <= 0 && !isDead)
            Dead();
    }

    virtual public void OffThePhysicsComponent()
    {
        this.GetComponent<Collider2D>().enabled = false;
        this.GetComponent<Rigidbody2D>().isKinematic = true;
    }

    virtual public void Dead()
    {
      
        isDead = true;
        anim.SetTrigger("isDead");
        Destroy(gameObject,anim.GetCurrentAnimatorStateInfo(0).length);
    }

   public bool EndAnimationDone(string EventEndAnimationName)

    {

        return anim.GetCurrentAnimatorStateInfo(0).IsName(EventEndAnimationName) &&

            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f;

    }
}
