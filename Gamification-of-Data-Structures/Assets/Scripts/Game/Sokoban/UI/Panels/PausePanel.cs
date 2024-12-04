using UnityEngine;

namespace Sokoban
{
    public class PausePanel : BasePanel
    {
        protected override void OnClick(string name)
        {
            switch (name)
            {
                case "ResumeButton":
                    UIManager.GetInstance().HidePanel("PausePanel");
                    Time.timeScale = 1;
                    break;
                case "MainMenuButton":
                    Time.timeScale = 1;
                    UIManager.GetInstance().HidePanel("PausePanel");
                    UIManager.GetInstance().HidePanel("GamePanel");
                    UIManager.GetInstance().ShowPanel<MainMenuPanel>("MainMenuPanel");
                    break;
            }
        }
    }
} 