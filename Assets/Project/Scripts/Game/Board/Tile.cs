using UnityEngine;

public class Tile : MonoBehaviour
{
    private bool isOccupied;
    private BasePiece placedPiece;
    private Vector2Int gridPosition;

    public Vector2Int GridPosition
    {
        get { return gridPosition; }
        set { gridPosition = value; }
    }

    public void SetTile(Sprite spriteTile)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = spriteTile;
    }
    
    public void PlacePiece(BasePiece piece)
    {
        isOccupied = true;
        placedPiece = piece;
    }

    public void RemovePiece()
    {
        isOccupied = false;
        placedPiece = null;
    }

    public bool IsOccupied(out BasePiece piece)
    {
        piece = placedPiece;
        return isOccupied;
    }

}