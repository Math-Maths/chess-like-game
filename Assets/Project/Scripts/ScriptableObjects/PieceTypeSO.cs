using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PieceType", menuName = "ScriptableObjects/Piece Type")]
public class PieceTypeSO : ScriptableObject
{
    public string pieceName;
    public Sprite pieceSprite;
    public Vector2Int[] movementPatterns;
    public int maxMoveDistance;
    public int firstMoveDistance;
    public bool canJumpOverPieces;
    public Vector2Int[] attackPatterns;
}
