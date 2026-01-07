using System.Collections.Generic;
using ChessGame;
using UnityEngine;
using UnityEngine.WSA;

public class BoardManager : MonoBehaviour
{
    [SerializeField] SpriteRenderer tileSelectionPrefab;
    [SerializeField] Sprite previewMoveSprite;
    [SerializeField] Sprite previewAttackSprite;
    [SerializeField] Sprite previewAttackRouteSprite;
    [SerializeField] Sprite selectionSprite;

    private Piece _selectedPiece;
    private BoardTile _selectedTile;
    private SpriteRenderer _selectionInstance;
    private List<SpriteRenderer> _previewInstances = new List<SpriteRenderer>();
    private List<BoardCreator.Coordinate> _validMoves;
    private List<BoardCreator.Coordinate> _validAttacks;
    private SelectionState _currentState;

    void Start()
    {
        _selectionInstance = Instantiate(tileSelectionPrefab);
        _selectionInstance.sprite = selectionSprite;
        _selectionInstance.sortingOrder = 2;
        _selectionInstance.gameObject.SetActive(false);
    }

    public void HandleSelection(BoardTile tile, out BoardTile selectedPiece)
    {
        // Deselect if the same tile is selected again
        if(_selectedTile == tile && _selectedPiece != null)
        {
            if(_selectedPiece.Type.hasSecondaryAttack && !_selectedPiece.SecondaryAttackUsed)
            {
                //Debug.Log("Secondary attack triggered!");
                ClearMovePreviews();
                ShowSecondaryPreview(tile);
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
                tile.GetOccupyingPiece().Side == _selectedPiece.Side)
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

        _selectedTile = tile;
        _selectedPiece = _selectedTile.GetOccupyingPiece();

        // if(_selectedPiece.Side != GameManager.Instance.CurrentTurn) 
        // {
        //     _selectedTile = null;
        //     _selectedPiece = null;
        //     selectedPiece = null;
        //     return;
        // }

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
            Debug.Log("Range Attack");
            Piece attacker = selectedBoardTile.GetOccupyingPiece();
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
            Piece pieceToMove = selectedBoardTile.GetOccupyingPiece();
            //Debug.Log($"Moving from ({selectedBoardTile.XCoord}, {selectedBoardTile.YCoord}) to ({targetTile.XCoord}, {targetTile.YCoord})");
            BoardCreator.Coordinate[] movePath = PathCreator.CreatePath(
                new BoardCreator.Coordinate(selectedBoardTile.XCoord, selectedBoardTile.YCoord),
                new BoardCreator.Coordinate(targetTile.XCoord, targetTile.YCoord)
            );

            if(pieceToMove != null)
            {
                pieceToMove.StartMove(movePath);
            
                selectedBoardTile.RemovePiece();

                _selectedTile = null;
                _selectedPiece = null;
                _selectionInstance.gameObject.SetActive(false);
                ClearMovePreviews();

                // Notify GameManager to switch turns
                //GameManager.Instance.EndPlayerTurn();
            }
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

    void ShowPieceMovePreview(BoardTile tile)
    {
        _validMoves = Validator.PreviewValidMoves(tile);
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

    void ShowSecondaryPreview(BoardTile tile)
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

}

public enum SelectionState
{
    Move,
    Attack,
    None
}