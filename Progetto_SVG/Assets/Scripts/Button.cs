using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Transform plane;
    public bool spostaSopra=false;
    public bool spostaSotto=false;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public int imageNum = 1;


    // Start is called before the first frame update
    void Awake()
    {
        startPosition = plane.position;
        endPosition = startPosition + new Vector3(0f, 1.6f, 0f);

    }

    // Update is called once per frame
    void Update()
    {
        if (spostaSopra) {
            spostaSopra = false;
            if (imageNum == 5)
            {
                plane.position = startPosition;
                imageNum = 1;
            }
            else
            {
                imageNum++;
                moveUp();
            }
        }
        if (spostaSotto)
        {
            spostaSotto = false;
            if (imageNum == 1)
            {
                plane.position = endPosition;
                imageNum = 5;
            }
            else
            {
                imageNum--;
                moveDown();
            }
        }
            

        
    }

    void moveUp() {
        plane.position += new Vector3(0f,0.4f,0f);
    }
    
    void moveDown() {
        plane.position -= new Vector3(0f, 0.4f, 0f);
    }

   
}
