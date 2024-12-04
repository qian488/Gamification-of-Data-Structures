using UnityEngine;

namespace Sokoban
{
    public class Box : MonoBehaviour
    {
        private bool isMoving = false;
        private Vector3 targetPosition;
        [SerializeField] private float moveSpeed = 5f;

        public bool TryMove(Vector3 direction)
        {
            // 检查前方是否有障碍物或其他箱子
            Vector3 nextPos = transform.position + direction;
            if (Physics.CheckBox(nextPos, Vector3.one * 0.4f, Quaternion.identity, 
                LayerMask.GetMask("Wall", "Box")))
            {
                return false;
            }

            // 移动箱子
            targetPosition = nextPos;
            isMoving = true;
            EventCenter.GetInstance().EventTrigger("BoxMove");
            return true;
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
                    GameManager.GetInstance()?.CheckWinCondition();
                }
            }
        }

        public void ResetState()
        {
            isMoving = false;
            targetPosition = transform.position;
        }
    }
} 