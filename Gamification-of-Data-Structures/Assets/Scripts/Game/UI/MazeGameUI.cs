using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MazeGameUI : BasePanel
{
    private GameObject finishPanel;

    protected override void Awake()
    {
        base.Awake();
        InitComponents();
    }

    private void InitComponents()
    {
        // 创建按钮面板
        var buttonPanel = CreatePanel("ButtonPanel", new Vector2(0.02f, 0.02f), new Vector2(0.3f, 0.3f));
        buttonPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);

        // 创建按钮
        CreateButton(buttonPanel, "GenerateBtn", "生成迷宫", new Vector2(0.1f, 0.75f), new Vector2(0.9f, 0.9f));
        CreateButton(buttonPanel, "DFSBtn", "深度优先搜索", new Vector2(0.1f, 0.5f), new Vector2(0.9f, 0.65f));
        CreateButton(buttonPanel, "BFSBtn", "广度优先搜索", new Vector2(0.1f, 0.25f), new Vector2(0.9f, 0.4f));
        CreateButton(buttonPanel, "ResetBtn", "重置", new Vector2(0.1f, 0.05f), new Vector2(0.9f, 0.2f));

        // 创建结束面板
        finishPanel = CreatePanel("FinishPanel", new Vector2(0.3f, 0.3f), new Vector2(0.7f, 0.7f));
        finishPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        var finishText = CreateText(finishPanel, "FinishText", "到达终点！", Vector2.zero, Vector2.one);
        finishText.fontSize = 40;
        finishText.alignment = TextAnchor.MiddleCenter;
        finishPanel.SetActive(false);
    }

    public void ShowFinishPanel()
    {
        if (finishPanel != null)
        {
            finishPanel.SetActive(true);
        }
    }

    protected override void OnClick(string btnName)
    {
        switch (btnName)
        {
            case "GenerateBtn":
                MazeManager.GetInstance().GenerateMaze();
                break;
            case "DFSBtn":
                MazeManager.GetInstance().StartPathFinding(true);
                break;
            case "BFSBtn":
                MazeManager.GetInstance().StartPathFinding(false);
                break;
            case "ResetBtn":
                MazeManager.GetInstance().InitializeMaze();
                break;
        }
    }

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

    private Button CreateButton(GameObject parent, string name, string text, Vector2 anchorMin, Vector2 anchorMax)
    {
        var buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent.transform, false);
        
        var rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.sizeDelta = Vector2.zero;
        
        var button = buttonObj.AddComponent<Button>();
        var image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f, 1);
        
        CreateText(buttonObj, "Text", text, Vector2.zero, Vector2.one);
        return button;
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
        text.fontSize = 24;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.font = UIFontManager.GetInstance().GetFont();
        
        return text;
    }
}