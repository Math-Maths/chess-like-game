using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public enum InputContext
    {
        Gameplay,
        TeamSelection,
        UIinteraction
    }

    private PlayerInput _playerInput;
    private BoardManager _boardManager;
    private BoardTile _selectedPiece;
    
    private InputAction _leftClickAction;
    private InputContext _currentInputContext;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _boardManager = GetComponent<BoardManager>();
        _leftClickAction = _playerInput.actions["Point"];
        _currentInputContext = InputContext.Gameplay;
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
        if(_currentInputContext == InputContext.Gameplay)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                if(hit.collider.TryGetComponent<BoardTile>(out BoardTile tile))
                {
                    //BoardManager handle tile selection
                    Piece piece = tile.GetOccupyingPiece();

                    if(_selectedPiece != null)
                    {
                        //Try to move the selected piece to the clicked tile
                        _boardManager.TryMoveSelectedPiece(_selectedPiece, tile);
                        _selectedPiece = null;
                        return;
                    }

                    if(piece != null)
                    {
                        Debug.Log($"Clicked on piece: {piece.name} at tile: {tile.name}");
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
