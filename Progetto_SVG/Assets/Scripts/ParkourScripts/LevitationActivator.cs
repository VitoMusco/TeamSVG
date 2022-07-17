using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitationActivator : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!GlobalEvents.HalfParkourCompletion) GlobalEvents.HalfParkourCompletion = true;
        if (other.gameObject.CompareTag("Player")) other.gameObject.GetComponent<PlayerController>().setLevitation(true);
    }
}
