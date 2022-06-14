using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalBehavior : MonoBehaviour
{
    public int sceneToLoad = 0;

    void OnTriggerEnter(Collider collidedObj) {
        if (collidedObj.gameObject.tag == "Player") {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
