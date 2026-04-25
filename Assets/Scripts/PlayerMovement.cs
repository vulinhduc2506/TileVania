using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] float swingSpeed = 10f;
    [SerializeField] Vector2 deathKick = new Vector2(10f, 10f);
    [SerializeField] GameObject arrow;
    [SerializeField] Transform bow;

    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    bool isAlive = true;
    bool isClimbingState = false;
    bool isShootingState = false;
    bool isSwingingState = false;


    float gravityScaleAtStart;


    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
    }


    void Update()
    {
        if (!isAlive) return;
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
        CheckMidAirSwing();
    }

    void OnMove(InputValue value)
    {
        if (!isAlive) return;
        moveInput = value.Get<Vector2>();

    }

    void OnFire(InputValue inputValue)
    {
        if (!isAlive) return;

        if (inputValue.isPressed && !isShootingState && !isClimbingState)
        {
            // --- LOGIC NGẮT CHIÊU (ANIMATION CANCELING) ---
            if (isSwingingState)
            {
                isSwingingState = false;
                myRigidbody.velocity = new Vector2(0f, myRigidbody.velocity.y); // Phanh gấp lại để bắn
            }

            // --- VÀO TRẠNG THÁI BẮN ---
            isShootingState = true;
            myAnimator.SetBool("isRunning", false); 
            myRigidbody.velocity = new Vector2(0f, myRigidbody.velocity.y);
            myAnimator.SetTrigger("Shooting"); 

            
        }
    }

    void OnSwing(InputValue inputValue)
    {
        if (!isAlive) return;
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }

        bool isTryingToRun = Mathf.Abs(moveInput.x) > Mathf.Epsilon;

        if (inputValue.isPressed && isTryingToRun && !isSwingingState && !isShootingState && !isClimbingState)
        {
            isSwingingState = true;
            // myAnimator.SetBool("isRunning", false);
            myAnimator.SetTrigger("Swinging");
            // myRigidbody.velocity = new Vector2(0f, myRigidbody.velocity.y);
        }
        else if (inputValue.isPressed && !isTryingToRun)
        {
            return;
        }
    }

    void CheckMidAirSwing()
    {
        if (!isSwingingState) { return; }

        bool isGrounded = myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));

        if (!isGrounded)
        {
            EndSwing();
            myRigidbody.velocity = new Vector2(0f, myRigidbody.velocity.y); 
        }
    }

    public void EndSwing()
    {
        isSwingingState = false;
    }

    public void EndShooting()
    {
        isShootingState = false;
    }

    public void SpawnArrowEvent() 
    {
        Instantiate(arrow, bow.position, transform.rotation);
    }

    void OnJump(InputValue value)
    {
        ExitClimbState();
        if (!isAlive) return;
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }
        if (value.isPressed)
        {
            myRigidbody.velocity += new Vector2(0f, jumpSpeed);
        }
    }
    void Run()
    {

        if (isShootingState) return;
        if (isSwingingState)
        {
            myRigidbody.velocity = new Vector2(moveInput.x * swingSpeed, 0f);

            return;
        }

        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    void FlipSprite()
    {
        if (isShootingState) { return; }
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }

    }


    void ClimbLadder()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            ExitClimbState();
            return;
        }

        bool hasVerticalInput = Mathf.Abs(moveInput.y) > Mathf.Epsilon;
        bool hasHorizontalInput = Mathf.Abs(moveInput.x) > Mathf.Epsilon;
        bool isGrounded = myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));

        // Đang leo thang, chân đã chạm đất, và người chơi muốn rẽ ngang (không bấm lên/xuống)
        if (isClimbingState && isGrounded && hasHorizontalInput && !hasVerticalInput)
        {
            ExitClimbState();
            return;
        }

        // 3. VÀO TRẠNG THÁI LEO:
        if (hasVerticalInput && !isClimbingState)
        {
            isClimbingState = true;
        }

        if (!isClimbingState) { return; }

        // 4. KHI ĐANG LEO THANG (isClimbingState == true)
        Vector2 climbVelocity = new Vector2(0f, moveInput.y * climbSpeed);
        myRigidbody.velocity = climbVelocity;
        myRigidbody.gravityScale = 0;

        // Hút vào giữa thang khi đang di chuyển dọc
        if (hasVerticalInput)
        {
            float targetX = Mathf.Floor(transform.position.x) + 0.5f;
            float smoothX = Mathf.MoveTowards(transform.position.x, targetX, 5f * Time.deltaTime);
            transform.position = new Vector2(smoothX, transform.position.y);
        }

        myAnimator.SetBool("isClimbing", true);
        if (hasVerticalInput)
        {
            myAnimator.speed = 1f;
        }
        else
        {
            myAnimator.speed = 0f;
        }
    }

    void ExitClimbState()
    {
        isClimbingState = false;
        myRigidbody.gravityScale = gravityScaleAtStart;
        myAnimator.SetBool("isClimbing", false);
        myAnimator.speed = 1f;
    }

    void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazard")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Die");
            myRigidbody.velocity = deathKick;
            StartCoroutine(WaitAndProcessDeath());
        }
    }

    IEnumerator WaitAndProcessDeath()
    {
        yield return new WaitForSeconds(0.5f);
        
        FindObjectOfType<GameSession>().ProccessPlayerDeath();
    }


}
