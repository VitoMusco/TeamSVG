using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//per cambiare scena

public class Menu : MonoBehaviour
{
    public void NewGame(){
        SceneManager.LoadScene(1);
    }
    public void ContinueGame(){
        SceneManager.LoadScene(2);
    }
    public void ExitGame(){
        Application.Quit();
        Debug.Log("Esci");
    }
    public void OptionGame(){
       
    }
}
