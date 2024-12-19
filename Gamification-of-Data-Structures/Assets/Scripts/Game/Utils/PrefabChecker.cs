using UnityEngine;

/// <summary>
/// 预制体检查工具类
/// 负责检查和配置游戏对象的必要组件
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. CheckAndAddPlayerComponents：配置玩家对象组件
///    - 添加控制器、物理组件、碰撞器和光源
/// 2. CheckAndAddMazeCellComponents：配置迷宫单元格组件
///    - 添加碰撞器、渲染器和网格组件
/// 3. CheckAndAddUIComponents：配置UI对象组件
///    - 添加Canvas相关组件
/// 
/// 使用方式：
/// PrefabChecker.CheckAndAddPlayerComponents(playerObj);
/// PrefabChecker.CheckAndAddMazeCellComponents(cellObj, isWall);
/// PrefabChecker.CheckAndAddUIComponents(uiObj);
/// </remarks>
public class PrefabChecker
{
    /// <summary>
    /// 检查并添加玩家对象所需的所有组件
    /// </summary>
    /// <param name="playerObject">玩家游戏对象</param>
    /// <remarks>
    /// 添加的组件包括：
    /// - PlayerController：控制玩家行为
    /// - Rigidbody：物理模拟
    /// - CapsuleCollider：碰撞检测
    /// - Light：玩家随身光源
    /// </remarks>
    public static void CheckAndAddPlayerComponents(GameObject playerObject)
    {
        // 检查并添加必需的组件
        if (!playerObject.TryGetComponent(out PlayerController controller))
        {
            controller = playerObject.AddComponent<PlayerController>();
        }

        if (!playerObject.TryGetComponent(out Rigidbody rb))
        {
            rb = playerObject.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.mass = 1f;
            rb.drag = 10f;
            rb.angularDrag = 0.5f;
        }

        if (!playerObject.TryGetComponent(out CapsuleCollider collider))
        {
            collider = playerObject.AddComponent<CapsuleCollider>();
            collider.height = 2f;
            collider.radius = 0.5f;
            collider.center = Vector3.up;
        }

        // 添加点光源
        if (!playerObject.TryGetComponent(out Light pointLight))
        {
            pointLight = playerObject.AddComponent<Light>();
            pointLight.type = LightType.Spot;                    
            pointLight.color = new Color(1f, 0.98f, 0.95f);      
            pointLight.intensity = 1.2f;                          
            pointLight.range = 20f;                              
            pointLight.spotAngle = 120f;                         
            pointLight.innerSpotAngle = 100f;                    
            pointLight.shadows = LightShadows.Soft;
            pointLight.shadowStrength = 0.4f;                    
            pointLight.renderMode = LightRenderMode.ForcePixel;
            pointLight.cookie = null;                            
            // 将光源放在玩家头部位置
            pointLight.transform.localPosition = new Vector3(0, 1.2f, 0);
            pointLight.transform.localRotation = Quaternion.Euler(90, 0, 0);  // 向下照射
        }
    }

    /// <summary>
    /// 检查并添加迷宫单元格所需的组件
    /// </summary>
    /// <param name="cellObject">单元格游戏对象</param>
    /// <param name="isWall">是否是墙壁</param>
    /// <remarks>
    /// 添加的组件包括：
    /// - BoxCollider：碰撞检测
    /// - MeshRenderer：网格渲染
    /// - MeshFilter：网格过滤器
    /// 如果是墙壁，还会添加Wall标签
    /// </remarks>
    public static void CheckAndAddMazeCellComponents(GameObject cellObject, bool isWall)
    {
        // 检查并添加必需的组件
        if (!cellObject.TryGetComponent(out BoxCollider collider))
        {
            collider = cellObject.AddComponent<BoxCollider>();
        }

        if (!cellObject.TryGetComponent(out MeshRenderer renderer))
        {
            renderer = cellObject.AddComponent<MeshRenderer>();
        }

        if (!cellObject.TryGetComponent(out MeshFilter filter))
        {
            filter = cellObject.AddComponent<MeshFilter>();
        }

        // 确保有材质
        if (renderer.sharedMaterial == null)
        {
            Material material = new Material(Shader.Find("Standard"));
            renderer.material = material;
        }

        // 如果是墙壁，设置tag
        if (isWall)
        {
            cellObject.tag = "Wall";
        }
    }

    public static void CheckAndAddUIComponents(GameObject uiObject)
    {
        if (!uiObject.TryGetComponent(out Canvas canvas))
        {
            canvas = uiObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        if (!uiObject.TryGetComponent(out UnityEngine.UI.CanvasScaler scaler))
        {
            scaler = uiObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }
        
        if (!uiObject.TryGetComponent(out UnityEngine.UI.GraphicRaycaster raycaster))
        {
            raycaster = uiObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }
    }
}