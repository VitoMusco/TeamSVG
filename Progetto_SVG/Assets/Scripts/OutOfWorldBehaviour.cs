using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfWorldBehaviour : MonoBehaviour
{
    void OnTriggerEnter(Collider collidedObj) {
        if (collidedObj.gameObject.tag == "Player")
            collidedObj.gameObject.GetComponent<PlayerMovement>().kill();
    }
}
