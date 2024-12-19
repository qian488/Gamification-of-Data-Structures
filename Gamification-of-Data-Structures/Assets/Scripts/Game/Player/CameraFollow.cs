using UnityEngine;

/// <summary>
/// 相机跟随控制类
/// 负责控制相机跟随玩家移动和视角切换
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 视角控制：
///    - SetFirstPersonView()：切换到第一人称视角
///    - SetTopDownView()：切换到俯视角视角
/// 2. 相机跟随：
///    - 平滑跟随目标移动
///    - 根据视角模式调整位置和旋转
/// 3. 参数配置：
///    - firstPersonOffset：第一人称视角偏移
///    - topDownOffset：俯视角视角偏移
///    - smoothSpeed：平滑移动速度
/// 
/// 使用方式：
/// - 将脚本挂载到主相机上
/// - 设置target为要跟随的玩家对象
/// - 通过SetFirstPersonView/SetTopDownView切换视角
/// </remarks>
public class CameraFollow : MonoBehaviour
{

    public Transform target;
    public Vector3 firstPersonOffset = new Vector3(0, 1.8f, 0);
    public Vector3 topDownOffset = new Vector3(0, 20f, 0);
    public float firstPersonSmoothSpeed = 0.15f;
    public float topDownSmoothSpeed = 0.05f;
    
    private bool isFirstPerson = true;
    private Vector3 currentOffset;
    private float currentSmoothSpeed;
    private Quaternion topDownRotation;

    private void Start()
    {
        SetFirstPersonView();
    }

    public void SetFirstPersonView()
    {
        isFirstPerson = true;
        currentOffset = firstPersonOffset;
        currentSmoothSpeed = firstPersonSmoothSpeed;
    }

    public void SetTopDownView()
    {
        isFirstPerson = false;
        currentOffset = topDownOffset;
        currentSmoothSpeed = topDownSmoothSpeed;

        // 计算俯视角旋转
        // 基于玩家当前朝向计算摄像机旋转
        // 使玩家的前方对应屏幕的上方
        if (target != null)
        {
            float targetYRotation = target.eulerAngles.y;
            topDownRotation = Quaternion.Euler(90, targetYRotation, 0);
            transform.rotation = topDownRotation;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + currentOffset;
        
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, currentSmoothSpeed);
        transform.position = smoothedPosition;

        if (isFirstPerson)
        {
            transform.rotation = target.rotation;
        }
        else
        {
            transform.rotation = topDownRotation;
        }
    }
}