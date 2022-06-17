using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorUnlockerBehaviour : MonoBehaviour
{
    public QuizBoxBehaviour[] quizBoxes;
    private bool doorUnlocked = false;
    private bool isCombinationCorrect;
    void Update() {
        if(!doorUnlocked) checkIfCombinationCorrect();
    }

    void checkIfCombinationCorrect() {
        foreach (QuizBoxBehaviour quizBox in quizBoxes)
        {
            isCombinationCorrect = true;
            if (!quizBox.getCorrectAnswer()) isCombinationCorrect = false;
        }
        if (isCombinationCorrect) unlockDoor();
        else isCombinationCorrect = false;
    }

    void unlockDoor() {
        Debug.Log("DOOR UNLOCKED!");
        doorUnlocked = true;
    }
}
