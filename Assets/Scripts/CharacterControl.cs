using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    public int faceDirection = 1;
    public bool jump = false;
    public bool hovering = false;
    public bool usedDoubleJump = true;
    public bool usedDash = true;

    public bool hasControl = false;
    public Vector2 savedVelocity;
    
    private bool prevHadControl = false;

    private float moveForce = 30f;
    private float maxSpeed = 5f;
    private float speedDecay = 0.4f;
    private float jumpForce = 320f;
    private float hoverForce = 60f;
    private float hoverTime = 0.15f;
    private float dashForce = 800;
    private float dashTime = 0.08f;
    private float dashFloatTime = 0.2f;
    private float hyperDashTime = 0.06f;
    private float groundedDelay = 0.05f;
    private float deathDelay = 0.25f;

    public Vector3 checkpoint;
    public bool canJump = false;
    public bool canDoubleJump = false;
    public bool canDash = false;

    private bool grounded = false;
    private float h = 0;
    private float hoverRemaining = 0;
    private float dashRemaining = 0;
    private float dashFloatRemaining = 0;
    private float groundedRemaining = 0;
    private float deathRemaining = 0;

    private Rigidbody2D rb2d;
    private CharacterSprite sprite;
    private DrawMap map;

    void Awake()
    {
        checkpoint = transform.position;
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<CharacterSprite>();
        map = GameObject.Find("Main Camera").GetComponent<DrawMap>();
    }

    void Update()
    {
        if (hasControl && deathRemaining <= 0)
        {
            h = Input.GetAxisRaw("Horizontal");

            bool noWalls = !Physics2D.Linecast(transform.position, transform.position + new Vector3(0.3f, 0, 0), 1 << LayerMask.NameToLayer("Ground"))
                        && !Physics2D.Linecast(transform.position, transform.position + new Vector3(-0.3f, 0, 0), 1 << LayerMask.NameToLayer("Ground"));
            bool groundBelow = Physics2D.Linecast(transform.position, transform.position + new Vector3(0, -0.3f, 0), 1 << LayerMask.NameToLayer("Ground"));
            grounded = (Physics2D.Linecast(transform.position + new Vector3(0.25f, 0, 0), transform.position + new Vector3(0.25f, -0.3f, 0), 1 << LayerMask.NameToLayer("Ground"))
                    || Physics2D.Linecast(transform.position + new Vector3(-0.25f, 0, 0), transform.position + new Vector3(-0.25f, -0.3f, 0), 1 << LayerMask.NameToLayer("Ground")))
                    && (noWalls || groundBelow);

            if (grounded && (dashFloatRemaining <= 0 || groundedRemaining > 0))
            {
                groundedRemaining = groundedDelay;
                usedDash = false;
                usedDoubleJump = false;
            }

            if (Input.GetButtonDown("Jump"))
            {
                if (groundedRemaining > 0 && canJump)
                {
                    DoJump();
                    if (dashRemaining > 0 && h != 0)
                        dashRemaining += hyperDashTime;
                    else
                        dashRemaining = 0;
                }
                else if (!grounded && !usedDoubleJump && canDoubleJump)
                {
                    DoJump();
                    usedDoubleJump = true;
                }
            }

            if (Input.GetButtonDown("Dash") && canDash && !usedDash && dashRemaining <= 0)
            {
                usedDash = true;
                dashRemaining = dashTime;
                dashFloatRemaining = dashFloatTime;
            }

            if (Input.GetButton("Jump"))
                hovering = true;
            if (Input.GetButtonUp("Jump") && hovering)
                hovering = false;

            UpdateSprite();
        }
    }

    private void DoJump()
    {
        jump = true;
        hoverRemaining = hoverTime;
        groundedRemaining = 0;
        dashFloatRemaining = 0;
    }

    void FixedUpdate()
    {
        if (hasControl && deathRemaining <= 0)
        {
            if (!prevHadControl)
            {
                prevHadControl = true;
                rb2d.WakeUp();
                rb2d.velocity = savedVelocity;
            }

            bool canMoveAir = !Physics2D.Linecast(transform.position + new Vector3(0, 0.25f, 0), transform.position + new Vector3(0.35f * faceDirection, 0.25f, 0), 1 << LayerMask.NameToLayer("Ground"))
                && !Physics2D.Linecast(transform.position + new Vector3(0, -0.25f, 0), transform.position + new Vector3(0.35f * faceDirection, -0.25f, 0), 1 << LayerMask.NameToLayer("Ground"));
            bool canMoveGround = grounded && !Physics2D.Linecast(transform.position + new Vector3(0, 0, 0), transform.position + new Vector3(0.35f * faceDirection, 0, 0), 1 << LayerMask.NameToLayer("Ground"));
            bool canMove = canMoveAir || canMoveGround;

            if (h * rb2d.velocity.x < maxSpeed && canMove)
                rb2d.AddForce(Vector2.right * h * moveForce);

            if (dashRemaining > 0 && canMove)
            {
                var adjustedDashForce = Mathf.Min(dashForce, dashRemaining / dashTime * dashForce);
                if (faceDirection > 0)
                    rb2d.AddForce(new Vector2(adjustedDashForce, 0f));
                else
                    rb2d.AddForce(new Vector2(-adjustedDashForce, 0f));
            }
            if (dashRemaining > 0)
                dashRemaining -= Time.fixedDeltaTime;

            if (dashFloatRemaining > 0)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
                dashFloatRemaining -= Time.fixedDeltaTime;
            }

            CapSpeed();

            if (h != 0 && h != faceDirection)
                Flip();

            if (hovering && !grounded && hoverRemaining > 0 && hoverRemaining <= 0.1 && canJump)
                rb2d.AddForce(new Vector2(0f, hoverForce));
            if (hoverRemaining > 0)
                hoverRemaining -= Time.fixedDeltaTime;

            if (groundedRemaining > 0)
                groundedRemaining -= Time.fixedDeltaTime;

            if (jump)
            {
                if (h == 0)
                    rb2d.velocity = new Vector2(0, 0);
                else
                    rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
                rb2d.AddForce(new Vector2(0f, jumpForce));
                jump = false;
            }
        }
        else
        {
            prevHadControl = false;
            savedVelocity = rb2d.velocity;
            rb2d.Sleep();
        }
        if (deathRemaining > 0)
            deathRemaining -= Time.fixedDeltaTime;
    }

    private void CapSpeed()
    {
        var curSpeed = Mathf.Abs(rb2d.velocity.x);
        if (curSpeed > maxSpeed)
        {
            var adjustedSpeed = Mathf.Lerp(maxSpeed, curSpeed, speedDecay);
            if (adjustedSpeed <= maxSpeed + 1)
                adjustedSpeed = maxSpeed;
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * adjustedSpeed, rb2d.velocity.y);
        }
    }

    void UpdateSprite()
    {
        bool doubleJumpAvailable = canDoubleJump && !usedDoubleJump;
        bool dashAvailable = canDash && !usedDash;

        int spriteNum = 0;
        if (doubleJumpAvailable && dashAvailable)
            spriteNum = 3;
        else if (dashAvailable)
            spriteNum = 2;
        else if (doubleJumpAvailable)
            spriteNum = 1;
        sprite.SetSprite(spriteNum);
    }

    void Flip()
    {
        faceDirection = -faceDirection;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        rb2d.velocity = new Vector2(rb2d.velocity.x / 3, rb2d.velocity.y);
    }

    public void ResetCheckpoint()
    {
        rb2d.velocity = new Vector2(0, 0);
        transform.position = checkpoint;
        jump = false;
        hovering = false;
        hoverRemaining = 0;
        dashRemaining = 0;
        dashFloatRemaining = 0;
        groundedRemaining = 0;
        deathRemaining = deathDelay;
    }

    public void SetCheckpoint(Vector3 pos)
    {
        checkpoint = new Vector3(pos.x, pos.y, checkpoint.z);
    }

    public void CollectAbility(int ability)
    {
        if (ability == 1)
            canJump = true;
        if (ability == 2)
            canDoubleJump = true;
        if (ability == 3)
            canDash = true;
        if (ability == 4)
            map.canViewMap = true;
        map.Collected(RoomType.Ability);
    }
}
