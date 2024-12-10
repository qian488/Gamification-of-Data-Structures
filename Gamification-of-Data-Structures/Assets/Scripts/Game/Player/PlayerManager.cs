using UnityEngine;

public class PlayerManager : BaseManager<PlayerManager>
{
    private GameObject playerObject;
    private PlayerController playerController;
    private Vector3 startPosition;
    private Camera mainCamera;

    public void Init()
    {
        // 获取主摄像机
        mainCamera = Camera.main;
        if(mainCamera != null)
        {
            var cameraFollow = mainCamera.gameObject.AddComponent<CameraFollow>();
            cameraFollow.offset = new Vector3(0, 10, -10);
            cameraFollow.smoothSpeed = 0.125f;

            // 设置相机渲染设置
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = Color.black;
        }

        // 加载玩家预制体
        ResourcesManager.GetInstance().LoadAsync<GameObject>("Prefabs/Player", (go) =>
        {
            playerObject = go;
            PrefabChecker.CheckAndAddPlayerComponents(playerObject);
            playerController = playerObject.GetComponent<PlayerController>();
            
            if(mainCamera != null)
            {
                mainCamera.GetComponent<CameraFollow>().target = playerObject.transform;
            }

            // 注册Update事件
            MonoManager.GetInstance().AddUpdateListener(OnUpdate);
        });
    }

    private void OnUpdate()
    {
        if (playerController == null) return;

        // 处理键盘输入
        float horizontal = 0;
        float vertical = 0;

        if (Input.GetKey(KeyCode.W)) vertical += 1;
        if (Input.GetKey(KeyCode.S)) vertical -= 1;
        if (Input.GetKey(KeyCode.A)) horizontal -= 1;
        if (Input.GetKey(KeyCode.D)) horizontal += 1;

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

    public void SetPlayerPosition(Vector3 position)
    {
        if (playerObject != null)
        {
            startPosition = position;
            playerObject.transform.position = position;
        }
    }

    public void ResetPlayer()
    {
        if (playerObject != null)
        {
            playerObject.transform.position = startPosition;
        }
    }
} 