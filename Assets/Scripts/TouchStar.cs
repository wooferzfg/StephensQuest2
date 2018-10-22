using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchStar : MonoBehaviour
{
    private bool collected;
    private bool activated;
    private float timeUntilRespawn;
    private float timeUntilActivation;

    private float respawnTime = 1f;
    private float activationTime = 0.03f;
    private float fadeRate = 10f;

    private SpriteRenderer sprite;
    private CharacterControl character;

    private void Awake()
    {
        collected = false;
        activated = false;
        sprite = GetComponent<SpriteRenderer>();
        character = GameObject.Find("Character").GetComponent<CharacterControl>();
    }

    private void Update()
    {
        if (collected)
        {
            var newAlpha = Mathf.Max(sprite.color.a - Time.deltaTime * fadeRate, 0);
            SetAlpha(newAlpha);
        }
    }

    private void FixedUpdate()
    {
        if (collected)
        {
            timeUntilRespawn -= Time.deltaTime;
            if (timeUntilRespawn < 0)
            {
                collected = false;
                SetAlpha(1);
            }

            timeUntilActivation -= Time.deltaTime;
            if (!activated && timeUntilActivation < 0)
            {
                character.StarJump();
                activated = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!collected && other.gameObject.name == "Character")
        {
            timeUntilRespawn = respawnTime;
            timeUntilActivation = activationTime;
            collected = true;
            activated = false;
        }
    }

    private void SetAlpha(float newAlpha)
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, newAlpha);
    }
}
