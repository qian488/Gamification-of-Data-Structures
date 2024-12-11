using UnityEngine;
using System.Collections;

/// <summary>
/// 游戏初始化类
/// 负责在游戏开始时初始化所有必要的管理器和组件，确保游戏系统的正确启动顺序
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 设置基本的游戏环境，如环境光照等
/// 2. 按特定顺序初始化各个管理器
/// 3. 生成初始迷宫
/// </remarks>
public class GameInit : MonoBehaviour
{
    /// <summary>
    /// Unity启动时调用，设置环境并开始初始化流程
    /// </summary>
    void Start()
    {
        // 调亮环境光
        RenderSettings.ambientLight = new Color(1f, 1f, 1f);  // 改为纯白色
        RenderSettings.ambientIntensity = 1.5f;  // 增加强度
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

        // 使用协程确保初始化顺序
        StartCoroutine(InitGame());
    }

    /// <summary>
    /// 游戏初始化协程
    /// 按顺序执行初始化流程，确保各系统正确启动
    /// </summary>
    private IEnumerator InitGame()
    {
        // 初始化所有管理器
        yield return InitAllManagers();
        
        // 等待一帧确保所有初始化完成
        yield return new WaitForEndOfFrame();
        
        // 生成迷宫
        MazeManager.GetInstance().GenerateMaze();
    }

    /// <summary>
    /// 管理器初始化协程
    /// 按照依赖关系顺序初始化各个管理器
    /// 1. 首先初始化PlayerManager
    /// 2. 然后初始化UI管理器
    /// 3. 最后初始化迷宫管理器
    /// </summary>
    private IEnumerator InitAllManagers()
    {
        // 先初始化PlayerManager，确保玩家只创建一次
        PlayerManager.GetInstance().Init();
        yield return new WaitForEndOfFrame();

        // 再初始化其他管理器
        GameUIManager.GetInstance().Init();
        yield return new WaitForEndOfFrame();

        MazeManager.GetInstance().Init();
        yield return new WaitForEndOfFrame();
    }
} 