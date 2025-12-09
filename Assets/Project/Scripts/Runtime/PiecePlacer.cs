using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PiecePlacer : MonoBehaviour
{
    [Header("Piece Setup")]
    public BasePiece[] piecePrefabs;
    public int currentPieceIndex;

    [Header("Board Reference")]
    public BoardCreator board;
    public Transform piecesParent;

    public List<BasePiece> placedPieces = new List<BasePiece>();

    public void PlacePieceAt(Vector2Int pos)
    {
        if (board == null || piecePrefabs.Length == 0) return;
        if (currentPieceIndex < 0 || currentPieceIndex >= piecePrefabs.Length) return;

        var prefab = piecePrefabs[currentPieceIndex];
        if (prefab == null) return;

        RemovePieceAt(pos);

        Vector3 worldPos = board.GetWorldPosition(pos.x, pos.y);
        BasePiece piece = Instantiate(prefab, worldPos, Quaternion.identity, piecesParent);
        piece.Initialize(board, pos);

        placedPieces.Add(piece);
    }

    public void RemovePieceAt(Vector2Int pos)
    {
        for (int i = placedPieces.Count - 1; i >= 0; i--)
        {
            if (placedPieces[i].gridPos == pos)
            {
                    if (Application.isPlaying)
                        Destroy(placedPieces[i].gameObject);
                    else
                        DestroyImmediate(placedPieces[i].gameObject);

                placedPieces.RemoveAt(i);
            }
        }
    }

    public void ClearAllPieces()
    {
        for (int i = placedPieces.Count - 1; i >= 0; i--)
        {
            if (placedPieces[i] != null)
                if (Application.isPlaying)
                    Destroy(placedPieces[i].gameObject);
                else
                    DestroyImmediate(placedPieces[i].gameObject);
        }
        placedPieces.Clear();
    }
}
