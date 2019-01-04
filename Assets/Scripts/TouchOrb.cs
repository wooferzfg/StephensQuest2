using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchOrb : MonoBehaviour
{
    private bool collected;
    private CharacterControl character;
    private SpriteRenderer sprite;
    private CameraMovement cameraMovement;

    private void Awake()
    {
        collected = false;
        character = GameObject.Find("Character").GetComponent<CharacterControl>();
        sprite = GetComponent<SpriteRenderer>();
        cameraMovement = GameObject.Find("Main Camera").GetComponent<CameraMovement>();
    }

    private void FixedUpdate()
    {
        if (collected)
        {
            if (ShouldOrbBeCollected())
            {
                character.GetComponent<GameControl>().CollectOrb();
                Destroy(gameObject);
            }
            else if (character.deathRemaining > 0)
            {
                collected = false;
            }
            SetAlpha(0.5f);
        }
        else
            SetAlpha(1f);
    }

    private bool ShouldOrbBeCollected()
    {
        if (cameraMovement.isMoving)
        {
            return cameraMovement.moveAmount == 0;
        }
        return character.grounded;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Character")
        {
            collected = true;
        }
    }

    private void SetAlpha(float newAlpha)
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, newAlpha);
    }
}
