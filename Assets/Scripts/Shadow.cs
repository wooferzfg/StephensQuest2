using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour {
    private SpriteRenderer sprite;
    private float initialAlpha = 0.075f;
    private float fadeRate = 0.5f;

    void Awake () {
        sprite = GetComponent<SpriteRenderer>();
        transform.Translate(new Vector3(0, 0, 10));
        setAlpha(initialAlpha);
    }
    
    void Update () {
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

    private void setAlpha(float newAlpha) {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, newAlpha);
    }
}
