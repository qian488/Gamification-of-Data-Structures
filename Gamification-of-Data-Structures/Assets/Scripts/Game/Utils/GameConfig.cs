using UnityEngine;

/// <summary>
/// 游戏配置静态类
/// 集中管理游戏中所有可配置的常量参数
/// </summary>
/// <remarks>
/// 配置项分类：
/// 1. 迷宫配置：DEFAULT_MAZE_WIDTH, DEFAULT_MAZE_HEIGHT
/// 2. 玩家配置：PLAYER_MOVE_SPEED, PLAYER_ROTATE_SPEED
/// 3. 算法配置：DEFAULT_ALGORITHM_SPEED, MIN/MAX_ALGORITHM_SPEED
/// 4. 寻路配置：PathFinding 类中的参数
/// 
/// 使用方式：
/// - 直接通过 GameConfig.参数名 访问配置项
/// - 通过 GameConfig.PathFinding.参数名 访问寻路相关配置
/// </remarks>
public static class GameConfig
{
    public static readonly int DEFAULT_MAZE_WIDTH = 21;
    public static readonly int DEFAULT_MAZE_HEIGHT = 21;
} 