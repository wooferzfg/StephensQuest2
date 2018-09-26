using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSprite : MonoBehaviour
{
    public List<Sprite> sprites;
    public int curSprite;
    public GameObject shadow;
    
    private SpriteRenderer spriteRenderer;
    private CharacterControl character;
    private float timeBetweenShadows = 0.05f;
    private float timeRemaining;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        character = GetComponent<CharacterControl>();
        timeRemaining = timeBetweenShadows;
    }

    void Update()
    {
        if (character.hasControl)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                timeRemaining += timeBetweenShadows;
                var newShadow = Instantiate(shadow, transform.position, Quaternion.identity);
                newShadow.GetComponent<SpriteRenderer>().sprite = spriteRenderer.sprite;
            }
        }
    }

    public void SetSprite(int spriteNum)
    {
        spriteRenderer.sprite = sprites[spriteNum];
    }
}
