using UnityEngine;
using ChessGame;

public class PiecePlacer : MonoBehaviour
{
    [SerializeField] private BasePiece piecePrefab;

    public void PlacePieces(BoardSettingsSO board, Transform pieceHolder, Color playerOutlineColor, Color enemyOutlineColor)
    {
        foreach (var startingPiece in board.playePieces)
        {
            PositionatePieces(startingPiece, pieceHolder, PieceSide.Player, playerOutlineColor, "player");
        }

        foreach (var startingPiece in board.enemyPieces)
        {
            PositionatePieces(startingPiece, pieceHolder, PieceSide.Enemy, enemyOutlineColor, "enemy");
        }
    }

    private void PositionatePieces(BoardSettingsSO.PieceData startingPiece, Transform pieceHolder, PieceSide side, Color outlineColor, string customName = "")
    {
        Vector3 tilePosition = BoardCreator.Instance.CoordinateToPosition(
            startingPiece.position.x, 
            startingPiece.position.y);

        BasePiece prefabToInstanciate = startingPiece.pieceType.piecePrefab;

        BasePiece newPiece = Instantiate(prefabToInstanciate, tilePosition, Quaternion.identity, pieceHolder);
        newPiece.Initialize(startingPiece.pieceType, side, outlineColor, BoardCreator.Instance.GetTileAt(startingPiece.position.x, startingPiece.position.y), customName);
        
        BoardCreator.Instance.SetPieceOnTile(
            startingPiece.position.x, 
            startingPiece.position.y, 
            newPiece);
    }
}