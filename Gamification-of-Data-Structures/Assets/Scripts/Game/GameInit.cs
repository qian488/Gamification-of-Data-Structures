using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

/// <summary>
/// 游戏初始化类
/// 负责在游戏开始时按特定顺序初始化所有必要的管理器和组件
/// </summary>
/// <remarks>
/// 初始化流程：
/// 1. 设置环境光照和渲染参数
/// 2. 按顺序初始化管理器：
///    - PlayerManager：玩家管理器
///    - GameUIManager：UI管理器
///    - MazeManager：迷宫管理器
/// 3. 生成初始迷宫
/// 
/// 使用方式：
/// - 将此脚本挂载到场景中的空物体上
/// - 游戏启动时会自动执行初始化流程
/// </remarks>
public class GameInit : MonoBehaviour
{
    /// <summary>
    /// Unity启动时调用，设置环境并开始初始化流程
    /// </summary>
    void Start()
    {
        MusicManager.GetInstance().PlayBGM("ForRiver");
        MusicManager.GetInstance().ChangeBGMValue(0.5f);

        RenderSettings.ambientLight = new Color(1f, 1f, 1f);  
        RenderSettings.ambientIntensity = 1.5f; 
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

        StartCoroutine(InitGame());
    }

    /// <summary>
    /// 游戏初始化协程
    /// 按顺序执行初始化流程，确保各系统正确启动
    /// </summary>
    private IEnumerator InitGame()
    {
        yield return InitAllManagers();
        
        yield return new WaitForEndOfFrame();
        
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
        PlayerManager.GetInstance().Init();
        yield return new WaitForEndOfFrame();

        GameUIManager.GetInstance().Init();
        yield return new WaitForEndOfFrame();

        MazeManager.GetInstance().Init();
        yield return new WaitForEndOfFrame();
    }
} 