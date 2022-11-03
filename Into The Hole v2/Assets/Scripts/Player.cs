using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float climbSpeed = 10f;
    [SerializeField] int extraJumpCount = 1;
    [SerializeField] int arrowCount = 5;
    [SerializeField] float invincibleTime = 1.5f;
    
    [Header("Bounce Boost")]  
    [SerializeField] Vector2 deathKick = new Vector2(0f,10f);
    [SerializeField] Vector2 stepBoost = new Vector2(0f,10f);
    
    /*[SerializeField] float defaultGravity = 10;
    [Header("Life")]
    [SerializeField] int lifeCount = 1;*/

    [Header("Arrow")]
    [SerializeField] Transform arrowSpawn;
    [SerializeField] GameObject arrow;

    int jumpCount;
    float defaultGravity;
    [SerializeField] bool isAlive = true;
    bool isMoving;
    bool isInvincible = false;
    

    BoxCollider2D myFeetCollider;
    CapsuleCollider2D myBodyCollider;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    Enemy enemy;

    Vector2 rawInput;

    void Start()
    {
        jumpCount = 1;
        myRigidbody = GetComponent<Rigidbody2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        enemy = FindObjectOfType<Enemy>();
        defaultGravity = myRigidbody.gravityScale;
        
    }

    void Update()
    {
        if (!isAlive) { return; }
        Run();
        FlipSprite();
        ResetJumpCount();
        Climbing();
        Die();
        SteppingEnemy();
    }

    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        rawInput = value.Get<Vector2>();
    }

    void Run()
    {
        
        Vector2 playerVelocity = new Vector2(rawInput.x * moveSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        isMoving = playerHasHorizontalSpeed;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
        
    }

    void Climbing()
    {
       
        if (!myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"))) 
        {
            myAnimator.SetBool("isClimbing", false);
            myRigidbody.gravityScale = defaultGravity;
            return; 
        }
        
        Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, rawInput.y * climbSpeed);
        myRigidbody.velocity = climbVelocity;
        myRigidbody.gravityScale = 0;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
        

    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }  
    }

    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }
        if (value.isPressed && jumpCount > 0)
        {
            myRigidbody.velocity = new Vector2(0f, jumpSpeed);
            jumpCount--;
        }
        else if(value.isPressed && myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && jumpCount == 0)
        {
            myRigidbody.velocity = new Vector2(0f, jumpSpeed);
            jumpCount--;
        }
    }

    void OnFire(InputValue value)
    {
        if (!isAlive) { return; }
        if (isMoving) { return; }
        if (value.isPressed && arrowCount > 0)
        {
            myAnimator.SetTrigger("triggerShooting");
            Instantiate(arrow, arrowSpawn.position, transform.rotation);
        }
    }

    void ResetJumpCount()
    {
        if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && jumpCount != extraJumpCount)
        {
            //Debug.Log(jumpCount);
            jumpCount = extraJumpCount;
        }
    }

    

    void Die()
    {
        if (isInvincible) { return; }
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazard")))
        {
            StartCoroutine(SetInvincibleState());
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
            SendFlying();
        }
    }

    void SteppingEnemy()
    {
        if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Enemy"))&& !isInvincible)
        {
            
            myRigidbody.velocity = stepBoost;
            Destroy(enemy.gameObject);
            //myAnimator.SetTrigger("triggerDie");
            //isAlive = false;
        }
    }

    public void SendFlying()
    {
        myRigidbody.velocity = deathKick;
        myAnimator.SetTrigger("triggerDie");
    }

    public void SetAlive()
    {
        if (isAlive)
        {
            isAlive = false;
            
        }
        
    }
   
    IEnumerator SetInvincibleState()
    {
        if (!isInvincible)
        {
            isInvincible = true;
            yield return new WaitForSeconds(invincibleTime);
            isInvincible = false; 
        }
    }
}
