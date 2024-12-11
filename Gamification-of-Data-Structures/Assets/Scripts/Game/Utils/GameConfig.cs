using UnityEngine;

/// <summary>
/// 游戏配置静态类
/// 集中管理游戏中所有可配置的常量参数
/// </summary>
/// <remarks>
/// 包含以下配置类别：
/// 1. 迷宫相关配置（尺寸等）
/// 2. 玩家相关配置（移动速度、旋转速度等）
/// 3. 算法相关配置（执行速度等）
/// 4. UI相关配置（颜色等）
/// 5. 寻路相关配置
/// </remarks>
public static class GameConfig
{
    // 迷宫配置
    /// <summary>迷宫默认宽度，必须是奇数以确保迷宫生成正确</summary>
    public static readonly int DEFAULT_MAZE_WIDTH = 21;
    
    /// <summary>迷宫默认高度，必须是奇数以确保迷宫生成正确</summary>
    public static readonly int DEFAULT_MAZE_HEIGHT = 21;
    
    // 玩家配置
    /// <summary>玩家移动速度（单位：米/秒）</summary>
    public static readonly float PLAYER_MOVE_SPEED = 5f;
    
    /// <summary>玩家旋转速度（单位：度/秒）</summary>
    public static readonly float PLAYER_ROTATE_SPEED = 120f;
    
    // 算法配置
    /// <summary>算法执行的默认速度倍率</summary>
    public static readonly float DEFAULT_ALGORITHM_SPEED = 1f;
    
    /// <summary>算法执行的最小速度倍率</summary>
    public static readonly float MIN_ALGORITHM_SPEED = 0.1f;
    
    /// <summary>算法执行的最大速度倍率</summary>
    public static readonly float MAX_ALGORITHM_SPEED = 3f;

    /// <summary>
    /// 路径搜索算法相关配置
    /// 包含搜索过程中的视觉效果和时间控制参数
    /// </summary>
    public static class PathFinding
    {
        /// <summary>算法每步执行的延迟时间（秒）</summary>
        public static float StepDelay = 0.1f;
        
        /// <summary>已访问节点的显示颜色</summary>
        public static Color VisitedColor = Color.yellow;
        
        /// <summary>最终路径的显示颜色</summary>
        public static Color PathColor = Color.green;
        
        /// <summary>起点的显示颜色（半透明绿色）</summary>
        public static Color StartColor = new Color(0f, 1f, 0f, 0.5f);
        
        /// <summary>终点的显示颜色（半透明红色）</summary>
        public static Color EndColor = new Color(1f, 0f, 0f, 0.5f);
    }
} 