using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    public int faceDirection = 1;
    public bool jump = false;
    public bool doubleJump = false;
    public bool hovering = false;
    public bool usedDoubleJump = true;
    public bool usedZip = true;

    public bool hasControl = false;
    public Vector2 savedVelocity;
    
    private bool prevHadControl = false;

    private float moveForce = 30f;
    private float maxSpeed = 5f;
    private float jumpForce = 320f;
    private float hoverForce = 60f;
    private float hoverTime = 0.15f;
    private float zipForce = 1200;
    private float zipTime = 0.08f;
    private float zipFloatTime = 0.2f;
    private float hyperZipTime = 0.06f;
    private float groundedDelay = 0.05f;
    private float deathDelay = 0.25f;

    public Vector3 checkpoint;
    public bool canJump = false;
    public bool canDoubleJump = false;
    public bool canZip = false;

    private bool grounded = false;
    private float h = 0;
    private float hoverRemaining = 0;
    private float zipRemaining = 0;
    private float zipFloatRemaining = 0;
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

            if (grounded && (zipFloatRemaining <= 0 || groundedRemaining > 0))
            {
                groundedRemaining = groundedDelay;
                usedZip = false;
                usedDoubleJump = false;
            }

            if (Input.GetButtonDown("Jump"))
            {
                if (groundedRemaining > 0 && canJump)
                {
                    jump = true;
                    hoverRemaining = hoverTime;
                    groundedRemaining = 0;
                    if (zipRemaining > 0)
                        zipRemaining += hyperZipTime;
                    zipFloatRemaining = 0;
                }
                else if (!grounded && !usedDoubleJump && canDoubleJump)
                {
                    doubleJump = true;
                    usedDoubleJump = true;
                    hoverRemaining = hoverTime;
                    zipFloatRemaining = 0;
                }
            }

            if (Input.GetButtonDown("Zip") && canZip && !usedZip && zipRemaining <= 0)
            {
                usedZip = true;
                zipRemaining = zipTime;
                zipFloatRemaining = zipFloatTime;
            }

            if (Input.GetButton("Jump"))
                hovering = true;
            if (Input.GetButtonUp("Jump") && hovering)
                hovering = false;

            UpdateSprite();
        }
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

            if (zipRemaining > 0 && canMove)
            {
                if (faceDirection > 0)
                    rb2d.AddForce(new Vector2(zipForce, 0f));
                else
                    rb2d.AddForce(new Vector2(-zipForce, 0f));
            }
            if (zipRemaining > 0)
                zipRemaining -= Time.fixedDeltaTime;

            if (zipFloatRemaining > 0)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
                zipFloatRemaining -= Time.fixedDeltaTime;
            }

            if (Mathf.Abs(rb2d.velocity.x) > maxSpeed)
                rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);

            if (h > 0 && faceDirection < 0)
                Flip();
            else if (h < 0 && faceDirection > 0)
                Flip();

            if (doubleJump)
            {
                rb2d.velocity = new Vector2(0, 0);
                doubleJump = false;
                jump = true;
            }

            if (hovering && !grounded && hoverRemaining > 0 && hoverRemaining <= 0.1 && canJump)
                rb2d.AddForce(new Vector2(0f, hoverForce));
            if (hoverRemaining > 0)
                hoverRemaining -= Time.fixedDeltaTime;

            if (groundedRemaining > 0)
                groundedRemaining -= Time.fixedDeltaTime;

            if (jump)
            {
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

    void UpdateSprite()
    {
        bool doubleJumpAvailable = canDoubleJump && !usedDoubleJump;
        bool zipAvailable = canZip && !usedZip;

        int spriteNum = 0;
        if (doubleJumpAvailable && zipAvailable)
            spriteNum = 3;
        else if (zipAvailable)
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
        doubleJump = false;
        hovering = false;
        hoverRemaining = 0;
        zipRemaining = 0;
        zipFloatRemaining = 0;
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
            canZip = true;
        if (ability == 4)
            map.canViewMap = true;
        map.Collected(RoomType.Ability);
    }
}
