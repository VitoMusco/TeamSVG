using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectParkourMovement : MonoBehaviour
{

    public Transform pointA, pointB;
    public bool reachedPoint = false; //False arriva a B, true arriva ad A

    private Rigidbody rb;
    private Vector3 currentPos;

    private bool playerOnPlatform = false;
    private CharacterController player;

    [SerializeField] bool Xaxis = true;
    [SerializeField] float timeFromAToB = 5f;
    [SerializeField] bool isDangerous;
    [SerializeField] bool rotate = false;
    private bool toMax = true;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(moveObject());
    }

    // Update is called once per frame
    void Update()
    {
        //if (rotate) transform.Rotate(new Vector3(0, 0, transform.localRotation.z + speed * Time.deltaTime * 500));
    }

    /*void FixedUpdate() {
        currentPos = Vector3.Lerp(pointA.position, pointB.position, Mathf.Cos(Time.time / timeFromAToB * Mathf.PI * 2) * -.5f + .5f);

        rb.MovePosition(currentPos);
    }*/
    
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
            platformVelocity.y = 0f;
            
            player.Move(platformVelocity * Time.deltaTime);
        }
    }
}
