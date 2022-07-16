using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBehaviour : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;

    void Update()
    {
        Vector3 cameraPos = playerCamera.position;
        cameraPos.y = transform.position.y;
        transform.LookAt(cameraPos);
    }
}
