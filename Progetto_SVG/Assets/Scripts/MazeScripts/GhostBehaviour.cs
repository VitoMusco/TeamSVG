using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostBehaviour : MonoBehaviour
{
    public Transform player;

    [SerializeField] private float timeBeforeDeath = 15f;
    private float timeAfterActivation = 0f;
    private NavMeshAgent agent;
    private Animator anim;
    private bool isActivated = false;

    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if(isActivated) agent.SetDestination(player.position);
        if (isActivated) timeAfterActivation += Time.deltaTime;
        if (isActivated && timeAfterActivation >= timeBeforeDeath) Destroy(gameObject);
    }

    public void activate() {
        isActivated = true;
        agent.enabled = true;
        anim.enabled = true;
    }
}
