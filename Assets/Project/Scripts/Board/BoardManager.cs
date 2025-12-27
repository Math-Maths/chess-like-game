using System.Collections.Generic;
using ChessGame;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] SpriteRenderer tileSelectionPrefab;
    [SerializeField] Sprite previewMoveSprite;
    [SerializeField] Sprite selectionSprite;

    private Piece _selectedPiece;
    private BoardTile _selectedTile, _tileToMove;
    private SpriteRenderer _selectionInstance;
    private List<SpriteRenderer> _previewInstances = new List<SpriteRenderer>();
    private List<BoardCreator.Coordinate> validMoves;

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
        if(_selectedTile == tile)
        {
            _selectedTile = null;
            _selectedPiece = null;
            _selectionInstance.gameObject.SetActive(false);
            ClearMovePreviews();
            selectedPiece = null;
            return;
        }

        _selectedTile = tile;
        _selectedPiece = _selectedTile.GetOccupyingPiece();

        // Only allow selection of white pieces
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
    }

    public void TryMoveSelectedPiece(BoardTile selectedBoardTile, BoardTile targetTile)
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
        validMoves = MoveValidator.PreviewValidMoves(tile);
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

    void ClearMovePreviews()
    {
        foreach (var preview in _previewInstances)
        {
            Destroy(preview.gameObject);
        }
        _previewInstances.Clear();
    }

}