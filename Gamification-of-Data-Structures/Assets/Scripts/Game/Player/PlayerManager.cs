using UnityEngine;
using System.Collections;

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
            cameraFollow.target = null;
        }

        // 加载玩家预制体
        ResourcesManager.GetInstance().LoadAsync<GameObject>("Prefabs/Player", (go) =>
        {
            playerObject = go;
            PrefabChecker.CheckAndAddPlayerComponents(playerObject);
            playerController = playerObject.GetComponent<PlayerController>();
            
            if(mainCamera != null)
            {
                var cameraFollow = mainCamera.GetComponent<CameraFollow>();
                if (cameraFollow != null)
                {
                    cameraFollow.target = playerObject.transform;
                }
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

    public void SetPlayerPosition(Vector3 position, bool isInitialPosition = false)
    {
        if (playerObject != null)
        {
            if (isInitialPosition)
            {
                startPosition = position;  // 只在初始化时设置起始位置
            }
            playerObject.transform.position = position;
        }
    }

    public void ResetPlayer()
    {
        if (playerObject != null)
        {
            // 开始重置动画
            MonoManager.GetInstance().StartCoroutine(ResetAnimation());
        }
    }

    private IEnumerator ResetAnimation()
    {
        float duration = 1.0f;  // 动画持续时间
        float heightMax = 10f;  // 最大上升高度
        Vector3 startPos = playerObject.transform.position;
        Vector3 upPos = startPos + Vector3.up * heightMax;

        // 上升阶段
        float elapsed = 0;
        while (elapsed < duration/2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed/(duration/2);
            // 使用平滑的插值
            float smoothT = Mathf.SmoothStep(0, 1, t);
            playerObject.transform.position = Vector3.Lerp(startPos, upPos, smoothT);
            yield return null;
        }

        // 短暂消失
        playerObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);

        // 设置到起始位置
        playerObject.transform.position = startPosition;
        
        // 重新出现
        playerObject.SetActive(true);
        
        // 下降阶段
        Vector3 finalPos = startPosition;
        Vector3 appearPos = finalPos + Vector3.up * heightMax;
        playerObject.transform.position = appearPos;
        
        elapsed = 0;
        while (elapsed < duration/2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed/(duration/2);
            float smoothT = Mathf.SmoothStep(0, 1, t);
            playerObject.transform.position = Vector3.Lerp(appearPos, finalPos, smoothT);
            yield return null;
        }

        // 确保最终位置准确
        playerObject.transform.position = finalPos;
    }

    public Vector3 GetPlayerPosition()
    {
        return playerObject != null ? playerObject.transform.position : Vector3.zero;
    }

    public Quaternion GetPlayerRotation()
    {
        return playerObject != null ? playerObject.transform.rotation : Quaternion.identity;
    }

    public void SetPlayerRotation(Quaternion rotation)
    {
        if (playerObject != null)
        {
            playerObject.transform.rotation = rotation;
        }
    }
} 