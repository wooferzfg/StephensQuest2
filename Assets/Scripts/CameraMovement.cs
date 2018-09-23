using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float screenTransitionLength = 1f;

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
            transform.position = Vector3.Lerp(moveOrigin, moveTarget, moveAmount);
            rb2d.Sleep();
        }
        else
        {
            isMoving = false;
            moveAmount = 0;
            rb2d.WakeUp();
            characterControl.usedDoubleJump = false;
            characterControl.usedZip = false;
        }
    }

    private void setMoveTarget(float targetX, float targetY)
    {
        moveTarget = new Vector3(transform.position.x + targetX, transform.position.y + targetY, transform.position.z);
        moveOrigin = transform.position;
        isMoving = true;
    }
}
