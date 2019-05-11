using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private bool IsActiveMinimap = false;

    private float movePower = 1f;
    private float jumpPower = 1f;
    
     private Rigidbody2D rigidy;
     private Animator animator;
     private Vector3 movement;
     public Gun gun;
     public RoomMgr roomMgr;
     private bool isJumping = false;
     private bool isDoubleJumping = false;

    public double ShootTime;
    public double shoot;

    public int hp = 10;
    public bool isDead;
    public void PlayerDamaged(int damage)
    {
        this.hp -= damage;

        if (this.hp <= 0 && !isDead)
            Dead();
        
    }
    public void Dead()
    {
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
        jumpPower = 15;
    }
    public void UpdatePlayerAnimation()
    {
        

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



        if (Input.GetButtonDown("Jump") && !isDoubleJumping)
        {

            animator.SetTrigger("Jumping");
            animator.SetBool("isJumping", true);
            isJumping = true;

        }

        if (Input.GetButton("Fire1"))
        {

            animator.SetBool("isAttack", true);
            if (shoot > ShootTime)
                Shoot();
        }
        else
        {
            animator.SetBool("isAttack", false);
        }
    }
    public void FixedUpdatePlayerMovement()
    {
        shoot += Time.deltaTime;
        Move();
        Jump();
    }
    public void UpdatePlayerMinimapOnOff()
    {
        //Minimap On/Off
        if (Input.GetKeyDown(KeyCode.F1))
        {
            IsActiveMinimap = !IsActiveMinimap;
            for (int i = 0; i < GameObject.Find("MiniMap").transform.childCount; i++)
                GameObject.Find("MiniMap").transform.GetChild(i).gameObject.SetActive(IsActiveMinimap);

        }
    }
  
  

    private void Move()
    {
        Vector3 moveVelocity = Vector3.zero;

        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            moveVelocity = Vector3.left;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
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
        if (!isJumping ) return;
        
        rigidy.velocity = Vector2.zero;

        Vector2 jumpVelocity = new Vector2(0, jumpPower);
        rigidy.AddForce(jumpVelocity, ForceMode2D.Impulse);
        isDoubleJumping = true;
        isJumping = false;
      
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            isDoubleJumping = false;
            if (rigidy.velocity.y < 0)
            {

                animator.SetBool("isJumping", false);

            }
        } 
    }
}
