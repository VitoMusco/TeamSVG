using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private bool isLastCheckPoint = false;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (isLastCheckPoint)
                if (!GlobalEvents.SeenRotatingAxes) GlobalEvents.SeenRotatingAxes = true;
            spawnPoint.transform.position= transform.localPosition;
            spawnPoint.transform.rotation = transform.localRotation;

        }
    }
}
