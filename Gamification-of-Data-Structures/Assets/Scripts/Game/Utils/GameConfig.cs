using UnityEngine;

public static class GameConfig
{
    // 迷宫配置
    public static readonly int DEFAULT_MAZE_WIDTH = 21;
    public static readonly int DEFAULT_MAZE_HEIGHT = 21;
    
    // 玩家配置
    public static readonly float PLAYER_MOVE_SPEED = 5f;
    public static readonly float PLAYER_ROTATE_SPEED = 120f;
    
    // 算法配置
    public static readonly float DEFAULT_ALGORITHM_SPEED = 1f;
    public static readonly float MIN_ALGORITHM_SPEED = 0.1f;
    public static readonly float MAX_ALGORITHM_SPEED = 3f;
    
    // UI配置
    public static readonly Color PATH_COLOR = Color.green;
    public static readonly Color VISITED_COLOR = Color.yellow;
    public static readonly Color WALL_COLOR = Color.gray;
    public static readonly Color FLOOR_COLOR = Color.white;
    
    // 路径搜索相关配置
    public static class PathFinding
    {
        public static float StepDelay = 0.1f;  // 每步延迟时间
        public static Color VisitedColor = Color.yellow;  // 已访问节点颜色
        public static Color PathColor = Color.green;      // 最终路径颜色
        public static Color StartColor = new Color(0f, 1f, 0f, 0.5f);  // 起点颜色
        public static Color EndColor = new Color(1f, 0f, 0f, 0.5f);    // 终点颜色
    }
} 