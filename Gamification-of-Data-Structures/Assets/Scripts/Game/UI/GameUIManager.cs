using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 游戏UI管理器类
/// 负责管理和协调所有游戏UI面板
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. UI面板管理：
///    - Init()：初始化所有UI面板
///    - SetUITransparency()：控制面板透明度
/// 2. 事件处理：
///    - OnGameFinish：处理游戏完成事件
///    - ShowExitConfirmPanel：显示退出确认
/// 3. 可视化控制：
///    - IncrementSteps：增加步数
///    - ResetVisualizer：重置可视化器
/// 
/// 使用方式：
/// - 通过GetInstance()获取单例
/// - 使用Init()初始化UI系统
/// - 通过提供的方法控制UI状态
/// </remarks>
public class GameUIManager : BaseManager<GameUIManager>
{
    private MazeGameUI mazeGameUI;
    private AlgorithmVisualizerUI algorithmVisualizer;

    /// <summary>
    /// 初始化所有UI面板
    /// 包括迷宫游戏UI和算法可视化UI
    /// </summary>
    public void Init()
    {
        Debug.Log("GameUIManager Init start");
        
        UIManager.GetInstance().ShowPanel<MazeGameUI>("MazeGameUI", E_UI_Layer.Top, (panel) =>
        {
            mazeGameUI = panel;
            SetPanelTransparency(panel.gameObject, 0f);
            Debug.Log("MazeGameUI loaded successfully");
        });

        UIManager.GetInstance().ShowPanel<AlgorithmVisualizerUI>("AlgorithmVisualizerUI", E_UI_Layer.Mid, (panel) =>
        {
            algorithmVisualizer = panel;
            SetPanelTransparency(panel.gameObject, 0f);
            Debug.Log("AlgorithmVisualizerUI loaded successfully");
        });

        EventCenter.GetInstance().AddEventListener("GameFinish", OnGameFinish);

    }

    /// <summary>
    /// 递归设置面板及其子物体的透明度
    /// </summary>
    private void SetPanelTransparency(GameObject panel, float alpha)
    {
        Image image = panel.GetComponent<Image>();
        if (image != null)
        {
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }
/*
        // 递归设置所有子物体的透明度
        foreach (Transform child in panel.transform)
        {
            SetPanelTransparency(child.gameObject, alpha);
        }
*/
    }

    /// <summary>
    /// 设置指定面板的透明度
    /// </summary>
    public void SetUITransparency(string panelName, float alpha)
    {
        switch (panelName)
        {
            case "MazeGameUI":
                if (mazeGameUI != null)
                    SetPanelTransparency(mazeGameUI.gameObject, alpha);
                break;
            case "AlgorithmVisualizerUI":
                if (algorithmVisualizer != null)
                    SetPanelTransparency(algorithmVisualizer.gameObject, alpha);
                break;
        }
    }

    /// <summary>
    /// 处理游戏完成事件
    /// 显示完成面板并播放完成音效
    /// </summary>
    private void OnGameFinish()
    {
        if(mazeGameUI != null)
        {
            mazeGameUI.ShowFinishPanel();
            MusicManager.GetInstance().PlaySFX("finish", false);
        }
    }

    public void IncrementSteps()
    {
        if (algorithmVisualizer != null)
        {
            algorithmVisualizer.IncrementSteps();
        }
    }

    public void ResetVisualizer()
    {
        if (algorithmVisualizer != null)
        {
            algorithmVisualizer.ResetSteps();
        }
    }

    public AlgorithmVisualizerUI GetAlgorithmVisualizer()
    {
        return algorithmVisualizer;
    }

    public MazeGameUI GetMazeGameUI()
    {
        return mazeGameUI;
    }

    public void ShowExitConfirmPanel()
    {
        if (mazeGameUI != null)
        {
            mazeGameUI.ShowExitConfirmPanel();
        }
    }
}