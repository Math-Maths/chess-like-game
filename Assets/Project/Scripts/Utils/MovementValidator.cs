using UnityEngine;

public static class MovementValidator
{
    public static bool IsValidMove(Tile targetTile, BasePiece basePiece)
    {

        if(targetTile.IsOccupied(out BasePiece piece))
        {
            return false;
        }

        if(!CheckDirection(basePiece, targetTile))
        {
            return false;
        }
    
        return true;
    }

    private static bool CheckDirection(BasePiece piece, Tile targetTile)
    {
        Vector2 piecePos = piece.gridPos;
        Vector2 targetPos = targetTile.GridPosition;
        Vector2 direction = (targetPos - piecePos).normalized;

        foreach(Vector2Int pieceMove in piece.definition.moveDirections)
        {
            Vector2 possibleDirection = new Vector2(pieceMove.x, pieceMove.y).normalized;
            if(direction == possibleDirection)
            {
                return true;
            }
        }

        return false;
    }
}
