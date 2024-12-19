using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 算法可视化UI面板类
/// 负责显示算法执行过程的实时状态和说明
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 状态显示：
///    - ShowDFSInfo()：显示深度优先搜索说明
///    - ShowBFSInfo()：显示广度优先搜索说明
///    - UpdateStatus()：更新当前执行状态
/// 2. 步骤管理：
///    - IncrementSteps()：增加步数计数
///    - ResetSteps()：重置步数计数
/// 3. 面板控制：
///    - TogglePanel()：切换面板显示状态（Tab键）
///    - FadePanel()：实现面板渐变效果
/// 
/// 使用方式：
/// - Tab键切换面板显示/隐藏
/// - 自动显示当前算法的执行状态
/// - 通过其他系统调用更新显示内容
/// </remarks>
public class AlgorithmVisualizerUI : BasePanel
{
    private Text stepsText;
    private int stepCount = 0;
    private Text operationGuideText;
    private Text algorithmInfoText;
    private Text statusText;
    private GameObject bgPanel;
    private bool isPanelVisible = true;

    protected override void Awake()
    {
        base.Awake();
        InitComponents();
        ShowOperationGuide();
        MonoManager.GetInstance().AddUpdateListener(CheckTabInput);
    }

    private void OnDestroy()
    {
        MonoManager.GetInstance().RemoveUpdateListener(CheckTabInput);
    }

