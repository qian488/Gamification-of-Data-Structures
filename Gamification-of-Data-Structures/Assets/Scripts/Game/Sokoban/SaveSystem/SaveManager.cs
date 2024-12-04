using UnityEngine;
using System.IO;

namespace Sokoban
{
    public class SaveManager : BaseManager<SaveManager>
    {
        private SaveData saveData;
        private const string SAVE_FILE = "save.json";

        public SaveManager()
        {
            LoadSaveData();
        }

        private void LoadSaveData()
        {
            string path = Path.Combine(Application.persistentDataPath, SAVE_FILE);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                saveData = JsonUtility.FromJson<SaveData>(json);
            }
            else
            {
                int levelCount = LevelConfigData.CreateDefaultConfig().levels.Length;
                saveData = new SaveData(levelCount);
                saveData.maxUnlockedLevel = 1;
                SaveGame();
            }
        }

        public void SaveGame()
        {
            string json = JsonUtility.ToJson(saveData);
            string path = Path.Combine(Application.persistentDataPath, SAVE_FILE);
            File.WriteAllText(path, json);
        }

        public void UpdateLevelProgress(int level, int steps)
        {
            if (level > saveData.maxUnlockedLevel)
                saveData.maxUnlockedLevel = level;

            if (steps < saveData.bestSteps[level - 1])
            {
                saveData.bestSteps[level - 1] = steps;
                // 计算星星数
                int stars = CalculateStars(level, steps);
                saveData.levelStars[level - 1] = stars;
            }

            SaveGame();
        }

        private int CalculateStars(int level, int steps)
        {
            // 根据步数计算星星
            LevelData levelData = GameManager.GetInstance().GetLevelData(level);
            if (steps <= levelData.starScore)
                return 3;
            else if (steps <= levelData.starScore * 1.5f)
                return 2;
            else
                return 1;
        }

        public bool IsLevelUnlocked(int level)
        {
            return level <= saveData.maxUnlockedLevel;
        }

        public int GetLevelStars(int level)
        {
            return saveData.levelStars[level - 1];
        }
    }
} 