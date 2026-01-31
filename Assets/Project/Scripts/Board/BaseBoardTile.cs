using UnityEngine;
using ChessGame;

public class BaseBoardTile : MonoBehaviour
{
    protected BasePiece _occupyingPiece;
    protected BoardCreator.Coordinate _coordinate;

    protected bool _isObstacle;

    public bool IsObstacle => _isObstacle;

    public int XCoord
    {
        get{ return _coordinate.x; }
    }

    public int YCoord
    {
        get{ return _coordinate.y; }
    }

    public virtual void SetTile(Sprite sprite, BoardCreator.Coordinate coord, Sprite aditional = null, bool obstacle = false)
    {
        _coordinate = coord;
        _occupyingPiece = null;
        gameObject.name = $"Tile ({_coordinate.x}, {_coordinate.y})";
        if(TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
        {
            spriteRenderer.sprite = sprite;
        }
    }

    public BasePiece GetOccupyingPiece()
    {
        return _occupyingPiece;
    }

    public void HandlePiecePlacement(BasePiece piece)
    {
        if(_occupyingPiece != null)
        {
            CapturePiece();
        }
        
        _occupyingPiece = piece;
        _occupyingPiece.SetPosition(transform.position);
        piece.ChangeOccupyingTile(this);
    }

    public void SwitchPiecesPlaces(BaseBoardTile otherTile)
    {
        BasePiece otherPiece = otherTile.GetOccupyingPiece();
        BasePiece thisPiece = _occupyingPiece;

        otherTile.RemovePiece();
        RemovePiece();
        otherTile.HandlePiecePlacement(thisPiece);
        HandlePiecePlacement(otherPiece);
    }

    public void CapturePiece()
    {
        _occupyingPiece.Die();
        RemovePiece();
    }

    public void RemovePiece()
    {
        _occupyingPiece = null;
    }

    public virtual void CheckOnPieceEnter(BasePiece piece)
    {
        //Debug.Log("Normal Tile");
    }
}