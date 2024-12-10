using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameUIManager : BaseManager<GameUIManager>
{
    private MazeGameUI mazeGameUI;
    private AlgorithmVisualizerUI algorithmVisualizerUI;

    public void Init()
    {
        Debug.Log("GameUIManager Init start");
        // 使用UIManager显示面板
        UIManager.GetInstance().ShowPanel<MazeGameUI>("MazeGameUI", E_UI_Layer.Mid, (panel) =>
        {
            mazeGameUI = panel;
            Debug.Log("MazeGameUI loaded successfully");
        });

        UIManager.GetInstance().ShowPanel<AlgorithmVisualizerUI>("AlgorithmVisualizerUI", E_UI_Layer.Top, (panel) =>
        {
            algorithmVisualizerUI = panel;
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

    public void UpdateVisualizerData<T>(IEnumerable<T> collection)
    {
        if (algorithmVisualizerUI != null)
        {
            algorithmVisualizerUI.UpdateStackQueueDisplay(collection);
            algorithmVisualizerUI.IncrementSteps();
        }
    }

    public void ResetVisualizer()
    {
        if (algorithmVisualizerUI != null)
        {
            algorithmVisualizerUI.ResetSteps();
        }
    }
}