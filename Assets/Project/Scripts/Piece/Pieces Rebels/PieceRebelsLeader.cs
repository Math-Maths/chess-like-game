using UnityEngine;
using ChessGame;
using System.Collections;

public class PieceRebelsLeader : BasePiece, IPieceHover
{
    public void OnPieceHover(BasePiece hoveredPiece)
    {
        if(hoveredPiece.IsAvailable)
        {
            hoveredPiece.OccupyingTile.SwitchPiecesPlaces(OccupyingTile);
            Debug.Log($"{gameObject.name} has switched places with {hoveredPiece.gameObject.name}!");
        }
    }

    public override void StartMove(BoardCreator.Coordinate[] path)
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
            BaseBoardTile checkTile = BoardCreator.Instance.GetTileAt(coord.x, coord.y);
            checkTile.CheckOnPieceEnter(this);

            transform.position = targetPosition;

            yield return new WaitForSeconds(.5f);
        }

        BaseBoardTile lastTile = BoardCreator.Instance.GetTileAt(path[path.Length - 1].x, path[path.Length - 1].y);

        if(lastTile != null)
        {
            if(lastTile.GetOccupyingPiece() != null && lastTile.GetOccupyingPiece().Side == Side)
            {
                OnPieceHover(lastTile.GetOccupyingPiece());
            }
            else
            {
                _occupyingTile.RemovePiece();
                lastTile.HandlePiecePlacement(this);
            }
        }

        firstMoveDone = true;

        FinishTurn();
    }
}