using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChessGame.UI
{
    public class AvailablePieceSelection : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Canvas _canvas;
        private GameObject _dragVisual;
        private Image _pieceImage;
        private ScrollRect _parentScroll;
        private PieceTypeSO _type;

        private bool _isAvailable;

        public PieceTypeSO Type => _type;


        public void Initialize(PieceTypeSO type, bool available = true)
        {
            _canvas = GetComponentInParent<Canvas>();
            _parentScroll = GetComponentInParent<ScrollRect>();
            _pieceImage = GetComponent<Image>();
            _type = type;
            _pieceImage.sprite = _type.pieceSprite;

            _isAvailable = available;
            if(!available)
                _pieceImage.color = new Color(.4f, .4f, .4f, 1);

            this.enabled = available;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(!_isAvailable)
                return;

            if (_parentScroll != null)
            _parentScroll.enabled = false;

            // Create visual copy
            _dragVisual = new GameObject("Drag Visual");
            _dragVisual.transform.SetParent(_canvas.transform, false);
            _dragVisual.transform.SetAsLastSibling();

            Image img = _dragVisual.AddComponent<Image>();
            img.sprite = _pieceImage.sprite;
            img.raycastTarget = false; 

            CanvasGroup cg = _dragVisual.AddComponent<CanvasGroup>();
            cg.alpha = 0.6f;

            RectTransform rt = _dragVisual.GetComponent<RectTransform>();
            rt.sizeDelta = GetComponent<RectTransform>().sizeDelta;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_dragVisual != null)
                _dragVisual.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_parentScroll != null)
            _parentScroll.enabled = true;
            Destroy(_dragVisual);
        }
        }
}