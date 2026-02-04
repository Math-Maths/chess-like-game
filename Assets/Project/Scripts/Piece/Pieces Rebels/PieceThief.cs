using UnityEngine;
using ChessGame;
using System.Collections;
using ChessGame.Managers;

public class PieceThief : BasePiece, IPieceHover
{
    public void OnPieceHover(BasePiece hoveredPiece)
    {
        if(hoveredPiece.IsAvailable)
        {
            hoveredPiece.DisablePiece(3);
        }
    }

    public override void StartMove(BoardCreator.Coordinate[] path)
    {
        StartCoroutine(WalkPath(path));
    }

    IEnumerator WalkPath(BoardCreator.Coordinate[] path)
    {
        EventManager.Instance.Invoke<GameState>(EventNameSaver.OnStateChange, GameState.Busy);
        _occupyingTile.RemovePiece();
        yield return new WaitForSeconds(.5f);

        foreach (var coord in path)
        {
            Vector3 targetPosition = BoardCreator.Instance.CoordinateToPosition(coord.x, coord.y);

            transform.position = targetPosition;
            BaseBoardTile currentTile = BoardCreator.Instance.GetTileAt(coord.x, coord.y);
            currentTile.CheckOnPieceEnter(this);
            if(currentTile.GetOccupyingPiece() != null && currentTile.GetOccupyingPiece() != this)
            {
                OnPieceHover(currentTile.GetOccupyingPiece());
            }

            yield return new WaitForSeconds(.5f);
        }

        BaseBoardTile lastTile = BoardCreator.Instance.GetTileAt(path[path.Length - 1].x, path[path.Length - 1].y);

        if(lastTile != null)
        {
            lastTile.HandlePiecePlacement(this);
        }

        firstMoveDone = true;

        FinishTurn();
    }
}