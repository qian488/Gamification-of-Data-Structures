using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 2f;
    private float mouseSensitivity = 1.5f;
    private Rigidbody rb;
    private bool canMove = false;
    private float rotationY = 0f;

    private void Start()
    {
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

        // 水平旋转
        transform.Rotate(Vector3.up * mouseDelta.x * mouseSensitivity);
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