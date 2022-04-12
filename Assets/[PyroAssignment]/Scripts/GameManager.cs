using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool isLevelFailed;
    public static bool isFinishLinePassed;
    public static bool isLevelCompleted;
    public static bool isHadouken;

    private void OnEnable()
    {
        EventManager.OnFinishLinePassed += FinishLinePassed;
        EventManager.OnLevelFail += LevelFail;
        EventManager.OnLevelCompleted += LevelCompleted;
        EventManager.OnHadouken += Hadouken;
    }

    private void OnDisable()
    {
        EventManager.OnFinishLinePassed -= FinishLinePassed;
        EventManager.OnLevelFail -= LevelFail;
        EventManager.OnLevelCompleted -= LevelCompleted;
        EventManager.OnHadouken -= Hadouken;
    }

    private void Start()
    {
        isLevelFailed = false;
        isFinishLinePassed = false;
        isLevelCompleted = false;
        isHadouken = false;

    }

    private void FinishLinePassed()
    {
        isFinishLinePassed = true;
    }

    private void LevelFail()
    {
        isLevelFailed = true;
    }

    private void LevelCompleted()
    {
        isLevelCompleted = true;
    }

    private void Hadouken()
    {
        isHadouken = true;
    }
}
