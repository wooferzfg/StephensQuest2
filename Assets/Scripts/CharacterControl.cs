using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour {

    [HideInInspector]
    public int faceDirection = 1;
    [HideInInspector]
    public bool jump = false;
    [HideInInspector]
    public bool doubleJump = false;
    [HideInInspector]
    public bool hovering = false;

    public bool hasControl = false;

    public float moveForce = 30f;
    public float maxSpeed = 5f;
    public float jumpForce = 200f;
    public float hoverForce = 50f;
    public float hoverTime = 0.17f;
    public float zipForce = 1200;
    public float zipTime = 0.1f;
    public float zipFloatTime = 0.17f;
    public float groundedDelay = 0.05f;

    public Vector3 checkpoint;
    public bool canJump = false;
    public bool canDoubleJump = false;
    public bool canZip = false;

    private bool grounded = false;
    private float h = 0;
    private bool usedDoubleJump = true;
    private bool usedZip = true;
    private Rigidbody2D rb2d;
    private Transform mainCam;
    private float hoverRemaining = 0;
    private float zipRemaining = 0;
    private float zipFloatRemaining = 0;
    private float groundedRemaining = 0;
    private bool firstUpdate = true;

    // Use this for initialization
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        checkpoint = new Vector3(0, -0.25f, 0);
        mainCam = GameObject.Find("Main Camera").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!firstUpdate)
        {
            if (hasControl)
                h = Input.GetAxisRaw("Horizontal");
            else
                h = 0;

            bool noWalls = !Physics2D.Linecast(transform.position, transform.position + new Vector3(0.3f, 0, 0), 1 << LayerMask.NameToLayer("Ground"))
                        && !Physics2D.Linecast(transform.position, transform.position + new Vector3(-0.3f, 0, 0), 1 << LayerMask.NameToLayer("Ground"));
            bool groundBelow = Physics2D.Linecast(transform.position, transform.position + new Vector3(0, -0.3f, 0), 1 << LayerMask.NameToLayer("Ground"));
            grounded = (Physics2D.Linecast(transform.position + new Vector3(0.25f, 0, 0), transform.position + new Vector3(0.25f, -0.3f, 0), 1 << LayerMask.NameToLayer("Ground"))
                    || Physics2D.Linecast(transform.position + new Vector3(-0.25f, 0, 0), transform.position + new Vector3(-0.25f, -0.3f, 0), 1 << LayerMask.NameToLayer("Ground")))
                    && (noWalls || groundBelow)
                    && zipFloatRemaining <= 0;

            if (grounded)
            {
                groundedRemaining = groundedDelay;
                usedZip = false;
                usedDoubleJump = false;
            }

            if (hasControl && Input.GetButtonDown("Jump") && groundedRemaining > 0 && canJump)
            {
                jump = true;
                hoverRemaining = hoverTime;
                groundedRemaining = 0;
            }

            if (hasControl && Input.GetButtonDown("Jump") && !grounded && !jump && !usedDoubleJump && canDoubleJump)
            {
                doubleJump = true;
                usedDoubleJump = true;
                hoverRemaining = hoverTime;
            }

            if (hasControl && Input.GetButtonDown("Zip") && canZip && !usedZip && zipRemaining <= 0)
            {
                usedZip = true;
                zipRemaining = zipTime;
                zipFloatRemaining = zipFloatTime;
            }

            if (hasControl && Input.GetButton("Jump"))
                hovering = true;
            if (hasControl && Input.GetButtonUp("Jump") && hovering)
                hovering = false;

            if (transform.position.y < -75)
                ResetCheckpoint();
        }
        else if (hasControl)
            firstUpdate = false;
    }

    void FixedUpdate()
    {
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

        if (hovering && !grounded && hoverRemaining > 0 && canJump)
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
        mainCam.position = new Vector3(transform.position.x, transform.position.y, -10);
        jump = false;
        doubleJump = false;
        hovering = false;
        hoverRemaining = 0;
        zipRemaining = 0;
        zipFloatRemaining = 0;
        groundedRemaining = 0;
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
    }
}
