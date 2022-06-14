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
            transform.localPosition = new Vector3(0, 0, transform.localPosition.z + speed * Time.deltaTime);

            //rotazione
            transform.Rotate(new Vector3(0, 0, transform.localRotation.z + speed * Time.deltaTime* 150));


            timeLived += Time.deltaTime;
            if (timeLived > lifetime)
            {
            timeLived = 0;
            gameObject.SetActive(false);
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("entrato nell'if");
            other.gameObject.GetComponent<PlayerController>().kill();
            
        }
    }
}
