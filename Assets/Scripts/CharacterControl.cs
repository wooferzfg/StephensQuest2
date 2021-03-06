﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    public bool hasControl = false;
    public bool canJump = false;
    public bool canDoubleJump = false;
    public bool canDash = false;
    public bool grounded = false;
    public bool usedDoubleJump = false;
    public bool usedDash = false;
    public float deathRemaining = 0;
    public float dashRemaining = 0;

    private float gravityScale = 2.6f;
    private float moveForce = 45f;
    private float maxHorizontalSpeed = 5f;
    private float maxVerticalSpeed = 12f;
    private float maxSpeedDecay = 0.4f;
    private float airSpeedDecay = 0.9f;
    private float jumpForce = 320f;
    private float hoverForce = 75f;
    private float delayBeforeHover = 0.06f;
    private float hoverTime = 0.08f;
    private float dashForce = 800;
    private float dashTime = 0.08f;
    private float dashFloatTime = 0.17f;
    private float hyperDashTime = 0.06f;
    private float groundedDelay = 0.08f;
    private float deathDelay = 0.25f;

    private float h = 0;
    private int faceDirection = 1;
    private bool jump = false;
    private bool hovering = false;
    private bool star = false;
    private float hoverRemaining = 0;
    private float dashFloatRemaining = 0;
    private float groundedRemaining = 0;
    private bool prevHadControl = false;
    private Vector2 savedVelocity;
    private int groundLayerMask;

    private Rigidbody2D rb2d;
    private CharacterSprite sprite;
    private DrawMap map;
    private CameraMovement cameraMovement;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<CharacterSprite>();
        map = GameObject.Find("Main Camera").GetComponent<DrawMap>();
        cameraMovement = map.GetComponent<CameraMovement>();
        groundLayerMask = 1 << LayerMask.NameToLayer("Ground");
    }

    private void Update()
    {
        if (hasControl && deathRemaining <= 0)
        {
            h = Input.GetAxisRaw("Movement");

            if (grounded && (dashFloatRemaining <= 0 || groundedRemaining > 0))
            {
                groundedRemaining = groundedDelay;
                usedDoubleJump = false;
                if (dashRemaining <= 0)
                    usedDash = false;
            }

            if (Input.GetButtonDown("Jump"))
            {
                if (groundedRemaining > 0 && canJump)
                {
                    DoJump();
                    usedDash = false;
                    if (dashRemaining > 0)
                        dashRemaining += hyperDashTime;
                }
                else if (!usedDoubleJump && canDoubleJump)
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
        }
        UpdateSprite();
    }

    public void StarJump()
    {
        DoJump();
        usedDash = false;
        usedDoubleJump = false;
        star = true;
        hoverRemaining = hoverTime;
    }

    private void DoJump()
    {
        jump = true;
        star = false;
        hoverRemaining = delayBeforeHover + hoverTime;
        groundedRemaining = 0;
        dashFloatRemaining = 0;
        if (h == 0)
            dashRemaining = 0;
    }

    private void FixedUpdate()
    {
        if (hasControl && deathRemaining <= 0)
        {
            if (!prevHadControl)
            {
                prevHadControl = true;
                rb2d.WakeUp();
                rb2d.velocity = savedVelocity;
                rb2d.gravityScale = gravityScale;
            }

            SetGrounded();

            var canMove = CanMove();

            if (h * rb2d.velocity.x < maxHorizontalSpeed && canMove)
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
                dashRemaining -= Time.deltaTime;

            if (dashFloatRemaining > 0)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
                rb2d.gravityScale = 0;
                dashFloatRemaining -= Time.deltaTime;
            }
            else
                rb2d.gravityScale = gravityScale;

            CapSpeed();

            if (h != 0 && h != faceDirection)
                Flip();

            if ((star || hovering) && !grounded && hoverRemaining > 0 && (star || hoverRemaining <= hoverTime) && canJump)
                rb2d.AddForce(new Vector2(0f, hoverForce));
            if (hoverRemaining > 0)
                hoverRemaining -= Time.deltaTime;

            if (groundedRemaining > 0)
                groundedRemaining -= Time.deltaTime;

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
            if (prevHadControl)
            {
                prevHadControl = false;
                savedVelocity = rb2d.velocity;
            }
            rb2d.Sleep();
            rb2d.gravityScale = 0;

            if (deathRemaining > 0)
                deathRemaining -= Time.deltaTime;
        }
    }

    private void SetGrounded()
    {
        var noWalls = !CheckCollision(0, -0.25f, 0.3f, -0.25f)
                       && !CheckCollision(0, -0.25f, -0.3f, -0.25f);
        var groundBelow = CheckCollision(0, -0.25f, 0, -0.3f);
        grounded = (CheckCollision(0.25f, -0.25f, 0.25f, -0.3f)
                 || CheckCollision(-0.25f, -0.25f, -0.25f, -0.3f))
                 && (noWalls || groundBelow);
    }

    private bool CanMove()
    {
        var canMoveAir = !CheckCollision(0, 0.25f, 0.35f * faceDirection, 0.25f)
                          && !CheckCollision(0, -0.25f, 0.35f * faceDirection, -0.25f);
        var canMoveGround = grounded && !CheckCollision(0, 0, 0.35f * faceDirection, 0);
        return canMoveAir || canMoveGround;
    }

    private bool CheckCollision(float startX, float startY, float endX, float endY)
    {
        return Physics2D.Linecast(transform.position + new Vector3(startX, startY, 0), transform.position + new Vector3(endX, endY, 0), groundLayerMask);
    }

    private void CapSpeed()
    {
        var horizontalSpeed = Mathf.Abs(rb2d.velocity.x);
        var verticalSpeed = Mathf.Abs(rb2d.velocity.y);

        if (dashFloatRemaining <= 0)
        {
            if (grounded && h == 0)
                horizontalSpeed = 0;
            else if (!grounded && h != Mathf.Sign(horizontalSpeed))
                horizontalSpeed *= airSpeedDecay;
        }

        if (horizontalSpeed > maxHorizontalSpeed)
        {
            horizontalSpeed = Mathf.Lerp(maxHorizontalSpeed, horizontalSpeed, maxSpeedDecay);
            if (horizontalSpeed <= maxHorizontalSpeed + 1)
                horizontalSpeed = maxHorizontalSpeed;
        }

        if (verticalSpeed > maxVerticalSpeed)
            verticalSpeed = maxVerticalSpeed;

        rb2d.velocity = new Vector2(horizontalSpeed * Mathf.Sign(rb2d.velocity.x),
                                    verticalSpeed * Mathf.Sign(rb2d.velocity.y));
    }

    private void UpdateSprite()
    {
        var doubleJumpAvailable = canDoubleJump && !usedDoubleJump;
        var dashAvailable = canDash && !usedDash;

        int spriteNum = 0;
        if (doubleJumpAvailable && dashAvailable)
            spriteNum = 3;
        else if (dashAvailable)
            spriteNum = 2;
        else if (doubleJumpAvailable)
            spriteNum = 1;
        sprite.SetSprite(spriteNum);
    }

    private void Flip()
    {
        faceDirection = -faceDirection;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;

        var newHorizontalVelocity = rb2d.velocity.x / 2;
        if (grounded)
            newHorizontalVelocity = 0;
        rb2d.velocity = new Vector2(newHorizontalVelocity, rb2d.velocity.y);
    }

    public void ResetCheckpoint()
    {
        sprite.CreateDeadShadow();

        Vector3 checkpoint = cameraMovement.GetCurrentCheckpoint();
        checkpoint.z = transform.position.z;
        transform.position = checkpoint;

        rb2d.velocity = new Vector2(0, 0);
        jump = false;
        hovering = false;
        star = false;
        usedDash = false;
        usedDoubleJump = false;
        hoverRemaining = 0;
        dashRemaining = 0;
        dashFloatRemaining = 0;
        groundedRemaining = 0;
        deathRemaining = deathDelay;
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
