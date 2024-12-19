using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 迷宫管理器类
/// 负责迷宫的生成、更新和交互管理
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 控制迷宫的生成和重置
/// 2. 管理迷宫的可视化显示
/// 3. 处理迷宫与玩家的交互
/// 4. 协调寻路算法的执行
/// 5. 管理特效系统（起点终点标记、地板发光）
/// 6. 处理迷宫状态的保存和加载
/// </remarks>
public class MazeManager : BaseManager<MazeManager>
{
    private int mazeWidth = 15;
    private int mazeHeight = 15;
    private float cellSize = 3f;
    private float wallHeight = 5f;
    private float boundaryHeight = 8f;
    private MazeCell[,] maze;
    private MazeGenerator mazeGenerator;
    private GameObject mazeContainer;
    private PathFinder currentPathFinder;

    private GameObject wallPrefab;
    private GameObject floorPrefab;
    
    private ParticleSystem startPointEffect;
    private ParticleSystem endPointEffect;
    
    public int MazeWidth => mazeWidth;
    public int MazeHeight => mazeHeight;
    public float CellSize => cellSize;

    private bool isFirstGeneration = true;      

    public const string EVENT_MAZE_GENERATED = "MazeGenerated";
    public const string EVENT_PATH_FOUND = "PathFound";
    public const string EVENT_MAZE_RESET = "MazeReset";
    public const string EVENT_CELL_VISITED = "CellVisited";
    public const string EVENT_MAZE_COMPLETED = "MazeCompleted"; 

    /// <summary>
    /// 初始化迷宫管理器
    /// 加载必要的资源并设置初始状态
    /// </summary>
    public void Init()
    {
        // 先清理可能存在的游离预制体
        var existingWall = GameObject.Find("Wall(Clone)");
        var existingFloor = GameObject.Find("Floor(Clone)");
        
        if (existingWall != null) ResourcesManager.GetInstance().Recycle("Prefabs/Wall", existingWall);
        if (existingFloor != null) ResourcesManager.GetInstance().Recycle("Prefabs/Floor", existingFloor);

        // 直接使用Resources.Load加载预制体资源，而不是实例化
        wallPrefab = Resources.Load<GameObject>("Prefabs/Wall");
        floorPrefab = Resources.Load<GameObject>("Prefabs/Floor");
        mazeGenerator = new MazeGenerator();

        if (wallPrefab == null)
            Debug.LogError("Failed to load Wall prefab");
        if (floorPrefab == null)
            Debug.LogError("Failed to load Floor prefab");
    }

