using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectParkourMovement : MonoBehaviour
{
    
    [SerializeField] bool Xaxis = true;
    [SerializeField] float maxDistance;
    [SerializeField] float minDistance;
    [SerializeField] float velocity;
    private bool toMax = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Xaxis)
        {
            if (toMax)
            {
                transform.position = new Vector3(transform.position.x + velocity * Time.deltaTime, transform.position.y, transform.position.z);
                if (transform.position.x > maxDistance) toMax = false;
            }
            else
            {
                transform.position = new Vector3(transform.position.x - velocity * Time.deltaTime, transform.position.y, transform.position.z);
                if (transform.position.x < minDistance) toMax = true;
            }
        }
        else
        {
            if (toMax)
            {
                transform.position = new Vector3(transform.position.x , transform.position.y + velocity * Time.deltaTime, transform.position.z);
                if (transform.position.y > maxDistance) toMax = false;
            }
            else
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - velocity * Time.deltaTime, transform.position.z);
                if (transform.position.y < minDistance) toMax = true;
            }
        }
    }

}
