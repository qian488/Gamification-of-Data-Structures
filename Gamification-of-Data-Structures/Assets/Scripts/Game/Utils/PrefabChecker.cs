using UnityEngine;

public class PrefabChecker
{
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
            pointLight.type = LightType.Spot;                     // 使用聚光灯
            pointLight.color = new Color(1f, 0.98f, 0.95f);      // 自然白光
            pointLight.intensity = 1.2f;                          // 适中的强度
            pointLight.range = 20f;                               // 较大的范围
            pointLight.spotAngle = 120f;                         // 更大的照射角度
            pointLight.innerSpotAngle = 100f;                    // 更大的内部光照角度
            pointLight.shadows = LightShadows.Soft;
            pointLight.shadowStrength = 0.4f;                    // 较弱的阴影强度
            pointLight.renderMode = LightRenderMode.ForcePixel;
            pointLight.cookie = null;                            // 不使用cookie贴图
            
            // 将光源放在玩家头部位置
            pointLight.transform.localPosition = new Vector3(0, 1.2f, 0);
            pointLight.transform.localRotation = Quaternion.Euler(90, 0, 0);  // 向下照射
        }
    }

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
            // 创建一个新的标准材质
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
        // 检查Canvas组件
        if (!uiObject.TryGetComponent(out Canvas canvas))
        {
            canvas = uiObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        // 检查CanvasScaler组件
        if (!uiObject.TryGetComponent(out UnityEngine.UI.CanvasScaler scaler))
        {
            scaler = uiObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }

        // 检查GraphicRaycaster组件
        if (!uiObject.TryGetComponent(out UnityEngine.UI.GraphicRaycaster raycaster))
        {
            raycaster = uiObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }
    }
}