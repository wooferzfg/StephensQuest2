using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSprite : MonoBehaviour {

	public List<Sprite> sprites;
	public int curSprite;
	
	private SpriteRenderer spriteRenderer;

	void Start() {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void SetSprite(int spriteNum) {
		spriteRenderer.sprite = sprites[spriteNum];
	}
}
