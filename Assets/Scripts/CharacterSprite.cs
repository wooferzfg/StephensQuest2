using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSprite : MonoBehaviour
{
    public List<Sprite> sprites;
    public int curSprite;
    public GameObject shadow;
    
    private SpriteRenderer spriteRenderer;
    private float timeBetweenShadows = 1/60f;
    private float timeRemaining;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        timeRemaining = timeBetweenShadows;
    }

    void Update()
    {
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            timeRemaining += timeBetweenShadows;
            var newShadow = Instantiate(shadow, transform.position, Quaternion.identity);
            newShadow.GetComponent<SpriteRenderer>().sprite = spriteRenderer.sprite;
        }
    }

    public void SetSprite(int spriteNum)
    {
        spriteRenderer.sprite = sprites[spriteNum];
    }
}
