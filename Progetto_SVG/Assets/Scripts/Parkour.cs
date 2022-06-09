using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parkour : MonoBehaviour
{

    
    public Transform playerTransform;
    private Vector3 startPosition;
    private Quaternion startRotation;
    // Start is called before the first frame update
    void Start()
    {
        
        startPosition = playerTransform.position;
        startRotation = playerTransform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerTransform.position.y <= -4)
        {
            playerTransform.position= startPosition;
        }
    }
}
