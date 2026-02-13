using ChessGame.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ChessGame.Managers
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private TeamSelectionManager _teamSelection;
        [SerializeField] private EventSystem _eventSystem;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private MainMenuScreenControl _mainMenuScreenControl;

        private async void Start()
        {
            BindEvents();
            BindObjects();
            //Show loading screen
            await InitializeObjects();
            PrepareMenu();
        }

        private void BindEvents()
        {
            EventManager.Instance.AddListener(EventNameSaver.OnTeamSelectionOpen, OpenTeamSelection);
            EventManager.Instance.AddListener(EventNameSaver.OnMainMenuOpen, OpenMainMenu);
        }

        private void OnDisable()
        {
            EventManager.Instance.RemoveListener(EventNameSaver.OnTeamSelectionOpen, OpenTeamSelection);
            EventManager.Instance.RemoveListener(EventNameSaver.OnMainMenuOpen, OpenMainMenu);
        }

        private void BindObjects()
        {
            _teamSelection = Instantiate(_teamSelection);
            _mainCamera = Instantiate(_mainCamera);
            _eventSystem = Instantiate(_eventSystem);
            _mainMenuScreenControl = Instantiate(_mainMenuScreenControl);
        }

        private async Awaitable InitializeObjects()
        {
            _teamSelection.Initialize(GameManager.Instance.GetUnlockedPiecesIDs());
            _mainMenuScreenControl.Initialize(GameManager.Instance.GetUnlockedLevels());
        }

        private void PrepareMenu()
        {
            _teamSelection.gameObject.SetActive(false);
            _mainCamera.gameObject.SetActive(true);
            _eventSystem.gameObject.SetActive(true);
            _mainMenuScreenControl.gameObject.SetActive(true);
        }

        private void OpenTeamSelection()
        {
            _teamSelection.gameObject.SetActive(true);
            _teamSelection.Initialize(GameManager.Instance.GetUnlockedPiecesIDs());
        }

        private void OpenMainMenu()
        {
            _mainMenuScreenControl.gameObject.SetActive(true);
            _mainMenuScreenControl.Initialize(GameManager.Instance.GetUnlockedLevels());
        }
    }
}