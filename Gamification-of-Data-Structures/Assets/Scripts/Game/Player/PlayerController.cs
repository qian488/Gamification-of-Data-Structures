using UnityEngine;

/// <summary>
/// 视角模式枚举
/// 定义游戏中可用的视角模式
/// </summary>
public enum ViewMode
{
    FirstPerson,
    TopDown
}

/// <summary>
/// 玩家控制器类
/// 负责处理玩家的输入和移动控制
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 移动控制：
///    - WASD键移动
///    - 鼠标视角控制
///    - 视角切换（V键）
/// 2. 状态管理：
///    - EnableControl()：启用控制
///    - DisableControl()：禁用控制
///    - 碰撞检测和位置更新
/// 3. 光照控制：
///    - L键开关聚光灯
///    - 自动照亮行走路径
/// 
/// 使用方式：
/// - 通过PlayerManager创建和管理
/// - 自动处理输入和移动
/// - 通过事件系统发送移动和状态更新
/// </remarks>
public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 5f;
    private float mouseSensitivity = 1.5f;
    private Rigidbody rb;
    private bool canMove = false;
    private ViewMode currentViewMode = ViewMode.FirstPerson;
    private Camera mainCamera;
    private bool spotLightEnabled = true;
    private float defaultSpotLightRange = 60f;
    private float defaultSpotLightAngle = 120f;

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

        UpdateCameraView(currentViewMode);

        // 初始化聚光灯
        PlayerManager.GetInstance().SetSpotLightEnabled(spotLightEnabled);
        PlayerManager.GetInstance().SetSpotLightRange(defaultSpotLightRange);
        PlayerManager.GetInstance().SetSpotLightAngle(defaultSpotLightAngle);
    }

    private void Update()
    {
        // 处理Alt键控制鼠标显示
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
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
        if (Input.GetKeyDown(KeyCode.Alpha1))  
        {
            MazeManager.GetInstance().GenerateMaze();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))  
        {
            MazeManager.GetInstance().StartPathFinding(true);  // true表示使用DFS
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))  
        {
            MazeManager.GetInstance().StartPathFinding(false);  // false表示使用BFS
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            MazeManager.GetInstance().ResetAll();
        }
        
        CheckAndLightFloor();
        CheckGameFinish();

        // 检测 ESC 键退出游戏
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            canMove = false;

            GameUIManager.GetInstance().ShowExitConfirmPanel();
        }

        // 添加聚光灯开关控制
        if (Input.GetKeyDown(KeyCode.L))
        {
            spotLightEnabled = !spotLightEnabled;
            PlayerManager.GetInstance().SetSpotLightEnabled(spotLightEnabled);
        }

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
            transform.Rotate(Vector3.up * mouseDelta.x * mouseSensitivity);
        }
        else
        {
            if (Input.GetMouseButton(1))  
            {
                transform.Rotate(Vector3.up * mouseDelta.x * mouseSensitivity);
                
                // 更新相机的俯视角旋转以匹配玩家方向
                var cameraFollow = mainCamera.GetComponent<CameraFollow>();
                if (cameraFollow != null)
                {
                    cameraFollow.SetTopDownView();
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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CheckAndLightFloor()
    {
        // 从玩家位置向下发射射线
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
        {
            Vector3 hitPoint = hit.point;
            
            float centerX, centerZ;
            MazeManager.GetInstance().GetMazeCenter(out centerX, out centerZ);
            
            int x = Mathf.RoundToInt((hitPoint.x - centerX) / MazeManager.GetInstance().CellSize);
            int z = Mathf.RoundToInt((hitPoint.z - centerZ) / MazeManager.GetInstance().CellSize);

            MazeManager.GetInstance().LightFloor(x, z);
        }
    }

    private void CheckGameFinish()
    {
        Vector3 playerPos = transform.position;
        float centerX, centerZ;
        MazeManager.GetInstance().GetMazeCenter(out centerX, out centerZ);
        
        int playerGridX = Mathf.RoundToInt((playerPos.x - centerX) / MazeManager.GetInstance().CellSize);
        int playerGridZ = Mathf.RoundToInt((playerPos.z - centerZ) / MazeManager.GetInstance().CellSize);
        
        if (playerGridX == MazeManager.GetInstance().MazeWidth - 2 && 
            playerGridZ == MazeManager.GetInstance().MazeHeight - 2)
        {
            canMove = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void EnableControl()
    {
        canMove = true;
    }

    public void DisableControl()
    {
        canMove = false;
    }
} 