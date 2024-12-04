using UnityEngine;

namespace Sokoban
{
    public class WinPanel : BasePanel
    {
        protected override void OnClick(string name)
        {
            switch (name)
            {
                case "NextLevelButton":
                    UIManager.GetInstance().HidePanel("WinPanel");
                    GameManager.GetInstance().StartLevel(GameManager.GetInstance().CurrentLevel + 1);
                    break;
                case "RestartButton":
                    UIManager.GetInstance().HidePanel("WinPanel");
                    GameManager.GetInstance().ResetCurrentLevel();
                    break;
                case "MainMenuButton":
                    UIManager.GetInstance().HidePanel("WinPanel");
                    UIManager.GetInstance().HidePanel("GamePanel");
                    UIManager.GetInstance().ShowPanel<MainMenuPanel>("MainMenuPanel");
                    break;
            }
        }
    }
} 