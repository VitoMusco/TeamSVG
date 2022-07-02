using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizBehaviour : MonoBehaviour
{
    public Transform ghostSpawner;
    public GhostBehaviour ghostPrefab;

    public void getGrabbed() {
        ghostPrefab.transform.position = ghostSpawner.position;
        ghostPrefab.transform.rotation = ghostSpawner.rotation;
        ghostPrefab.activate();
        Destroy(gameObject);
    }
}
