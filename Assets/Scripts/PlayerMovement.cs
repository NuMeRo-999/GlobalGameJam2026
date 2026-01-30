using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider2D;
    private Animator animator;
    private Vector2 moveInput;

    [Header("Movement Settings")]
    [SerializeField] private float maxMoveSpeed = 8f;
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float deceleration = 20f;
    [SerializeField] private float maxFallSpeed = 25f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpInitialVelocity = 20f;
    [SerializeField] private float maxJumpHeight = 4f;
    [SerializeField] private float coyoteTime = 0.3f;

    [Header("Push Off Ledges Settings")]
    [SerializeField] private float ceilingCheckDistance = 0.1f;
    [SerializeField] private float pushOffForce = 0.1f;
    [SerializeField] private float pushSmoothSpeed = 8f;

    [Header("Gravity Multipliers")]
    [SerializeField] private float fallMultiplier = 4f;
    [SerializeField] private float lowJumpMultiplier = 10f;

    [Header("Ground Check Settings")]
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    private SpriteMaskEffect spriteMaskEffect;

    // --- Debug ---
    private bool wasGroundedLastFrame = false;
    private bool hasJumpedSinceLastGrounded = false;
    private float jumpBufferTime = 0.15f;
    private bool jumpPressedThisFrame = false;
    private bool isJumpPressed = false;
    private bool canJump = true;
    public bool isGrounded { get; private set; }
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private float jumpTimeCounter;
    private bool isJumping;

    public Rigidbody2D Rigidbody => rb;
    public float MaxMoveSpeed => maxMoveSpeed;
    public Vector2 MoveInput => moveInput;

    private void Awake()
    {

        rb = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        //animator = visualEffects.GetComponent<Animator>();
        float gravityForJump = -(jumpInitialVelocity * jumpInitialVelocity) / (2 * maxJumpHeight);
        rb.gravityScale = gravityForJump / Physics2D.gravity.y;

    }


    private void FixedUpdate()
    {
                CheckGrounded();

        if (!wasGroundedLastFrame && isGrounded)
            hasJumpedSinceLastGrounded = false;

        wasGroundedLastFrame = isGrounded;

        // --- Coyote Time ---
        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        // --- Jump Buffer ---
        if (jumpPressedThisFrame)
        {
            jumpBufferCounter = jumpBufferTime;
            jumpPressedThisFrame = false;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // --- Jump Start ---
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f && !isJumping && !hasJumpedSinceLastGrounded)
        {
            JumpStart();
            jumpBufferCounter = 0f;
            hasJumpedSinceLastGrounded = true;
        }

        // --- Jump Hold ---
        if (isJumping && isJumpPressed)
        {
            if (jumpTimeCounter > 0)
                jumpTimeCounter -= Time.deltaTime;
            else
                isJumping = false;
        }

        if (!isJumpPressed)
            isJumping = false;

        // --- Movimiento horizontal ---
        float targetSpeed = moveInput.x * maxMoveSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, 0.9f) * Mathf.Sign(speedDiff);
        rb.AddForce(new Vector2(movement, 0f));
        ApplyJumpPhysics();
        HandlePushOffLedges();

        // Clamp fall speed
        if (rb.linearVelocity.y < -maxFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFallSpeed);
        }

        if (Mathf.Abs(rb.linearVelocity.x) > maxMoveSpeed)
        {
            rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * maxMoveSpeed, rb.linearVelocity.y);
        }

        ApplyJumpPhysics();
        HandlePushOffLedges();
    }

    private void CheckGrounded()
    {
        Vector2 origin = (Vector2)transform.position + capsuleCollider2D.offset - Vector2.up * capsuleCollider2D.size.y * 0.5f * transform.localScale.y;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null && !hit.collider.isTrigger;
    }

    private void JumpStart()
    {
        isJumping = true;
        //animator.SetTrigger("Jump");
        jumpTimeCounter = maxJumpHeight / jumpInitialVelocity;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpInitialVelocity);
        coyoteTimeCounter = 0f;
    }

    private void ApplyJumpPhysics()
    {
        if (rb.linearVelocity.y < 0)
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        else if (rb.linearVelocity.y > 0 && !isJumpPressed)
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
    }

    private void HandlePushOffLedges()
    {
        if (rb.linearVelocity.y <= 0f)
            return;

        float halfWidth = capsuleCollider2D.size.x / 2f * 0.9f;
        float innerOffset = halfWidth * 0.3f;
        float topHeight = capsuleCollider2D.size.y / 2f;

        Vector2 topLeftOuter =
            (Vector2)transform.position + Vector2.up * topHeight + Vector2.left * halfWidth;
        Vector2 topLeftInner =
            (Vector2)transform.position + Vector2.up * topHeight + Vector2.left * innerOffset;
        Vector2 topRightOuter =
            (Vector2)transform.position + Vector2.up * topHeight + Vector2.right * halfWidth;
        Vector2 topRightInner =
            (Vector2)transform.position + Vector2.up * topHeight + Vector2.right * innerOffset;

        RaycastHit2D hitLeftOuter = Physics2D.Raycast(
            topLeftOuter,
            Vector2.up,
            ceilingCheckDistance,
            groundLayer
        );
        RaycastHit2D hitLeftInner = Physics2D.Raycast(
            topLeftInner,
            Vector2.up,
            ceilingCheckDistance,
            groundLayer
        );
        RaycastHit2D hitRightOuter = Physics2D.Raycast(
            topRightOuter,
            Vector2.up,
            ceilingCheckDistance,
            groundLayer
        );
        RaycastHit2D hitRightInner = Physics2D.Raycast(
            topRightInner,
            Vector2.up,
            ceilingCheckDistance,
            groundLayer
        );

        bool leftBlocked =
            (hitLeftOuter.collider != null && !hitLeftOuter.collider.isTrigger)
            || (hitLeftInner.collider != null && !hitLeftInner.collider.isTrigger);
        bool rightBlocked =
            (hitRightOuter.collider != null && !hitRightOuter.collider.isTrigger)
            || (hitRightInner.collider != null && !hitRightInner.collider.isTrigger);
        Vector2 targetPosition = rb.position;
        if (leftBlocked && !rightBlocked)
            targetPosition += Vector2.right * pushOffForce;
        else if (rightBlocked && !leftBlocked)
            targetPosition += Vector2.left * pushOffForce;

        rb.position = Vector2.Lerp(rb.position, targetPosition, Time.deltaTime * pushSmoothSpeed);
    }



    // === INPUTS ===
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        if (animator != null)
            animator.SetBool("isWalking", Mathf.Abs(moveInput.x) > 0.01f);

        if (moveInput.x > 0.1f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < -0.1f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    public void OnJump(InputValue value)
    {
        bool pressed = value.isPressed;

        if (pressed && !isJumpPressed && canJump)
        {
            jumpPressedThisFrame = true;
            canJump = false;
        }

        if (!pressed)
            canJump = true;

        isJumpPressed = pressed;
    }
}