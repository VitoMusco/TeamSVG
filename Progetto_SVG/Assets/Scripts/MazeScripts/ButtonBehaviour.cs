using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehaviour : MonoBehaviour
{
    public bool buttonType = true; //True per bottone di sopra, false per bottone di giu
    public QuizBoxBehaviour quizBox;

    public void pressButton() {
        if (buttonType) quizBox.moveUp();
        else quizBox.moveDown();
    }

}
