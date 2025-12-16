using System;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static bool IsGamePaused { get; private set; } = false;

    public static event Action<bool> OnPauseChanged;

    public static void SetPaused(bool pause)
    {
        if (IsGamePaused == pause) return;

        IsGamePaused = pause;
        OnPauseChanged?.Invoke(pause); 
    }
}
