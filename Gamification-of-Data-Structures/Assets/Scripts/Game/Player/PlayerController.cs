using UnityEngine;

/// <summary>
/// 视角模式枚举
/// 定义游戏中可用的视角模式
/// </summary>
public enum ViewMode
{
    /// <summary>第一人称视角</summary>
    FirstPerson,
    /// <summary>俯视角视角</summary>
    TopDown
}

/// <summary>
/// 玩家控制器类
/// 负责处理玩家的输入、移动和交互
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 处理键盘和鼠标输入
/// 2. 控制玩家移动和旋转
/// 3. 管理视角切换
/// 4. 处理碰撞检测
/// 5. 与迷宫环境交互
/// </remarks>
public class PlayerController : MonoBehaviour
{
    /// <summary>玩家移动速度</summary>
    private float moveSpeed = 2f;
    /// <summary>鼠标灵敏度</summary>
    private float mouseSensitivity = 1.5f;
    /// <summary>刚体组件引用</summary>
    private Rigidbody rb;
    /// <summary>是否允许移动</summary>
    private bool canMove = false;
    /// <summary>当前视角模式</summary>
    private ViewMode currentViewMode = ViewMode.FirstPerson;
    /// <summary>主相机引用</summary>
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.mass = 1f;
            rb.drag = 10f;
            rb.angularDrag = 0.5f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            canMove = true;
        }
        
        // 锁定并隐藏鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        EventCenter.GetInstance().AddEventListener<Vector2>("PlayerMove", OnPlayerMove);
        EventCenter.GetInstance().AddEventListener<Vector2>("MouseMove", OnMouseMove);

        // 初始化相机位置
        UpdateCameraView(currentViewMode);
    }

    private void Update()
    {
        // 处理Alt键控制鼠标显示
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            // Alt按下时显示鼠标
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            canMove = false;  // 显示鼠标时暂停移动
            
            // 启用UI交互
            Debug.Log("Triggering EnableUIInteraction"); // 添加调试日志
            EventCenter.GetInstance().EventTrigger("EnableUIInteraction");
        }
        else if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
        {
            // Alt松开时隐藏鼠标
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            canMove = true;  // 隐藏鼠标时恢复移动
            
            // 禁用UI交互
            Debug.Log("Triggering DisableUIInteraction"); // 添加调试日志
            EventCenter.GetInstance().EventTrigger("DisableUIInteraction");
        }

        // 检测视角切换输入
        if (Input.GetKeyDown(KeyCode.V))
        {
            SwitchView();
        }

        // 检测算法控制输入
        if (Input.GetKeyDown(KeyCode.Alpha1))  // 按1重新生成迷宫
        {
            MazeManager.GetInstance().GenerateMaze();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))  // 按2进行DFS演示
        {
            MazeManager.GetInstance().StartPathFinding(true);  // true表示使用DFS
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))  // 按3进行BFS演示
        {
            MazeManager.GetInstance().StartPathFinding(false);  // false表示使用BFS
        }
        else if (Input.GetKeyDown(KeyCode.R))  // 按R重置位置
        {
            MazeManager.GetInstance().ResetAll();
        }
        
        // 检测当前位置的地板
        CheckAndLightFloor();
    }

    private void SwitchView()
    {
        currentViewMode = (currentViewMode == ViewMode.FirstPerson) ? ViewMode.TopDown : ViewMode.FirstPerson;
        UpdateCameraView(currentViewMode);
    }

    private void UpdateCameraView(ViewMode mode)
    {
        if (mainCamera == null) return;
        
        var cameraFollow = mainCamera.GetComponent<CameraFollow>();
        if (cameraFollow == null) return;

        switch (mode)
        {
            case ViewMode.FirstPerson:
                cameraFollow.SetFirstPersonView();
                break;

            case ViewMode.TopDown:
                cameraFollow.SetTopDownView();
                break;
        }
    }

    private void OnPlayerMove(Vector2 input)
    {
        if (!canMove || rb == null) return;

        // 使用 MovePosition 而不是直接设置 velocity
        Vector3 moveDirection = transform.forward * input.y + transform.right * input.x;
        moveDirection.Normalize();
        
        Vector3 targetPosition = rb.position + moveDirection * moveSpeed * Time.deltaTime;
        rb.MovePosition(targetPosition);
    }

    private void OnMouseMove(Vector2 mouseDelta)
    {
        if (!canMove) return;

        // 根据视角模式处理旋转
        if (currentViewMode == ViewMode.FirstPerson)
        {
            // 第一人称模式：直接旋转玩家和相机
            transform.Rotate(Vector3.up * mouseDelta.x * mouseSensitivity);
        }
        else
        {
            // 俯视角模式：按住右键时旋转玩家
            if (Input.GetMouseButton(1))  // 1 代表鼠标右键
            {
                transform.Rotate(Vector3.up * mouseDelta.x * mouseSensitivity);
                
                // 更新相机的俯视角旋转以匹配玩家方向
                var cameraFollow = mainCamera.GetComponent<CameraFollow>();
                if (cameraFollow != null)
                {
                    cameraFollow.SetTopDownView();  // 这会根据玩家的新方向更新相机旋转
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            EventCenter.GetInstance().EventTrigger("GameFinish");
            // 显示并解锁鼠标
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void OnDestroy()
    {
        EventCenter.GetInstance().RemoveEventListener<Vector2>("PlayerMove", OnPlayerMove);
        EventCenter.GetInstance().RemoveEventListener<Vector2>("MouseMove", OnMouseMove);
        // 解锁鼠标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CheckAndLightFloor()
    {
        // 从玩家位置向下发射射线
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
        {
            // 获取击中��的世界坐标
            Vector3 hitPoint = hit.point;
            
            // 获取迷宫中心坐标
            float centerX, centerZ;
            MazeManager.GetInstance().GetMazeCenter(out centerX, out centerZ);
            
            // 计算相对位置
            int x = Mathf.RoundToInt((hitPoint.x - centerX) / MazeManager.GetInstance().CellSize);
            int z = Mathf.RoundToInt((hitPoint.z - centerZ) / MazeManager.GetInstance().CellSize);

            // 设置地板发光
            MazeManager.GetInstance().LightFloor(x, z);
        }
    }
} 