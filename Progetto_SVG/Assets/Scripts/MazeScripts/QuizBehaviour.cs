using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizBehaviour : MonoBehaviour
{
    public int id;
    public Transform ghostSpawner;
    public GhostBehaviour ghostPrefab;
    public DoorUnlockerBehaviour quizHandler;

    public void getGrabbed() {
        quizHandler.foundQuiz(id);
        ghostPrefab.transform.position = ghostSpawner.position;
        ghostPrefab.transform.rotation = ghostSpawner.rotation;
        ghostPrefab.activate();
        Destroy(gameObject);
    }
}
