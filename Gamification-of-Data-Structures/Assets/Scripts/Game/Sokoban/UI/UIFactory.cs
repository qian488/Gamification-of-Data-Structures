using UnityEngine;
using UnityEngine.UI;

namespace Sokoban
{
    public class UIFactory
    {
        public static GameObject CreateMainMenuPanel()
        {
            GameObject panel = CreateBasePanel("MainMenuPanel");

            // 创建开始按钮
            CreateButton(panel.transform, "StartButton", "开始游戏", new Vector2(0, 100));
            // 创建关卡选择按钮
            CreateButton(panel.transform, "LevelSelectButton", "关卡选择", new Vector2(0, 0));
            // 创建退出按钮
            CreateButton(panel.transform, "QuitButton", "退出游戏", new Vector2(0, -100));

            panel.AddComponent<MainMenuPanel>();
            return panel;
        }

        public static GameObject CreateGamePanel()
        {
            GameObject panel = CreateBasePanel("GamePanel");
            panel.GetComponent<Image>().enabled = false;

            // 创建步数文本
            GameObject stepCountObj = new GameObject("StepCountText", typeof(RectTransform));
            stepCountObj.transform.SetParent(panel.transform, false);
            Text stepCountText = stepCountObj.AddComponent<Text>();
            stepCountText.text = "Steps: 0";
            stepCountText.font = UIFontManager.DefaultFont;
            stepCountText.fontSize = 24;
            stepCountText.alignment = TextAnchor.UpperLeft;
            stepCountText.color = Color.white;
            RectTransform stepRect = stepCountText.GetComponent<RectTransform>();
            stepRect.anchorMin = new Vector2(0, 1);
            stepRect.anchorMax = new Vector2(0, 1);
            stepRect.pivot = new Vector2(0, 1);
            stepRect.anchoredPosition = new Vector2(20, -20);
            stepRect.sizeDelta = new Vector2(200, 50);

            // 创建按钮容器
            GameObject buttonContainer = new GameObject("ButtonContainer", typeof(RectTransform));
            buttonContainer.transform.SetParent(panel.transform, false);
            RectTransform containerRect = buttonContainer.GetComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(1, 1);
            containerRect.anchorMax = new Vector2(1, 1);
            containerRect.pivot = new Vector2(1, 1);
            containerRect.anchoredPosition = new Vector2(-20, -20);
            containerRect.sizeDelta = new Vector2(100, 120);

            // 创建暂停按钮
            CreateButton(buttonContainer.transform, "PauseButton", "暂停", new Vector2(0, 0));
            // 创建重置按钮
            CreateButton(buttonContainer.transform, "ResetButton", "重置", new Vector2(0, -60));

            panel.AddComponent<GamePanel>();
            return panel;
        }

        public static GameObject CreatePausePanel()
        {
            GameObject panel = CreateBasePanel("PausePanel");

            // 创建继续按钮
            CreateButton(panel.transform, "ResumeButton", "继续游戏", new Vector2(0, 50));
            // 创建返回主菜单按钮
            CreateButton(panel.transform, "MainMenuButton", "主菜单", new Vector2(0, -50));

            panel.AddComponent<PausePanel>();
            return panel;
        }

        public static GameObject CreateWinPanel()
        {
            GameObject panel = CreateBasePanel("WinPanel");

            // 创建下一关按钮
            CreateButton(panel.transform, "NextLevelButton", "下一关", new Vector2(0, 100));
            // 创建重试按钮
            CreateButton(panel.transform, "RestartButton", "重试", new Vector2(0, 0));
            // 创建返回主菜单按钮
            CreateButton(panel.transform, "MainMenuButton", "主菜单", new Vector2(0, -100));

            panel.AddComponent<WinPanel>();
            return panel;
        }

