using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmButtonBehaviour : MonoBehaviour
{
    public QuizBoxBehaviour quizBox;

    public void pressButton() {
        quizBox.confirmAnswer();
    }
}
