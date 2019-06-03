using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    //minimap 활성화 해주는 변수 'F1'으로 활성화 시킴
    private bool IsActiveMinimap = false;
    //물리적 변수들
    private float movePower = 1f;
    private float jumpPower = 1f;
    private float horizontalMove;
    private float verticalMove;

    //인스펙터에서 받아올 물리 객체들
    private Rigidbody2D rigidy;
    private Animator animator;
    private Vector3 movement;

    //Find로 받아올 객체들
    public Gun gun;
    public RoomMgr roomMgr;

    //점프 구현
    private bool isJumping = false;

    //플레이어 공격 관련
    public double ShootTime;
    public double shoot;

    //중독
    public bool isPoisoned = false;
    public double PoisonTime = 0;
    private float PoisonColoredTime = 0;
    private float PoisonColoredStart = 0;
    private float PoisonColoredEnd = 1;

    public int hp = 10;
    public bool isDead;
    public float footSound = 0;
    public void PlayerDamaged(int damage)
    {
        this.hp -= damage;
        if (this.hp <= 0 && !isDead)
            Death();
        
    }
    public System.Action OnDeath;
    public void Death()
    {
        if (OnDeath != null)
            OnDeath();
        isDead = true;
        animator.SetBool("isDeath", true);
        Destroy(gameObject);

    }
    public void PlayerInit()
    {
        rigidy = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        gun = transform.GetChild(0).GetComponent<Gun>();
        roomMgr = GameObject.Find("GameController").GetComponent<RoomMgr>();
       
        transform.position = roomMgr.rooms[roomMgr.StartYpos, roomMgr.StartXpos].dungeon.transform.Find("StartPoint").position;
       
        ShootTime = 0.2;
        shoot = 0.0;
        movePower = 10;
        jumpPower = 20;
    }
    private void UpdatePlayerAnimation()
    {
        if (isDead) return;

        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            animator.SetBool("isMoving", false);
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {

            animator.SetBool("isMoving", true);
            animator.SetFloat("Direction", -1f);

        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            animator.SetBool("isMoving", true);
            animator.SetFloat("Direction", 1f);
        }

       

       

        if (Input.GetButton("Fire1"))
        {
            SoundManager.GetInst().PlaySound("Attack");
            animator.SetBool("isAttack", true);
            if (shoot > ShootTime)
                Shoot();
        }
        else
        {
            animator.SetBool("isAttack", false);
        }
    }
    public void UpdatePlayerMovement()
    {
        if (!isDead)
        {
            horizontalMove = Input.GetAxisRaw("Horizontal");
            verticalMove = Input.GetAxisRaw("Horizontal");

            footSound += Time.deltaTime;
           
          
        }
    }
    public void FixedUpdatePlayerMovement()
    {
        if (!isDead)
        {
            shoot += Time.deltaTime;
            Jump();
            Move();
            UpdatePlayerAnimation();

            //
            UIinformationMgr.GetInst().SetPlayerHp(this.hp);
        }
      
    }
    public void UpdateSpecialKey()
    {
        //Minimap On/Off
        if (Input.GetKeyDown(KeyCode.F1))
        {
            IsActiveMinimap = !IsActiveMinimap;
            for (int i = 0; i < GameObject.Find("MiniMap").transform.childCount; i++)
                GameObject.Find("MiniMap").transform.GetChild(i).gameObject.SetActive(IsActiveMinimap);

        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
          
            if (gun.GunType == Gun.GUNTYPE.NORMAL)
            {
                gun.GunType = Gun.GUNTYPE.LASER;               
            }
            else
            {
                gun.GunType = Gun.GUNTYPE.NORMAL;
            }
        }
        //총알 변환 함수 넣을거면 넣자 -> 시간없으면 안넣음.
    }
  
  

    private void Move()
    {
        Vector3 moveVelocity = Vector3.zero;
       
        if (horizontalMove< 0)
        {
            if (footSound > 0.2 && !isJumping)
            {
                SoundManager.GetInst().PlaySound("Foot");
                footSound = 0;
            }

           
            moveVelocity = Vector3.left;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (horizontalMove > 0)
        {
            if (footSound > 0.2 && !isJumping)
            {
                SoundManager.GetInst().PlaySound("Foot");
                footSound = 0;
            }
           
            moveVelocity = Vector3.right;
            transform.localScale = new Vector3(1, 1, 1);
        }

        transform.position += moveVelocity * movePower * Time.deltaTime;
    }
    private void Shoot()
    {
        gun.UpdateGunShoot(this);
    }
    private void Jump()
    {
        if (rigidy.velocity.y == 0)
        {
           
            animator.SetBool("isJumping", false);
        }
       
      
        if (Input.GetButtonDown("Jump") && isJumping ==false)
        {
             isJumping = true;
             rigidy.velocity = Vector2.zero;
            animator.SetTrigger("Jumping");
            animator.SetBool("isJumping", true);
            Vector2 jumpVelocity = new Vector2(0, jumpPower);
            rigidy.AddForce(jumpVelocity, ForceMode2D.Impulse);
             
            SoundManager.GetInst().PlaySound("Jump");
                
           
        }
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (collision.gameObject.layer == 8 && rigidy.velocity.y <0)
        {
            animator.SetBool("isJumping", false);
            isJumping = false;
          
        }
    }

    public void StartPoisonedCorutine(int damage, double poisonDelay = 6)
    {
        this.isPoisoned = true;
        StartCoroutine(Poisoned(damage, poisonDelay));
        StartCoroutine(PoisonedChangeColor());
    }

    public IEnumerator PoisonedChangeColor()
    {
        Color PoisonedColor = GetComponent<SpriteRenderer>().color;
        PoisonColoredTime = 0;
        PoisonedColor.r = Mathf.Lerp(PoisonColoredStart, PoisonColoredEnd, PoisonColoredTime);
        PoisonedColor.b = Mathf.Lerp(PoisonColoredStart, PoisonColoredEnd, PoisonColoredTime);
        while (PoisonedColor.r <1f || PoisonedColor.b <1f)
        {
            PoisonColoredTime += Time.deltaTime / 2;
            PoisonedColor.a = Mathf.Lerp(PoisonColoredStart, PoisonColoredEnd, PoisonColoredTime);
            GetComponent<SpriteRenderer>().color = PoisonedColor;

            yield return null;
        }


    }
  
    public IEnumerator Poisoned(int damage, double poisonDelay = 6)
    {
       
        while (PoisonTime <poisonDelay)
        {          
           
            PoisonTime += 1;
            this.hp -= damage;
            yield return new WaitForSeconds(1);
        }
        PoisonTime = 0;
        this.isPoisoned = false;
      
    }
}
