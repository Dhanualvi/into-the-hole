using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float climbSpeed = 10f;
    //[SerializeField] float defaultGravity = 10;
    float defaultGravity;
    [SerializeField] int extraJumpCount = 1;
    
    int jumpCount;

    BoxCollider2D myFeetCollider;
    CapsuleCollider2D myBodyCollider;
    Rigidbody2D myRigidbody;
    Animator myAnimator;

    Vector2 rawInput;
    // Start is called before the first frame update
    void Start()
    {
        jumpCount = 1;
        myRigidbody = GetComponent<Rigidbody2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        defaultGravity = myRigidbody.gravityScale;

    }

    // Update is called once per frame
    void Update()
    {
        Run();
        FlipSprite();
        resetJumpCount();
        Climbing();
    }

    void OnMove(InputValue value)
    {
        rawInput = value.Get<Vector2>();
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(rawInput.x * moveSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
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

    void resetJumpCount()
    {
        if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && jumpCount != extraJumpCount)
        {
            Debug.Log(jumpCount);
            jumpCount = extraJumpCount;
        }
    }

    void UpdateGravity()
    {

    }
}
