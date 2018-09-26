using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private float topBound = 0f;
    private float leftBound = -32f;
    private float rightBound = 48f;
    private float bottomBound = -45f;

    private float screenTransitionLength = 1f;
    private float transitionMovement = 0.7f;

    private Transform character;
    private CharacterControl characterControl;
    private DrawMap map;

    private float cameraHeight;
    private float cameraWidth;
    private bool isMoving;
    private float moveAmount;
    private Vector3 moveTarget;
    private Vector3 moveOrigin;

    void Awake()
    {
        character = GameObject.Find("Character").transform;
        characterControl = character.GetComponent<CharacterControl>();
        map = GetComponent<DrawMap>();
        setCameraSize();
    }

    private void setCameraSize()
    {
        var camera = GetComponent<Camera>();
        cameraHeight = camera.orthographicSize;
        cameraWidth = camera.orthographicSize * 16f / 9;
    }

    void FixedUpdate()
    {
        if (!isMoving)
        {
            if (character.position.x > transform.position.x + cameraWidth)
            {
                setMoveTarget(cameraWidth * 2, 0);
                map.MoveRight();
            }
            else if (character.position.x < transform.position.x - cameraWidth)
            {
                setMoveTarget(-cameraWidth * 2, 0);
                map.MoveLeft();
            }
            else if (character.position.y > transform.position.y + cameraHeight)
            {
                setMoveTarget(0, cameraHeight * 2);
                map.MoveUp();
            }
            else if (character.position.y < transform.position.y - cameraHeight)
            {
                setMoveTarget(0, -cameraHeight * 2);
                map.MoveDown();
            }
        }
        else if (moveAmount < 1)
        {
            moveAmount += Time.deltaTime / screenTransitionLength;
            transform.position = Vector3.Lerp(moveOrigin, moveTarget, getAdjustedMoveAmount(moveAmount));
            characterControl.hasControl = false;

            var moveDirection = (moveTarget - moveOrigin).normalized;
            character.Translate(moveDirection * transitionMovement * Time.deltaTime);
        }
        else
        {
            isMoving = false;
            moveAmount = 0;
            characterControl.hasControl = true;
            characterControl.usedDash = false;
            characterControl.usedDoubleJump = false;
        }
    }

    private float getAdjustedMoveAmount(float t)
    {
        return 1 - Mathf.Pow(t - 1, 2);
    }

    private void setMoveTarget(float targetX, float targetY)
    {
        var newX = Mathf.Round(transform.position.x + targetX);
        var newY = Mathf.Round(transform.position.y + targetY);
        if (newX >= leftBound && newX <= rightBound && newY >= bottomBound && newY <= topBound)
        {
            moveTarget = new Vector3(newX, newY, transform.position.z);
            moveOrigin = transform.position;
            isMoving = true;
        }
    }
}
