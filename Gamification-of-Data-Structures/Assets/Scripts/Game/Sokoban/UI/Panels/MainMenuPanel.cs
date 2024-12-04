using UnityEngine;
using UnityEngine.UI;

namespace Sokoban
{
    public class MainMenuPanel : BasePanel
    {
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnClick(string name)
        {
            switch (name)
            {
                case "StartButton":
                    GameManager.GetInstance().StartLevel(1);
                    UIManager.GetInstance().HidePanel("MainMenuPanel");
                    UIManager.GetInstance().ShowPanel<GamePanel>("GamePanel");
                    break;
                case "LevelSelectButton":
                    UIManager.GetInstance().HidePanel("MainMenuPanel");
                    UIManager.GetInstance().ShowPanel<LevelSelectPanel>("LevelSelectPanel");
                    break;
                case "QuitButton":
                    Application.Quit();
                    break;
            }
        }
    }
} 