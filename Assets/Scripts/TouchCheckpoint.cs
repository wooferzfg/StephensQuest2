using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchCheckpoint : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Character")
        {
            other.GetComponent<CharacterControl>().SetCheckpoint(transform.position);
        }
    }
}
