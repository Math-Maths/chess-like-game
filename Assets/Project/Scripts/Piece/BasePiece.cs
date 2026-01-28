using System.Collections;
using UnityEngine;
using ChessGame;

public class BasePiece : MonoBehaviour
{
    [SerializeField] SpriteRenderer outlineSprite;
    [SerializeField] BaseProjectile projectile;
    [SerializeField] GameObject disabledTileSprite;

    protected BaseBoardTile _currentTarget;
    protected BaseBoardTile _occupyingTile;
    protected SpriteRenderer spriteRenderer;
    protected PieceTypeSO type;
    protected PieceSide color;
    protected bool firstMoveDone = false;
    protected bool _secondaryAttackUsed = false;
    protected bool _available = true;
    protected int _turnsUntilAvailable = 0;
    protected GameObject _disabledTileInstance;
    protected Color _outlineOriginalColor;

    public PieceSide Side
    {
        get { return color; }
    }

    public PieceTypeSO Type
    {
        get { return type; }
    }

    public BaseBoardTile OccupyingTile
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

    public bool IsAvailable
    {
        get { return _available; }
    }

    public void Initialize(PieceTypeSO newType, PieceSide side, Color outlineColor, BaseBoardTile occupyingTile, string customName = "")
    {
        EventManager.Instance.AddListener(EventNameSaver.OnTurnChange, OnTurnChange);

        spriteRenderer = GetComponent<SpriteRenderer>();

        type = newType;
        color = side;
        _occupyingTile = occupyingTile;
        spriteRenderer.sprite = type.pieceSprite;
        outlineSprite.sprite = type.pieceOutlineSprite;
        outlineSprite.color = outlineColor;
        _outlineOriginalColor = outlineSprite.color;
        gameObject.name = type.pieceName + (customName != "" ? $"_{customName}" : "");
        spriteRenderer.sortingOrder = type.pieceOrderValue;
        outlineSprite.sortingOrder = type.pieceOrderValue + 1;
        firstMoveDone = false;
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventNameSaver.OnTurnChange, OnTurnChange);
    }

    public virtual void StartMove(BoardCreator.Coordinate[] path)
    {
        StartCoroutine(WalkPath(path));
    }

    IEnumerator WalkPath(BoardCreator.Coordinate[] path)
    {
        GameManager.Instance.CurrentGameState = GameState.Busy;
        _occupyingTile.RemovePiece();
        yield return new WaitForSeconds(.5f);

        foreach (var coord in path)
        {
            Vector3 targetPosition = BoardCreator.Instance.CoordinateToPosition(coord.x, coord.y);

            transform.position = targetPosition;
            yield return new WaitForSeconds(.5f);
        }

        BaseBoardTile lastTile = BoardCreator.Instance.GetTileAt(path[path.Length - 1].x, path[path.Length - 1].y);

        if(lastTile != null)
        {
            _occupyingTile.RemovePiece();
            lastTile.HandlePiecePlacement(this);
        }

        firstMoveDone = true;

        FinishTurn();
    }

    public void RangeAttack(BaseBoardTile targetTile)
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
        _currentTarget.CapturePiece();
        GameManager.Instance.ToggleTurn();
        GameManager.Instance.CurrentGameState = GameState.Gameplay;
        _currentTarget = null;
    }

    public void ChangeOccupyingTile(BaseBoardTile newTile)
    {
        _occupyingTile = newTile;
    }

    public void DisablePiece(int turns)
    {
        _available = false;
        _turnsUntilAvailable = turns * 2;
        _disabledTileInstance = Instantiate(disabledTileSprite, transform.position, Quaternion.identity);
        _disabledTileInstance.transform.SetParent(transform);
        _disabledTileInstance.GetComponent<SpriteRenderer>().sprite = spriteRenderer.sprite;
        outlineSprite.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        _disabledTileInstance.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder + 2;
    }

    public void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    private void OnTurnChange()
    {
        if(!_available)
        {
            _turnsUntilAvailable--;
            if(_turnsUntilAvailable <= 0)
            {
                _available = true;
                _turnsUntilAvailable = 0;
                if(_disabledTileInstance != null)
                {
                    outlineSprite.color = _outlineOriginalColor;
                    Destroy(_disabledTileInstance);
                }
            }
        }
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
