using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchOrb : MonoBehaviour
{
    private bool collected;
    private CharacterControl character;
    private SpriteRenderer sprite;

    private void Awake()
    {
        collected = false;
        character = GameObject.Find("Character").GetComponent<CharacterControl>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (collected)
        {
            if (character.grounded)
            {
                character.GetComponent<GameControl>().CollectOrb();
                Destroy(gameObject);
            }
            else if (character.deathRemaining > 0)
            {
                collected = false;
            }
            setAlpha(0.5f);
        }
        else
            setAlpha(1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Character")
        {
            collected = true;
        }
    }

    private void setAlpha(float newAlpha)
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, newAlpha);
    }
}
