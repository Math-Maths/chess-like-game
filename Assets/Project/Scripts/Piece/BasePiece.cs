using System.Collections;
using UnityEngine;
using ChessGame;

public class BasePiece : MonoBehaviour
{
    [SerializeField] SpriteRenderer outlineSprite;
    [SerializeField] BaseProjectile projectile;

    protected BoardTile _currentTarget;
    protected BoardTile _occupyingTile;
    protected SpriteRenderer spriteRenderer;
    protected PieceTypeSO type;
    protected PieceSide color;
    protected bool firstMoveDone = false;
    protected bool _secondaryAttackUsed = false;

    public PieceSide Side
    {
        get { return color; }
    }

    public PieceTypeSO Type
    {
        get { return type; }
    }

    public BoardTile OccupyingTile
    {
        get { return _occupyingTile; }
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

    public void Initialize(PieceTypeSO newType, PieceSide side, Color outlineColor, BoardTile occupyingTile, string customName = "")
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        type = newType;
        color = side;
        _occupyingTile = occupyingTile;
        spriteRenderer.sprite = type.pieceSprite;
        outlineSprite.sprite = type.pieceOutlineSprite;
        outlineSprite.color = outlineColor;
        gameObject.name = type.pieceName + (customName != "" ? $"_{customName}" : "");
        spriteRenderer.sortingOrder = type.pieceOrderValue;
        outlineSprite.sortingOrder = type.pieceOrderValue + 1;
        firstMoveDone = false;
    }

    public virtual void StartMove(BoardCreator.Coordinate[] path)
    {
        StartCoroutine(WalkPath(path));
    }

    IEnumerator WalkPath(BoardCreator.Coordinate[] path)
    {
        GameManager.Instance.CurrentGameState = GameState.Busy;
        yield return new WaitForSeconds(.5f);

        foreach (var coord in path)
        {
            Vector3 targetPosition = BoardCreator.Instance.CoordinateToPosition(coord.x, coord.y);

            transform.position = targetPosition;
            yield return new WaitForSeconds(.5f);
        }

        BoardTile lastTile = BoardCreator.Instance.GetTileAt(path[path.Length - 1].x, path[path.Length - 1].y);

        if(lastTile != null)
        {
            lastTile.PlacePiece(this);
        }

        firstMoveDone = true;

        FinishTurn();
    }

    public void RangeAttack(BoardTile targetTile)
    {
        //TODO
        //starts a coroutine that waits the animation ends to kill the piece
        _currentTarget = targetTile;
        Vector3 targetPosition = BoardCreator.Instance.CoordinateToPosition(_currentTarget.XCoord, _currentTarget.YCoord);
        BaseProjectile projectileGO = Instantiate(projectile, transform.position, Quaternion.identity);
        projectileGO.OnHitTarget += KillEnemyPiece;
        projectileGO.GoToTarget(targetPosition, type.attackSpeed);
    }

    public void ToggleSecondaryAttack(bool state)
    {
        if(type.hasSecondaryAttack)
        {
            _secondaryAttackUsed = state;
        }
    }

    void KillEnemyPiece()
    {
        _currentTarget.PieceAttack(this, true);
        GameManager.Instance.ToggleTurn();
        GameManager.Instance.CurrentGameState = GameState.Gameplay;
        _currentTarget = null;
    }

    public void ChangeOccupyingTile(BoardTile newTile)
    {
        _occupyingTile = newTile;
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }

    protected void FinishTurn()
    {
        GameManager.Instance.ToggleTurn();
        GameManager.Instance.CurrentGameState = GameState.Gameplay;
    }
}
