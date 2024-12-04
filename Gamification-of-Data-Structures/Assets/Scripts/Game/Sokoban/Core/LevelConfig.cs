using UnityEngine;

namespace Sokoban
{
    public static class LevelConfigData
    {
        public static LevelConfig CreateDefaultConfig()
        {
            LevelConfig config = new LevelConfig();
            config.levels = new LevelData[3]; // 先创建3个测试关卡

            // 创建第一关
            config.levels[0] = new LevelData
            {
                width = 9,
                height = 9,
                mapData = new int[][]
                {
                    new int[] {1,1,1,1,1,1,1,1,1},
                    new int[] {1,0,0,0,0,0,0,0,1},
                    new int[] {1,0,0,0,2,0,0,0,1},
                    new int[] {1,0,4,0,0,0,2,0,1},
                    new int[] {1,0,0,0,3,0,0,0,1},
                    new int[] {1,0,0,2,0,3,0,0,1},
                    new int[] {1,0,0,0,3,0,0,0,1},
                    new int[] {1,0,0,0,0,0,0,0,1},
                    new int[] {1,1,1,1,1,1,1,1,1}
                },
                levelName = "Level 1",
                starScore = 10
            };

            // 创建第二关
            config.levels[1] = new LevelData
            {
                width = 5,
                height = 5,
                mapData = new int[][]
                {
                    new int[] {1,1,1,1,1},
                    new int[] {1,4,2,0,1},
                    new int[] {1,0,0,0,1},
                    new int[] {1,0,3,0,1},
                    new int[] {1,1,1,1,1}
                },
                levelName = "Level 2",
                starScore = 12
            };

            // 创建第三关
            config.levels[2] = new LevelData
            {
                width = 5,
                height = 5,
                mapData = new int[][]
                {
                    new int[] {1,1,1,1,1},
                    new int[] {1,4,2,0,1},
                    new int[] {1,2,3,0,1},
                    new int[] {1,0,3,0,1},
                    new int[] {1,1,1,1,1}
                },
                levelName = "Level 3",
                starScore = 15
            };

            return config;
        }
    }
} 