    public void InitializeMaze()
    {
        if (mazeContainer != null)
        {
            MonoManager.GetInstance().RemoveUpdateListener(UpdateMaze);
            
            // 先回收所有子物体到对象池
            foreach (Transform child in mazeContainer.transform)
            {
                string prefabPath = child.gameObject.name.Contains("Wall") ? "Prefabs/Wall" : "Prefabs/Floor";
                ResourcesManager.GetInstance().Recycle(prefabPath, child.gameObject);
            }
            
            // 清理粒子效果
            if (startPointEffect != null)
                GameObject.Destroy(startPointEffect.gameObject);
            if (endPointEffect != null)
                GameObject.Destroy(endPointEffect.gameObject);
            
            // 再销毁容器
            GameObject.Destroy(mazeContainer);
        }
        
        mazeContainer = new GameObject("MazeContainer");
        maze = new MazeCell[mazeWidth, mazeHeight];
        
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                maze[x, y] = new MazeCell(x, y);
            }
        }
    }

    /// <summary>
    /// 生成新的迷宫
    /// 包括清理旧迷宫和创建新迷宫的过程
    /// </summary>
    public void GenerateMaze()
    {
        MonoManager.GetInstance().StopAllCoroutines();

        if (mazeContainer != null)
        {
            List<GameObject> wallsToRecycle = new List<GameObject>();
            foreach (Transform child in mazeContainer.transform)
            {
                if (child != null && child.gameObject.name.Contains("Wall"))
                {
                    child.gameObject.SetActive(false);
                    wallsToRecycle.Add(child.gameObject);
                }
            }

            foreach (var wall in wallsToRecycle)
            {
                if (wall != null)
                {
                    ResourcesManager.GetInstance().Recycle("Prefabs/Wall", wall);
                }
            }

            if (startPointEffect != null)
            {
                startPointEffect.Stop();
                GameObject.Destroy(startPointEffect.gameObject);
                startPointEffect = null;
            }
            if (endPointEffect != null)
            {
                endPointEffect.Stop();
                GameObject.Destroy(endPointEffect.gameObject);
                endPointEffect = null;
            }

            PoolManager.GetInstance().ClearPool("Prefabs/Wall");
        }

        MonoManager.GetInstance().StartCoroutine(DelayedMazeGeneration());
    }

    private IEnumerator DelayedMazeGeneration()
    {
        yield return null;  

        if (mazeContainer == null)
        {
            mazeContainer = new GameObject("MazeContainer");
        }
        
        maze = new MazeCell[mazeWidth, mazeHeight];
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                maze[x, y] = new MazeCell(x, y);
            }
        }

        mazeGenerator.GenerateMaze(maze);
        
        if (isFirstGeneration)
        {
            yield return CreateMazeVisuals();
            isFirstGeneration = false;
        }
        else
        {
            yield return CreateWalls();
        }
        
        CreatePointEffects();
        
        float centerX = -mazeWidth * cellSize / 2f;
        float centerZ = -mazeHeight * cellSize / 2f;
        Vector3 startPos = new Vector3(centerX + cellSize, 1.5f, centerZ + cellSize);
        PlayerManager.GetInstance().SetPlayerPosition(startPos, true);
        
        MusicManager.GetInstance().PlaySFX("maze_generate", false);
    }

    /// <summary>
    /// 创建迷宫的视觉效果
    /// 包括墙壁、地板和特效的创建
    /// </summary>
    private IEnumerator CreateMazeVisuals()
    {
        float centerX = -mazeWidth * cellSize / 2f;
        float centerZ = -mazeHeight * cellSize / 2f;

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                int currentX = x;
                int currentY = y;
                Vector3 position = new Vector3(centerX + x * cellSize, 0, centerZ + y * cellSize);
                
                ResourcesManager.GetInstance().LoadAsync<GameObject>("Prefabs/Floor", (cell) =>
                {
                    if (cell != null && currentX < mazeWidth && currentY < mazeHeight)
                    {
                        cell.transform.SetParent(mazeContainer.transform);
                        cell.transform.position = position;
                        cell.transform.rotation = Quaternion.identity;
                        cell.transform.localScale = new Vector3(cellSize, 0.1f, cellSize);
                        PrefabChecker.CheckAndAddMazeCellComponents(cell, false);
                        
                        var renderer = cell.GetComponent<MeshRenderer>();
                        if (renderer != null)
                        {
                            ResourcesManager.GetInstance().LoadAsync<Material>("Materials/FloorMaterial", (material) =>
                            {
                                if (material != null)
                                {
                                    Material instanceMaterial = new Material(material);
                                    renderer.material = instanceMaterial;
                                }
                            });
                        }
                        
                        maze[currentX, currentY].CellObject = cell;
                    }
                }, true);
            }
            yield return null;
        }

        yield return CreateWalls();
    }

    // 创建墙壁的独立方法
    private IEnumerator CreateWalls()
    {
        float centerX = -mazeWidth * cellSize / 2f;
        float centerZ = -mazeHeight * cellSize / 2f;

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                if (maze[x, y].IsWall)
                {
                    int currentX = x;
                    int currentY = y;
                    Vector3 position = new Vector3(
                        centerX + x * cellSize,
                        wallHeight / 2f,
                        centerZ + y * cellSize
                    );

                    ResourcesManager.GetInstance().LoadAsync<GameObject>("Prefabs/Wall", (cell) =>
                    {
                        if (cell != null && currentX < mazeWidth && currentY < mazeHeight)
                        {
                            cell.transform.SetParent(mazeContainer.transform);
                            cell.transform.position = position;
                            cell.transform.rotation = Quaternion.identity;

                            float height = (currentX == 0 || currentX == mazeWidth - 1 || 
                                         currentY == 0 || currentY == mazeHeight - 1) 
                                ? boundaryHeight 
                                : wallHeight;

                            cell.transform.localScale = new Vector3(
                                cellSize * 1.01f,
                                height,
                                cellSize * 1.01f
                            );
                            
                            var renderer = cell.GetComponent<MeshRenderer>();
                            if (renderer != null)
                            {
                                Material wallMaterial = Resources.Load<Material>("Materials/WallMaterial");
                                if (wallMaterial != null)
                                {
                                    Material instanceMaterial = new Material(wallMaterial);
                                    Color wallColor = (currentX == 0 || currentX == mazeWidth - 1 || 
                                                    currentY == 0 || currentY == mazeHeight - 1)
                                        ? new Color(0.2f, 0.2f, 0.6f)  // 深蓝色
                                        : new Color(0.4f, 0.4f, 0.9f); // 蓝
                                    instanceMaterial.color = wallColor;
                                    renderer.material = instanceMaterial;
                                }
                            }
                            
                            PrefabChecker.CheckAndAddMazeCellComponents(cell, true);
                            maze[currentX, currentY].CellObject = cell;
                        }
                    }, true);
                }
            }
            yield return null;
        }
    }

    public void ResetAll()
    {
        MonoManager.GetInstance().StopAllCoroutines();
        
        ResetPathVisuals();
        
        float centerX = -mazeWidth * cellSize / 2f;
        float centerZ = -mazeHeight * cellSize / 2f;
        Vector3 startPos = new Vector3(centerX + cellSize, 1.5f, centerZ + cellSize);
        PlayerManager.GetInstance().SetPlayerPosition(startPos, true);
        
        var playerController = PlayerManager.GetInstance().GetPlayer().GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.EnableControl();
        }
        
        var visualizer = GameUIManager.GetInstance().GetAlgorithmVisualizer();
        if (visualizer != null)
        {
            visualizer.UpdateStatus("已重置");
        }
    }

    public void StartPathFinding(bool useDFS = true)
    {
        MonoManager.GetInstance().StopAllCoroutines();

        Vector3 playerPos = PlayerManager.GetInstance().GetPlayerPosition();
        float centerX, centerZ;
        GetMazeCenter(out centerX, out centerZ);

        int playerGridX = Mathf.RoundToInt((playerPos.x - centerX) / cellSize);
        int playerGridZ = Mathf.RoundToInt((playerPos.z - centerZ) / cellSize);

        if (playerGridX != 1 || playerGridZ != 1)
        {
            Debug.Log("Player not at start position, resetting...");
            PlayerManager.GetInstance().ResetPlayer();
            MonoManager.GetInstance().StartCoroutine(DelayedPathFinding(useDFS));
        }
        else
        {
            StartPathFindingInternal(useDFS);
        }
    }

    private void StartPathFindingInternal(bool useDFS)
    {
        ResetPathVisuals();
        
        var visualizer = GameUIManager.GetInstance().GetAlgorithmVisualizer();
        if (useDFS)
        {
            currentPathFinder = new DFSPathFinder(maze);
            visualizer.ShowDFSInfo();
            visualizer.UpdateStatus("开始深度优先搜索...");
        }
        else
        {
            currentPathFinder = new BFSPathFinder(maze);
            visualizer.ShowBFSInfo();
            visualizer.UpdateStatus("开始广度优先搜索...");
        }

        MonoManager.GetInstance().StartCoroutine(PathFindingProcess());
    }

    private IEnumerator PathFindingProcess()
    {
        var pathFinder = currentPathFinder.FindPathStepByStep();
        while (pathFinder.MoveNext())
        {
            yield return pathFinder.Current;
        }

        if (currentPathFinder.HasFoundPath())
        {
            var visualizer = GameUIManager.GetInstance().GetAlgorithmVisualizer();
            visualizer.UpdateStatus("找到路径！开始移动...");
            
            yield return MonoManager.GetInstance().StartCoroutine(MovePlayerAlongPath(currentPathFinder.GetFinalPath()));
        }
    }

    private IEnumerator MovePlayerAlongPath(List<Vector2Int> path)
    {
        float centerX, centerZ;
        GetMazeCenter(out centerX, out centerZ);
        float moveSpeed = PlayerManager.GetInstance().GetMoveSpeed();
        float rotateSpeed = 15f;
        float arrivalDistance = 0.1f;

        foreach (var gridPos in path)
        {
            Vector3 targetPos = new Vector3(
                centerX + gridPos.x * cellSize,
                1.5f,
                centerZ + gridPos.y * cellSize
            );

            while (Vector3.Distance(PlayerManager.GetInstance().GetPlayerPosition(), targetPos) > arrivalDistance)
            {
                Vector3 direction = (targetPos - PlayerManager.GetInstance().GetPlayerPosition()).normalized;
                
                // 旋转
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                PlayerManager.GetInstance().SetPlayerRotation(
                    Quaternion.Lerp(PlayerManager.GetInstance().GetPlayerRotation(), 
                                  targetRotation, 
                                  rotateSpeed * Time.deltaTime)
                );
                
                // 移动
                Vector3 newPos = Vector3.MoveTowards(
                    PlayerManager.GetInstance().GetPlayerPosition(), 
                    targetPos, 
                    moveSpeed * Time.deltaTime
                );
                PlayerManager.GetInstance().SetPlayerPosition(newPos);
                
                yield return null;
            }
        }

        EventCenter.GetInstance().EventTrigger(EVENT_MAZE_COMPLETED);
    }

    public void ResetPathVisuals()
    {
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                if (!maze[x, y].IsWall && maze[x, y].CellObject != null)
                {
                    maze[x, y].IsLit = false;
                    
                    // 获取 body 子物体
                    Transform bodyTransform = maze[x, y].CellObject.transform.Find("body");
                    if (bodyTransform != null)
                    {
                        var renderer = bodyTransform.GetComponent<MeshRenderer>();
                        if (renderer != null)
                        {
                            Material defaultMaterial = Resources.Load<Material>("Materials/FloorMaterial");
                            if (defaultMaterial != null)
                            {
                                renderer.material = defaultMaterial;
                            }
                            else
                            {
                                Debug.LogError("Failed to load default floor material");
                            }
                        }
                    }
                }
            }
        }
    }

    private void UpdateMaze()
    {

    }

    public void SetMazeSize(int width, int height)
    {
        mazeWidth = width;
        mazeHeight = height;
    }

    public void InitializePlayer()
    {
        // 不创建玩家，只设置位置
        float centerX = -mazeWidth * cellSize / 2f;
        float centerZ = -mazeHeight * cellSize / 2f;
        Vector3 startPos = new Vector3(centerX + cellSize, 1.5f, centerZ + cellSize);
        PlayerManager.GetInstance().SetPlayerPosition(startPos, true);
    }

    private void OnDestroy()
    {
        if (mazeContainer != null)
        {
            foreach (Transform child in mazeContainer.transform)
            {
                string prefabPath = child.gameObject.name.Contains("Wall") ? "Prefabs/Wall" : "Prefabs/Floor";
                ResourcesManager.GetInstance().Recycle(prefabPath, child.gameObject);
            }
            GameObject.Destroy(mazeContainer);
        }
    }

    public void LightFloor(int x, int z)
    {
        if (maze == null || x < 0 || x >= mazeWidth || z < 0 || z >= mazeHeight)
        {
            Debug.LogError($"Invalid coordinates: ({x}, {z})");
            return;
        }
        
        if (!maze[x, z].IsWall && !maze[x, z].IsLit && maze[x, z].CellObject != null)
        {
            maze[x, z].IsLit = true;
            
            // 获取 body 子物体
            Transform bodyTransform = maze[x, z].CellObject.transform.Find("body");
            if (bodyTransform != null)
            {
                var renderer = bodyTransform.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    Material playerMaterial = Resources.Load<Material>("Materials/PlayerMaterial");
                    if (playerMaterial != null)
                    {
                        Debug.Log($"Applying PlayerMaterial to floor body at ({x}, {z})");
                        renderer.material = playerMaterial;
                    }
                    else
                    {
                        Debug.LogError("Failed to load PlayerMaterial");
                    }
                }
            }
            else
            {
                Debug.LogError($"No 'body' child found on floor at ({x}, {z})");
            }
        }
    }

    public void GetMazeCenter(out float centerX, out float centerZ)
    {
        centerX = -mazeWidth * cellSize / 2f;
        centerZ = -mazeHeight * cellSize / 2f;
    }

    private void CreatePointEffects()
    {
        startPointEffect = CreateCircleEffect(new Color(0f, 1f, 0f, 0.5f));  
        Vector3 startPos = GetWorldPosition(1, 1, 0.1f);  // 稍微抬高一点避免穿透
        startPointEffect.transform.position = startPos;

        endPointEffect = CreateCircleEffect(new Color(1f, 0f, 0f, 0.5f));  
        Vector3 endPos = GetWorldPosition(mazeWidth - 2, mazeHeight - 2, 0.1f);
        endPointEffect.transform.position = endPos;
    }

    private ParticleSystem CreateCircleEffect(Color color)
    {
        GameObject effectObj = new GameObject("CircleEffect");
        effectObj.transform.SetParent(mazeContainer.transform);
        
        ParticleSystem ps = effectObj.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);  
        
        var main = ps.main;
        main.loop = true;
        main.duration = 1f;
        main.startLifetime = 1f;
        main.startSpeed = 0f;
        main.startSize = cellSize * 0.8f;  // 光圈大小略小于格子
        main.maxParticles = 100;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 10;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = cellSize * 0.4f;
        shape.arc = 360f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.8f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        colorOverLifetime.color = gradient;

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.sortMode = ParticleSystemSortMode.Distance;

        ps.Play();

        return ps;
    }

    private Vector3 GetWorldPosition(int x, int y, float height)
    {
        float centerX, centerZ;
        GetMazeCenter(out centerX, out centerZ);
        return new Vector3(centerX + x * cellSize, height, centerZ + y * cellSize);
    }

    private IEnumerator DelayedPathFinding(bool useDFS)
    {
        // 等待重置动画完成（动画时间 + 额外缓冲）
        yield return new WaitForSeconds(1.5f);
        
        StartPathFindingInternal(useDFS);
    }
}