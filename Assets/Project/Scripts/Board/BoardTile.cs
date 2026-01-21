using UnityEngine;
using ChessGame;
using Unity.XR.Oculus.Input;

public class BoardTile : MonoBehaviour
{
    private BasePiece occupyingPiece = null;
    [SerializeField] private BoardCreator.Coordinate coordinate;

    public int XCoord
    {
        get{ return coordinate.x; }
    }

    public int YCoord
    {
        get{ return coordinate.y; }
    }

    public void SetTile(Sprite sprite, BoardCreator.Coordinate coord)
    {
        coordinate = coord;
        gameObject.name = $"Tile ({coordinate.x}, {coordinate.y})";
        if(TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
        {
            spriteRenderer.sprite = sprite;
        }
    }

    public BasePiece GetOccupyingPiece()
    {
        return occupyingPiece;
    }

    public void PlacePiece(BasePiece piece)
    {
        if(occupyingPiece != null)
        {
            //Debug.LogWarning($"Tile ({XCoord}, {YCoord}) is already occupied by {occupyingPiece.gameObject.name}. Overwriting.");
            occupyingPiece.Die();
            RemovePiece();
        }
        
        occupyingPiece = piece;
        piece.ChangeOccupyingTile(this);
    }

    public void PieceAttack(BasePiece attacker, bool isRanged)
    {
        if(!isRanged)
        {
            occupyingPiece.Die();
            RemovePiece();
            PlacePiece(attacker);
            return;
        }

        occupyingPiece.Die();
        RemovePiece();
    }

    public void RemovePiece()
    {
        occupyingPiece = null;
    }
}