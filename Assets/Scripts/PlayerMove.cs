using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public SoundManager soundManager;
    public float maxSpeed;
    public float jumpPower;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        //Jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            soundManager.PlaySound("JUMP");
        }

        // Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        // Direction Sprite
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        // Animation
        if (Mathf.Abs(rigid.velocity.normalized.x) <= 0.1)
        {
            anim.SetBool("isWalking", false);   // Idle
        }
        else
        {
            anim.SetBool("isWalking", true);    // Walk
        }
    }

    void FixedUpdate()
    {
        // Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h * 2, ForceMode2D.Impulse);

        // Max Speed
        if (rigid.velocity.x > maxSpeed)    
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < maxSpeed * (-1))
        {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }

        // Landing Platform
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0f, 1f, 0f));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1f, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                    anim.SetBool("isJumping", false);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            // Attack : 낙하중이면서, 플레이어의 현재 y축이 콜리젼의 현재 y 축보다 위에 있다면
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
                OnAttack(collision.transform);
            else
                OnDamaged(collision.transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            bool isBroze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBroze)
                gameManager.stagePoint += 50;
            else if (isSilver)
                gameManager.stagePoint += 100;
            else if (isGold)
                gameManager.stagePoint += 300;

            // DeActive Item
            collision.gameObject.SetActive(false);
            soundManager.PlaySound("ITEM");
        }
        else if (collision.gameObject.tag == "Finish")
        {
            // Next Stage
            gameManager.NextStage();
            soundManager.PlaySound("FINISH");
        }
    }

    void OnAttack(Transform enemy)
    {
        //Point
        gameManager.stagePoint += 100;

        // Reaction Force
        rigid.AddForce(Vector2.up * 10f,  ForceMode2D.Impulse);

        // Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
        soundManager.PlaySound("ATTACK");
    }

    void OnDamaged(Vector2 targetPos)
    {
        // Health Down
        gameManager.HealthDown();

        gameObject.layer = 11;                              
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        int dir = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dir, 1) * 7, ForceMode2D.Impulse);

        anim.SetTrigger("doDamaged");
        soundManager.PlaySound("DAMAGED");
        Invoke("OffDamaged", 2.5f);
    }

    void OffDamaged()
    {
        gameObject.layer = 10;                          
        spriteRenderer.color = new Color(1, 1, 1, 1);   
    }

    public void OnDie()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        spriteRenderer.flipY = true;
        capsuleCollider.enabled = false;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        soundManager.PlaySound("DIE");
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}
