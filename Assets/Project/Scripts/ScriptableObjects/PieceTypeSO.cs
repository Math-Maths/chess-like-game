using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PieceType", menuName = "ScriptableObjects/Piece Type")]
public class PieceTypeSO : ScriptableObject
{
    [Header("General Properties")]
    public string pieceName;
    public Sprite pieceSprite;
    public Sprite pieceOutlineSprite;
    public int pieceOrderValue;
    public BasePiece piecePrefab;
    public PieceCategory category;

    [Space(20)]
    [Header("Movement Properties")]
    public int maxMoveDistance;
    public int firstMoveDistance;
    public bool canJumpOverPieces;
    public bool hasSecondaryAttack;
    public bool hasSecondaryMove;

    [Space(20)]
    [Header("Movement and Attack Patterns")]
    public Vector2Int[] movementPatterns;
    public Vector2Int[] attackPatterns;
    public Vector2Int[] secondaryMovePatterns;
    public int secondaryMoveDistance;

    [Space(20)]
    [Header("Secondary Attack Properties")]
    public bool canShootProjectiles;

    [ConditionalField("canShootProjectiles")]
    public int attackRange;
    [ConditionalField("canShootProjectiles")]
    public float attackSpeed;
    [ConditionalField("canShootProjectiles")]
    public Sprite attackSprite;

    public enum PieceCategory
    {
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King,
        Custom
    }
}
