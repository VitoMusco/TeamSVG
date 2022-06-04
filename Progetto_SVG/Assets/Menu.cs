using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.SceneManagement;//per cambiare scena

public class Menu : MonoBehaviour
{
    public void NewGame(){
        SceneManager.LoadScene(2);
    }
     public void ContinueGame(){
        
    }
     public void ExitGame(){
        Application.Quit();
        Debug.log("Esci");
    }
     public void OptionGame(){
        
    }
}
