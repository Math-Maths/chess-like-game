using ChessGame;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class Validator
{
    public static List<BoardCreator.Coordinate> PreviewValidMoves(BoardTile tile, bool secondaryMove = false)
    {
        BasePiece piece = tile.GetOccupyingPiece();
        if (piece == null)
            return new List<BoardCreator.Coordinate>();

        PieceTypeSO type = piece.Type;
        int pieceSide = piece.Side == PieceSide.Player ? 1 : -1;
        var validMoves = new List<BoardCreator.Coordinate>();

        int checkMoveDistance = piece.IsFirstMoveDone ? type.maxMoveDistance : type.firstMoveDistance;

        if(secondaryMove)
        {
            checkMoveDistance = type.secondaryMoveDistance;
        }
        
        Vector2Int[] currentPatterns = secondaryMove ? type.secondaryMovePatterns : type.movementPatterns;

        foreach (var pattern in currentPatterns)
        {
            if(checkMoveDistance == -1)
            {
                checkMoveDistance = Math.Max(BoardCreator.Instance.GetCurrentBoard().boardSize.x, BoardCreator.Instance.GetCurrentBoard().boardSize.y);
            }

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
                    if(type.canJumpOverPieces)
                    {
                        continue;
                    }

                    break;
                }

                validMoves.Add(new BoardCreator.Coordinate(targetX, targetY));
            }
        }

        return validMoves;
    }

    public static List<BoardCreator.Coordinate> CheckPossibleAttacks(BoardTile tile)
    {
        BasePiece piece = tile.GetOccupyingPiece();
        if (piece == null)
            return new List<BoardCreator.Coordinate>();

        PieceTypeSO type = piece.Type;
        int pieceSide = piece.Side == PieceSide.Player ? 1 : -1;
        var validMoves = new List<BoardCreator.Coordinate>();

        foreach (var attackPattern in type.attackPatterns)
        {
            for (int distance = 1; distance <= type.maxMoveDistance; distance++)
            {
                int targetX = tile.XCoord + attackPattern.x * pieceSide * distance;
                int targetY = tile.YCoord + attackPattern.y * pieceSide * distance;

                // Check if the target coordinates are within board bounds
                if (targetX < 0 || targetX >= BoardCreator.Instance.GetCurrentBoard().boardSize.x ||
                    targetY < 0 || targetY >= BoardCreator.Instance.GetCurrentBoard().boardSize.y)
                {
                    continue; // Out of bounds, skip this attack pattern
                }

                BoardTile targetTile = BoardCreator.Instance.GetTileAt(targetX, targetY);
                if (targetTile.GetOccupyingPiece() != null)
                {
                    if (!type.canJumpOverPieces && targetTile.GetOccupyingPiece().Side == GameManager.Instance.CurrentTurn)
                        break; // Can't jump over pieces, stop checking further in this direction

                    if (targetTile.GetOccupyingPiece().Side != piece.Side)
                        validMoves.Add(new BoardCreator.Coordinate(targetX, targetY));

                    if (!type.canJumpOverPieces && targetTile.GetOccupyingPiece().Side != piece.Side)
                        break; // Stop checking further in this direction after finding an occupied tile
                
                }
            }
        }

        return validMoves;
    }

    public static List<BoardCreator.Coordinate> PreviewProjectile(BoardTile tile, out List<BoardCreator.Coordinate> attackPositions)
    {
        BasePiece piece = tile.GetOccupyingPiece();
        if (piece == null)
        {
            attackPositions = new List<BoardCreator.Coordinate>();
            return new List<BoardCreator.Coordinate>();
        }

        PieceTypeSO type = piece.Type;
        int pieceSide = piece.Side == PieceSide.Player ? 1 : -1;
        var attacksPreview = new List<BoardCreator.Coordinate>();
        var supportAttackPositions = new List<BoardCreator.Coordinate>();

        foreach(var pattern in type.movementPatterns)
        {
            for (int range = 1; range <= type.attackRange; range++)
            {
                int targetX = tile.XCoord + pattern.x * pieceSide * range;
                int targetY = tile.YCoord + pattern.y * pieceSide * range;

                if (targetX < 0 || targetX >= BoardCreator.Instance.GetCurrentBoard().boardSize.x ||
                    targetY < 0 || targetY >= BoardCreator.Instance.GetCurrentBoard().boardSize.y)
                {
                    continue;
                }

                BoardTile targetTile = BoardCreator.Instance.GetTileAt(targetX, targetY);
                if (targetTile.GetOccupyingPiece() != null && targetTile.GetOccupyingPiece().Side != piece.Side)
                {
                    supportAttackPositions.Add(new BoardCreator.Coordinate(targetX, targetY));
                    continue;
                }

                attacksPreview.Add(new BoardCreator.Coordinate(targetX, targetY));
            }

        }

        attackPositions = supportAttackPositions;
        return attacksPreview;
    }
}