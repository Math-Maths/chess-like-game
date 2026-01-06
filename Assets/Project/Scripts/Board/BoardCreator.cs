using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace ChessGame
{
public class BoardCreator : MonoBehaviour
{
    public static BoardCreator Instance { get; private set; }

    [SerializeField] private int selectedBoardIndex = 0;
    [SerializeField] private BoardSettingsSO[] boards;

    [SerializeField] private BoardTile tilePrefab;
    [SerializeField] private string holderName = "Tile Holder";
    [SerializeField] private string pieceHolderName = "Pieces Holder"; 

    private BoardTile[,] tiles;
    private BoardSettingsSO _currentBoard;
    private List<Coordinate> _allTileCoordinates;
    private PiecePlacer piecePlacer;

    private Transform _boardHolder;
    private Transform _pieceHolder;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }  

    private void Start()
    {
        piecePlacer = GetComponent<PiecePlacer>();
        GenerateBoard();
    }

    public void GenerateBoard()
    {
        if(boards.Length == 0 || tilePrefab == null || selectedBoardIndex < 0 || selectedBoardIndex >= boards.Length)
        {
            Debug.LogWarning("Boards array is empty or tilePrefab is not assigned.");
            return;
        }

        _currentBoard = boards[selectedBoardIndex];
        tiles = new BoardTile[_currentBoard.boardSize.x, _currentBoard.boardSize.y];

        _allTileCoordinates = new List<Coordinate>();

        for (int x = 0; x < _currentBoard.boardSize.x; x++)
        {
            for (int y = 0; y < _currentBoard.boardSize.y; y++)
            {
                _allTileCoordinates.Add(new Coordinate(x,y));
            }
        }

        SetHolders();

        float zOffset = -0.1f;

        for (int x = 0; x < _currentBoard.boardSize.x; x++)
        {
            for (int y = 0; y < _currentBoard.boardSize.y; y++)
            {
                Vector3 tilePosition = CoordinateToPosition(x, y) + new Vector3(0, 0, zOffset);
                BoardTile newTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, _boardHolder);

                if((x + y) % 2 == 0)
                {
                    newTile.SetTile(_currentBoard.whiteTileSprite, new Coordinate(x, y));
                }
                else
                {
                    newTile.SetTile(_currentBoard.blackTileSprite, new Coordinate(x, y));
                }

                tiles[x, y] = newTile;
                zOffset += 0.01f;
            }

            zOffset = -0.1f;
        }

        if(piecePlacer != null)
            piecePlacer.PlacePieces(_currentBoard, _pieceHolder, _currentBoard.playerSideColor, _currentBoard.botSideColor);
    }

    private void SetHolders()
    {
        if(transform.Find(pieceHolderName))
        {
            DestroyImmediate(transform.Find(pieceHolderName).gameObject);
        }

        _pieceHolder = new GameObject(pieceHolderName).transform;
        _pieceHolder.parent = transform;

        if(transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        _boardHolder = new GameObject(holderName).transform;
        _boardHolder.parent = transform;
    }

    public BoardSettingsSO GetCurrentBoard()
    {
        return _currentBoard;
    }

    public BoardTile GetTileAt(int x, int y)
    {
        if(x < 0 || x >= _currentBoard.boardSize.x || y < 0 || y >= _currentBoard.boardSize.y)
        {
            Debug.LogWarning("Coordinates out of bounds.");
            return null;
        }

        return tiles[x, y];
    }

    public Vector3 CoordinateToPosition(int x, int y)
    {
        float offsetX = _currentBoard.boardSize.x % 2 == 0 ? 0.5f : 0f;
        float offsetY = _currentBoard.boardSize.y % 2 == 0 ? 0.5f : 0f;

        return new Vector3(-_currentBoard.boardSize.x/2 + offsetX + x, -_currentBoard.boardSize.y/2 + offsetY + y, 0);
    }

    public void SetPieceOnTile(int x, int y, Piece piece)
    {
        if(x < 0 || x >= _currentBoard.boardSize.x || y < 0 || y >= _currentBoard.boardSize.y)
        {
            Debug.LogWarning("Coordinates out of bounds.");
            return;
        }

        tiles[x, y].PlacePiece(piece);
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

        public static bool operator == (Coordinate c1, Coordinate c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator != (Coordinate c1, Coordinate c2)
        {
            return !(c1 == c2);
        } 
    }
}
}
