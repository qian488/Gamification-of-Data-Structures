using UnityEngine;
using System.Collections;

public class GameInit : MonoBehaviour
{
    void Start()
    {
        // 设置更暗的环境光
        RenderSettings.ambientLight = new Color(0.02f, 0.02f, 0.05f);
        RenderSettings.ambientIntensity = 0.1f;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

        // 使用协程确保初始化顺序
        StartCoroutine(InitGame());
    }

    private IEnumerator InitGame()
    {
        // 初始化所有管理器
        yield return InitAllManagers();
        
        // 等待一帧确保所有初始化完成
        yield return new WaitForEndOfFrame();
        
        // 开始游戏
        StartGame();
    }

    private IEnumerator InitAllManagers()
    {
        // 先清理场景
        var existingObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (var obj in existingObjects)
        {
            if (obj.name.Contains("(Clone)"))
            {
                GameObject.Destroy(obj);
            }
        }

        // 初始化管理器
        GameUIManager.GetInstance().Init();
        yield return new WaitForEndOfFrame();
        
        MazeManager.GetInstance().Init();
        yield return new WaitForEndOfFrame();
        
        PlayerManager.GetInstance().Init();
    }

    private void StartGame()
    {
        // 生成迷宫
        MazeManager.GetInstance().GenerateMaze();
    }
} 