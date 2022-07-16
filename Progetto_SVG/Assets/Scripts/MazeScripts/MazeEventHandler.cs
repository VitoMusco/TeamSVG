using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeEventHandler : MonoBehaviour
{
    public PlayerController player;

    public List<QuizBehaviour> quizes;
    public List<GhostBehaviour> ghosts;
    public DoorUnlockerBehaviour quizGroup;

    void Update()
    {
        if (!player.checkIfAlive()) {
            Invoke(nameof(resetLevel), player.getTimeToRespawn());
        }
    }

    void resetLevel() {
        resetQuizes();
        killAllGhosts();
        resetQuizGroup();
    }

    void resetQuizes() {
        foreach (QuizBehaviour quiz in quizes) {
            quiz.reset();
        }
    }
    void killAllGhosts() {
        foreach (GhostBehaviour ghost in ghosts)
        {
            ghost.kill();
        }
    }

    void resetQuizGroup() {
        quizGroup.reset();
    }
}
