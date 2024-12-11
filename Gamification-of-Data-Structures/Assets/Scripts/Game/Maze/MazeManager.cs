using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    private bool isFirstGeneration = true;  // 添加标记

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

    public void GenerateMaze()
    {
        // 先停止所有正在进行的协程
        MonoManager.GetInstance().StopAllCoroutines();

        // 先清理旧的迷宫
        if (mazeContainer != null)
        {
            // 先禁用所有墙体
            List<GameObject> wallsToRecycle = new List<GameObject>();
            foreach (Transform child in mazeContainer.transform)
            {
                if (child != null && child.gameObject.name.Contains("Wall"))
                {
                    child.gameObject.SetActive(false);
                    wallsToRecycle.Add(child.gameObject);
                }
            }

            // 回收墙体
            foreach (var wall in wallsToRecycle)
            {
                if (wall != null)
                {
                    ResourcesManager.GetInstance().Recycle("Prefabs/Wall", wall);
                }
            }

            // 清理粒子效果
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

            // 清理墙体对象池
            PoolManager.GetInstance().ClearPool("Prefabs/Wall");
        }

        // 等待一帧确保所有清理完成
        MonoManager.GetInstance().StartCoroutine(DelayedMazeGeneration());
    }

    private IEnumerator DelayedMazeGeneration()
    {
        yield return null;  // 等待一帧

        // 初始化新迷宫
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

        // 生成迷宫数据
        mazeGenerator.GenerateMaze(maze);
        
        // 第一次生成时创建地板和墙体，后续只创建墙体
        if (isFirstGeneration)
        {
            yield return CreateMazeVisuals();
            isFirstGeneration = false;
        }
        else
        {
            yield return CreateWallsOnly();
        }
        
        // 创建起点和终点的光圈效果
        CreatePointEffects();
        
        // 初始化玩家和播放音效
        InitializePlayer();
        MusicManager.GetInstance().PlaySFX("maze_generate", false);
    }

    private IEnumerator CreateMazeVisuals()
    {
        float centerX = -mazeWidth * cellSize / 2f;
        float centerZ = -mazeHeight * cellSize / 2f;

        if (mazeContainer == null)
        {
            Debug.LogError("MazeContainer is null when creating visuals");
            yield break;
        }

        // 先创建所有地板
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                if (!maze[x, y].IsWall)
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
                            
                            // 设置地板材质
                            var renderer = cell.GetComponent<MeshRenderer>();
                            if (renderer != null)
                            {
                                Material newMaterial = new Material(Shader.Find("Standard"));
                                newMaterial.color = new Color(0.15f, 0.15f, 0.15f);  // 深色地板
                                newMaterial.SetFloat("_Glossiness", 0.1f);           // 低光泽
                                newMaterial.SetFloat("_Metallic", 0.0f);             // 非金属
                                renderer.material = newMaterial;
                                renderer.receiveShadows = true;                       // 接收阴影
                            }
                            
                            maze[currentX, currentY].CellObject = cell;
                        }
                    }, true);
                }
            }
            yield return null;
        }

        // 再创建墙壁
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
                            
                            // 设置墙体颜色
                            var renderer = cell.GetComponent<MeshRenderer>();
                            if (renderer != null)
                            {
                                Material newMaterial = new Material(Shader.Find("Standard"));
                                Color wallColor = (currentX == 0 || currentX == mazeWidth - 1 || 
                                                currentY == 0 || currentY == mazeHeight - 1)
                                    ? new Color(0.1f, 0.1f, 0.4f)  // 深蓝色
                                    : new Color(0.3f, 0.3f, 0.8f); // 浅蓝色
                                newMaterial.color = wallColor;
                                newMaterial.SetFloat("_Glossiness", 0.1f);
                                newMaterial.SetFloat("_Metallic", 0.0f);
                                renderer.material = newMaterial;
                                renderer.receiveShadows = true;
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

    // 新增方法：只创建墙体
    private IEnumerator CreateWallsOnly()
    {
        float centerX = -mazeWidth * cellSize / 2f;
        float centerZ = -mazeHeight * cellSize / 2f;

        // 创建墙壁
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
                            
                            // 设置墙体颜色
                            var renderer = cell.GetComponent<MeshRenderer>();
                            if (renderer != null)
                            {
                                Material newMaterial = new Material(Shader.Find("Standard"));
                                Color wallColor = (currentX == 0 || currentX == mazeWidth - 1 || 
                                                currentY == 0 || currentY == mazeHeight - 1)
                                    ? new Color(0.1f, 0.1f, 0.4f)  // 深蓝色
                                    : new Color(0.3f, 0.3f, 0.8f); // 浅蓝色
                                newMaterial.color = wallColor;
                                newMaterial.SetFloat("_Glossiness", 0.1f);
                                newMaterial.SetFloat("_Metallic", 0.0f);
                                renderer.material = newMaterial;
                                renderer.receiveShadows = true;
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
        // 停止所有正在运行的协程
        MonoManager.GetInstance().StopAllCoroutines();
        
        // 重置路径显示
        ResetPathVisuals();
        
        // 重置玩家位置
        InitializePlayer();
        
        // 重置UI显示
        var visualizer = GameUIManager.GetInstance().GetAlgorithmVisualizer();
        if (visualizer != null)
        {
            visualizer.UpdateStatus("已重置");
        }
    }

    public void StartPathFinding(bool useDFS = true)
    {
        // 先停止所有正在运行的协程
        MonoManager.GetInstance().StopAllCoroutines();

        // 获取玩家当前位置
        Vector3 playerPos = PlayerManager.GetInstance().GetPlayerPosition();
        float centerX, centerZ;
        GetMazeCenter(out centerX, out centerZ);

        // 计算玩家在迷宫中的网格坐标
        int playerGridX = Mathf.RoundToInt((playerPos.x - centerX) / cellSize);
        int playerGridZ = Mathf.RoundToInt((playerPos.z - centerZ) / cellSize);

        // 检查玩家是否在起点(1,1)
        if (playerGridX != 1 || playerGridZ != 1)
        {
            // 如果不在起点，先重置玩家位置
            Debug.Log("Player not at start position, resetting...");
            PlayerManager.GetInstance().ResetPlayer();
            
            // 等待重置动画完成后再开始寻路
            MonoManager.GetInstance().StartCoroutine(DelayedPathFinding(useDFS));
        }
        else
        {
            // 玩家在起点，直接开始寻路
            StartPathFindingInternal(useDFS);
        }
    }

    private IEnumerator DelayedPathFinding(bool useDFS)
    {
        // 等待重置动画完成（动画时间 + 额外缓冲）
        yield return new WaitForSeconds(1.5f);
        
        // 开始寻路
        StartPathFindingInternal(useDFS);
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

        // 直接开始寻路和移动过程
        MonoManager.GetInstance().StartCoroutine(PathFindingAndMove());
    }

    private IEnumerator PathFindingAndMove()
    {
        var visualizer = GameUIManager.GetInstance().GetAlgorithmVisualizer();
        visualizer.UpdateStatus("正在搜索路径...");
        
        // 获取当前位置的网格坐标
        float centerX, centerZ;
        GetMazeCenter(out centerX, out centerZ);
        Vector3 currentPos = PlayerManager.GetInstance().GetPlayerPosition();
        Vector2Int currentGrid = new Vector2Int(
            Mathf.RoundToInt((currentPos.x - centerX) / cellSize),
            Mathf.RoundToInt((currentPos.z - centerZ) / cellSize)
        );

        // 设置移动参数
        float moveSpeed = 10f;
        float rotateSpeed = 15f;
        float arrivalDistance = 0.1f;

        // 开始寻路过程
        var pathFinder = currentPathFinder.FindPathStepByStep();
        while (pathFinder.MoveNext())
        {
            // 等待寻路器的每一步
            yield return pathFinder.Current;

            // 获取当前探索到的位置
            Vector2Int explorePos = currentPathFinder.GetCurrentExploringPosition();
            if (explorePos != currentGrid) // 如果探索位置不是当前位置
            {
                // 注释掉穿墙检查
                // if (CanMoveBetween(currentGrid, explorePos))
                {
                    // 将网格坐标转换为世界坐标
                    Vector3 targetPos = new Vector3(
                        centerX + explorePos.x * cellSize,
                        1.5f,
                        centerZ + explorePos.y * cellSize
                    );

                    // 移动到探索位置
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

                    currentGrid = explorePos;
                }
            }

            // 更新UI显示
            visualizer.UpdateStatus($"已探索节点数：{currentPathFinder.GetExploredCount()}");
        }

        // 寻路完成后的处理
        var path = currentPathFinder.GetPath();
        if (path != null && path.Count > 0)
        {
            visualizer.UpdateStatus($"找到路径！步数：{path.Count}");
        }
        else
        {
            visualizer.UpdateStatus("未找到有效路径！");
        }
    }

    // 修改辅助方法来检查两个位置之间是否可以移动
    private bool CanMoveBetween(Vector2Int from, Vector2Int to)
    {
        // 检查起点和终点是否都是有效的空地
        if (maze[from.x, from.y].IsWall || maze[to.x, to.y].IsWall)
            return false;

        // 检查是否是同一个位置
        if (from == to)
            return true;

        // 尝试通过中间点连接
        Vector2Int midPoint1 = new Vector2Int(from.x, to.y); // 横向移动后竖向移动
        Vector2Int midPoint2 = new Vector2Int(to.x, from.y); // 竖向移动后横向移动

        // 检查两条可能的路径
        bool canUsePath1 = !maze[midPoint1.x, midPoint1.y].IsWall && 
                          CheckStraightPath(from, midPoint1) && 
                          CheckStraightPath(midPoint1, to);

        bool canUsePath2 = !maze[midPoint2.x, midPoint2.y].IsWall && 
                          CheckStraightPath(from, midPoint2) && 
                          CheckStraightPath(midPoint2, to);

        return canUsePath1 || canUsePath2;
    }

    // 添加辅助方法检查直线路径
    private bool CheckStraightPath(Vector2Int from, Vector2Int to)
    {
        // 如果两点相同，返回true
        if (from == to) return true;

        // 确保两点在同一直线上
        if (from.x != to.x && from.y != to.y)
            return false;

        if (from.x == to.x)
        {
            // 在同一列上，检查中间是否有墙
            int minY = Mathf.Min(from.y, to.y);
            int maxY = Mathf.Max(from.y, to.y);
            for (int y = minY; y <= maxY; y++)
            {
                if (maze[from.x, y].IsWall)
                    return false;
            }
        }
        else
        {
            // 在同一行上，检查中间是否有墙
            int minX = Mathf.Min(from.x, to.x);
            int maxX = Mathf.Max(from.x, to.x);
            for (int x = minX; x <= maxX; x++)
            {
                if (maze[x, from.y].IsWall)
                    return false;
            }
        }

        return true;
    }

    public void ResetPathVisuals()
    {
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                if (!maze[x, y].IsWall && maze[x, y].CellObject != null)
                {
                    // 如果地板已经发光，保持发光状态
                    if (!maze[x, y].IsLit)
                    {
                        maze[x, y].CellObject.GetComponent<Renderer>().material.color = Color.white;
                    }
                }
            }
        }
    }

    private void UpdateMaze()
    {
        // 用于处理迷宫的更新逻辑
    }

    public void SetMazeSize(int width, int height)
    {
        mazeWidth = width;
        mazeHeight = height;
    }

    public void InitializePlayer()
    {
        float centerX = -mazeWidth * cellSize / 2f;
        float centerZ = -mazeHeight * cellSize / 2f;
        // 设置玩家在起点位置(1,1)，稍微抬高起始位置以避免穿透地面
        Vector3 startPos = new Vector3(centerX + cellSize, 1.5f, centerZ + cellSize);
        PlayerManager.GetInstance().SetPlayerPosition(startPos, true);  // 标记为初始位置
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
        // 检查坐标是否有效
        if (x < 0 || x >= mazeWidth || z < 0 || z >= mazeHeight) return;
        
        // 如果是地板且还未发光
        if (!maze[x, z].IsWall && !maze[x, z].IsLit)
        {
            maze[x, z].IsLit = true;
            
            // 获取地板对象
            GameObject floor = maze[x, z].CellObject;
            if (floor != null)
            {
                var renderer = floor.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    // 创建发光材质
                    Material glowMaterial = new Material(renderer.material);
                    glowMaterial.color = new Color(0.3f, 0.3f, 0.4f);  // 发光颜色
                    glowMaterial.SetFloat("_Glossiness", 0.5f);        // 增加光泽
                    glowMaterial.SetFloat("_Metallic", 0.2f);          // 增加金属感
                    
                    // 如果需要自发光效果，可以设置自发光颜色
                    glowMaterial.EnableKeyword("_EMISSION");
                    glowMaterial.SetColor("_EmissionColor", new Color(0.1f, 0.1f, 0.2f));
                    
                    renderer.material = glowMaterial;
                }
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
        // 创建起点效果
        startPointEffect = CreateCircleEffect(new Color(0f, 1f, 0f, 0.5f));  // 绿色光圈
        Vector3 startPos = GetWorldPosition(1, 1, 0.1f);  // 稍微抬高一点避免穿透
        startPointEffect.transform.position = startPos;

        // 创建终点效果
        endPointEffect = CreateCircleEffect(new Color(1f, 0f, 0f, 0.5f));  // 红色光圈
        Vector3 endPos = GetWorldPosition(mazeWidth - 2, mazeHeight - 2, 0.1f);
        endPointEffect.transform.position = endPos;
    }

    private ParticleSystem CreateCircleEffect(Color color)
    {
        GameObject effectObj = new GameObject("CircleEffect");
        effectObj.transform.SetParent(mazeContainer.transform);
        
        ParticleSystem ps = effectObj.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);  // 先停止粒子系统
        
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

        // 设置完所有参数后再启动
        ps.Play();

        return ps;
    }

    private Vector3 GetWorldPosition(int x, int y, float height)
    {
        float centerX, centerZ;
        GetMazeCenter(out centerX, out centerZ);
        return new Vector3(centerX + x * cellSize, height, centerZ + y * cellSize);
    }
}