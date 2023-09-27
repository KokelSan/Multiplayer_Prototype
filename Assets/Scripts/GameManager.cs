using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{
    public static bool IsGamePaused = false;

    public static void SetPauseState(bool newState)
    {
        IsGamePaused = newState;
    }
}