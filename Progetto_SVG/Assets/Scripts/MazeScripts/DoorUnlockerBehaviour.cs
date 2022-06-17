using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorUnlockerBehaviour : MonoBehaviour
{
    public QuizBoxBehaviour[] quizBoxes;

    public Transform door;
    public Transform doorEndTransform;

    private Vector3 doorStartPosition;
    private bool doorUnlocked = false;
    private bool isCombinationCorrect;


    void Awake() {
        doorStartPosition = door.position;
    }

    void Update() {
        if(!doorUnlocked) checkIfCombinationCorrect();
    }

    void checkIfCombinationCorrect() {
        foreach (QuizBoxBehaviour quizBox in quizBoxes)
        {
            isCombinationCorrect = true;
            if (!quizBox.getCorrectAnswer()) {
                isCombinationCorrect = false;
                break;
            }
        }
        if (isCombinationCorrect) StartCoroutine(unlockDoor());
        else isCombinationCorrect = false;
    }

    IEnumerator unlockDoor() {
        float timeElapsed = 0f;
        float timeToMove = 5f;
        doorUnlocked = true;

        while (timeElapsed < timeToMove) {
            timeElapsed += Time.deltaTime;
            door.position = Vector3.Lerp(doorStartPosition, doorEndTransform.position, timeElapsed / timeToMove);

            yield return null;
        }
        door.position = doorEndTransform.position;
    }
}
