using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DoorUnlockerBehaviour : MonoBehaviour
{
    public QuizBoxBehaviour[] quizBoxes;

    public Transform door;
    public Transform doorEndTransform;

    public List<string> quizes;
    public string defaultQuizLine;
    public string completionText;
    private string currentQuiz;
    private int currentQuizId = 0;
    [SerializeField] private short[] foundQuizes; //0- non trovato 1-trovato ma non risolto 2-trovato e risolto

    public TMP_Text quizText;

    private bool quizIsAwaitingSolution = false;
    private Vector3 doorStartPosition;
    private bool isCombinationCorrect;

    void Awake() {
        doorStartPosition = door.position;
        setDefaultQuizLine();
        foundQuizes = new short[quizes.Count];
    }

    IEnumerator unlockDoor() {
        float timeElapsed = 0f;
        float timeToMove = 5f;

        while (timeElapsed < timeToMove) {
            timeElapsed += Time.deltaTime;
            door.position = Vector3.Lerp(doorStartPosition, doorEndTransform.position, timeElapsed / timeToMove);

            yield return null;
        }
        door.position = doorEndTransform.position;
    }

    public void foundQuiz(int quizId) {
        if (!quizIsAwaitingSolution) {
            quizBoxes[quizId - 1].unlock();
            askQuiz(quizId - 1);
        }
        foundQuizes[quizId - 1] = 1;
    }

    public void solvedQuiz(int quizId) {
        if (foundQuizes[quizId - 1] != 1) return;
        quizIsAwaitingSolution = false;
        foundQuizes[quizId - 1] = 2;
        quizBoxes[quizId - 1].lockQuiz();
        for (int i=0; i<foundQuizes.Length; i++) {
            if (foundQuizes[i] == 1) {
                quizBoxes[i].unlock();
                askQuiz(i);
                break;
            }
        }
        if (!quizIsAwaitingSolution) {
            isCombinationCorrect = true;
            for (int i = 0; i < foundQuizes.Length; i++)
            {
                if (foundQuizes[i] != 2)
                {
                    isCombinationCorrect = false;
                }
            }
            if (isCombinationCorrect)
            {
                quizText.text = completionText;
                StartCoroutine(unlockDoor());
            }
            else setDefaultQuizLine();
        }
    }

    void askQuiz(int quizId) {
        quizIsAwaitingSolution = true;
        quizText.text = quizes[quizId];
        currentQuizId = quizId;
    }

    void setDefaultQuizLine() {
        quizText.text = defaultQuizLine;
    }

    public void reset() {
        for(int i = 0; i < foundQuizes.Length; i++){
            foundQuizes[i] = 0;
        }
        for (int i = 0; i < quizBoxes.Length; i++) {
            quizBoxes[i].reset();
        }
        quizText.text = defaultQuizLine;
    }
}
