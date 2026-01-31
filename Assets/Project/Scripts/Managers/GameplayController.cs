using ChessGame.UI;
using UnityEngine;

namespace ChessGame.Managers
{
    public class GameplayController : MonoBehaviour
    {
        [Header("Prefabs References")]
        [SerializeField] private InputHandler _inputHandler;
        [SerializeField] private BoardCreator _boardCreator;
        [SerializeField] private GameplayUIManager _uiManager;
        [SerializeField] private Camera _mainCamera;
    }
}