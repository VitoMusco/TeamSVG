using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBehaviour : MonoBehaviour
{
    public Transform player;
    public GameObject dmgObject;
    public Transform cannonBallSpawner;
    public LayerMask whatIsPlayer;

    private Vector3 playerPosition;
    [SerializeField] private float rangeOfVision = 10f;
    [SerializeField] private float spawnTime;
    [SerializeField, Range(0, 10)] private float shootingOffset;
    private float timeToSpawn = 0;
    private bool playerInShootArea = false;


    void Awake()
    {
        timeToSpawn -= shootingOffset;
    }

    void Update()
    {
        checkIfPlayerIsInShootArea();
        followPlayer();
        handleShooting();
    }

    void checkIfPlayerIsInShootArea()
    {
        playerInShootArea = Physics.CheckSphere(transform.position, rangeOfVision, whatIsPlayer);
    }

    void followPlayer()
    {
        playerPosition = player.position;
        playerPosition.y = transform.position.y;
        transform.LookAt(playerPosition);
    }

    void handleShooting()
    {
        if (!playerInShootArea) return;
        timeToSpawn += Time.deltaTime;

        if (timeToSpawn >= spawnTime)
        {
            Instantiate(dmgObject, cannonBallSpawner.position, transform.rotation);
            timeToSpawn = 0;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, rangeOfVision);
    }
}
