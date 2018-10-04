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
    private float shadowAlpha = 0.05f;
    private float dashShadowAlpha = 0.08f;
    private float deadShadowAlpha = 0.2f;
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
                currentTime = (currentTime - curTimeBetweenShadows) % curTimeBetweenShadows;
                if (character.dashRemaining > 0)
                    CreateDashShadow();
                else
                    CreateShadow();
            }
        }
    }

    public void CreateShadow()
    {
        CreateShadow(shadowAlpha);
    }

    public void CreateDashShadow()
    {
        CreateShadow(dashShadowAlpha);
    }

    public void CreateDeadShadow()
    {
        CreateShadow(deadShadowAlpha);
    }

    private void CreateShadow(float alpha)
    {
        var newShadow = Instantiate(shadow, transform.position, Quaternion.identity);
        newShadow.GetComponent<SpriteRenderer>().sprite = spriteRenderer.sprite;
        newShadow.GetComponent<Shadow>().SetAlpha(alpha);
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
