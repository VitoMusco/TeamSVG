using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DmgObjectScript : MonoBehaviour
{
    float speed = 25;
    float lifetime = 2;
    private float timeLived;
    
    



    void Start()
    {
        timeLived = 0;   
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.rotation.x != 0) transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + speed * Time.deltaTime * transform.rotation.x);
        if(transform.rotation.z != 0) transform.position = new Vector3(transform.position.x + speed * Time.deltaTime * transform.rotation.z * -1, transform.position.y , transform.position.z);

        timeLived += Time.deltaTime;
        if (timeLived > lifetime) Destroy(gameObject);

    }
}
