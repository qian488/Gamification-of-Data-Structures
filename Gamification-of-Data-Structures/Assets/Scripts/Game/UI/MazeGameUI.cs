using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// 迷宫游戏UI面板类
/// 负责显示和处理迷宫游戏的主要UI界面
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 创建和管理游戏控制按钮
/// 2. 处理按钮点击事件
/// 3. 显示游戏完成面板
/// 4. 提供UI创建的辅助方法
/// </remarks>
public class MazeGameUI : BasePanel
{
    /// <summary>游戏完成时显示的面板</summary>
    private GameObject finishPanel;

    private bool canInteract = false;

    protected override void Awake()
    {
        base.Awake();
        
        // 确保场景中有 EventSystem
        if (FindObjectOfType<EventSystem>() == null)
        {
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            Debug.Log("Created new EventSystem");
        }
        
        InitComponents();

        // 注册UI交互事件
        EventCenter.GetInstance().AddEventListener("EnableUIInteraction", OnEnableUIInteraction);
        EventCenter.GetInstance().AddEventListener("DisableUIInteraction", OnDisableUIInteraction);
    }

    private void OnDestroy()
    {
        // 移除事件监听
        EventCenter.GetInstance().RemoveEventListener("EnableUIInteraction", OnEnableUIInteraction);
        EventCenter.GetInstance().RemoveEventListener("DisableUIInteraction", OnDisableUIInteraction);
    }

    private void OnEnableUIInteraction()
    {
        Debug.Log("UI interaction enabled");
        canInteract = true;
    }

    private void OnDisableUIInteraction()
    {
        Debug.Log("UI interaction disabled");
        canInteract = false;
    }

    /// <summary>
    /// 初始化UI组件
    /// 创建按钮面板和结束面板
    /// </summary>
    private void InitComponents()
    {
        // 创建按钮面板
        var buttonPanel = CreatePanel("ButtonPanel", new Vector2(0.02f, 0.02f), new Vector2(0.3f, 0.3f));
        buttonPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        buttonPanel.GetComponent<Image>().raycastTarget = true;  // 确保面板可以接收射线检测

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

    /// <summary>
    /// 显示游戏完成面板
    /// 当玩家到达终点时调用
    /// </summary>
    public void ShowFinishPanel()
    {
        if (finishPanel != null)
        {
            finishPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 处理按钮点击事件
    /// </summary>
    /// <param name="btnName">被点击的按钮名称</param>
    /// <remarks>
    /// 支持的按钮：
    /// - GenerateBtn：生成新迷宫
    /// - DFSBtn：开始深度优先搜索
    /// - BFSBtn：开始广度优先搜索
    /// - ResetBtn：重置当前迷宫
    /// </remarks>
    protected override void OnClick(string btnName)
    {
        Debug.Log($"Button clicked: {btnName}");
        
        // 只有在可交互状态下才响应点击
        if (!canInteract)
        {
            Debug.Log("UI interaction is disabled");
            return;
        }

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
        image.raycastTarget = true;  // 确保图片组件可以接收射线检测
        
        // 创建文本
        var textObj = CreateText(buttonObj, "Text", text, Vector2.zero, Vector2.one);
        textObj.GetComponent<Text>().raycastTarget = false;  // 文本不需要射线检测
        
        // 添加点击事件监听
        button.onClick.AddListener(() => {
            Debug.Log($"Button {name} clicked directly");  // 直接在点击事件中添加日志
            OnClick(name);
        });

        // 添加事件触发器组件来测试按钮是否可以接收输入
        var trigger = buttonObj.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { Debug.Log($"Mouse entered button {name}"); });
        trigger.triggers.Add(entry);
        
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