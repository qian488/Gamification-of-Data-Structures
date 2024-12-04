using UnityEngine;
using UnityEngine.UI;

namespace Sokoban
{
    public class GamePanel : BasePanel
    {
        private Text stepCountText;

        protected override void Awake()
        {
            base.Awake();
            stepCountText = GetUIComponent<Text>("StepCountText");
            
            EventCenter.GetInstance().AddEventListener("PlayerMove", OnPlayerMove);
            EventCenter.GetInstance().AddEventListener("LevelReset", OnLevelReset);
        }

        protected override void OnClick(string name)
        {
            switch (name)
            {
                case "PauseButton":
                    UIManager.GetInstance().ShowPanel<PausePanel>("PausePanel", E_UI_Layer.Top);
                    Time.timeScale = 0;
                    break;
                case "ResetButton":
                    GameManager.GetInstance().ResetCurrentLevel();
                    break;
            }
        }

        private void OnPlayerMove()
        {
            stepCountText.text = $"Steps: {GameManager.GetInstance().CurrentSteps}";
        }

        private void OnLevelReset()
        {
            stepCountText.text = $"Steps: {GameManager.GetInstance().CurrentSteps}";
        }

        private void OnDestroy()
        {
            EventCenter.GetInstance().RemoveEventListener("PlayerMove", OnPlayerMove);
            EventCenter.GetInstance().RemoveEventListener("LevelReset", OnLevelReset);
        }
    }
} 