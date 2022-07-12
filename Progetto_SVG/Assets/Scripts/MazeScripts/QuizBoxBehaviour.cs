using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizBoxBehaviour : MonoBehaviour
{
    public int id;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public int currentPosition = -1; //-1 = Bloccato, 0= Sbloccato
    public int solutionPosition = 1;
    public DoorUnlockerBehaviour quizHandler;
    public int maxPosition = 5;
    public int minPosition = 1;

    // Start is called before the first frame update
    void Awake()
    {
        startPosition = transform.position + new Vector3(0f, 0.8f, 0f);
        endPosition = startPosition + new Vector3(0f, 1.6f, 0f);
    }

    void Update() {
        if (currentPosition == solutionPosition) {
            quizHandler.solvedQuiz(id);
        }
    }

    public void moveUp() {
        if (currentPosition == -1) return;
        else if (currentPosition >= maxPosition) {
            transform.position = startPosition;
            currentPosition = minPosition;
        }
        else {
            currentPosition++;
            transform.position += new Vector3(0f, 0.4f, 0f);
        }
    }

    public void moveDown() {
        if (currentPosition == -1) return;
        if (currentPosition <= minPosition) {
            transform.position = endPosition;
            currentPosition = maxPosition;
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

    public void unlock() {
        currentPosition++;
        transform.position += new Vector3(0f, 0.4f, 0f);
    }

    public void lockQuiz() {
        currentPosition = -1;
        transform.position = startPosition - new Vector3(0f, 1.2f, 0f);
    }

    public void reset() {
        currentPosition = -1;
        transform.position = startPosition - new Vector3(0f, 0.8f, 0f);
    }
}
