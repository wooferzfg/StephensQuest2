using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    private Transform character;

	// Use this for initialization
	void Awake () {
        character = GameObject.Find("Character").transform;
	}
	
	// Update is called once per frame
	void Update () {
        if (character.position.x > transform.position.x + 2)
            transform.Translate(new Vector3(character.position.x - transform.position.x - 2, 0, 0));
        if (character.position.x < transform.position.x - 2)
            transform.Translate(new Vector3(character.position.x - transform.position.x + 2, 0, 0));
        if (character.position.y > transform.position.y + 0.75)
            transform.Translate(new Vector3(0, character.position.y - transform.position.y - 0.75f, 0));
        if (character.position.y < transform.position.y - 0.75)
            transform.Translate(new Vector3(0, character.position.y - transform.position.y + 0.75f, 0));
    }
}
