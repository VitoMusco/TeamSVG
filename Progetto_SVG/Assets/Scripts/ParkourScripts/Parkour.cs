using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parkour : MonoBehaviour
{


    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(player.transform.position.y <= -4)
        {
            respawn();
        }

    }
    
    public void respawn()
    {
        //player.GetComponent<PlayerMovement>().respawn();
    }
}
