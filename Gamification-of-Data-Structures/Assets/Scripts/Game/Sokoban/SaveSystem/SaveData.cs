using System;

namespace Sokoban
{
    [Serializable]
    public class SaveData
    {
        public int maxUnlockedLevel = 1;
        public int[] levelStars; // 每关获得的星星数
        public int[] bestSteps; // 每关最佳步数

        public SaveData(int levelCount)
        {
            levelStars = new int[levelCount];
            bestSteps = new int[levelCount];
            for(int i = 0; i < levelCount; i++)
            {
                levelStars[i] = 0;
                bestSteps[i] = int.MaxValue;
            }
        }
    }
} 