using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 游戏UI管理器类
/// 负责管理和协调所有游戏UI面板的显示和交互
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 初始化并管理迷宫游戏UI
/// 2. 初始化并管理算法可视化UI
/// 3. 处理游戏完成事件
/// 4. 协调不同UI面板之间的交互
/// </remarks>
public class GameUIManager : BaseManager<GameUIManager>
{
    /// <summary>迷宫游戏主UI面板</summary>
    private MazeGameUI mazeGameUI;
    /// <summary>算法可视化UI面板</summary>
    private AlgorithmVisualizerUI algorithmVisualizer;

    /// <summary>
    /// 初始化所有UI面板
    /// 包括迷宫游戏UI和算法可视化UI
    /// </summary>
    public void Init()
    {
        Debug.Log("GameUIManager Init start");
        
        // 使用UIManager显示迷宫游戏UI
        UIManager.GetInstance().ShowPanel<MazeGameUI>("MazeGameUI", E_UI_Layer.Top, (panel) =>
        {
            mazeGameUI = panel;
            SetPanelTransparency(panel.gameObject, 0f);
            Debug.Log("MazeGameUI loaded successfully");
        });

        // 使用UIManager显示算法可视化UI
        UIManager.GetInstance().ShowPanel<AlgorithmVisualizerUI>("AlgorithmVisualizerUI", E_UI_Layer.Mid, (panel) =>
        {
            algorithmVisualizer = panel;
            SetPanelTransparency(panel.gameObject, 0f);
            Debug.Log("AlgorithmVisualizerUI loaded successfully");
        });

        // 注册游戏完成事件
        EventCenter.GetInstance().AddEventListener("GameFinish", OnGameFinish);
    }

    /// <summary>
    /// 递归设置面板及其子物体的透明度
    /// </summary>
    private void SetPanelTransparency(GameObject panel, float alpha)
    {
        // 设置当前物体的Image组件透明度
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
}