using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSplatterBehaviour : MonoBehaviour
{
    private BoxCollider splatterCollider;

    void Awake() {
        splatterCollider = GetComponent<BoxCollider>();
    }

    void OnTriggerEnter() {
        print("pino1");
        if (splatterCollider.CompareTag("Player")) {
            print("pino2");
            splatterCollider.gameObject.GetComponent<PlayerController>().kill();
        }
    }
}
