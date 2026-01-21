using System.Collections.Generic;
using ChessGame;
using UnityEngine;
using UnityEngine.WSA;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

    [SerializeField] SpriteRenderer tileSelectionPrefab;
    [SerializeField] Sprite previewMoveSprite;
    [SerializeField] Sprite previewAttackSprite;
    [SerializeField] Sprite previewAttackRouteSprite;
    [SerializeField] Sprite selectionSprite;

    private BasePiece _selectedPiece;
    private BoardTile _selectedTile;
    private SpriteRenderer _selectionInstance;
    private List<SpriteRenderer> _previewInstances = new List<SpriteRenderer>();
    private List<BoardCreator.Coordinate> _validMoves;
    private List<BoardCreator.Coordinate> _validAttacks;
    private SelectionState _currentState;

    public SelectionState CurrentState => _currentState;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _selectionInstance = Instantiate(tileSelectionPrefab);
        _selectionInstance.sprite = selectionSprite;
        _selectionInstance.sortingOrder = 2;
        _selectionInstance.gameObject.SetActive(false);
    }

    public void HandleSelection(BoardTile tile, out BoardTile selectedPiece)
    {

        //Debug.Log(_currentState);
        // Deselect if the same tile is selected again
        if(_selectedTile == tile && _selectedPiece != null)
        {
            if(_currentState == SelectionState.SecondaryMove)
            {
                selectedPiece = _selectedTile;
                return;
            }

            if(_selectedPiece.Type.hasSecondaryAttack && !_selectedPiece.SecondaryAttackUsed)
            {
                //Debug.Log("Secondary attack triggered!");
                ClearMovePreviews();
                ShowRangeAttackPreview(tile);
                _selectedPiece.ToggleSecondaryAttack(true);
                selectedPiece = _selectedTile;
                return;
            }

            _selectedPiece.ToggleSecondaryAttack(false);
            _selectedTile = null;
            _selectedPiece = null;
            _selectionInstance.gameObject.SetActive(false);
            ClearMovePreviews();
            selectedPiece = null;
            return;
        }
        else if(_selectedTile != tile && _selectedPiece != null)
        {
            if(tile.GetOccupyingPiece() != _selectedPiece && 
                tile.GetOccupyingPiece() != null && 
                tile.GetOccupyingPiece().Side == _selectedPiece.Side &&
                _currentState != SelectionState.SecondaryMove)
            {
                SelectPieceOnTile(tile, out selectedPiece);
                return;
            }

            if(_currentState == SelectionState.Move)
            {
                TryMoveSelectedPiece(_selectedTile, tile);
            }
            else if(_currentState == SelectionState.Attack)
            {
                TryAttackSelectedPiece(_selectedTile, tile);
            }
            else if(_currentState == SelectionState.SecondaryMove)
            {
                TryMoveSelectedPiece(_selectedTile, tile);
                selectedPiece = _selectedTile;
                return;
            }

            _selectedTile = null;
            _selectedPiece = null;
            _selectionInstance.gameObject.SetActive(false);
            ClearMovePreviews();
            selectedPiece = null;
            return;
        }

        SelectPieceOnTile(tile, out selectedPiece);
    }

    void SelectPieceOnTile(BoardTile tile, out BoardTile selectedPiece)
    {
        if(tile.GetOccupyingPiece().Side != GameManager.Instance.CurrentTurn)
        {
            selectedPiece = null;
            return;
        }

        if(_currentState == SelectionState.SecondaryMove)
        {
            selectedPiece = _selectedTile;
            return;
        }

        _selectedTile = tile;
        _selectedPiece = _selectedTile.GetOccupyingPiece();

        Vector3 selectionPosition = BoardCreator.Instance.CoordinateToPosition(_selectedTile.XCoord, _selectedTile.YCoord);
        _selectionInstance.transform.position = selectionPosition;
        _selectionInstance.gameObject.SetActive(true);
        _selectedPiece.ToggleSecondaryAttack(false);
        selectedPiece = _selectedTile;
        ClearMovePreviews();
        ShowPieceMovePreview(_selectedTile);
        ShowPieceAttackPreview(_selectedTile);
    }

    void TryAttackSelectedPiece(BoardTile selectedBoardTile, BoardTile targetTile)
    {
        if(selectedBoardTile == null || targetTile == null)
            return;

        if(_validAttacks == null || _validAttacks.Count == 0)
            return;

        if(_validAttacks.Contains(new BoardCreator.Coordinate(targetTile.XCoord, targetTile.YCoord)))
        {
            //Debug.Log("Range Attack");
            BasePiece attacker = selectedBoardTile.GetOccupyingPiece();
            attacker.RangeAttack(targetTile);

            _selectedTile = null;
            _selectedPiece = null;
            _selectionInstance.gameObject.SetActive(false);
            ClearMovePreviews();
        }

        _currentState = SelectionState.None;
    }

    void TryMoveSelectedPiece(BoardTile selectedBoardTile, BoardTile targetTile)
    {
        if(selectedBoardTile == null || targetTile == null)
            return;

        if(_validMoves == null || _validMoves.Count == 0)
            return;

        if(_validMoves.Contains(new BoardCreator.Coordinate(targetTile.XCoord, targetTile.YCoord)))
        {
            BasePiece pieceToMove = selectedBoardTile.GetOccupyingPiece();
            //Debug.Log($"Moving from ({selectedBoardTile.XCoord}, {selectedBoardTile.YCoord}) to ({targetTile.XCoord}, {targetTile.YCoord})");
            BoardCreator.Coordinate[] movePath = PathCreator.CreatePath(
                new BoardCreator.Coordinate(selectedBoardTile.XCoord, selectedBoardTile.YCoord),
                new BoardCreator.Coordinate(targetTile.XCoord, targetTile.YCoord)
            );

            if(pieceToMove != null)
            {
                pieceToMove.StartMove(movePath);
            
                selectedBoardTile.RemovePiece();

                _selectionInstance.gameObject.SetActive(false);
                ClearMovePreviews();
            }
        }
        else if (_currentState == SelectionState.SecondaryMove)
        {
            //Debug.Log("Invalid secondary move selected.");
            return;
        }
        else
        {
            ClearMovePreviews();
            _selectedTile = null;
            _selectedPiece = null;
            _selectionInstance.gameObject.SetActive(false);
            
        }

        _currentState = SelectionState.None;
        
    }

    public List<BoardCreator.Coordinate> ShowPieceMovePreview(BoardTile tile, bool seconderyMove = false)
    {
        _validMoves = Validator.PreviewValidMoves(tile, seconderyMove);
        foreach (var move in _validMoves)
        {
            Vector3 previewPosition = BoardCreator.Instance.CoordinateToPosition(move.x, move.y);
            SpriteRenderer previewInstance = Instantiate(tileSelectionPrefab);
            previewInstance.sprite = previewMoveSprite;
            previewInstance.sortingOrder = 1;
            previewInstance.transform.position = previewPosition;
            _previewInstances.Add(previewInstance);
        }

        _currentState = SelectionState.Move;

        return _validMoves;
    }

    void ShowPieceAttackPreview(BoardTile tile)
    {
        var attackMoves = Validator.CheckPossibleAttacks(tile);
        foreach (var move in attackMoves)
        {
            Vector3 previewPosition = BoardCreator.Instance.CoordinateToPosition(move.x, move.y);
            SpriteRenderer previewInstance = Instantiate(tileSelectionPrefab);
            previewInstance.sprite = previewAttackSprite;
            previewInstance.sortingOrder = 1;
            previewInstance.transform.position = previewPosition;
            _previewInstances.Add(previewInstance);
            _validMoves.Add(move);
        }
    }

    void ShowRangeAttackPreview(BoardTile tile)
    {   
        var attackRoutePreview = Validator.PreviewProjectile(tile, out _validAttacks);

        foreach(var preview in attackRoutePreview)
        {
            Vector3 previewPosition = BoardCreator.Instance.CoordinateToPosition(preview.x, preview.y);
            SpriteRenderer previewInstance = Instantiate(tileSelectionPrefab);
            previewInstance.sprite = previewAttackRouteSprite;
            previewInstance.sortingOrder = 1;
            previewInstance.transform.position = previewPosition;
            _previewInstances.Add(previewInstance);
        }

        foreach(var previewAttack in _validAttacks)
        {
            Vector3 previewPosition = BoardCreator.Instance.CoordinateToPosition(previewAttack.x, previewAttack.y);
            SpriteRenderer previewInstance = Instantiate(tileSelectionPrefab);
            previewInstance.sprite = previewAttackSprite;
            previewInstance.sortingOrder = 1;
            previewInstance.transform.position = previewPosition;
            _previewInstances.Add(previewInstance);
        }

        _currentState = SelectionState.Attack;
    }

    public void SetCurrentState(SelectionState state)
    {
        _currentState = state;
    }

    void ClearMovePreviews()
    {
        foreach (var preview in _previewInstances)
        {
            Destroy(preview.gameObject);
        }
        _validAttacks = null;
        _previewInstances.Clear();
        _validMoves = null;
        _currentState = SelectionState.None;
    }

    public void SetSelectedPiece(BasePiece piece)
    {
        _selectedPiece = piece;
        _selectedTile = piece.OccupyingTile;
    }

}

public enum SelectionState
{
    Move,
    SecondaryMove,
    Attack,
    None
}