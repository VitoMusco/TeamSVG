using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassBehaviour : MonoBehaviour
{
    public List<GameObject> quizes = new List<GameObject>();
    public Transform exitPortal;

    private bool canSearchForQuizes = false;

    // Update is called once per frame
    void Update()
    {
        if(canSearchForQuizes) findAndPointToNearestQuiz();
    }

    void findAndPointToNearestQuiz() {
        if (quizes.Count == 0) {
            transform.LookAt(exitPortal);
            return;
        }
        Vector3 nearestQuiz = new Vector3();
        float minDistance = 0f;
        float distance = 0f;
        foreach (GameObject quiz in quizes)
        {
            distance = Vector3.Distance(quiz.transform.position, transform.position);
            if (minDistance == 0f) {
                minDistance = distance;
                nearestQuiz = quiz.transform.position;
            }
            if (distance < minDistance)
            {
                nearestQuiz = quiz.transform.position;
                minDistance = distance;
            }
        }
        transform.LookAt(nearestQuiz, Vector3.up);
    }

    public void foundQuiz(GameObject quiz) {
        quizes.Remove(quiz);
    }

    public void enable() {
        canSearchForQuizes = true;
    }
}
