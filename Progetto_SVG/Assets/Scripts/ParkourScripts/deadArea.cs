using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deadArea : MonoBehaviour
{
    // Start is called before the first frame update

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
