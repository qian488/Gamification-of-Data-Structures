using UnityEngine;

public class GameDebugger : MonoBehaviour
{
    private static bool isDebugMode = false;

    public static void Log(string message)
    {
        if (isDebugMode)
        {
            Debug.Log($"[Game] {message}");
        }
    }

    public static void ToggleDebugMode()
    {
        isDebugMode = !isDebugMode;
        Debug.Log($"Debug Mode: {isDebugMode}");
    }
} 