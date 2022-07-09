using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizBoxBehaviour : MonoBehaviour
{
    public int id;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public int currentPosition = 1;
    public int solutionPosition = 1;
    public DoorUnlockerBehaviour quizHandler;

    // Start is called before the first frame update
    void Awake()
    {
        startPosition = transform.position;
        endPosition = startPosition + new Vector3(0f, 1.6f, 0f);
    }

    void Update() {
        if (currentPosition == solutionPosition) {
            quizHandler.solvedQuiz(id);
        }
    }

    public void moveUp() {
        if (currentPosition == 5) {
            transform.position = startPosition;
            currentPosition = 1;
        }
        else {
            currentPosition++;
            transform.position += new Vector3(0f, 0.4f, 0f);
        }
    }

    public void moveDown() {
        if (currentPosition == 1) {
            transform.position = endPosition;
            currentPosition = 5;
        }
        else {
            currentPosition--;
            transform.position -= new Vector3(0f, 0.4f, 0f);
        }
    }

    public bool getCorrectAnswer() {
        if (currentPosition == solutionPosition) return true;
        else return false;
    }
   
}
