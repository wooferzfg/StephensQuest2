using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour {
    private SpriteRenderer sprite;
    private CharacterControl character;
    private float initialAlpha = 0.05f;
    private float fadeRate = 0.3f;

    private void Awake ()
    {
        sprite = GetComponent<SpriteRenderer>();
        character = GameObject.Find("Character").GetComponent<CharacterControl>();
        transform.Translate(new Vector3(0, 0, 10));
        setAlpha(initialAlpha);
    }
    
    private void Update ()
    {
        if (character.hasControl)
        {
            var newAlpha = Mathf.Max(sprite.color.a - Time.deltaTime * fadeRate, 0);
            if (newAlpha == 0)
            {
                Destroy(gameObject);
            }
            else
            {
                setAlpha(newAlpha);
            }
        }
    }

    private void setAlpha(float newAlpha)
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, newAlpha);
    }
}
