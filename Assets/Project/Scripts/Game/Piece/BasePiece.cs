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
    }

    public bool TryMove(Vector2Int targetPos)
    {
        if (MovementValidator.IsValidMove(this, targetPos, board))
        {
            board.MovePiece(this, targetPos);
            return true;
        }

        return false;
    }
}
