using UnityEngine;

/// <summary>
/// 相机跟随控制类
/// 负责控制相机跟随玩家移动和视角切换
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 第一人称视角跟随
/// 2. 俯视角视角控制
/// 3. 平滑过渡的相机移动
/// 4. 视角切换时的位置和旋转调整
/// </remarks>
public class CameraFollow : MonoBehaviour
{
    /// <summary>跟随的目标对象（通常是玩家）</summary>
    public Transform target;
    /// <summary>第一人称视角的偏移量（相对于玩家位置）</summary>
    public Vector3 firstPersonOffset = new Vector3(0, 1.8f, 0);
    /// <summary>俯视角视角的偏移量（相对于玩家位置）</summary>
    public Vector3 topDownOffset = new Vector3(0, 20f, 0);
    /// <summary>第一人称视角的平滑移动速度</summary>
    public float firstPersonSmoothSpeed = 0.15f;
    /// <summary>俯视角视角的平滑移动速度</summary>
    public float topDownSmoothSpeed = 0.05f;
    
    /// <summary>当前是否为第一人称视角</summary>
    private bool isFirstPerson = true;
    /// <summary>当前使用的偏移量</summary>
    private Vector3 currentOffset;
    /// <summary>当前使用的平滑移动速度</summary>
    private float currentSmoothSpeed;
    /// <summary>俯视角时的固定旋转</summary>
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

        // 计算目标位置
        Vector3 desiredPosition = target.position + currentOffset;
        
        // 平滑移动
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, currentSmoothSpeed);
        transform.position = smoothedPosition;

        // 根据视角模式更新旋转
        if (isFirstPerson)
        {
            transform.rotation = target.rotation;
        }
        else
        {
            // 保持俯视角的固定旋转
            transform.rotation = topDownRotation;
        }
    }
}