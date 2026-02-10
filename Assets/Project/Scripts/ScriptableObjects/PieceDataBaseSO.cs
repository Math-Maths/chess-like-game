using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Piece Database")]
public class PieceDatabaseSO : ScriptableObject
{
    public List<PieceTypeSO> allPieces;

    private Dictionary<string, PieceTypeSO> pieceLookup;

    public void Init()
    {
        pieceLookup = new Dictionary<string, PieceTypeSO>();

        foreach (var piece in allPieces)
            pieceLookup[piece.id] = piece;
    }

    public PieceTypeSO GetPieceByID(string id)
    {
        if (pieceLookup == null)
            Init();

        pieceLookup.TryGetValue(id, out var piece);
        return piece;
    }
}
