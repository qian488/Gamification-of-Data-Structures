using UnityEngine;
using System.Collections;

/// <summary>
/// 玩家管理器类
/// 负责管理玩家对象的生命周期和状态
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 管理玩家对象的创建和销毁
/// 2. 控制玩家的位置和朝向
/// 3. 处理玩家的重生和重置
/// 4. 协调玩家与其他系统的交互
/// 5. 维护玩家的全局状态
/// </remarks>
public class PlayerManager : BaseManager<PlayerManager>
{
    /// <summary>玩家游戏对象引用</summary>
    private GameObject player;
    /// <summary>相机跟随组件引用</summary>
    private CameraFollow cameraFollow;
    /// <summary>玩家初始位置</summary>
    private Vector3 startPosition;
    private float moveSpeed = 10f;  // 基础移动速度

    /// <summary>
    /// 初始化玩家管理器
    /// 加载玩家预制体并设置必要的组件
    /// </summary>
    public void Init()
    {
        // 先检查是否已经初始化过
        if (player != null) return;

        // 加载玩家预制体
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

        MonoManager.GetInstance().AddUpdateListener(OnUpdate);
    }

    /// <summary>
    /// 处理玩家的输入更新
    /// 包括移动和旋转控制
    /// </summary>
    private void OnUpdate()
    {
        // 处理键盘输入
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

        // 处理鼠标输入
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
            // 开始重置动画
            MonoManager.GetInstance().StartCoroutine(ResetAnimation());
        }
    }

    /// <summary>
    /// 重置动画协程
    /// 控制玩家重置时的动画效果
    /// </summary>
    private IEnumerator ResetAnimation()
    {
        float duration = 1.0f;  // 动画持续时间
        float heightMax = 10f;  // 最大上升高度
        Vector3 startPos = player.transform.position;
        Vector3 upPos = startPos + Vector3.up * heightMax;

        // 上升阶段
        float elapsed = 0;
        while (elapsed < duration/2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed/(duration/2);
            // 使用平滑的插值
            float smoothT = Mathf.SmoothStep(0, 1, t);
            player.transform.position = Vector3.Lerp(startPos, upPos, smoothT);
            yield return null;
        }

        // 短暂消失
        player.SetActive(false);
        yield return new WaitForSeconds(0.2f);

        // 设置到起始位置
        player.transform.position = startPosition;
        
        // 重新出现
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
} 