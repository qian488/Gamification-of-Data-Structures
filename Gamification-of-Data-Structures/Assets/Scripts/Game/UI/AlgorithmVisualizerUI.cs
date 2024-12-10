using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AlgorithmVisualizerUI : BasePanel
{
    private Text stackQueueText;
    private Text stepsText;
    private int stepCount = 0;

    protected override void Awake()
    {
        base.Awake();
        InitComponents();
    }

    private void InitComponents()
    {
        // 创建背景面板，修改位置到中间上方
        var bgPanel = CreatePanel("BackgroundPanel", 
            new Vector2(0.3f, 0.8f),    // anchorMin: 从左边30%，从下边80%开始
            new Vector2(0.7f, 0.98f));  // anchorMax: 到左边70%，到下边98%结束
        bgPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);

        // 创建文本，调整相对位置
        stackQueueText = CreateText(bgPanel, "StackQueueText", "数据结构内容：", 
            new Vector2(0.05f, 0.4f),   // anchorMin
            new Vector2(0.95f, 0.9f));  // anchorMax
        stackQueueText.alignment = TextAnchor.UpperLeft;
        
        stepsText = CreateText(bgPanel, "StepsText", "执行步骤：0", 
            new Vector2(0.05f, 0.1f),   // anchorMin
            new Vector2(0.95f, 0.3f));  // anchorMax
        stepsText.alignment = TextAnchor.MiddleLeft;
    }

    public void UpdateStackQueueDisplay<T>(IEnumerable<T> collection)
    {
        if (stackQueueText != null)
        {
            string elements = string.Join(" -> ", collection);
            stackQueueText.text = $"数据结构内容：\n{elements}";
        }
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