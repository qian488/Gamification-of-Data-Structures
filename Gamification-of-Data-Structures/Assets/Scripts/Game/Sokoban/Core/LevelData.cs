using UnityEngine;
using System;

namespace Sokoban
{
    [Serializable]
    public class LevelData
    {
        public int width;
        public int height;
        private int[,] _map;
        public int[][] mapData; // 用于序列化
        public string levelName;
        public int starScore;

        public int[,] map
        {
            get
            {
                if (_map == null)
                {
                    _map = new int[width, height];
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            _map[i, j] = mapData[i][j];
                        }
                    }
                }
                return _map;
            }
        }
    }

    [Serializable]
    public class LevelConfig
    {
        public LevelData[] levels;
    }
} 