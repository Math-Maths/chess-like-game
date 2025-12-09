using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    private Camera mainCamera;
    private BasePiece selectedPiece;
    private Tile firstSelectedTile;
    private BoardCreator board;
    private PlayerInput playerInput;

    private void Awake()
    {
        mainCamera = Camera.main;
        board = BoardCreator.Instance;
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        playerInput.actions["LeftClick"].performed += OnClickPerformed;
    }

    private void OnDisable()
    {
        playerInput.actions["LeftClick"].performed -= OnClickPerformed;
    }

    private void OnClickPerformed(InputAction.CallbackContext ctx)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null)
        {
            Debug.Log("clicked");
            if(hit.collider.TryGetComponent<Tile>(out Tile clickedTile))
            {
                HandleTitleClick(clickedTile);
            }
        }
    }

    private void HandleTitleClick(Tile tile)
    {
        if (tile.IsOccupied(out BasePiece pieceOnTile))
        {
            if(pieceOnTile == selectedPiece)
            {
                // Deselect if the same piece is clicked
                firstSelectedTile = null;
                selectedPiece = null;
                Debug.Log("Deselected piece");
                return;
            }
            else
            {
                firstSelectedTile = tile;
                selectedPiece = pieceOnTile;
                Debug.Log("Selected piece: " + selectedPiece.name);
                return;
            }
        }
        else
        {
            Debug.Log("Clicked on empty tile");
        }

        if (selectedPiece != null)
        {
            if (selectedPiece.TryMove(tile))
            {
                firstSelectedTile.RemovePiece();
                firstSelectedTile = null;
                selectedPiece = null; // Clear selection after move
            }
            else
            {
                Debug.Log("Invalid move");
            }
        }
    }
}