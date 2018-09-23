using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private float topBound = 0f;
    private float leftBound = -48f;
    private float rightBound = 48f;
    private float bottomBound = -54f;

    private float screenTransitionLength = 1f;
    private float transitionCharacterMovement = 0.7f;
    private float upwardTransitionJumpHeight = 2f;

    private Transform character;
    private Rigidbody2D rb2d;
    private CharacterControl characterControl;

    private float cameraHeight;
    private float cameraWidth;
    private bool isMoving;
    private float moveAmount;
    private Vector3 moveTarget;
    private Vector3 moveOrigin;

    // Use this for initialization
    void Awake()
    {
        character = GameObject.Find("Character").transform;
        rb2d = character.GetComponent<Rigidbody2D>();
        characterControl = character.GetComponent<CharacterControl>();
        setCameraSize();
    }

    private void setCameraSize()
    {
        var camera = GetComponent<Camera>();
        cameraHeight = camera.orthographicSize;
        cameraWidth = camera.orthographicSize * 16f / 9;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
        {
            if (character.position.x > transform.position.x + cameraWidth)
            {
                setMoveTarget(cameraWidth * 2, 0);
            }
            else if (character.position.x < transform.position.x - cameraWidth)
            {
                setMoveTarget(-cameraWidth * 2, 0);
            }
            else if (character.position.y > transform.position.y + cameraHeight)
            {
                setMoveTarget(0, cameraHeight * 2);
            }
            else if (character.position.y < transform.position.y - cameraHeight)
            {
                setMoveTarget(0, -cameraHeight * 2);
            }
        }
        else if (moveAmount < 1)
        {
            moveAmount += Time.deltaTime / screenTransitionLength;
            transform.position = Vector3.Lerp(moveOrigin, moveTarget, getAdjustedMoveAmount(moveAmount));
            rb2d.Sleep();
            characterControl.hasControl = false;

            var moveDirection = (moveTarget - moveOrigin).normalized;
            if (moveDirection.y > 0)
                moveDirection *= upwardTransitionJumpHeight;
            else
                moveDirection *= transitionCharacterMovement;
            character.Translate(moveDirection * Time.deltaTime);
        }
        else
        {
            isMoving = false;
            moveAmount = 0;
            rb2d.WakeUp();
            characterControl.hasControl = true;
        }
    }

    private float getAdjustedMoveAmount(float t)
    {
        return 1 - Mathf.Pow(t - 1, 2);
    }

    private void setMoveTarget(float targetX, float targetY)
    {
        var newX = transform.position.x + targetX;
        var newY = transform.position.y + targetY;
        if (newX >= leftBound && newX <= rightBound && newY >= bottomBound && newY <= topBound)
        {
            moveTarget = new Vector3(newX, newY, transform.position.z);
            moveOrigin = transform.position;
            isMoving = true;
        }
    }
}
