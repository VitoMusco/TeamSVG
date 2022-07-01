using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizBehaviour : MonoBehaviour
{
    public Transform ghostSpawner;
    public GameObject ghostPrefab;

    public void getGrabbed(Transform playerPos) {
        GameObject ghost = (GameObject)Instantiate(ghostPrefab, ghostSpawner.position, ghostSpawner.rotation);
        ghost.GetComponent<GhostBehaviour>().assignPlayer(playerPos);
        Destroy(gameObject);
    }
}
