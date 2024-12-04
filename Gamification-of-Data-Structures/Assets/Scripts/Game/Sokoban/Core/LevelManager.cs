using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Sokoban
{
    public class LevelManager : MonoBehaviour
    {
        private int currentLevel = 0;
        private GameObject[] boxes;
        private Transform[] targets;
        
        private const string BOX_PREFAB_PATH = "Prefab/Box";
        private const string WALL_PREFAB_PATH = "Prefab/Wall";
        private const string TARGET_PREFAB_PATH = "Prefab/Target";
        private const string PLAYER_PREFAB_PATH = "Prefab/Player";

        private CameraController cameraController;

        private LevelData currentLevelData;
        private Vector3[] initialBoxPositions;
        private Vector3 initialPlayerPosition;

        private TileManager tileManager;

        private int remainingObjects = 0; // 移到类级别
        private bool isLoading = false;   // 添加加载状态标记
        private int boxCount = 0;  // 用于跟踪box的数量

        public int CurrentLevel => currentLevel;

        private void Start()
        {
            // 获取或创建相机控制器
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraController = mainCamera.GetComponent<CameraController>();
                if (cameraController == null)
                {
                    cameraController = mainCamera.gameObject.AddComponent<CameraController>();
                }
            }

            tileManager = gameObject.AddComponent<TileManager>();
        }

        public void LoadLevel(int levelIndex)
        {
            if (isLoading) return; // 如果正在加载，忽略新的加载请求
            isLoading = true;

            if (levelIndex <= 0)
            {
                Debug.LogError($"Invalid level index: {levelIndex}");
                isLoading = false;
                return;
            }

            LevelData levelData = GameManager.GetInstance().GetLevelData(levelIndex);
            if (levelData == null)
            {
                Debug.LogError($"Failed to load level data for index: {levelIndex}");
                isLoading = false;
                return;
            }

            ClearLevel(() => {
                StartLoadLevel(levelIndex, levelData);
            });
        }

        private void StartLoadLevel(int levelIndex, LevelData levelData)
        {
            currentLevel = levelIndex;
            currentLevelData = levelData;
            
            int expectedBoxCount = 0;

            tileManager.CreateFloor(currentLevelData.width, currentLevelData.height);

            remainingObjects = 0; // 重置计数器

            // 预先计算box数量
            for (int x = 0; x < currentLevelData.width; x++)
            {
                for (int z = 0; z < currentLevelData.height; z++)
                {
                    if (currentLevelData.map[x, z] == 2) // Box
                    {
                        expectedBoxCount++;
                    }
                }
            }

            // 预分配数组
            initialBoxPositions = new Vector3[expectedBoxCount];
            boxCount = 0;  // 重置计数器

            // 生成地图
            for (int x = 0; x < currentLevelData.width; x++)
            {
                for (int z = 0; z < currentLevelData.height; z++)
                {
                    Vector3 pos = new Vector3(x, 0, z);
                    int tileType = currentLevelData.map[x, z];
                    if (tileType == 0) continue;

                    string prefabPath = GetPrefabPath(tileType);
                    if (string.IsNullOrEmpty(prefabPath)) continue;

                    int currentX = x;
                    int currentZ = z;
                    PoolManager.GetInstance().GetGameObject(prefabPath, (obj) =>
                    {
                        if (obj != null)
                        {
                            SetupGameObject(obj, tileType, new Vector3(currentX, 0, currentZ));
                        }
                        remainingObjects--;
                        if (remainingObjects == 0)
                        {
                            OnLevelLoaded();
                        }
                    });
                }
            }
        }

        private string GetPrefabPath(int tileType)
        {
            switch (tileType)
            {
                case 1: return WALL_PREFAB_PATH;
                case 2: return BOX_PREFAB_PATH;
                case 3: return TARGET_PREFAB_PATH;
                case 4: return PLAYER_PREFAB_PATH;
                default: return null;
            }
        }

        private void SetupGameObject(GameObject obj, int tileType, Vector3 pos)
        {
            switch (tileType)
            {
                case 1: // 墙
                    obj.transform.SetParent(transform);
                    obj.transform.position = pos;
                    obj.transform.rotation = Quaternion.identity;
                    obj.transform.localScale = new Vector3(1f, 2f, 1f);
                    obj.tag = "Wall";
                    break;
                case 2: // 箱子
                    obj.transform.SetParent(transform);
                    obj.transform.position = pos;
                    obj.transform.rotation = Quaternion.identity;
                    obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    obj.tag = "Box";
                    initialBoxPositions[boxCount++] = pos;  // 按顺序记录位置
                    break;
                case 3: // 目标点
                    obj.transform.SetParent(transform);
                    obj.transform.position = pos;
                    obj.transform.rotation = Quaternion.identity;
                    obj.tag = "Target";
                    break;
                case 4: // 玩家起点
                    obj.transform.position = pos;
                    obj.transform.rotation = Quaternion.identity;
                    obj.tag = "Player";
                    initialPlayerPosition = pos;
                    break;
            }
        }

        private void ClearLevel(System.Action onCleared = null)
        {
            // 清理前重置所有引用
            boxes = null;
            targets = null;
            initialBoxPositions = null;

            List<GameObject> objectsToClear = new List<GameObject>();

            // 收集所有需要清理的对象
            foreach (Transform child in transform)
            {
                if (child != null && child.gameObject != null)
                {
                    objectsToClear.Add(child.gameObject);
                }
            }
            
            // 清除玩家
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                objectsToClear.Add(player);
            }

            // 清理收集到的对象
            foreach (var obj in objectsToClear)
            {
                if (obj != null)
                {
                    string prefabPath = GetPrefabPathByTag(obj.tag);
                    if (!string.IsNullOrEmpty(prefabPath))
                    {
                        PoolManager.GetInstance().PushGameObject(prefabPath, obj);
                    }
                }
            }

            onCleared?.Invoke();
        }

        private string GetPrefabPathByTag(string tag)
        {
            switch (tag)
            {
                case "Wall": return WALL_PREFAB_PATH;
                case "Box": return BOX_PREFAB_PATH;
                case "Target": return TARGET_PREFAB_PATH;
                case "Player": return PLAYER_PREFAB_PATH;
                default: return string.Empty;
            }
        }

        private void OnLevelLoaded()
        {
            isLoading = false;

            // 设置相机
            if (cameraController != null)
            {
                Vector3 mapCenter = new Vector3(currentLevelData.width / 2f, 0, currentLevelData.height / 2f);
                Vector2 mapSize = new Vector2(currentLevelData.width, currentLevelData.height);
                cameraController.SetupCamera(mapCenter, mapSize);
            }

            // 更新引用
            boxes = GameObject.FindGameObjectsWithTag("Box");
            // 确保boxPositions和boxes数量匹配
            if (boxes.Length != initialBoxPositions.Length)
            {
                Debug.LogError($"Box count mismatch after level load: found {boxes.Length}, positions {initialBoxPositions.Length}");
                // 重新加载关卡
                LoadLevel(currentLevel);
                return;
            }
            targets = GameObject.FindGameObjectsWithTag("Target")
                .Select(go => go.transform).ToArray();
        }

        public void ReloadCurrentLevel()
        {
            LoadLevel(currentLevel);
        }

        public bool IsLevelComplete()
        {
            // 添加空检查
            if (targets == null || boxes == null)
            {
                return false;
            }

            // 检查所有箱子是否都在目标点上
            foreach(var target in targets)
            {
                if (target == null) continue;

                bool hasBox = false;
                foreach(var box in boxes)
                {
                    if (box == null) continue;

                    if(Vector3.Distance(box.transform.position, target.position) < 0.1f)
                    {
                        hasBox = true;
                        break;
                    }
                }
                if(!hasBox) return false;
            }
            return true;
        }

        public void ResetLevel()
        {
            if (initialBoxPositions == null)
            {
                Debug.LogError("Cannot reset level: initialBoxPositions is null");
                return;
            }

            // 重置所有box的位置
            boxes = GameObject.FindGameObjectsWithTag("Box");
            if (boxes.Length != initialBoxPositions.Length)
            {
                Debug.LogError($"Box count mismatch during reset: found {boxes.Length}, expected {initialBoxPositions.Length}");
                return;
            }

            for (int i = 0; i < boxes.Length; i++)
            {
                if (boxes[i] != null)
                {
                    boxes[i].transform.position = initialBoxPositions[i];
                    boxes[i].GetComponent<Box>()?.ResetState();
                }
            }

            // 重置玩家位置
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = initialPlayerPosition;
                player.GetComponent<PlayerController>()?.ResetState();
            }

            // 清除发光效果
            tileManager?.ClearAllGlow();

            // 触发重置事件
            EventCenter.GetInstance().EventTrigger("LevelReset");
        }

        private void OnDestroy()
        {
            // 场景关闭时不需要回收到对象池
            foreach (Transform child in transform)
            {
                if (child != null)
                {
                    Destroy(child.gameObject);
                }
            }

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Destroy(player);
            }
        }
    }
} 