using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DmgObjectScript : MonoBehaviour
{

    public Transform playerTransform;
    public Transform playerStartPosition;
    float speed = 1f;
    float lifetime = 5;
    private float timeLived;
    



    void Start()
    {
        timeLived = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = transform.forward * Time.deltaTime * speed;
        timeLived += Time.deltaTime;
        if (timeLived > lifetime) Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "player")
        {
            playerTransform = playerStartPosition;
        }
    }
}
