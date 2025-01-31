using CGD.Gameplay;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CGD.MiniGames
{
    public enum GameState 
    {
        Loading = 0,
        Start = 1,
        Won = 2,
        Lost = 3
    }

    public abstract class MiniGame : MonoBehaviour
    {
        /// <summary>
        /// current state of minigame
        /// </summary>
        private GameState gameState;

        /// <summary>
        /// number of stages needed to complete minigame
        /// </summary>
        private int stages;

        /// <summary>
        /// reference to time left
        /// </summary>
        private float timer;

        /// <summary>
        /// seconds to complete minigame
        /// </summary>
        [SerializeField] private float timeToComplete;

        /// <summary>
        /// icon for displaying time left
        /// </summary>
        [SerializeField] private Image turnTimerImg;

        /// <summary>
        /// displays status message
        /// </summary>
        [SerializeField] private TextMeshProUGUI statusTMP;

        #region Properties  
        public GameState GameState
        {
            get
            {
                return gameState;
            }
            private set
            {
                gameState = value;
                //GameManagerEvents.OnGameStateChanged?.Invoke(value);
            }
        }
        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            GameManagerEvents.OnGameStateChanged += GameStateChanged;
            InitialiseMiniGame();
            StartCoroutine(MiniGameTimer());
        }
        private void OnDestroy()
        {
            GameManagerEvents.OnGameStateChanged -= GameStateChanged;
        }
        private void Update()
        {
            if(UnityEngine.Input.GetKeyDown(KeyCode.I)) { ForceWin(); }
            if(UnityEngine.Input.GetKeyDown(KeyCode.O)) { ForceLoss(); }
        }
        IEnumerator MiniGameTimer() 
        {
            while(timer < timeToComplete) 
            {
                yield return new WaitForSecondsRealtime(1);

                timer += 1;
                turnTimerImg.fillAmount = 1f - (timer/ timeToComplete);
            }

            if (gameState < GameState.Won) MiniGameFinished(false);
        }
        #endregion

        #region MiniGame
        protected virtual void InitialiseMiniGame() 
        {
        
        }

        protected virtual void CheckPuzzleSolved() { }

        protected void MiniGameFinished(bool won) 
        {
            statusTMP.text = won ? "Puzzle Solved!" : "<color=red>Puzzle Failed</color>";
            GameState = won ? GameState.Won : GameState.Lost;
            Invoke("Finish", 2);
        }

        protected void Finish() 
        {
            if(MiniGameManager.Instance != null) 
            {
                MiniGameManager.Instance.UnloadMiniGame(GameState == GameState.Won);
            }
        }
        #endregion

        #region Callbacks
        private void GameStateChanged(CGD.Gameplay.GameState state) { }
        #endregion

        #region Debug
        private void ForceWin() => MiniGameFinished(true);
        private void ForceLoss() => MiniGameFinished(false);
        #endregion

    }
}
