using UnityEngine;
using UnityEngine.InputSystem;
using ChessGame;

public class InputHandler : MonoBehaviour
{
    private PlayerInput _playerInput;
    private BoardManager _boardManager;
    private BoardTile _selectedPiece;
    
    private InputAction _leftClickAction;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _boardManager = GetComponent<BoardManager>();
        _leftClickAction = _playerInput.actions["Point"];
    }

    void OnEnable()
    {
        _leftClickAction.performed += OnPointPerfomed;
    }

    void OnDisable()
    {
        _leftClickAction.performed -= OnPointPerfomed;
    }

    private void OnPointPerfomed(InputAction.CallbackContext context)
    {
        if(GameManager.Instance.CurrentGameState == GameState.Gameplay)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                if(hit.collider.TryGetComponent<BoardTile>(out BoardTile tile))
                {
                    BasePiece piece = tile.GetOccupyingPiece();

                    #region Deprecated
                    // if(_selectedPiece != null)
                    // {
                    //     //Try to move the selected piece to the clicked tile
                    //     _boardManager.TryMoveSelectedPiece(_selectedPiece, tile);
                    //     _selectedPiece = null;
                    //     return;
                    // }
                    #endregion

                    if(piece != null || _selectedPiece != null || _boardManager.CurrentState == SelectionState.SecondaryMove)
                    {
                        _boardManager.HandleSelection(tile, out _selectedPiece);
                    }
                    else
                    {
                        Debug.Log($"Clicked on empty tile: {tile.name}");
                    }
                }
                else
                {
                    Debug.Log("Clicked on non-tile object.");
                }
            }
        }

        
    }
}
