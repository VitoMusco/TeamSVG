using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deadArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) other.gameObject.GetComponent<PlayerController>().kill();
    }
}
