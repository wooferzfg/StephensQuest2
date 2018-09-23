using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchAbility : MonoBehaviour {
    public int ability;
    public GameObject checkpoint;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Character")
        {
            var charControl = other.GetComponent<CharacterControl>();
            charControl.CollectAbility(ability);
            charControl.SetCheckpoint(transform.position);
            Destroy(gameObject);
        }
    }

    public void SetNewGamePlus()
    {
        Instantiate(checkpoint, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
