using System.Collections.Generic;
using ChessGame.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChessGame.UI
{
    public class TeamSelectionManager : MonoBehaviour
    {
        public static TeamSelectionManager Instance {get; private set;}

        [SerializeField] private Canvas _canvas;
        [SerializeField] private GameObject scrollContent;
        [SerializeField] private TMP_Text pieceCategoryText;
        [SerializeField] private PlayerPieceTeam[] tilePosition;

        private Dictionary<PieceTypeSO.PieceCategory, List<GameObject>> _piecesByCategory = new Dictionary<PieceTypeSO.PieceCategory, List<GameObject>>();
        private Dictionary<string,bool> unlockedPieces = new Dictionary<string, bool>();

        private TeamSO _playerTeam;
        private TeamSO _tempTeam;
        private PieceDatabaseSO _pieceDatabase;

        private void Awake()
        {
            if(Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void Initialize(List<string> _ids)
        {
            EventManager.Instance.AddListener<PlayerPieceTeam>(EventNameSaver.OnTeamPieceSelected, OnNewSelection);
            //Initialize the Dictionary
            foreach (PieceTypeSO.PieceCategory category in System.Enum.GetValues(typeof(PieceTypeSO.PieceCategory)))
            {
                if(!_piecesByCategory.ContainsKey(category))
                    _piecesByCategory[category] = new List<GameObject>();
            }

            unlockedPieces.Clear();
            foreach(string pieceID in _ids)
            {
                if(!unlockedPieces.ContainsKey(pieceID))
                    unlockedPieces.Add(pieceID, true);
            }

            _playerTeam = GameManager.Instance.PlayerTeam;
            _tempTeam = _playerTeam;

            if(_pieceDatabase != GameManager.Instance.GetAllPiecesDatabase())
            {
                _pieceDatabase = GameManager.Instance.GetAllPiecesDatabase();
                InstantiateAvaiblePieces();
            }

            InstantiatePlayerTeam(_playerTeam);
        }

        private void InstantiatePlayerTeam(TeamSO _playerTeam)
        {
            for(int i = 0; i < _playerTeam.teamPieces.Length; i ++)
            {
                PieceTypeSO pieceToGo = GameManager.Instance.GetPieceByID(_playerTeam.teamPieces[i].pieceID);
                tilePosition[i].Initialize(pieceToGo, i);
            }

            EventManager.Instance.Invoke<PlayerPieceTeam>(EventNameSaver.OnTeamPieceSelected, tilePosition[0]);
            ShowOnlyCategory(tilePosition[0].Type.category);
        }

        private void InstantiateAvaiblePieces()
        {
            for(int i = 0; i < _pieceDatabase.allPieces.Count; i++)
            {
                GameObject newPiece = CreatedImage(_pieceDatabase.allPieces[i]);
                RegisterPieceUI(_pieceDatabase.allPieces[i].category, newPiece);
            }
        }

        private GameObject CreatedImage(PieceTypeSO piece)
        {
            GameObject imageGO = new GameObject(piece.name);
            imageGO.transform.SetParent(scrollContent.transform, false);

            RectTransform imageRect = imageGO.AddComponent<RectTransform>();
            imageRect.sizeDelta = new Vector2(100, 100);
            imageRect.anchoredPosition = Vector2.zero;

            imageGO.AddComponent<Image>();

            AvailablePieceSelection pieceSelection = imageGO.AddComponent<AvailablePieceSelection>();
            pieceSelection.Initialize(piece, unlockedPieces.ContainsKey(piece.id));

            return imageGO;
        }

        private void RegisterPieceUI(PieceTypeSO.PieceCategory _category, GameObject _piece)
        {
            _piecesByCategory[_category].Add(_piece);
        }

        private void ShowOnlyCategory(PieceTypeSO.PieceCategory _showCategory)
        {
            foreach(var piece in _piecesByCategory)
            {
                bool show = piece.Key == _showCategory;

                piece.Value.ForEach(go => go.SetActive(show));
            }

            pieceCategoryText.text = _showCategory.ToString();
        }

        private void OnNewSelection(PlayerPieceTeam piece)
        {
            var category = piece.Type.category;
            ShowOnlyCategory(category);
        }

        public void ChangePlayerPiece(PieceTypeSO selectedPiece, int index)
        {
            _tempTeam.teamPieces[index].pieceID = selectedPiece.id; 
        }

        public void SaveNewTeam()
        {
            _playerTeam = _tempTeam;
            GameManager.Instance.SaveTeam(_playerTeam);
            EventManager.Instance.Invoke(EventNameSaver.OnMainMenuOpen);
        }

        public void CancelChanges()
        {
            _tempTeam = _playerTeam;
            EventManager.Instance.Invoke(EventNameSaver.OnMainMenuOpen);
        }
    }
}