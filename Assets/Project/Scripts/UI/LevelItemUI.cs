using UnityEngine;
using UnityEngine.UI;

public class LevelItemUI : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Transform visualRoot; // the part we scale
    [SerializeField] private GameObject lockIcon;

    private bool _isLocked;

    public bool IsLocked => _isLocked;

    public void SetLocked(bool locked)
    {
        _isLocked = locked;
        lockIcon.SetActive(locked);
        canvasGroup.alpha = locked ? 0.5f : 1f;
    }
}
