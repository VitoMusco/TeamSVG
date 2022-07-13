using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCommandsAvailable : MonoBehaviour
{

    public List<string> commandsAvailable;
    public TMP_Text commandText;
    public GameObject player;
    private PlayerController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = player.GetComponent<PlayerController>();
        setText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void setText()
    {        
        string commandToSet = "";
        foreach(string command in commandsAvailable)
        {
            commandToSet += "\n" + command;
        }

        commandText.text = commandToSet;

    }

}
