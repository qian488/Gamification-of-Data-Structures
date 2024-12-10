using UnityEngine;
using System.Collections;

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
    
    public int MazeWidth => mazeWidth;
    public int MazeHeight => mazeHeight;
    public float CellSize => cellSize;

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
        // 先生成迷宫数据
        InitializeMaze();
        mazeGenerator.GenerateMaze(maze);
        
        // 使用协程创建迷宫视觉效果，并在完成后初始化玩家
        MonoManager.GetInstance().StartCoroutine(GenerateMazeCoroutine());
    }

    private IEnumerator GenerateMazeCoroutine()
    {
        // 创建视觉效果
        yield return CreateMazeVisuals();
        
        // 视觉效果创建完成后再初始化玩家和播放音效
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

    public void StartPathFinding(bool useDFS = true)
    {
        ResetPathVisuals();
        
        if (useDFS)
        {
            currentPathFinder = new DFSPathFinder(maze);
        }
        else
        {
            currentPathFinder = new BFSPathFinder(maze);
        }

        MonoManager.GetInstance().StartCoroutine(currentPathFinder.FindPath());
    }

    private void ResetPathVisuals()
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
        PlayerManager.GetInstance().SetPlayerPosition(startPos);
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
}