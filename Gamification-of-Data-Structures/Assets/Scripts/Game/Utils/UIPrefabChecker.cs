using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI预制体检查工具类
/// 专门负责检查和配置UI相关的预制体
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. CheckAndAddMazeGameUIComponents：配置迷宫游戏UI
///    - 创建按钮面板、操作按钮和完成面板
/// 2. CheckAndAddAlgorithmVisualizerUIComponents：配置算法可视化UI
///    - 创建数据结构显示和步骤计数器
/// 
/// 使用方式：
/// UIPrefabChecker.CheckAndAddMazeGameUIComponents(uiObj);
/// UIPrefabChecker.CheckAndAddAlgorithmVisualizerUIComponents(uiObj);
/// </remarks>
public static class UIPrefabChecker
{
    public static void CheckAndAddMazeGameUIComponents(GameObject uiObject)
    {
        if (!uiObject.TryGetComponent(out Canvas canvas))
        {
            canvas = uiObject.AddComponent<Canvas>();
        }
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1;
        canvas.pixelPerfect = true;

        if (!uiObject.TryGetComponent(out CanvasScaler scaler))
        {
            scaler = uiObject.AddComponent<CanvasScaler>();
        }
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 1;

        if (!uiObject.TryGetComponent(out GraphicRaycaster raycaster))
        {
            raycaster = uiObject.AddComponent<GraphicRaycaster>();
        }

        var rectTransform = uiObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;

        var buttonPanel = CreateOrGetPanel(uiObject, "ButtonPanel", new Vector2(0.02f, 0.02f), new Vector2(0.3f, 0.3f));
        buttonPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);

        CreateOrGetButton(buttonPanel, "GenerateBtn", "生成迷宫", new Vector2(0.1f, 0.75f), new Vector2(0.9f, 0.9f));
        CreateOrGetButton(buttonPanel, "DFSBtn", "深度优先搜索", new Vector2(0.1f, 0.5f), new Vector2(0.9f, 0.65f));
        CreateOrGetButton(buttonPanel, "BFSBtn", "广度优先搜索", new Vector2(0.1f, 0.25f), new Vector2(0.9f, 0.4f));
        CreateOrGetButton(buttonPanel, "ResetBtn", "重置", new Vector2(0.1f, 0.05f), new Vector2(0.9f, 0.2f));

        var finishPanel = uiObject.transform.Find("FinishPanel")?.gameObject;
        if (finishPanel == null)
        {
            finishPanel = new GameObject("FinishPanel");
            finishPanel.transform.SetParent(uiObject.transform, false);
            
            var finishPanelRect = finishPanel.AddComponent<RectTransform>();
            finishPanelRect.anchorMin = new Vector2(0.3f, 0.3f);
            finishPanelRect.anchorMax = new Vector2(0.7f, 0.7f);
            finishPanelRect.sizeDelta = Vector2.zero;
            
            var panelImage = finishPanel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.5f);
            
            var finishText = new GameObject("FinishText");
            finishText.transform.SetParent(finishPanel.transform, false);
            
            var finishTextRect = finishText.AddComponent<RectTransform>();
            finishTextRect.anchorMin = Vector2.zero;
            finishTextRect.anchorMax = Vector2.one;
            finishTextRect.sizeDelta = Vector2.zero;
            
            var text = finishText.AddComponent<Text>();
            text.text = "到达终点！";
            text.fontSize = 40;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
        }
        finishPanel.SetActive(false);
    }

    public static void CheckAndAddAlgorithmVisualizerUIComponents(GameObject uiObject)
    {
        if (!uiObject.TryGetComponent(out Canvas canvas))
        {
            canvas = uiObject.AddComponent<Canvas>();
        }
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 2;
        canvas.pixelPerfect = true;

        if (!uiObject.TryGetComponent(out CanvasScaler scaler))
        {
            scaler = uiObject.AddComponent<CanvasScaler>();
        }
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 1;

        if (!uiObject.TryGetComponent(out GraphicRaycaster raycaster))
        {
            raycaster = uiObject.AddComponent<GraphicRaycaster>();
        }

        var rectTransform = uiObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;

        var stackQueueText = CreateOrGetUIText(uiObject, "StackQueueText", "数据结构内容：", new Vector2(0.05f, 0.8f), new Vector2(0.95f, 0.95f));
        stackQueueText.color = Color.white;

        var stepsText = CreateOrGetUIText(uiObject, "StepsText", "执行步骤：0", new Vector2(0.05f, 0.7f), new Vector2(0.95f, 0.75f));
        stepsText.color = Color.white;
    }

    private static Text CreateOrGetUIText(GameObject parent, string name, string defaultText, Vector2 anchorMin, Vector2 anchorMax)
    {
        var textObj = parent.transform.Find(name)?.gameObject;
        if (textObj == null)
        {
            textObj = new GameObject(name);
            textObj.transform.SetParent(parent.transform, false);
            
            var rectTransform = textObj.AddComponent<RectTransform>();
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.sizeDelta = Vector2.zero;
            
            var text = textObj.AddComponent<Text>();
            text.text = defaultText;
            text.fontSize = 24;
            text.alignment = TextAnchor.MiddleLeft;
            text.color = Color.white;
            return text;
        }
        return textObj.GetComponent<Text>();
    }

    private static GameObject CreateOrGetPanel(GameObject parent, string name, Vector2 anchorMin, Vector2 anchorMax)
    {
        var panel = parent.transform.Find(name)?.gameObject;
        if (panel == null)
        {
            panel = new GameObject(name);
            panel.transform.SetParent(parent.transform, false);
            
            var rectTransform = panel.AddComponent<RectTransform>();
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.sizeDelta = Vector2.zero;
            
            var image = panel.AddComponent<Image>();
            image.color = Color.white;
        }
        return panel;
    }

    private static Button CreateOrGetButton(GameObject parent, string name, string text, Vector2 anchorMin, Vector2 anchorMax)
    {
        var buttonObj = parent.transform.Find(name)?.gameObject;
        if (buttonObj == null)
        {
            buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent.transform, false);
            
            var rectTransform = buttonObj.AddComponent<RectTransform>();
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.sizeDelta = Vector2.zero;
            
            var button = buttonObj.AddComponent<Button>();
            var image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 1);
            
            var textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);
            
            var textRectTransform = textObj.AddComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;
            
            var buttonText = textObj.AddComponent<Text>();
            buttonText.text = text;
            buttonText.fontSize = 24;
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.color = Color.white;
            
            return button;
        }
        return buttonObj.GetComponent<Button>();
    }
}