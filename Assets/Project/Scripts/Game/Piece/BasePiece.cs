using UnityEngine;

public class BasePiece : MonoBehaviour
{
    public PieceDefinitionSO definition;
    public Vector2Int gridPos;

    private BoardCreator board;     

    public void Initialize(BoardCreator board, Vector2Int startPos)
    {
        this.board = board;
        gridPos = startPos;
        transform.position = board.GetWorldPosition(startPos.x, startPos.y);
        board.PlacePiece(this, startPos);
    }

    public void UpdatePiecePlace(Vector2Int newPos)
    {
        gridPos = newPos;
        transform.position = board.GetWorldPosition(newPos.x, newPos.y);
        board.PlacePiece(this, newPos);
    }

    public bool TryMove(Tile targetTile)
    {
        if (MovementValidator.IsValidMove(targetTile, this))
        {
            board.MovePiece(this, targetTile.GridPosition);
            UpdatePiecePlace(targetTile.GridPosition);
            return true;
        }

        return false;
    }
}
