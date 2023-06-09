using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditorInternal;

public class Player : Character
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float speed = 300;
    [SerializeField] private float JumpForce = 350;

    [SerializeField] private Kunai kunaiPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject attackArea;
    [SerializeField] private GameObject ThrowPoint;


    
    private bool isGrounded = true ;
    private bool isJumping = false;
    private bool isAttack = false;
    private bool isThrow = false;
    private bool isDeath = false;
    private float horizontal;
    private float vertical;
    

    private int coin = 0;

    private Vector3 savePoint;

    private void Awake()
    {
        coin = PlayerPrefs.GetInt("coin", 0);
    }


    void Update()
    {


        if (IsDead)
        {
            return;
        }

        isGrounded = CheckGrounded();

        horizontal = Input.GetAxisRaw("Horizontal");
        //vertical = Input.GetAxisRaw("Vertical");


       


        if (isGrounded)
        {
            if (isJumping)
            {
                return;
            }

            //Jump
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {

                Jump();
            }

            // change anim run
            if (Mathf.Abs(horizontal) > 0.1f)
            {
                ChangeAnim("run");
            }

            // attack
            if (Input.GetKeyDown(KeyCode.C) && isGrounded)
            {
                Attack();
            }


            //throw
            if (Input.GetKeyDown(KeyCode.V) && isGrounded)
            {
                Throw();
            }


        }


        if (!isGrounded && rb.velocity.y < 0)
        {
            ChangeAnim("fall");
            isJumping = false;
        }

        if (Mathf.Abs(horizontal) > 0.1f)
        {
            ChangeAnim("run");
            rb.velocity = new Vector2(horizontal  * speed, rb.velocity.y);

            transform.rotation = Quaternion.Euler(new Vector3(
                0,horizontal > 0 ? 0 : 180,0));
            

        }
        else if (isGrounded)
        {
            ChangeAnim("idle");
            rb.velocity = Vector2.zero;
        }
    }


    public override void OnInit()
    {
        base.OnInit();
        isAttack = false;

        transform.position = savePoint;
        ChangeAnim("idle");
        DeActiveAttack();
        SavePoint();

        UIManager.instance.SetCoin(coin);
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        OnInit();
    }

    protected override void OnDeath()
    {
        base.OnDeath();
    }

    private bool CheckGrounded()
    {
        Debug.DrawLine(transform.position, transform.position
            + Vector3.down * 1.1f, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position, Vector2.down, 1.1f,groundLayer);

        return hit.collider != null;
    }

    public void Attack()
    {
        if (isAttack)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        ChangeAnim("attack");
        isAttack = true;
        Invoke(nameof(ResetAttack), 0.5f);
        ActiveAttack();
        Invoke(nameof(DeActiveAttack), 0.5f);
    }

    public void Throw()
    {
        if (isThrow)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        ChangeAnim("throw");
        isThrow = true;
        Invoke(nameof(ResetThrow), 0.5f);
        ActiveThrow();
        Invoke(nameof(DeActiveThrow), 0.5f);
        Instantiate(kunaiPrefab, throwPoint.position,
            throwPoint.rotation);
    }

    private void ResetAttack()
    {
        ChangeAnim("ilde");
        isAttack = false;
    }
    private void ResetThrow()
    {
        ChangeAnim("throw");
        isThrow = false;
    }

    public void Jump()
    {
        if (isJumping)
        {
            return;
        }
        if (isGrounded)
        {
            isJumping = true;
            ChangeAnim("jump");
            rb.AddForce(JumpForce * Vector2.up);
        }
    }


    internal void SavePoint()
    {
        savePoint = transform.position;
    }

    private void ActiveAttack()
    {
        attackArea.SetActive(true);
    }
    private void DeActiveAttack()
    {
        attackArea.SetActive(false);
    }
    private void ActiveThrow()
    {
        ThrowPoint.SetActive(true);
    }

    private void DeActiveThrow()
    {
        ThrowPoint.SetActive(false);
    }
    private void isCheckTele()
    {
        transform.position = new Vector2(55f, 6f);
    }
    public void SetMove(float horizontal)
    {
        this.horizontal = horizontal;
    }
    /*public void SetMove2(float vertical)
    {
        this.vertical = vertical;
    }*/
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Coin")
        {
            coin++;
            PlayerPrefs.SetInt("coin", coin);
            UIManager.instance.SetCoin(coin);
            Destroy(collision.gameObject);
        }
        if (collision.tag == "DeathZone")
        {
            isDeath = true;
            ChangeAnim("die");

            Invoke(nameof(OnInit), 1f);
        }
        if(collision.tag == "Tele")
        {
            Invoke(nameof(isCheckTele), 1f);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Wall")
        {
            
        }
    }

}
