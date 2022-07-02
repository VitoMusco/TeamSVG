using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostBehaviour : MonoBehaviour
{
    public Transform player;

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
    }

    public void activate() {
        isActivated = true;
        agent.enabled = true;
        anim.enabled = true;
    }
}
