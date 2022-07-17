using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSplatterBehaviour : MonoBehaviour
{
    void OnTriggerEnter(Collider collidedObject) {
        if (collidedObject.CompareTag("Player")) {
            collidedObject.gameObject.GetComponent<PlayerController>().kill();
        }
    }
}
