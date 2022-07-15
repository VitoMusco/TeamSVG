using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    //[SerializeField] bool Xaxis = true;
    [SerializeField] float spawnTime;
    [SerializeField] float waitTime;
    private float timeToSpawn = 0;
    public GameObject dmgObject;

    // Start is called before the first frame update
    void Start()
    {
        timeToSpawn -= waitTime;
    }

    // Update is called once per frame
    void Update()
    {
        timeToSpawn += Time.deltaTime;
        
        
        if(timeToSpawn >= spawnTime)
        {

            Instantiate(dmgObject, transform.position, transform.rotation);
            timeToSpawn = 0;

        }
    }
}
