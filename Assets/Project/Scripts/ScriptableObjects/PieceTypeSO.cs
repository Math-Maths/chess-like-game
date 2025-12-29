using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PieceType", menuName = "ScriptableObjects/Piece Type")]
public class PieceTypeSO : ScriptableObject
{
    [Header("General Properties")]
    public string pieceName;
    public Sprite pieceSprite;
    public int pieceOrderValue;
    public bool hasSecondaryAttack;

    [Space(20)]
    [Header("Movement Properties")]
    public int maxMoveDistance;
    public int firstMoveDistance;
    public bool canJumpOverPieces;

    [Space(20)]
    [Header("Movement and Attack Patterns")]
    public Vector2Int[] movementPatterns;
    public Vector2Int[] attackPatterns;
}
