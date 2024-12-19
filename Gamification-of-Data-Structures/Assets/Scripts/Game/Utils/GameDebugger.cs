using UnityEngine;

/// <summary>
/// 游戏调试工具类
/// 提供统一的调试日志输出和调试模式控制
/// </summary>
/// <remarks>
/// 主要功能：
/// - Log(string message)：输出带格式的调试日志
/// - ToggleDebugMode()：切换调试模式开关
/// 
/// 使用方式：
/// GameDebugger.Log("调试信息");  // 输出调试信息
/// GameDebugger.ToggleDebugMode();  // 切换调试模式
/// </remarks>
public class GameDebugger : MonoBehaviour
{
    /// <summary>
    /// 调试模式状态标志
    /// true表示开启调试模式，false表示关闭
    /// </summary>
    private static bool isDebugMode = false;

    /// <summary>
    /// 输出调试日志
    /// 只有在调试模式开启时才会输出
    /// </summary>
    /// <param name="message">要输出的调试信息</param>
    public static void Log(string message)
    {
        if (isDebugMode)
        {
            Debug.Log($"[Game] {message}");
        }
    }

    /// <summary>
    /// 切换调试模式的开启/关闭状态
    /// 同时会输出一条日志表明当前状态
    /// </summary>
    public static void ToggleDebugMode()
    {
        isDebugMode = !isDebugMode;
        Debug.Log($"Debug Mode: {isDebugMode}");
    }
} 