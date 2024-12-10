using UnityEngine;

public enum ViewMode
{
    FirstPerson,
    TopDown
}

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 2f;
    private float mouseSensitivity = 1.5f;
    private Rigidbody rb;
    private bool canMove = false;
    private ViewMode currentViewMode = ViewMode.FirstPerson;
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
            // 重置玩家位置
            PlayerManager.GetInstance().ResetPlayer();
            // 重置寻路显示
            MazeManager.GetInstance().ResetPathVisuals();
            // 重置算法可视化
            GameUIManager.GetInstance().ResetVisualizer();
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
            // 获取击中点的世界坐标
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