    private void CheckTabInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePanel();
        }
    }

    private void TogglePanel()
    {
        isPanelVisible = !isPanelVisible;
        if (bgPanel != null)
        {
            StartCoroutine(FadePanel(isPanelVisible));
        }
    }

    private IEnumerator FadePanel(bool fadeIn)
    {
        float duration = 0.2f;  
        float elapsed = 0;
        
        if (fadeIn)
        {
            bgPanel.SetActive(true);
        }
        
        Text[] texts = bgPanel.GetComponentsInChildren<Text>();
        Image bgImage = bgPanel.GetComponent<Image>();
        
        Color bgColor = bgImage.color;
        Color[] textColors = new Color[texts.Length];
        for (int i = 0; i < texts.Length; i++)
        {
            textColors[i] = texts[i].color;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float alpha = fadeIn ? t : 1 - t;

            bgColor.a = alpha * 0.7f;  
            bgImage.color = bgColor;

            for (int i = 0; i < texts.Length; i++)
            {
                Color color = textColors[i];
                color.a = alpha;
                texts[i].color = color;
            }

            yield return null;
        }

        if (!fadeIn)
        {
            bgPanel.SetActive(false);
        }
        else
        {
            bgColor.a = 0.7f;
            bgImage.color = bgColor;
            for (int i = 0; i < texts.Length; i++)
            {
                Color color = textColors[i];
                color.a = 1f;
                texts[i].color = color;
            }
        }
    }

    private void InitComponents()
    {
        bgPanel = CreatePanel("BackgroundPanel", 
            new Vector2(0.05f, 0.70f),    
            new Vector2(0.95f, 0.98f));  
        bgPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0.7f);

        // 三个并排的面板，每个占据背景面板的三分之一宽度
        float panelSpacing = 0.005f;  

        // 左侧面板：状态信息
        var leftPanel = CreatePanel("LeftPanel", 
            new Vector2(0f, 0f), 
            new Vector2(0.33f - panelSpacing, 1f));
        leftPanel.transform.SetParent(bgPanel.transform, false);
        leftPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0f);

        statusText = CreateText(leftPanel, "StatusText", "", 
            new Vector2(0.02f, 0f),
            new Vector2(0.98f, 1f));
        statusText.alignment = TextAnchor.UpperLeft;
        statusText.fontSize = 22; 

        // 中间面板：操作指南
        var middlePanel = CreatePanel("MiddlePanel", 
            new Vector2(0.33f + panelSpacing, 0f), 
            new Vector2(0.66f - panelSpacing, 1f));
        middlePanel.transform.SetParent(bgPanel.transform, false);
        middlePanel.GetComponent<Image>().color = new Color(0, 0, 0, 0f);

        operationGuideText = CreateText(middlePanel, "OperationGuideText", "", 
            new Vector2(0.02f, 0f),
            new Vector2(0.98f, 1f));
        operationGuideText.alignment = TextAnchor.UpperLeft;
        operationGuideText.fontSize = 22;  

        // 右侧面板：算法信息
        var rightPanel = CreatePanel("RightPanel", 
            new Vector2(0.66f + panelSpacing, 0f), 
            new Vector2(1f, 1f));
        rightPanel.transform.SetParent(bgPanel.transform, false);
        rightPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0f);

        algorithmInfoText = CreateText(rightPanel, "AlgorithmInfoText", "", 
            new Vector2(0.02f, 0f),
            new Vector2(0.98f, 1f));
        algorithmInfoText.alignment = TextAnchor.UpperLeft;
        algorithmInfoText.fontSize = 22;  
    }

    public void ShowOperationGuide()
    {
        operationGuideText.text = "【操作指南】\n" +
            "WASD - 移动角色\n" +
            "鼠标左键 - 第一人称视角控制\n" +
            "鼠标右键 - 俯视角旋转\n" +
            "V键 - 切换第一/俯视视角\n" +
            "1键 - 重新生成迷宫\n" +
            "2键 - 深度优先搜索演示\n" +
            "3键 - 广度优先搜索演示\n" +
            "R键 - 重置位置和路径\n" +
            "Tab键 - 显示/隐藏面板\n" +
            "【目标】从绿色起点到达红色终点";
    }

    public void IncrementSteps()
    {
        stepCount++;
        if (stepsText != null)
        {
            stepsText.text = $"执行步骤：{stepCount}";
        }
    }

    public void ResetSteps()
    {
        stepCount = 0;
        if (stepsText != null)
        {
            stepsText.text = $"执行步骤：{stepCount}";
        }
    }

    public void ShowDFSInfo()
    {
        algorithmInfoText.text = "【深度优先搜索(DFS)】\n" +
            "一种迷宫探索策略，特点是：\n" +
            "1.从起点开始，优先往深处探索\n" +
            "2.遇到死路就回溯到最近的分岔口\n" +
            "3.继续探索未访��的新路径\n" +
            "4.直到找到终点或探索完整个迷宫\n\n" +
            "【演示说明】\n" +
            "黄色 - 当前访问的路径\n" +
            "灰色 - 回溯的路径\n" +
            "绿色 - 最终找到的路径";
    }

    public void ShowBFSInfo()
    {
        algorithmInfoText.text = "【广度优先搜索(BFS)】\n" +
            "一种系统的搜索策略，特点是：\n" +
            "1.从起点开始，先探索所有相邻路径\n" +
            "2.按距离递增的顺序逐层扩展\n" +
            "3.保证找到的是最短路径\n" +
            "4.搜索范围像水波般向外扩散\n\n" +
            "【演示说明】\n" +
            "黄色 - 当前层级的路径\n" +
            "灰色 - 已访问的路径\n" +
            "绿色 - 最终最短路径";
    }

    public void UpdateStatus(string status)
    {
        if (statusText != null)
        {
            statusText.text = "【执行状态】\n" + 
                status + "\n" +
                "当前执行步骤：" + stepCount + "\n" +
                "提示：按R键可以重置演示";
        }
    }

    public void ResetInfo()
    {
        if (algorithmInfoText != null)
        {
            algorithmInfoText.text = "";
        }
        if (statusText != null)
        {
            statusText.text = "";
        }
    }

    // 辅助方法
    private GameObject CreatePanel(string name, Vector2 anchorMin, Vector2 anchorMax)
    {
        var panel = new GameObject(name);
        panel.transform.SetParent(transform, false);
        
        var rectTransform = panel.AddComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.sizeDelta = Vector2.zero;
        
        panel.AddComponent<Image>();
        return panel;
    }

    private Text CreateText(GameObject parent, string name, string content, Vector2 anchorMin, Vector2 anchorMax)
    {
        var textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform, false);
        
        var rectTransform = textObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.sizeDelta = Vector2.zero;
        
        var text = textObj.AddComponent<Text>();
        text.text = content;
        text.fontSize = 20;
        text.color = Color.white;
        text.font = UIFontManager.GetInstance().GetFont();  // 使用框架的字体管理器
        
        return text;
    }
}