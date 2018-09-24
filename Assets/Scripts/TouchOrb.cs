using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchOrb : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Character")
        {
            other.GetComponent<GameControl>().CollectOrb();
            Destroy(gameObject);
        }
    }
}
