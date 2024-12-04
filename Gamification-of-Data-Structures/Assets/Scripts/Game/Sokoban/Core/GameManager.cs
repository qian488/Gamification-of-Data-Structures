using UnityEngine;

namespace Sokoban
{
    public class GameManager : SingletonMono<GameManager>
    {
        private LevelManager levelManager;
        private PlayerController playerController;
        private LevelConfig levelConfig;
        private int currentSteps = 0;

        public int CurrentLevel => levelManager.CurrentLevel;
        public int CurrentSteps => currentSteps;

        protected override void Awake()
        {
            base.Awake();
            var uiManager = UIManager.GetInstance();
            if (uiManager.canvas == null)
            {
                Debug.LogError("UI system failed to initialize properly");
                return;
            }
            levelManager = GetComponent<LevelManager>();
            playerController = FindObjectOfType<PlayerController>();
            LoadLevelConfig();
            EventCenter.GetInstance().AddEventListener("PlayerMove", OnPlayerMove);
            EventCenter.GetInstance().AddEventListener("LevelReset", OnLevelReset);
            UIManager.GetInstance().ShowPanel<MainMenuPanel>("MainMenuPanel");
        }

        private void OnPlayerMove()
        {
            currentSteps++;
        }

        private void OnLevelReset()
        {
            currentSteps = 0;
        }

        private void LoadLevelConfig()
        {
            levelConfig = LevelConfigData.CreateDefaultConfig();
        }

        public LevelData GetLevelData(int level)
        {
            if (level > 0 && level <= levelConfig.levels.Length)
                return levelConfig.levels[level - 1];
            return null;
        }

        public void StartLevel(int level)
        {
            levelManager.LoadLevel(level);
            currentSteps = 0;
            InputManager.GetInstance().StartOREndCheck(true);
        }

        public void CheckWinCondition()
        {
            if(levelManager.IsLevelComplete())
            {
                // 触发胜利事件
                EventCenter.GetInstance().EventTrigger("LevelComplete");
                // 更新存档
                SaveManager.GetInstance().UpdateLevelProgress(
                    levelManager.CurrentLevel,
                    currentSteps
                );
            }
        }

        public void ResetCurrentLevel()
        {
            levelManager.ResetLevel();
        }

        private void OnDestroy()
        {
            EventCenter.GetInstance().RemoveEventListener("PlayerMove", OnPlayerMove);
            EventCenter.GetInstance().RemoveEventListener("LevelReset", OnLevelReset);
        }
    }
} 