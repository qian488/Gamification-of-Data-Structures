using UnityEngine;

namespace Sokoban
{
    public class AudioManager : SingletonMono<AudioManager>
    {
        protected override void Awake()
        {
            base.Awake();
            // 注册事件监听
            EventCenter.GetInstance().AddEventListener("PlayerMove", OnPlayerMove);
            EventCenter.GetInstance().AddEventListener("BoxMove", OnBoxMove);
            EventCenter.GetInstance().AddEventListener("LevelComplete", OnLevelComplete);
        }

        private void OnPlayerMove()
        {
            MusicManager.GetInstance().PlaySFX("PlayerMove", false);
        }

        private void OnBoxMove()
        {
            MusicManager.GetInstance().PlaySFX("BoxMove", false);
        }

        private void OnLevelComplete()
        {
            MusicManager.GetInstance().PlaySFX("Win", false);
        }

        private void OnDestroy()
        {
            EventCenter.GetInstance().RemoveEventListener("PlayerMove", OnPlayerMove);
            EventCenter.GetInstance().RemoveEventListener("BoxMove", OnBoxMove);
            EventCenter.GetInstance().RemoveEventListener("LevelComplete", OnLevelComplete);
        }
    }
} 