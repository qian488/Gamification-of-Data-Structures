using UnityEngine;
using System.Collections.Generic;

namespace Sokoban
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        private Vector3 targetPosition;
        private bool isMoving = false;
        private TileManager tileManager;
        private Stack<Vector2Int> moveHistory = new Stack<Vector2Int>();

        private void Start()
        {
            targetPosition = transform.position;
            EventCenter.GetInstance().AddEventListener<KeyCode>("KeyDown", OnKeyDown);
            tileManager = FindObjectOfType<TileManager>();
        }

        private void Update()
        {
            if (isMoving)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 
                    moveSpeed * Time.deltaTime);
                
                if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                {
                    transform.position = targetPosition;
                    isMoving = false;
                }
            }
        }

        private void OnKeyDown(KeyCode key)
        {
            if(isMoving) return;

            Vector3 moveDirection = Vector3.zero;
            switch(key)
            {
                case KeyCode.W: moveDirection = Vector3.forward; break;
                case KeyCode.S: moveDirection = Vector3.back; break;
                case KeyCode.A: moveDirection = Vector3.left; break;
                case KeyCode.D: moveDirection = Vector3.right; break;
            }

            if(moveDirection != Vector3.zero)
            {
                transform.forward = moveDirection;
                TryMove(moveDirection);
            }
        }

        private void TryMove(Vector3 direction)
        {
            Vector3 nextPos = transform.position + direction;
            
            bool canMove = true;

            // 检查前方是否有墙
            if (Physics.CheckBox(nextPos, Vector3.one * 0.4f, Quaternion.identity, 
                LayerMask.GetMask("Wall")))
            {
                canMove = false;
            }

            // 检查前方是否有箱子
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, 1f, 
                LayerMask.GetMask("Box")))
            {
                Box box = hit.collider.GetComponent<Box>();
                if (box && !box.TryMove(direction))
                {
                    canMove = false;
                }
            }

            if (canMove)
            {
                // 移动玩家
                targetPosition = nextPos;
                isMoving = true;
                Vector2Int currentPos = new Vector2Int(Mathf.RoundToInt(transform.position.x),
                    Mathf.RoundToInt(transform.position.z));
                tileManager.SetTileGlow(currentPos, true);
                moveHistory.Push(currentPos);

                // 触发移动事件
                EventCenter.GetInstance().EventTrigger("PlayerMove");
            }
        }

        private void OnDestroy()
        {
            EventCenter.GetInstance().RemoveEventListener<KeyCode>("KeyDown", OnKeyDown);
        }

        public void ResetState()
        {
            isMoving = false;
            targetPosition = transform.position;
        }

        public void UndoMove()
        {
            if (moveHistory.Count > 0)
            {
                Vector2Int lastPos = moveHistory.Pop();
                tileManager.SetTileGlow(lastPos, false);
            }
        }
    }
} 