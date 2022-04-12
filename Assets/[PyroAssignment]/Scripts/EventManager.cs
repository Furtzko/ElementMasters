using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static event Action OnFightBegin;
    public static void _onFightBegin()
    {
        OnFightBegin?.Invoke();
    }

    public static event Action OnFightEnd;
    public static void _onFightEnd()
    {
        OnFightEnd?.Invoke();
    }

    public static event Action OnGateTriggered;
    public static void _onGateTriggered()
    {
        OnGateTriggered?.Invoke();
    }

    public static event Action OnLevelFail;
    public static void _onLevelFail()
    {
        OnLevelFail?.Invoke();
    }

    public static event Action OnFinishLinePassed;
    public static void _onFinishLinePassed()
    {
        OnFinishLinePassed?.Invoke();
    }

    public static event Action OnLevelCompleted;
    public static void _onLevelCompleted()
    {
        OnLevelCompleted?.Invoke();
    }

    public static event Action OnHadouken;
    public static void _onHadouken()
    {
        OnHadouken?.Invoke();
    }
}
