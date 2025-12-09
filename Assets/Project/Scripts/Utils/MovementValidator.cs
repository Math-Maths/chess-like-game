using UnityEngine;

public static class MovementValidator
{
    public static bool IsValidMove(BasePiece piece, Vector2Int targetPos, BoardCreator board)
    {
        Vector2Int start = piece.gridPos;
        Vector2Int delta = targetPos - start;

        // 1) Inside board?
        if (!board.IsInsideBoard(targetPos))
            return false;

        // 2) If there's another piece there, can't move
        if (board.GetPieceAt(targetPos) != null)
            return false;

        // 3) Check if delta matches a valid direction
        foreach (var dir in piece.definition.moveDirections)
        {
            // Same direction?
            if (IsSameDirection(delta, dir))
            {
                int dist = Mathf.Abs(delta.x != 0 ? delta.x : delta.y);

                // Unlimited movement?
                if (piece.definition.maxMoveDistance < 0)
                    return true;

                // Limited movement?
                if (dist <= piece.definition.maxMoveDistance)
                    return true;
            }
        }

        return false;
    }

    private static bool IsSameDirection(Vector2Int delta, Vector2Int dir)
    {
        if (dir == Vector2Int.zero)
            return false;

        // Normalize sign
        delta = new Vector2Int(
            delta.x == 0 ? 0 : (int)Mathf.Sign(delta.x),
            delta.y == 0 ? 0 : (int)Mathf.Sign(delta.y)
        );

        return delta == dir;
    }
}
