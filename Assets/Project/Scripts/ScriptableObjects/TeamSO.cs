using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PieceType", menuName = "ScriptableObjects/Team")]
public class TeamSO : ScriptableObject
{
    public TeamPiecesData[] teamPieces;
}

[Serializable]
public class TeamPiecesData
{
    public int pieceIndexPosition;
    public string pieceID;
    //public PieceTypeSO piece;
}