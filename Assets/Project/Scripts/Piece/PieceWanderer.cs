using UnityEngine;
using ChessGame;
using System.Collections;

public class PieceWanderer : BasePiece, IPieceCapture
{
    public void OnPieceCaptured(BoardTile lastPieceTile)
    {
        lastPieceTile.PlacePiece(this);

        if(BoardManager.Instance.ShowPieceMovePreview(_occupyingTile, true).Count > 0)
        {
            BoardManager.Instance.SetCurrentState(SelectionState.SecondaryMove);
            BoardManager.Instance.SetSelectedPiece(this);
            GameManager.Instance.CurrentGameState = GameState.Gameplay;
            //BoardManager.Instance.ShowPieceMovePreview(_occupyingTile, true);
        }
        else
        {
            FinishTurn();
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

            transform.position = targetPosition;
            yield return new WaitForSeconds(.5f);
        }

        BoardTile lastTile = BoardCreator.Instance.GetTileAt(path[path.Length - 1].x, path[path.Length - 1].y);

        if(lastTile != null && lastTile.GetOccupyingPiece() != null && lastTile.GetOccupyingPiece() != this)
        {
            OnPieceCaptured(lastTile);
        }
        else if(lastTile != null)
        {
            lastTile.PlacePiece(this);
            FinishTurn();
        }
    }
}