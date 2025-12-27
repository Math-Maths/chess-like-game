using UnityEngine;
using ChessGame;

[CreateAssetMenu(fileName = "BoardSettings", menuName = "ScriptableObjects/BoardSettingsSO", order = 1)]
public class BoardSettingsSO : ScriptableObject
{
    [Header("Board Settings")]
    public string boardName;
    public BoardCreator.Coordinate boardSize;
    public Sprite whiteTileSprite;
    public Sprite blackTileSprite;

    [Space(10)]
    [Header("Enemy Pieces Configuration")]
    public PieceData[] enemyPieces;

    [Space(10)]
    [Header("Enemy Pieces Configuration")]
    public PieceData[] playePieces;

    [System.Serializable]
    public class PieceData
    {
        public PieceTypeSO pieceType;
        public BoardCreator.Coordinate position;
    }
}