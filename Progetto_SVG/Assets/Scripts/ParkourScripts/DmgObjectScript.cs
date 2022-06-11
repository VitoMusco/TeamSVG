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
        if(transform.rotation.x != 0) transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + speed * Time.deltaTime * transform.rotation.x);
        if(transform.rotation.z != 0) transform.position = new Vector3(transform.position.x + speed * Time.deltaTime * transform.rotation.z * -1, transform.position.y , transform.position.z);

        timeLived += Time.deltaTime;
        if (timeLived > lifetime)
        {
            timeLived = 0;
            gameObject.SetActive(false);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("entrato nell'if");
            other.gameObject.GetComponent<PlayerMovement>().respawn();
            
        }
    }
}
