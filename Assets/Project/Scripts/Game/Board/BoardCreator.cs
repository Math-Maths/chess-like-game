using UnityEngine;
using System.Collections.Generic;

public class BoardCreator : MonoBehaviour
{
    public static BoardCreator Instance { get; private set; }

    [Header("Board Definitions")]
    [SerializeField] private BoardDefinitionSO[] boardDefinitions;
    [SerializeField] private int boardIndex;

    [Space(10)]
    [Header("Tile Settings")]
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private float tileSize = 1f;
    [Range(0,1)]
    [SerializeField] private float outlinePercent = 0f;

    [Space(10)]
    [Header("Parents")]
    [SerializeField] private Transform boardParent;
    [SerializeField] private Transform piecesParent;

    private BoardDefinitionSO _currentBoard;
    private List<Coordinate> _allCoordinates;
    private Queue<Coordinate> _shuffledCoordinates;
    private Tile[,] tiles;

    public int CurrentWidth => _currentBoard.width;
    public int CurrentHeight => _currentBoard.height;
    public BoardDefinitionSO currentDefinition => _currentBoard;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GenerateBoard();
    }

    public void GenerateBoard()
    {
        if (boardDefinitions == null || boardDefinitions.Length == 0)
        {
            Debug.LogWarning("BoardCreator: No BoardDefinition found.");
            return;
        }

        boardIndex = Mathf.Clamp(boardIndex, 0, boardDefinitions.Length - 1);
        _currentBoard = boardDefinitions[boardIndex];

        if (_currentBoard == null)
        {
            Debug.LogWarning("BoardCreator: The Selected Board is null.");
            return;
        }

        // Generate all coordinates
        _allCoordinates = new List<Coordinate>();
        for (int x = 0; x < _currentBoard.width; x++)
        {
            for (int y = 0; y < _currentBoard.height; y++)
            {
                _allCoordinates.Add(new Coordinate(x, y));
            }
        }

        _shuffledCoordinates = new Queue<Coordinate>(_allCoordinates);

        // Erase old board
        string holderName = "Generated Board";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        // Create board holder
        Transform container = new GameObject(holderName).transform;
        container.parent = transform;

        if (boardParent == null)
            boardParent = container;
        else
            boardParent = container;

        tiles = new Tile[_currentBoard.width, _currentBoard.height];

        // Generate tiles
        for (int x = 0; x < _currentBoard.width; x++)
        {
            for (int y = 0; y < _currentBoard.height; y++)
            {
                Vector3 tilePos = CoordinateToPosition(x, y);

                Tile tile = Instantiate(tilePrefab, tilePos, Quaternion.identity, boardParent);
                tile.transform.name = $"Tile {x},{y}"; 
                tile.transform.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                tile.GridPosition = new Vector2Int(x, y);
                
                bool isA = (x + y) % 2 == 0;
                if(isA)
                {
                    tile.SetTile(_currentBoard.tileA);
                }
                else
                {
                    tile.SetTile(_currentBoard.tileB);
                }
                
                tiles[x, y] = tile;
            }
        }

        //Debug.Log($"Board generated using: { _currentBoard.name }");
    }

    private Vector3 CoordinateToPosition(int x, int y)
    {
        return new Vector3(
            -_currentBoard.width / 2f + 0.5f + x,
            -_currentBoard.height / 2f + 0.5f + y,
            0
        ) * tileSize;
    }

    public Coordinate GetRandomCoordinate()
    {
        Coordinate coord = _shuffledCoordinates.Dequeue();
        _shuffledCoordinates.Enqueue(coord);
        return coord;
    }

    [System.Serializable]
    public struct Coordinate
    {
        public int x;
        public int y;

        public Coordinate(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coordinate a, Coordinate b)
            => a.x == b.x && a.y == b.y;

        public static bool operator !=(Coordinate a, Coordinate b)
            => !(a == b);
    }

    public bool IsInsideBoard(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < _currentBoard.width &&
               pos.y >= 0 && pos.y < _currentBoard.height;
    }

    public BasePiece GetPieceAt(Vector2Int pos)
    {
        foreach (Transform pieceTransform in piecesParent)
        {
            BasePiece piece = pieceTransform.GetComponent<BasePiece>();
            if (piece != null && piece.gridPos == pos)
                return piece;
        }
        return null;
    }

    public void MovePiece(BasePiece piece, Vector2Int targetPos)
    {
        piece.gridPos = targetPos;
        piece.transform.position = GetWorldPosition(targetPos.x, targetPos.y);
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return CoordinateToPosition(x, y);
    }

    public Tile GetTileAtWorldPos(Vector2Int worldPos)
    {
        if (IsInsideBoard(worldPos))
        {
            return tiles[worldPos.x, worldPos.y];
        }

        return null;
    }

    public Tile GetTileAtWorldPos(int x, int y)
    {
        Vector2Int worldPos = new Vector2Int(x, y);
        if (IsInsideBoard(worldPos))
        {
            return tiles[worldPos.x, worldPos.y];
        }

        return null;
    }

    public BasePiece GetPieceAtTile(Tile tile)
    {
        if(tile.IsOccupied(out BasePiece piece))
        {
            return piece;
        }

        return null;
    }

    public void PlacePiece(BasePiece piece, Vector2Int pos)
    {
        Tile tile = GetTileAtWorldPos(pos);
        if(tile != null)
        {
            tile.PlacePiece(piece);
        }
    }
}
