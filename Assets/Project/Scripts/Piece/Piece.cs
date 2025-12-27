using System.Collections;
using UnityEngine;
using ChessGame;

public class Piece : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private PieceTypeSO type;
    private PieceSide color;
    private bool firstMoveDone = false;

    public PieceSide Side
    {
        get { return color; }
    }

    public PieceTypeSO Type
    {
        get { return type; }
    }

    public bool IsFirstMoveDone
    {
        get { return firstMoveDone; }
        set { firstMoveDone = value; }
    }

    public void Initialize(PieceTypeSO newType, PieceSide side)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        type = newType;
        color = side;
        spriteRenderer.sprite = type.pieceSprite;
        gameObject.name = type.pieceName;
        spriteRenderer.sortingOrder = 1;
        firstMoveDone = false;
    }

    public void StartMove(BoardCreator.Coordinate[] path)
    {
        // Implement movement along the given path
        // This could involve animations or instant movement
        // For simplicity, we'll just move instantly to the last position in the path
        StartCoroutine(WalkPath(path));
        
    }

    IEnumerator WalkPath(BoardCreator.Coordinate[] path)
    {
        yield return new WaitForSeconds(1f);

        foreach (var coord in path)
        {
            Vector3 targetPosition = BoardCreator.Instance.CoordinateToPosition(coord.x, coord.y);
            //Debug.Log($"Moving to {coord.x}, {coord.y}");
            // Simple instant move for now; can be replaced with smooth movement
            transform.position = targetPosition;
            yield return new WaitForSeconds(1f); // Pause briefly between steps
        }

        BoardTile lastTile = BoardCreator.Instance.GetTileAt(path[path.Length - 1].x, path[path.Length - 1].y);
        if(lastTile != null)
        {
            lastTile.PlacePiece(this);
        }

        firstMoveDone = true;
        GameManager.Instance.ToggleTurn();
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }
}
