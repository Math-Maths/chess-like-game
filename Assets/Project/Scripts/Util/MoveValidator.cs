using ChessGame;
using System.Collections.Generic;

public static class MoveValidator
{
    public static List<BoardCreator.Coordinate> PreviewValidMoves(BoardTile tile)
    {
        Piece piece = tile.GetOccupyingPiece();
        if (piece == null)
            return new List<BoardCreator.Coordinate>();

        PieceTypeSO type = piece.Type;
        int pieceSide = piece.Side == PieceSide.Player ? 1 : -1;
        var validMoves = new List<BoardCreator.Coordinate>();

        int checkMoveDistance = piece.IsFirstMoveDone ? type.maxMoveDistance : type.firstMoveDistance;

        foreach (var pattern in type.movementPatterns)
        {
            for (int distance = 1; distance <= checkMoveDistance; distance++)
            {
                int targetX = tile.XCoord + pattern.x * distance * pieceSide;
                int targetY = tile.YCoord + pattern.y * distance * pieceSide;

                // Check if the target coordinates are within board bounds
                if (targetX < 0 || targetX >= BoardCreator.Instance.GetCurrentBoard().boardSize.x ||
                    targetY < 0 || targetY >= BoardCreator.Instance.GetCurrentBoard().boardSize.y)
                {
                    break; // Out of bounds, stop checking this direction
                }

                // If the target tile is occupied, stop checking further in this direction
                BoardTile targetTile = BoardCreator.Instance.GetTileAt(targetX, targetY);
                if(targetTile.GetOccupyingPiece() != null)
                {
                    if(targetTile.GetOccupyingPiece().Side != piece.Side)
                    {
                        validMoves.Add(new BoardCreator.Coordinate(targetX, targetY));

                        if(!type.canJumpOverPieces)
                            break; // Can't jump over pieces, stop here
                    }
                    else if(type.canJumpOverPieces)
                    {
                        continue; // Can jump over pieces, continue checking
                    }
                    else
                    {
                        break; // Can't jump over pieces, stop here
                    }
                }

                validMoves.Add(new BoardCreator.Coordinate(targetX, targetY));
            }
        }

        return validMoves;
    }
}