using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchFinalOrb : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Character")
        {
            other.GetComponent<GameControl>().CollectFinalOrb();
            Destroy(gameObject);
        }
    }
}
