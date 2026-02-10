using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

namespace ChessGame.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private DataManager _dataManager;
        private EventManager _eventManager;
        private GameStatus _gameStatus;

        [SerializeField] private PieceDatabaseSO allPieces;
        [SerializeField] private TeamSO playerTeam;

        public GameState CurrentGameState { get; private set; } = GameState.Gameplay;
        public TeamSO PlayerTeam => playerTeam;

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

            _dataManager = GetComponent<DataManager>();
            _eventManager = GetComponent<EventManager>();

            LoadData();
        }

        private void Start()
        {
            BindEvents();
        }
        
        private void BindEvents()
        {
            _eventManager.AddListener<GameState>(EventNameSaver.OnStateChange, ChangeGameState);
        }

        private void OnDisable()
        {
            _eventManager.RemoveListener<GameState>(EventNameSaver.OnStateChange, ChangeGameState);
        }

        private void LoadData()
        {
            SaveData data = _dataManager.Load();

            _gameStatus = new GameStatus();

            if(data == null)
            {
                _gameStatus.playerName = "player";
                _gameStatus.currentLevel = 0;
                _gameStatus.unlockedPiecesIDS = new List<string>();
                _gameStatus.team = new List<TeamPiecesData>();
                return;
            }
            
            _gameStatus.playerName = data.playerName;
            _gameStatus.currentLevel = data.currentLevel;
            _gameStatus.unlockedPiecesIDS = data.unlockedPiecesID;
            _gameStatus.team = data.team;

            LoadTeamIntoSO(playerTeam);
        }

        private void LoadTeamIntoSO(TeamSO team)
        {
            TeamPiecesData[] rebuilt = new TeamPiecesData[_gameStatus.team.Count];

            for(int i = 0; i < rebuilt.Length; i++)
            {
                var savedPiece = _gameStatus.team[i];

                rebuilt[i] = new TeamPiecesData
                {
                    pieceIndexPosition = savedPiece.pieceIndexPosition,
                    pieceID = savedPiece.pieceID  
                };
            }

            team.teamPieces = rebuilt;
        }

        public void SaveTeam(TeamSO team)
        {
            _gameStatus.team.Clear();

            foreach(var pieceData in team.teamPieces)
            {
                TeamPiecesData savePiece = new TeamPiecesData
                {
                    pieceIndexPosition = pieceData.pieceIndexPosition,
                    pieceID = pieceData.pieceID
                };

                _gameStatus.team.Add(savePiece);
            }

            SaveData(_gameStatus);
        }

        private void SaveData(GameStatus _data)
        {
            SaveData data = new SaveData
            {
                playerName = _data.playerName,
                currentLevel = _data.currentLevel,
                unlockedPiecesID = _data.unlockedPiecesIDS,
                team = _data.team
            };

            _dataManager.Save(data);
        }

        private void ChangeGameState(GameState state)
        {
            if(state == GameState.GameOver)
                _canChangeState = false;

            if(_canChangeState)
                CurrentGameState = state;
        }

        public PieceTypeSO GetPieceByID(string id)
        {
            return allPieces.GetPieceByID(id);
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

    public class GameStatus
    {
        public string playerName;
        public int currentLevel;
        public List<string> unlockedPiecesIDS;
        public List<TeamPiecesData> team;
    }
}