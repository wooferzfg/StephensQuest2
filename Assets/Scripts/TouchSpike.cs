using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchSpike : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Character")
        {
            other.GetComponent<CharacterControl>().ResetCheckpoint();
        }
    }
}
