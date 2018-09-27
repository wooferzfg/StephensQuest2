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
    private float timeBetweenDashShadows = 0.0167f;
    private float currentTime;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        character = GetComponent<CharacterControl>();
        currentTime = 0;
    }

    private void Update()
    {
        if (character.hasControl)
        {
            currentTime += Time.deltaTime;
            var curTimeBetweenShadows = GetTimeBetweenShadows();
            if (currentTime > curTimeBetweenShadows)
            {
                currentTime -= curTimeBetweenShadows;
                var newShadow = Instantiate(shadow, transform.position, Quaternion.identity);
                newShadow.GetComponent<SpriteRenderer>().sprite = spriteRenderer.sprite;
            }
        }
    }

    private float GetTimeBetweenShadows()
    {
        if (character.dashRemaining > 0)
            return timeBetweenDashShadows;

        return timeBetweenShadows;
    }

    public void SetSprite(int spriteNum)
    {
        spriteRenderer.sprite = sprites[spriteNum];
    }
}
