using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parkour : MonoBehaviour
{

    
    public Transform playerTransform;
    public Transform startPosition;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(playerTransform.position.y <= -4)
        {
            playerTransform .position= startPosition.position;
        }
    }
}
