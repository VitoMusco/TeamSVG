using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizBoxBehaviour : MonoBehaviour
{
    public Transform plane;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public int imageNum = 1;


    // Start is called before the first frame update
    void Awake()
    {
        startPosition = plane.position;
        endPosition = startPosition + new Vector3(0f, 1.6f, 0f);

    }

    public void moveUp() {
        if (imageNum == 5) {
            plane.position = startPosition;
            imageNum = 1;
        }
        else {
            imageNum++;
            plane.position += new Vector3(0f, 0.4f, 0f);
        }
    }

    public void moveDown() {
        if (imageNum == 1) {
            plane.position = endPosition;
            imageNum = 5;
        }
        else {
            imageNum--;
            plane.position -= new Vector3(0f, 0.4f, 0f);
        }
    }

   
}
