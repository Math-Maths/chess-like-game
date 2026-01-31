using ChessGame;
using UnityEngine;

public class TrapBoardTile : BaseBoardTile
{
    [SerializeField] private SpriteRenderer spikesHolder;

    public override void SetTile(Sprite sprite, BoardCreator.Coordinate coord, Sprite aditional = null, bool obstacle = false)
    {
        base.SetTile(sprite, coord);
        spikesHolder.sprite = aditional;
        spikesHolder.gameObject.SetActive(false);
    }

    public override void CheckOnPieceEnter(BasePiece piece)
    {
        spikesHolder.sortingOrder = piece.GetComponent<SpriteRenderer>().sortingOrder + 1;
        spikesHolder.gameObject.SetActive(true);
        piece.Die(true);
        _isObstacle = true;
        //Debug.Log("Spike enabled");
    }

}