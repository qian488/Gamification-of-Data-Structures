using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 算法可视化UI面板类
/// 负责显示和控制算法执行过程的可视化界面
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 显示算法执行的实时状态
/// 2. 提供操作指南说明
/// 3. 显示算法原理介绍
/// 4. 支持UI面板的显示/隐藏
/// 5. 记录和显示算法执行步骤
/// </remarks>
public class AlgorithmVisualizerUI : BasePanel
{
    /// <summary>显示执行步骤的文本组件</summary>
    private Text stepsText;
    /// <summary>当前执行步骤计数</summary>
    private int stepCount = 0;
    /// <summary>显示操作指南的文本组件</summary>
    private Text operationGuideText;
    /// <summary>显示算法信息的文本组件</summary>
    private Text algorithmInfoText;
    /// <summary>显示当前状态的文本组件</summary>
    private Text statusText;
    /// <summary>背景面板</summary>
    private GameObject bgPanel;
    /// <summary>面板显示状态</summary>
    private bool isPanelVisible = true;

    protected override void Awake()
    {
        base.Awake();
        InitComponents();
        ShowOperationGuide();
        // 注册Update事件来监听Tab键
        MonoManager.GetInstance().AddUpdateListener(CheckTabInput);
    }

    private void OnDestroy()
    {
        // 移除Update事件监听
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
            // 使用渐变效果
            StartCoroutine(FadePanel(isPanelVisible));
        }
    }

    private IEnumerator FadePanel(bool fadeIn)
    {
        float duration = 0.2f;  // 渐变持续时间
        float elapsed = 0;
        
        // 如果要显示面板，先激活它
        if (fadeIn)
        {
            bgPanel.SetActive(true);
        }
        
        // 获取所有Text组件
        Text[] texts = bgPanel.GetComponentsInChildren<Text>();
        Image bgImage = bgPanel.GetComponent<Image>();
        
        // 设置初始透明度
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

            // 更新背景透明度
            bgColor.a = alpha * 0.7f;  // 0.7f 是原始背景透明度
            bgImage.color = bgColor;

            // 更新所有文本透明度
            for (int i = 0; i < texts.Length; i++)
            {
                Color color = textColors[i];
                color.a = alpha;
                texts[i].color = color;
            }

            yield return null;
        }

        // 确保最终状态正确
        if (!fadeIn)
        {
            bgPanel.SetActive(false);
        }
        else
        {
            // 确保完全显示
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
        // 创建背景面板
        bgPanel = CreatePanel("BackgroundPanel", 
            new Vector2(0.05f, 0.70f),    
            new Vector2(0.95f, 0.98f));  
        bgPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0.7f);

        // 三个并排的面板，每个占据背景面板的三分之一宽度
        float panelSpacing = 0.005f;  // 减小面板间距

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
            "3.继续探索未访问的新路径\n" +
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

    // 使用与MazeGameUI相同的辅助方法
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