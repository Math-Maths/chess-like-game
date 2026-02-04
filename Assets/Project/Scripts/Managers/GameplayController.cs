using ChessGame.UI;
using UnityEngine;

namespace ChessGame.Managers
{
    public class GameplayController : MonoBehaviour
    {
        public static GameplayController Instance { get; private set; }

        public PieceSide CurrentTurn { get; private set; }

        [Header("Prefabs References")]
        [SerializeField] private InputHandler _inputHandler;
        [SerializeField] private BoardCreator _boardCreator;
        [SerializeField] private GameplayUIManager _uiManager;
        [SerializeField] private Camera _mainCamera;

        private async void Start()
        {
            BindEvents();
            BindObjects();
            //Show loading screen
            await InitialiazeObjects();
            //Get GameManager Data
            PrepareGame();
            //Hide Loading Screen
            //Start Game
        }

        private void BindEvents()
        {
            EventManager.Instance.AddListener(EventNameSaver.OnTurnChange, ToggleTurn);
            EventManager.Instance.AddListener(EventNameSaver.OnGameOver, EndGame);
        }

        private void OnDisable()
        {
            EventManager.Instance.RemoveListener(EventNameSaver.OnTurnChange, ToggleTurn);
            EventManager.Instance.RemoveListener(EventNameSaver.OnGameOver, EndGame);
        }

        private void BindObjects()
        {
            if(Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            _inputHandler = Instantiate(_inputHandler);
            _boardCreator = Instantiate(_boardCreator);
            _uiManager = Instantiate(_uiManager);
            _mainCamera = Instantiate(_mainCamera);
        }

        private async Awaitable InitialiazeObjects()
        {
            _boardCreator.Initialize();
            _inputHandler.Initialize();
            _uiManager.Initialize();
        }

        private void PrepareGame()
        {
            _boardCreator.PrepareBoard(1);
            _inputHandler.PrepareInput();
            CurrentTurn = PieceSide.Player;
        }

        private void ToggleTurn()
        {
            CurrentTurn = CurrentTurn == PieceSide.Player ? PieceSide.Enemy : PieceSide.Player;
        }

        private void EndGame()
        {
            EventManager.Instance.Invoke(EventNameSaver.OnStateChange, GameState.GameOver);
        }
    }

    public enum PieceSide
    {
        Player,
        Enemy,
        None
    }
}