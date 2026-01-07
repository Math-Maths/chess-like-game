using System.Collections;
using UnityEngine;
using ChessGame;

public class Piece : MonoBehaviour
{
    [SerializeField] SpriteRenderer outlineSprite;

    private SpriteRenderer spriteRenderer;
    private PieceTypeSO type;
    private PieceSide color;
    private bool firstMoveDone = false;
    private bool _secondaryAttackUsed = false;

    public PieceSide Side
    {
        get { return color; }
    }

    public PieceTypeSO Type
    {
        get { return type; }
    }

    public bool SecondaryAttackUsed
    {
        get { return _secondaryAttackUsed; }
        set { _secondaryAttackUsed = value; }
    }

    public bool IsFirstMoveDone
    {
        get { return firstMoveDone; }
        set { firstMoveDone = value; }
    }

    public void Initialize(PieceTypeSO newType, PieceSide side, Color outlineColor)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        type = newType;
        color = side;
        spriteRenderer.sprite = type.pieceSprite;
        outlineSprite.sprite = type.pieceOutlineSprite;
        outlineSprite.color = outlineColor;
        gameObject.name = type.pieceName;
        spriteRenderer.sortingOrder = type.pieceOrderValue;
        outlineSprite.sortingOrder = type.pieceOrderValue + 1;
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
        GameManager.Instance.CurrentGameState = GameState.Busy;
        yield return new WaitForSeconds(.5f);

        foreach (var coord in path)
        {
            Vector3 targetPosition = BoardCreator.Instance.CoordinateToPosition(coord.x, coord.y);
            //Debug.Log($"Moving to {coord.x}, {coord.y}");
            // Simple instant move for now; can be replaced with smooth movement
            transform.position = targetPosition;
            yield return new WaitForSeconds(.5f); // Pause briefly between steps
        }

        BoardTile lastTile = BoardCreator.Instance.GetTileAt(path[path.Length - 1].x, path[path.Length - 1].y);

        if(lastTile != null)
        {
            lastTile.PlacePiece(this);
        }

        firstMoveDone = true;
        GameManager.Instance.ToggleTurn();
        GameManager.Instance.CurrentGameState = GameState.Gameplay;
    }

    public void RangeAttack(BoardTile targetTile)
    {
        //TODO
        //shoots a projectile or play a attack animation
        //starts a coroutine that waits the animation ends to kill the piece

        targetTile.PieceAttack(this, true);
        GameManager.Instance.ToggleTurn();
        GameManager.Instance.CurrentGameState = GameState.Gameplay;
    }

    public void ToggleSecondaryAttack(bool state)
    {
        if(type.hasSecondaryAttack)
        {
            _secondaryAttackUsed = state;
        }
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }
}
