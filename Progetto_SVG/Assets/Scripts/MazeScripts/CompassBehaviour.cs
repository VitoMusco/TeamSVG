using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassBehaviour : MonoBehaviour
{
    public List<Transform> quizes;

    // Update is called once per frame
    void Update()
    {
        findAndPointToNearestQuiz();
    }

    void findAndPointToNearestQuiz() {
        Vector3 nearestQuiz = new Vector3();
        float minDistance = 0f;
        float distance = 0f;
        foreach (Transform quiz in quizes)
        {
            distance = Vector3.Distance(quiz.position, transform.position);
            if (minDistance == 0f) {
                minDistance = distance;
                nearestQuiz = quiz.position;
            }
            if (distance < minDistance)
            {
                print(quiz.position);
                nearestQuiz = quiz.position;
                minDistance = distance;
            }
        }
        lookAt(nearestQuiz);
        //transform.LookAt(nearestQuiz, Vector3.up);
    }

    void lookAt(Vector3 nearestQuiz) {
        Vector3 lookPosition = new Vector3();
        Quaternion whereToLook = new Quaternion();

        lookPosition = nearestQuiz - transform.position;
        whereToLook = Quaternion.LookRotation(lookPosition);
        transform.rotation = whereToLook;
    }
}
