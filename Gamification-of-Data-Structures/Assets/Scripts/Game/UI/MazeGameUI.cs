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

    private GameObject exitConfirmPanel;  // 添加退出确认面板引用

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

        // 注册到达终点事件
        EventCenter.GetInstance().AddEventListener(MazeManager.EVENT_MAZE_COMPLETED, OnMazeCompleted);
    }

    private void OnDestroy()
    {
        // 移除事件监听
        EventCenter.GetInstance().RemoveEventListener("EnableUIInteraction", OnEnableUIInteraction);
        EventCenter.GetInstance().RemoveEventListener("DisableUIInteraction", OnDisableUIInteraction);
        EventCenter.GetInstance().RemoveEventListener(MazeManager.EVENT_MAZE_COMPLETED, OnMazeCompleted);
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

    private void OnMazeCompleted()
    {
        Debug.Log("Maze completed event received");
        ShowFinishPanel();
    }

    /// <summary>
    /// 初始化UI组件
    /// 创建按钮面板和结束面板
    /// </summary>
    private void InitComponents()
    {
        InitButtonPanel();

        InitFinishPanel();

        InitExitConfirmPanel();
    }

    private void InitButtonPanel()
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
    }

    private void InitFinishPanel()
    {
        // 创建结束面板
        finishPanel = CreatePanel("FinishPanel", new Vector2(0.3f, 0.3f), new Vector2(0.7f, 0.7f));
        finishPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0.8f);
        
        // 添加点击事件
        var panelButton = finishPanel.AddComponent<Button>();
        panelButton.onClick.AddListener(HideFinishPanel);
        
        var finishText = CreateText(finishPanel, "FinishText", "到达终点！", Vector2.zero, Vector2.one);
        finishText.fontSize = 40;
        finishText.alignment = TextAnchor.MiddleCenter;
        finishText.color = Color.white;
        
        // 添加提示文本
        var hintText = CreateText(finishPanel, "HintText", "点击任意处关闭", new Vector2(0, 0.2f), new Vector2(1, 0.3f));
        hintText.fontSize = 24;
        hintText.alignment = TextAnchor.MiddleCenter;
        hintText.color = new Color(0.8f, 0.8f, 0.8f);
        
        finishPanel.SetActive(false);
    }

    private void InitExitConfirmPanel()
    {
        // 创建退出确认面板
        exitConfirmPanel = CreatePanel("ExitConfirmPanel", new Vector2(0.3f, 0.3f), new Vector2(0.7f, 0.7f));
        exitConfirmPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0.8f);

        // 添加标题文本
        var titleText = CreateText(exitConfirmPanel, "TitleText", "确认退出？", new Vector2(0, 0.6f), new Vector2(1, 0.8f));
        titleText.fontSize = 40;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;

        // 创建按钮容器
        var buttonContainer = CreatePanel("ButtonContainer", new Vector2(0.1f, 0.2f), new Vector2(0.9f, 0.4f));
        buttonContainer.transform.SetParent(exitConfirmPanel.transform, false);
        buttonContainer.GetComponent<Image>().color = new Color(0, 0, 0, 0);

        // 创建确认和取消按钮，并将它们添加到面板的按钮列表中
        var confirmBtn = CreateButton(buttonContainer, "ExitConfirmBtn", "确认", new Vector2(0.1f, 0), new Vector2(0.45f, 1));
        var cancelBtn = CreateButton(buttonContainer, "ExitCancelBtn", "取消", new Vector2(0.55f, 0), new Vector2(0.9f, 1));

        exitConfirmPanel.SetActive(false);
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
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void HideFinishPanel()
    {
        if (finishPanel != null)
        {
            finishPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ShowExitConfirmPanel()
    {
        if (exitConfirmPanel != null)
        {
            exitConfirmPanel.SetActive(true);
            EventCenter.GetInstance().EventTrigger("EnableUIInteraction");
        }
    }

    public void HideExitConfirmPanel()
    {
        if (exitConfirmPanel != null)
        {
            exitConfirmPanel.SetActive(false);
            // 恢复游戏状态
            Cursor.lockState = CursorLockMode.Locked;  // 添加这行，重新锁定鼠标
            Cursor.visible = false;
            
            // 恢复玩家控制
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var controller = player.GetComponent<PlayerController>();
                if (controller != null)
                {
                    controller.EnableControl();  // 需要在 PlayerController 中添加这个方法
                }
            }
            
            EventCenter.GetInstance().EventTrigger("DisableUIInteraction");
        }
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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
    /// - ExitConfirmBtn：确认退出游戏
    /// - ExitCancelBtn：取消退出游戏
    /// </remarks>
    protected override void OnClick(string btnName)
    {
        Debug.Log($"Button clicked: {btnName}");
        
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
                MazeManager.GetInstance().ResetAll();
                break;
            case "ExitConfirmBtn":
                QuitGame();
                break;
            case "ExitCancelBtn":
                HideExitConfirmPanel();
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