        public static GameObject CreateLevelSelectPanel()
        {
            GameObject panel = CreateBasePanel("LevelSelectPanel");

            // 创建标题
            GameObject titleObj = new GameObject("Title", typeof(RectTransform));
            titleObj.transform.SetParent(panel.transform, false);
            Text titleText = titleObj.AddComponent<Text>();
            titleText.text = "选择关卡";
            titleText.font = UIFontManager.DefaultFont;
            titleText.fontSize = 36;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;
            RectTransform titleRect = titleText.GetComponent<RectTransform>();
            titleRect.anchoredPosition = new Vector2(0, 200);
            titleRect.sizeDelta = new Vector2(200, 50);

            // 创建关卡按钮容器
            GameObject containerObj = new GameObject("LevelButtonContainer", typeof(RectTransform));
            containerObj.transform.SetParent(panel.transform, false);
            RectTransform containerRect = containerObj.GetComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);
            containerRect.anchoredPosition = Vector2.zero;
            containerRect.sizeDelta = new Vector2(600, 400);

            // 添加Grid Layout组件
            GridLayoutGroup grid = containerObj.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(150, 150);
            grid.spacing = new Vector2(20, 20);
            grid.padding = new RectOffset(10, 10, 10, 10);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.MiddleCenter;

            // 创建返回按钮
            CreateButton(panel.transform, "BackButton", "返回", new Vector2(0, -250));

            panel.AddComponent<LevelSelectPanel>();

            return panel;
        }

        public static GameObject CreateLevelButton(string text, int level)
        {
            GameObject buttonObj = new GameObject($"LevelButton_{level}", typeof(RectTransform));
            
            // 添加按钮组件
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.8f, 0.8f, 0.8f);
            Button button = buttonObj.AddComponent<Button>();
            button.targetGraphic = buttonImage;

            // 设置按钮大小
            RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(100, 100);

            // 创建按钮文本
            GameObject textObj = new GameObject("Text", typeof(RectTransform));
            textObj.transform.SetParent(buttonObj.transform, false);
            Text buttonText = textObj.AddComponent<Text>();
            buttonText.text = text;
            buttonText.font = UIFontManager.DefaultFont;
            buttonText.fontSize = 20;
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.color = Color.black;

            // 设置文本大小
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            // 创建星星容器
            GameObject starsObj = new GameObject("Stars", typeof(RectTransform));
            starsObj.transform.SetParent(buttonObj.transform, false);
            RectTransform starsRect = starsObj.GetComponent<RectTransform>();
            starsRect.anchorMin = new Vector2(0, 0);
            starsRect.anchorMax = new Vector2(1, 0.3f);
            starsRect.offsetMin = Vector2.zero;
            starsRect.offsetMax = Vector2.zero;

            // 创建星星
            for (int i = 0; i < 3; i++)
            {
                GameObject star = new GameObject($"Star_{i}", typeof(RectTransform));
                star.transform.SetParent(starsObj.transform, false);
                Image starImage = star.AddComponent<Image>();
                starImage.color = Color.yellow;
                RectTransform starRect = star.GetComponent<RectTransform>();
                starRect.sizeDelta = new Vector2(20, 20);
                float x = (i - 1) * 25;
                starRect.anchoredPosition = new Vector2(x, 0);
                star.SetActive(false);
            }

            return buttonObj;
        }

        public static GameObject CreatePlayer()
        {
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.tag = "Player";
            player.layer = LayerMask.NameToLayer("Player");

            // 添加组件
            Rigidbody rb = player.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            player.AddComponent<PlayerController>();

            return player;
        }

        private static GameObject CreateBasePanel(string name)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform));
            
            // 添加面板背景
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            // 设置RectTransform
            RectTransform rectTransform = panel.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;

            return panel;
        }

        private static GameObject CreateButton(Transform parent, string name, string text, Vector2 position)
        {
            GameObject buttonObj = new GameObject(name, typeof(RectTransform));
            buttonObj.transform.SetParent(parent, false);

            // 添加按钮组件
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.8f, 0.8f, 0.8f);
            Button button = buttonObj.AddComponent<Button>();
            button.targetGraphic = buttonImage;

            // 设按钮大小和位置
            RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(200, 50);
            rectTransform.anchoredPosition = position;

            // 创建按钮文本
            GameObject textObj = new GameObject("Text", typeof(RectTransform));
            textObj.transform.SetParent(buttonObj.transform, false);
            Text buttonText = textObj.AddComponent<Text>();
            buttonText.text = text;
            buttonText.font = UIFontManager.DefaultFont;
            buttonText.fontSize = 24;
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.color = Color.black;
            buttonText.raycastTarget = false;  // 防止文本阻挡按钮点击

            // 设置文本大小
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            return buttonObj;
        }
    }
} 