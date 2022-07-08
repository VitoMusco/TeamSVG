using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectParkourMovement : MonoBehaviour
{

    public Transform pointA, pointB;
    
    private Rigidbody rb;
    private Vector3 currentPos;
    private bool reachedPoint = false; //False arriva a B, true arriva ad A

    private CharacterController player;

    [SerializeField] float timeFromAToB = 5f;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(moveObject());
    }
    
    IEnumerator moveObject() {
        float timeElapsed = 0f;

        while (timeElapsed < timeFromAToB) {
            timeElapsed += Time.deltaTime;

            if(reachedPoint)
                currentPos = Vector3.Lerp(pointB.position, pointA.position, timeElapsed / timeFromAToB);
            else
                currentPos = Vector3.Lerp(pointA.position, pointB.position, timeElapsed / timeFromAToB);

            rb.MovePosition(currentPos);
            yield return null;
        }

        reachedPoint = !reachedPoint;
        StartCoroutine(moveObject());
    }

    void OnTriggerEnter(Collider collidedObject)
    {
        if (collidedObject.CompareTag("Player"))
        {
            player = collidedObject.gameObject.GetComponent<CharacterController>();
        }
    }

    void OnTriggerStay(Collider collidedObject)
    {
        Vector3 platformVelocity;
        if (collidedObject.CompareTag("Player"))
        {
            platformVelocity = rb.velocity;
            //platformVelocity.y = 0f;
            
            player.Move(platformVelocity * Time.deltaTime);
        }
    }
}
