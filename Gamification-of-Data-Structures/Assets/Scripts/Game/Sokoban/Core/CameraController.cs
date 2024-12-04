using UnityEngine;

namespace Sokoban
{
    public class CameraController : MonoBehaviour
    {
        private Camera cam;
        private float cameraHeight = 15f;  // 相机高度
        private float cameraAngle = 45f;   // 相机角度

        private void Awake()
        {
            cam = GetComponent<Camera>();
            if (cam != null)
            {
                cam.fieldOfView = 60f;
                transform.rotation = Quaternion.Euler(cameraAngle, 0, 0);
            }
            else
            {
                Debug.LogError("No Camera component found on CameraController object");
            }
        }

        public void SetupCamera(Vector3 mapCenter, Vector2 mapSize)
        {
            // 计算相机位置，使其能看到整个地图
            float maxSize = Mathf.Max(mapSize.x, mapSize.y);
            float distance = maxSize * 0.8f;  // 根据地图大小调整距离
            
            // 设置相机位置，稍微往后移以便看到整个地图
            Vector3 position = mapCenter + new Vector3(0, cameraHeight, -distance);
            transform.position = position;
        }
    }
} 