using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchCheckpoint : MonoBehaviour
{
    void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = null;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Character")
        {
            other.GetComponent<CharacterControl>().SetCheckpoint(transform.position);
        }
    }
}
