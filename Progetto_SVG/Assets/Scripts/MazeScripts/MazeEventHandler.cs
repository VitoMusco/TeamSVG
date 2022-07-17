using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeEventHandler : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    [SerializeField] private List<QuizBehaviour> quizes;
    [SerializeField] private List<GhostBehaviour> ghosts;
    [SerializeField] private DoorUnlockerBehaviour quizGroup;

    [SerializeField] private AudioClip firstMazeSpawnClip;
    [SerializeField] private string firstMazeSpawnClipSubtitles;
    
    [SerializeField] private AudioClip usedCompassClip;
    [SerializeField] private string usedCompassClipSubtitles;

    [SerializeField] private AudioClip takenQuizClip;
    [SerializeField] private string takenQuizClipSubtitles;

    [SerializeField] private AudioClip ghostSpawnClip;
    [SerializeField] private string ghostSpawnClipSubtitles;

    [SerializeField] private AudioClip ghostDamageClip;
    [SerializeField] private string ghostDamageClipSubtitles;

    [SerializeField] private AudioClip ghostDeathClip;
    [SerializeField] private string ghostDeathClipSubtitles;

    [SerializeField] private AudioClip wrongAnswerClip;
    [SerializeField] private string wrongAnswerClipSubtitles;

    [SerializeField] private AudioClip firstCorrectAnswerClip;
    [SerializeField] private string firstCorrectAnswerClipSubtitles;

    [SerializeField] private AudioClip secondCorrectAnswerClip;
    [SerializeField] private string secondCorrectAnswerClipSubtitles;

    [SerializeField] private AudioClip thirdCorrectAnswerClip;
    [SerializeField] private string thirdCorrectAnswerClipSubtitles;

    [SerializeField] private AudioClip fourthCorrectAnswerClip;
    [SerializeField] private string fourthCorrectAnswerClipSubtitles;

    [SerializeField] private AudioClip fifthCorrectAnswerClip;
    [SerializeField] private string fifthCorrectAnswerClipSubtitles;

    [SerializeField] private AudioClip openedMazeDoorClip;
    [SerializeField] private string openedMazeDoorClipSubtitles;
    

    void Awake() {
        if (!GlobalEvents.SpawnedInMaze)
        {
            GlobalEvents.SpawnedInMaze = player.playVoiceLine(firstMazeSpawnClip, firstMazeSpawnClipSubtitles);
        }
    }

    void Update()
    {
        if (!player.checkIfAlive()) {
            Invoke(nameof(resetLevel), player.getTimeToRespawn());
        }
        handleVoiceLines();

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

    void handleVoiceLines() {
        if (GlobalEvents.UsedCompass && !GlobalEvents.UsedCompassPlayed)
        {
            GlobalEvents.UsedCompassPlayed = player.playVoiceLine(usedCompassClip, usedCompassClipSubtitles);
        }
        if (GlobalEvents.UsedCompass && !GlobalEvents.UsedCompassPlayed)
        {
            GlobalEvents.UsedCompassPlayed = player.playVoiceLine(usedCompassClip, usedCompassClipSubtitles);
        }
        if (GlobalEvents.TakenFirstQuiz && !GlobalEvents.TakenFirstQuizPlayed)
        {
            GlobalEvents.TakenFirstQuizPlayed = player.playVoiceLine(takenQuizClip, takenQuizClipSubtitles);
        }
        if (GlobalEvents.FirstGhostSpawn && !GlobalEvents.FirstGhostSpawnPlayed)
        {
            GlobalEvents.FirstGhostSpawnPlayed = player.playVoiceLine(ghostSpawnClip, ghostSpawnClipSubtitles);
        }
        if (GlobalEvents.FirstGhostSlap && !GlobalEvents.FirstGhostSlapPlayed)
        {
            GlobalEvents.FirstGhostSlapPlayed = player.playVoiceLine(ghostDamageClip, ghostDamageClipSubtitles);
        }
        if (GlobalEvents.FirstGhostDeath && !GlobalEvents.FirstGhostDeathPlayed)
        {
            GlobalEvents.FirstGhostDeathPlayed = player.playVoiceLine(ghostDeathClip, ghostDeathClipSubtitles);
        }
        if (GlobalEvents.FirstWrongAnswer && !GlobalEvents.FirstWrongAnswerPlayed)
        {
            GlobalEvents.FirstWrongAnswerPlayed = player.playVoiceLine(wrongAnswerClip, wrongAnswerClipSubtitles);
        }
        if (GlobalEvents.FirstQuizCorrectAnswer && !GlobalEvents.FirstQuizCorrectAnswerPlayed)
        {
            GlobalEvents.FirstQuizCorrectAnswerPlayed = player.playVoiceLine(firstCorrectAnswerClip, firstCorrectAnswerClipSubtitles);
        }
        if (GlobalEvents.SecondQuizCorrectAnswer && !GlobalEvents.SecondQuizCorrectAnswerPlayed)
        {
            GlobalEvents.SecondQuizCorrectAnswerPlayed = player.playVoiceLine(secondCorrectAnswerClip, secondCorrectAnswerClipSubtitles);
        }
        if (GlobalEvents.ThirdQuizCorrectAnswer && !GlobalEvents.ThirdQuizCorrectAnswerPlayed)
        {
            GlobalEvents.ThirdQuizCorrectAnswerPlayed = player.playVoiceLine(thirdCorrectAnswerClip, thirdCorrectAnswerClipSubtitles);
        }
        if (GlobalEvents.FourthQuizCorrectAnswer && !GlobalEvents.FourthQuizCorrectAnswerPlayed)
        {
            GlobalEvents.FourthQuizCorrectAnswerPlayed = player.playVoiceLine(fourthCorrectAnswerClip, fourthCorrectAnswerClipSubtitles);
        }
        if (GlobalEvents.FifthQuizCorrectAnswer && !GlobalEvents.FifthQuizCorrectAnswerPlayed)
        {
            GlobalEvents.FifthQuizCorrectAnswerPlayed = player.playVoiceLine(fifthCorrectAnswerClip, fifthCorrectAnswerClipSubtitles);
        }
        if (GlobalEvents.OpenedMazeDoor && !GlobalEvents.OpenedMazeDoorPlayed)
        {
            GlobalEvents.OpenedMazeDoorPlayed = player.playVoiceLine(openedMazeDoorClip, openedMazeDoorClipSubtitles);
        }
    }
}
