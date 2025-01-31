using CGD.Case;
using UnityEngine.Events;

namespace CGD.Gameplay
{
    public class GameMenuManager : MenuManager
    {
        /// <summary>
        /// Casts Base static instance to this class
        /// </summary>
        public new static GameMenuManager Instance
        {
            get
            {
                return (GameMenuManager)_instance;
            }
        }

        private void OnEnable()
        {
            GameManagerEvents.OnGameStateChanged += GameStateChanged;
        }
        private void OnDestroy()
        {
            GameManagerEvents.OnGameStateChanged -= GameStateChanged;
        }

        private void GameStateChanged(GameState state) 
        {
            switch (state) 
            {
                case GameState.Countdown:
                    OpenMenu("countdown");
                    break;
                case GameState.Start:
                    OpenMenu("hud");
                    break;
                case GameState.Meeting:
                    OpenMenu("board");
                    break;
                case GameState.Finished:
                    OpenMenu("gameend");
                    break;
            }
        }

        public void OpenCluePanel(Clue clue) 
        {
            var cluePanel = GetMenu("clue");

            if (cluePanel != null)
            {
                ((CluePanel)cluePanel).SetPanel(clue.icon, clue.analysedDescription);
                OpenMenu("clue");
            }

        }

        public void CloseMenu() => OpenMenu("");
    }
}
