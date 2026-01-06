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
    private List<BoardCreator.Coordinate> validMoves;
    private List<BoardCreator.Coordinate> validAttacks;

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
                Debug.Log("Secondary attack triggered!");
                ClearMovePreviews();
                ShowSecondaryPreview(tile);
                _selectedPiece.ToggleSecondaryAttack();
                selectedPiece = _selectedTile;
                return;
            }

            _selectedPiece.ToggleSecondaryAttack();
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

            TryMoveSelectedPiece(_selectedTile, tile);
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
        _selectedTile = tile;
        _selectedPiece = _selectedTile.GetOccupyingPiece();

        if(_selectedPiece.Side != GameManager.Instance.CurrentTurn) 
        {
            _selectedTile = null;
            _selectedPiece = null;
            selectedPiece = null;
            return;
        }

        Vector3 selectionPosition = BoardCreator.Instance.CoordinateToPosition(_selectedTile.XCoord, _selectedTile.YCoord);
        _selectionInstance.transform.position = selectionPosition;
        _selectionInstance.gameObject.SetActive(true);
        selectedPiece = _selectedTile;
        ClearMovePreviews();
        ShowPieceMovePreview(_selectedTile);
        ShowPieceAttackPreview(_selectedTile);
    }

    void TryMoveSelectedPiece(BoardTile selectedBoardTile, BoardTile targetTile)
    {
        if(selectedBoardTile == null || targetTile == null)
            return;

        if(validMoves == null || validMoves.Count == 0)
            return;

        if(validMoves.Contains(new BoardCreator.Coordinate(targetTile.XCoord, targetTile.YCoord)))
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

                // Clear selection and previews
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
        
    }

    void ShowPieceMovePreview(BoardTile tile)
    {
        validMoves = Validator.PreviewValidMoves(tile);
        foreach (var move in validMoves)
        {
            Vector3 previewPosition = BoardCreator.Instance.CoordinateToPosition(move.x, move.y);
            SpriteRenderer previewInstance = Instantiate(tileSelectionPrefab);
            previewInstance.sprite = previewMoveSprite;
            previewInstance.sortingOrder = 1;
            previewInstance.transform.position = previewPosition;
            _previewInstances.Add(previewInstance);
        }
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
            validMoves.Add(move);
        }
    }

    void ShowSecondaryPreview(BoardTile tile)
    {   
        List<BoardCreator.Coordinate> possibleAttack = new List<BoardCreator.Coordinate>();
        var attackRoutePreview = Validator.PreviewProjectile(tile, out possibleAttack);

        foreach(var preview in attackRoutePreview)
        {
            Vector3 previewPosition = BoardCreator.Instance.CoordinateToPosition(preview.x, preview.y);
            SpriteRenderer previewInstance = Instantiate(tileSelectionPrefab);
            previewInstance.sprite = previewAttackRouteSprite;
            previewInstance.sortingOrder = 1;
            previewInstance.transform.position = previewPosition;
            _previewInstances.Add(previewInstance);
        }

        foreach(var previewAttack in possibleAttack)
        {
            Vector3 previewPosition = BoardCreator.Instance.CoordinateToPosition(previewAttack.x, previewAttack.y);
            SpriteRenderer previewInstance = Instantiate(tileSelectionPrefab);
            previewInstance.sprite = previewAttackSprite;
            previewInstance.sortingOrder = 1;
            previewInstance.transform.position = previewPosition;
            _previewInstances.Add(previewInstance);
        }
    }

    void ClearMovePreviews()
    {
        foreach (var preview in _previewInstances)
        {
            Destroy(preview.gameObject);
        }
        _previewInstances.Clear();
        validMoves = null;
    }

}