using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float cameraHeight;
    public float cameraWidth;
    public bool isMoving;
    public Vector3 moveTarget;
    public Vector3 moveOrigin;
    public float cycle = 0;

    private float topBound = 0f;
    private float leftBound = -16f;
    private float rightBound = 48f;
    private float bottomBound = -36f;

    private float screenTransitionLength = 1f;
    private float transitionMovement = 0.7f;

    private Transform character;
    private CharacterControl characterControl;
    private DrawMap map;

    private float moveAmount;
    private int rowDelta;
    private int columnDelta;

    private void Awake()
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

    private void FixedUpdate()
    {
        if (!isMoving)
        {
            if (character.position.x > transform.position.x + cameraWidth)
            {
                SetMoveTarget(1, 0);
            }
            else if (character.position.x < transform.position.x - cameraWidth)
            {
                SetMoveTarget(-1, 0);
            }
            else if (character.position.y > transform.position.y + cameraHeight)
            {
                SetMoveTarget(0, 1);
            }
            else if (character.position.y < transform.position.y - cameraHeight)
            {
                SetMoveTarget(0, -1);
            }
            cycle += Time.deltaTime;
        }
        else if (moveAmount < 1)
        {
            moveAmount += Time.deltaTime / screenTransitionLength;
            transform.position = Vector3.Lerp(moveOrigin, moveTarget, GetAdjustedMoveAmount(moveAmount));
            characterControl.hasControl = false;

            var moveDirection = (moveTarget - moveOrigin).normalized;
            character.Translate(moveDirection * transitionMovement * Time.deltaTime);
        }
        else
        {
            isMoving = false;
            moveAmount = 0;
            cycle = 0;
            characterControl.hasControl = true;
            characterControl.usedDash = false;
            characterControl.usedDoubleJump = false;
            map.MoveBetweenRooms(rowDelta, columnDelta);
        }
    }

    private float GetAdjustedMoveAmount(float t)
    {
        return 1 - Mathf.Pow(t - 1, 2);
    }

    private void SetMoveTarget(int deltaX, int deltaY)
    {
        rowDelta = -deltaY;
        columnDelta = deltaX;
        var newX = Mathf.Round(transform.position.x + deltaX * cameraWidth * 2);
        var newY = Mathf.Round(transform.position.y + deltaY * cameraHeight * 2);
        if (newX >= leftBound && newX <= rightBound && newY >= bottomBound && newY <= topBound)
        {
            moveTarget = new Vector3(newX, newY, transform.position.z);
            moveOrigin = transform.position;
            isMoving = true;
        }
    }

    public Vector2 GetCurrentCheckpoint()
    {
        return CreateCheckpoint.GetCurrentCheckpoint(moveTarget.x, moveTarget.y, moveOrigin.x, moveOrigin.y);
    }
}
