using UnityEngine;
using System.Collections;

/// <summary>
/// 玩家管理器类
/// 负责管理玩家对象的生命周期和状态
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 玩家管理：
///    - Init()：初始化玩家对象
///    - SetPlayerPosition()：设置玩家位置
///    - GetPlayer()：获取玩家对象
/// 2. 光照控制：
///    - SetSpotLightEnabled()：开关聚光灯
///    - SetSpotLightRange()：设置照射范围
///    - SetSpotLightAngle()：设置照射角度
/// 3. 移动控制：
///    - SetMoveSpeed()：设置移动速度
///    - GetMoveSpeed()：获取移动速度
/// 
/// 使用方式：
/// - 通过GetInstance()获取单例
/// - 使用Init()初始化玩家
/// - 通过提供的方法控制玩家状态
/// </remarks>
public class PlayerManager : BaseManager<PlayerManager>
{
    private GameObject player;
    private CameraFollow cameraFollow;
    private Vector3 startPosition;
    private float moveSpeed = 10f;  
    private Light spotLight;
    private float spotLightRange = 60f; 
    private float spotLightAngle = 120f;  
    private float spotLightIntensity = 1.5f;

    /// <summary>
    /// 初始化玩家管理器
    /// 加载玩家预制体并设置必要的组件
    /// </summary>
    public void Init()
    {
        if (player != null) return;

        ResourcesManager.GetInstance().LoadAsync<GameObject>("Prefabs/Player", (prefab) =>
        {
            if (prefab != null)
            {
                // 不要再次实例化，直接使用加载的对象
                player = prefab;
                player.tag = "Player";
                SetupPlayer();
            }
        });
    }

    /// <summary>
    /// 设置玩家的初始组件和属性
    /// 包括物理组件、碰撞器和相机设置
    /// </summary>
    private void SetupPlayer()
    {
        // 添加必要的组件
        if (player.GetComponent<PlayerController>() == null)
        {
            player.AddComponent<PlayerController>();
        }
        if (player.GetComponent<Rigidbody>() == null)
        {
            var rb = player.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.mass = 1f;
            rb.drag = 10f;
        }
        if (player.GetComponent<CapsuleCollider>() == null)
        {
            var capsule = player.AddComponent<CapsuleCollider>();
            capsule.height = 2f;
            capsule.radius = 0.5f;
            capsule.center = Vector3.up;
        }

        // 设置相机
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            cameraFollow = mainCamera.GetComponent<CameraFollow>();
            if (cameraFollow != null)
            {
                cameraFollow.target = player.transform;
            }
        }

        // 添加聚光灯
        GameObject lightObj = new GameObject("PlayerSpotLight");
        lightObj.transform.SetParent(player.transform);
        lightObj.transform.localPosition = new Vector3(0, 1f, 0); // 设置在玩家头部位置
        
        spotLight = lightObj.AddComponent<Light>();
        spotLight.type = LightType.Spot;
        spotLight.range = spotLightRange;
        spotLight.spotAngle = spotLightAngle;
        spotLight.intensity = spotLightIntensity;
        spotLight.color = new Color(1f, 1f, 0.9f); 
        spotLight.innerSpotAngle = spotLightAngle * 0.8f;  
        spotLight.cookie = null;
        
        MonoManager.GetInstance().AddUpdateListener(OnUpdate);
    }

    /// <summary>
    /// 处理玩家的输入更新
    /// 包括移动和旋转控制
    /// </summary>
    private void OnUpdate()
    {
        float horizontal = 0;
        float vertical = 0;

        if (Input.GetKey(KeyCode.W)) vertical += 2;
        if (Input.GetKey(KeyCode.S)) vertical -= 2;
        if (Input.GetKey(KeyCode.A)) horizontal -= 2;
        if (Input.GetKey(KeyCode.D)) horizontal += 2;

        if (horizontal != 0 || vertical != 0)
        {
            EventCenter.GetInstance().EventTrigger("PlayerMove", new Vector2(horizontal, vertical));
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        if (mouseX != 0 || mouseY != 0)
        {
            EventCenter.GetInstance().EventTrigger("MouseMove", new Vector2(mouseX, mouseY));
        }
    }

    /// <summary>
    /// 设置玩家位置
    /// </summary>
    /// <param name="position">目标位置</param>
    /// <param name="resetRotation">是否重置旋转</param>
    public void SetPlayerPosition(Vector3 position, bool resetRotation = false)
    {
        if (player != null)
        {
            if (resetRotation)
            {
                startPosition = position;
            }
            player.transform.position = position;
            if (resetRotation)
            {
                player.transform.rotation = Quaternion.identity;
                if (cameraFollow != null && cameraFollow.target != null)
                {
                    cameraFollow.target.rotation = Quaternion.identity;
                }
            }
        }
    }

    /// <summary>
    /// 重置玩家到初始状态
    /// 包含上升和下降的动画效果
    /// </summary>
    public void ResetPlayer()
    {
        if (player != null)
        {
            MonoManager.GetInstance().StartCoroutine(ResetAnimation());
        }
    }

    /// <summary>
    /// 重置动画协程
    /// 控制玩家重置时的动画效果
    /// </summary>
    private IEnumerator ResetAnimation()
    {
        float duration = 1.0f;  
        float heightMax = 10f;  
        Vector3 startPos = player.transform.position;
        Vector3 upPos = startPos + Vector3.up * heightMax;

        // 上升阶段
        float elapsed = 0;
        while (elapsed < duration/2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed/(duration/2);
            float smoothT = Mathf.SmoothStep(0, 1, t);
            player.transform.position = Vector3.Lerp(startPos, upPos, smoothT);
            yield return null;
        }

        // 短暂消失
        player.SetActive(false);
        yield return new WaitForSeconds(0.2f);

        // 设置到起始位置
        player.transform.position = startPosition;
        player.SetActive(true);
        
        // 下降阶段
        Vector3 finalPos = startPosition;
        Vector3 appearPos = finalPos + Vector3.up * heightMax;
        player.transform.position = appearPos;
        
        elapsed = 0;
        while (elapsed < duration/2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed/(duration/2);
            float smoothT = Mathf.SmoothStep(0, 1, t);
            player.transform.position = Vector3.Lerp(appearPos, finalPos, smoothT);
            yield return null;
        }

        // 确保最终位置准确
        player.transform.position = finalPos;
    }

    public Vector3 GetPlayerPosition()
    {
        return player != null ? player.transform.position : Vector3.zero;
    }

    public Quaternion GetPlayerRotation()
    {
        return player != null ? player.transform.rotation : Quaternion.identity;
    }

    public void SetPlayerRotation(Quaternion rotation)
    {
        if (player != null)
        {
            player.transform.rotation = rotation;
        }
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void SetSpotLightEnabled(bool enabled)
    {
        if (spotLight != null)
        {
            spotLight.enabled = enabled;
        }
    }

    public void SetSpotLightRange(float range)
    {
        spotLightRange = range;
        if (spotLight != null)
        {
            spotLight.range = range;
        }
    }

    public void SetSpotLightAngle(float angle)
    {
        spotLightAngle = angle;
        if (spotLight != null)
        {
            spotLight.spotAngle = angle;
            spotLight.innerSpotAngle = angle * 0.8f;
        }
    }

    public void SetSpotLightIntensity(float intensity)
    {
        spotLightIntensity = intensity;
        if (spotLight != null)
        {
            spotLight.intensity = intensity;
        }
    }

    private void OnDestroy()
    {
        if (spotLight != null)
        {
            GameObject.Destroy(spotLight.gameObject);
        }
    }

    public GameObject GetPlayer()
    {
        return player;
    }
} 