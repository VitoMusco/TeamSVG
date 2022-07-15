using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBehaviour : MonoBehaviour
{
    public Transform player;
    public GameObject dmgObject;
    public Transform cannonBallSpawner;

    private Vector3 playerPosition;
    [SerializeField] private float spawnTime;
    [SerializeField, Range(0,10)] private float shootingOffset;
    private float timeToSpawn = 0;
    

    void Awake() {
        timeToSpawn -= shootingOffset;
    }

    void Update()
    {
        followPlayer();
        handleShooting();
    }

    void followPlayer() {
        playerPosition = player.position;
        playerPosition.y = transform.position.y;
        transform.LookAt(playerPosition);
    }

    void handleShooting() {
        timeToSpawn += Time.deltaTime;

        if (timeToSpawn >= spawnTime)
        {
            Instantiate(dmgObject, cannonBallSpawner.position, transform.rotation);
            timeToSpawn = 0;
        }
    }
}
