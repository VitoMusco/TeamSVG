using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHandler : MonoBehaviour
{
    private static int levelToPlay = 1; //1-Labirinto, 2-Parkour, 3-Combattimento
    
    public static void completedLevel() {
        levelToPlay++;
    }

    public static int getLevelToPlay() {
        return levelToPlay;
    }
}
