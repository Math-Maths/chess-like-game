using ChessGame.Managers;
using TMPro;
using UnityEngine;

namespace ChessGame.UI
{
    public class GameplayUIManager : MonoBehaviour
    {
        [Header("End Screen References")]
        [SerializeField] private GameObject gameOverScreen;
        [SerializeField] private TMP_Text gameResult;
        [SerializeField] private string playerWinText;
        [SerializeField] private string enemyWinText;
        [SerializeField] private string drawText;

        public void Initialize()
        {
            EventManager.Instance.AddListener<PieceSide>(EventNameSaver.OnKingCapture, ShowGameOverScreen);
        }

        private void OnDisable()
        {
            EventManager.Instance.RemoveListener<PieceSide>(EventNameSaver.OnKingCapture, ShowGameOverScreen);
        }

        private void ShowGameOverScreen(PieceSide winner)
        {
            EventManager.Instance.Invoke(EventNameSaver.OnGameOver);

            if(winner == PieceSide.Player)
                gameResult.text = playerWinText;
            else if(winner == PieceSide.Enemy)
                gameResult.text = enemyWinText;
            else
                gameResult.text = drawText;

            gameOverScreen.SetActive(true);
        }

        public void ResetGame()
        {
            Debug.Log("Reset");
        }

        public void GoToMenu()
        {
            Debug.Log("Going to menu");
        }
    }
}