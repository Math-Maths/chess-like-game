using UnityEngine;
using ChessGame;

public class PiecePlacer : MonoBehaviour
{
    [SerializeField] private Piece piecePrefab;

    public void PlacePieces(BoardSettingsSO board, Transform pieceHolder, Color playerOutlineColor, Color enemyOutlineColor)
    {
        foreach (var startingPiece in board.playePieces)
        {
            PositionatePieces(startingPiece, pieceHolder, PieceSide.Player, playerOutlineColor);
        }

        foreach (var startingPiece in board.enemyPieces)
        {
            PositionatePieces(startingPiece, pieceHolder, PieceSide.Enemy, enemyOutlineColor);
        }
    }

    private void PositionatePieces(BoardSettingsSO.PieceData startingPiece, Transform pieceHolder, PieceSide side, Color outlineColor)
    {
        Vector3 tilePosition = BoardCreator.Instance.CoordinateToPosition(
            startingPiece.position.x, 
            startingPiece.position.y);

        Piece newPiece = Instantiate(piecePrefab, tilePosition, Quaternion.identity, pieceHolder);
        newPiece.Initialize(startingPiece.pieceType, side, outlineColor);
        
        BoardCreator.Instance.SetPieceOnTile(
            startingPiece.position.x, 
            startingPiece.position.y, 
            newPiece);
    }
}