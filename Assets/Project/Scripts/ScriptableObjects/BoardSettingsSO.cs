using UnityEngine;
using ChessGame;

[CreateAssetMenu(fileName = "BoardSettings", menuName = "ScriptableObjects/BoardSettingsSO", order = 1)]
public class BoardSettingsSO : ScriptableObject
{
    [Header("Board Settings")]
    public string boardName;
    public BoardCreator.Coordinate boardSize;
    public BoardTileSetting defaultTile;
    public Color playerSideColor;
    public Color botSideColor;
    public bool hasSpecialTile;
    
    [Space(5)]
    [Header("Special Tiles")]
    public BoardTileSetting[] specialTiles;

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

    [System.Serializable]
    public class BoardTileSetting
    {
        public BaseBoardTile tilePrefab;
        public Sprite whiteTile;
        public Sprite blackTile;
        public BoardCreator.Coordinate tileCoordinate;
        public bool isObstacle;

        [Header("Use this field if you need a second sprite to hover over this tile")]
        public Sprite aditionalSprite;
    }
}