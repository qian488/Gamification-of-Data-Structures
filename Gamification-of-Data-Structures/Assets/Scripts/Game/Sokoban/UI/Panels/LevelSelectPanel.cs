using UnityEngine;
using UnityEngine.UI;

namespace Sokoban
{
    public class LevelSelectPanel : BasePanel
    {
        private Transform buttonContainer;

        protected override void Awake()
        {
            base.Awake();
            buttonContainer = transform.Find("LevelButtonContainer");
            InitializeLevelButtons();
        }

        private void InitializeLevelButtons()
        {
            if (buttonContainer == null)
            {
                Debug.LogError("LevelButtonContainer not found!");
                return;
            }

            int levelCount = LevelConfigData.CreateDefaultConfig().levels.Length;
            var saveManager = SaveManager.GetInstance();
            if (saveManager == null)
            {
                Debug.LogError("SaveManager is null!");
                return;
            }

            for (int i = 1; i <= levelCount; i++)
            {
                GameObject buttonObj = UIFactory.CreateLevelButton($"Level {i}", i);
                buttonObj.transform.SetParent(buttonContainer, false);
                
                int levelNumber = i;

                // 设置按钮状态
                bool isUnlocked = saveManager.IsLevelUnlocked(levelNumber);
                buttonObj.GetComponent<Button>().interactable = isUnlocked;

                // 设置星星显示
                if (isUnlocked)
                {
                    int stars = saveManager.GetLevelStars(levelNumber);
                    Transform starsParent = buttonObj.transform.Find("Stars");
                    if (starsParent != null)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            Transform star = starsParent.GetChild(j);
                            if (star != null)
                            {
                                star.gameObject.SetActive(j < stars);
                            }
                        }
                    }
                }

                buttonObj.GetComponent<Button>().onClick.AddListener(() => OnLevelSelected(levelNumber));
            }
        }

        private void OnLevelSelected(int level)
        {
            GameManager.GetInstance().StartLevel(level);
            UIManager.GetInstance().ShowPanel<GamePanel>("GamePanel");
            UIManager.GetInstance().HidePanel("LevelSelectPanel");
        }

        protected override void OnClick(string name)
        {
            if (name == "BackButton")
            {
                UIManager.GetInstance().HidePanel("LevelSelectPanel");
                UIManager.GetInstance().ShowPanel<MainMenuPanel>("MainMenuPanel");
            }
        }
    }
} 