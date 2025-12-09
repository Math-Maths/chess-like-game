using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PiecePlacer : MonoBehaviour
{
    [Header("Piece Setup")]
    public GameObject[] piecePrefabs;
    public int currentPieceIndex;

    [Header("Board Reference")]
    public BoardCreator board;
    public Transform piecesParent;

    [System.Serializable]
    public class PlacedPiece
    {
        public GameObject prefab;
        public Vector2Int gridPos;
        public GameObject instance;
    }

    public List<PlacedPiece> placedPieces = new List<PlacedPiece>();

    public void PlacePieceAt(Vector2Int pos)
    {
        if (board == null || piecePrefabs.Length == 0) return;
        if (currentPieceIndex < 0 || currentPieceIndex >= piecePrefabs.Length) return;

        var prefab = piecePrefabs[currentPieceIndex];
        if (prefab == null) return;

        RemovePieceAt(pos);

        var piece = new PlacedPiece
        {
            prefab = prefab,
            gridPos = pos
        };

        Vector3 worldPos = board.GetWorldPosition(pos.x, pos.y);
        piece.instance = (Application.isPlaying)
            ? Instantiate(prefab, worldPos, Quaternion.identity, piecesParent)
            : Instantiate(prefab, worldPos, Quaternion.identity, piecesParent);

        placedPieces.Add(piece);
    }

    public void RemovePieceAt(Vector2Int pos)
    {
        for (int i = placedPieces.Count - 1; i >= 0; i--)
        {
            if (placedPieces[i].gridPos == pos)
            {
                if (placedPieces[i].instance != null)
                    if (Application.isPlaying)
                        Destroy(placedPieces[i].instance);
                    else
                        DestroyImmediate(placedPieces[i].instance);

                placedPieces.RemoveAt(i);
            }
        }
    }

    public void ClearAllPieces()
    {
        for (int i = placedPieces.Count - 1; i >= 0; i--)
        {
            if (placedPieces[i].instance != null)
                if (Application.isPlaying)
                    Destroy(placedPieces[i].instance);
                else
                    DestroyImmediate(placedPieces[i].instance);
        }
        placedPieces.Clear();
    }
}
