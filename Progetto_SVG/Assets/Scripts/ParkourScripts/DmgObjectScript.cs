using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DmgObjectScript : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float lifetime;
    private float timeLived;
   
    void Start()
    {
        timeLived = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (lifetime != 0 && speed != 0)
        { 
            transform.Translate(Vector3.forward * speed * Time.deltaTime);


            timeLived += Time.deltaTime;
            if (timeLived > lifetime) Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //other.gameObject.GetComponent<PlayerController>().kill();
            
        }
    }
}
