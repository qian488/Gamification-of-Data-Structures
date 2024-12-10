using UnityEngine;

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
    private Quaternion topDownRotation;  // 保存切换到俯视角时的方向

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