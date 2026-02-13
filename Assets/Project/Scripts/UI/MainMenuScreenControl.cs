using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChessGame.Managers
{
    public class MainMenuScreenControl : MonoBehaviour
    {
        [Header("Carousel Settings and References")]
        [SerializeField] private RectTransform container;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private float slideDuration = 0.35f;
        [SerializeField] private float itemWidth = 560f;
        [SerializeField] private AnimationCurve easingCurve;

        [Header("Buttons Reference")]
        [SerializeField] private Image playButtonImage;
        [SerializeField] private TMP_Text playText;
    
        private int _currentIndex = 0;
        private int _totalLevels;
        private int _unlockedLevels = 0;
        private Coroutine _slideRoutine;
        private LevelItemUI[] _levelItems;
        private Color _playOriginalColor;
        private Color _originalPlayTextColor;
    
        public void Initialize(int unlockedLevels)
        {
            _unlockedLevels = unlockedLevels;
            _totalLevels = container.childCount;
            _levelItems = container.GetComponentsInChildren<LevelItemUI>();
            _playOriginalColor = playButtonImage.color;
            _originalPlayTextColor = playText.color;
    
            LoadLevelLocks();
            UpdatePositionInstant();
        }

        private void Start()
        {
            _totalLevels = container.childCount;
            _levelItems = container.GetComponentsInChildren<LevelItemUI>();
            _playOriginalColor = playButtonImage.color;
            _originalPlayTextColor = playText.color;
    
            LoadLevelLocks();
            UpdatePositionInstant();
        }

        #region Buttons
    
        public void NextLevel()
        {
            if (_slideRoutine != null) return;
    
            _currentIndex++;
            if (_currentIndex >= _totalLevels)
                _currentIndex = 0;
    
            CheckLevelLocked();
            SlideToCurrent();
        }
    
        public void PreviousLevel()
        {
            if (_slideRoutine != null) return;
    
            _currentIndex--;
            if (_currentIndex < 0)
                _currentIndex = _totalLevels - 1;
    
            CheckLevelLocked();
            SlideToCurrent();
        }

        public void OpenTeamSelection()
        {
            EventManager.Instance.Invoke(EventNameSaver.OnTeamSelectionOpen);
        }
    
        #endregion

        private void CheckLevelLocked()
        {
            if(_currentIndex > _unlockedLevels)
            {
                playButtonImage.color = new Color(.4f,.4f,.4f, 1f);
                playText.color = new Color(.2f,.2f,.2f, 1f);
            }
            else
            {
                playButtonImage.color = _playOriginalColor;
                playText.color = _originalPlayTextColor;
            }
        }
    
        private void SlideToCurrent()
        {
            _slideRoutine = StartCoroutine(SmoothSlide());
        }
    
        private IEnumerator SmoothSlide()
        {
            leftButton.interactable = false;
            rightButton.interactable = false;
    
            Vector2 startPos = container.anchoredPosition;
            Vector2 targetPos = new Vector2(-_currentIndex * itemWidth, startPos.y);
    
            float time = 0f;
    
            while (time < slideDuration)
            {
                time += Time.deltaTime;
                float t = time / slideDuration;
                float eased = easingCurve.Evaluate(t);
    
                container.anchoredPosition = Vector2.Lerp(startPos, targetPos, eased);
    
    
                yield return null;
            }
    
            container.anchoredPosition = targetPos;
    
            leftButton.interactable = true;
            rightButton.interactable = true;
            _slideRoutine = null;
        }
    
        private void UpdatePositionInstant()
        {
            container.anchoredPosition = new Vector2(-_currentIndex * itemWidth, 0);
        }
    
        private void LoadLevelLocks()
        {
            for (int i = 0; i < _levelItems.Length; i++)
            {
                bool locked = i > _unlockedLevels;
                _levelItems[i].SetLocked(locked);
            }
        }
    }
}