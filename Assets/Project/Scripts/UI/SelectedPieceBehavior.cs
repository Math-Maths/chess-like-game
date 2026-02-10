using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ChessGame.UI
{
    public class SelectedPieceBehavior : MonoBehaviour, IDropHandler
    {
        [SerializeField] private Image selectedPiece;

        private PlayerPieceTeam _currentPiece;
        

        private void OnEnable()
        {
            EventManager.Instance.AddListener<PlayerPieceTeam>(EventNameSaver.OnTeamPieceSelected, ChangeSelectedPiece);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if(!eventData.pointerDrag.TryGetComponent<AvailablePieceSelection>(out AvailablePieceSelection availablePiece))
                return;

            selectedPiece.sprite = availablePiece.Type.pieceSprite;
            _currentPiece.SetNewPiece(availablePiece.Type, false);
        }

        private void ChangeSelectedPiece(PlayerPieceTeam pieceToSelect)
        {
            _currentPiece = pieceToSelect;
            selectedPiece.sprite = _currentPiece.Type.pieceSprite;
        }
    }
}