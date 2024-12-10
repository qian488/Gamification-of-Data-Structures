using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameUIManager : BaseManager<GameUIManager>
{
    private MazeGameUI mazeGameUI;
    private AlgorithmVisualizerUI algorithmVisualizer;

    public void Init()
    {
        Debug.Log("GameUIManager Init start");
        
        // 使用UIManager显示迷宫游戏UI
        UIManager.GetInstance().ShowPanel<MazeGameUI>("MazeGameUI", E_UI_Layer.Mid, (panel) =>
        {
            mazeGameUI = panel;
            Debug.Log("MazeGameUI loaded successfully");
        });

        // 使用UIManager显示算法可视化UI
        UIManager.GetInstance().ShowPanel<AlgorithmVisualizerUI>("AlgorithmVisualizerUI", E_UI_Layer.Top, (panel) =>
        {
            algorithmVisualizer = panel;
            Debug.Log("AlgorithmVisualizerUI loaded successfully");
        });

        // 注册游戏完成事件
        EventCenter.GetInstance().AddEventListener("GameFinish", OnGameFinish);
    }

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