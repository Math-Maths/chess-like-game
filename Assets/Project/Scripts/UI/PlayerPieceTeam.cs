using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChessGame.UI
{
    public class PlayerPieceTeam : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image pieceSprite;
        [SerializeField] private Image selectionTile;

        private PieceTypeSO _pieceType;

        private int _pieceIndex;

        public PieceTypeSO Type => _pieceType;

        public void Initialize(PieceTypeSO startPiece, int index)
        {
            EventManager.Instance.AddListener<PlayerPieceTeam>(EventNameSaver.OnTeamPieceSelected, ToggleSelection);
            _pieceIndex = index;
            SetNewPiece(startPiece);
        }

        public void OnPointerClick(PointerEventData pointerEventData)
        {
            EventManager.Instance.Invoke<PlayerPieceTeam>(EventNameSaver.OnTeamPieceSelected, this);
        }

        public void SetNewPiece(PieceTypeSO newPiece, bool firstSet = true)
        {
            _pieceType = newPiece;
            pieceSprite.sprite = _pieceType.pieceSprite;

            if(!firstSet)
                TeamSelectionManager.Instance.ChangePlayerPiece(newPiece, _pieceIndex);
        }

        private void ToggleSelection(PlayerPieceTeam selected)
        {
            if(selected == this)
                selectionTile.gameObject.SetActive(true);
            else
                selectionTile.gameObject.SetActive(false);
        }
    }
}