using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBehaviour : MonoBehaviour
{
    public Transform player;
    private Vector3 playerPosition;

    void Update()
    {
        followPlayer();        
    }

    void followPlayer() {
        playerPosition = player.position;
        playerPosition.y = transform.position.y;
        transform.LookAt(playerPosition);
    }
}
