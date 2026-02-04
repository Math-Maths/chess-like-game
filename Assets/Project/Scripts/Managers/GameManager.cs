using UnityEngine;

namespace ChessGame.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState CurrentGameState { get; private set; } = GameState.Gameplay;

        private bool _canChangeState = true;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            BindEvents();
        }
        
        private void BindEvents()
        {
            EventManager.Instance.AddListener<GameState>(EventNameSaver.OnStateChange, ChangeGameState);
        }

        private void OnDisable()
        {
            EventManager.Instance.RemoveListener<GameState>(EventNameSaver.OnStateChange, ChangeGameState);
        }

        private void ChangeGameState(GameState state)
        {
            if(state == GameState.GameOver)
                _canChangeState = false;

            if(_canChangeState)
                CurrentGameState = state;
        }
    }

    public enum GameState
    {
        TeamSelection,
        Gameplay,
        Paused,
        Busy,
        GameOver
    }
}