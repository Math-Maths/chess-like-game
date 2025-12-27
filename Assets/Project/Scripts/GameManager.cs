using UnityEngine;

namespace ChessGame
{
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PieceSide CurrentTurn { get; private set; } = PieceSide.Player;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        CurrentTurn = PieceSide.Player;
    }   

    public void ToggleTurn()
    {
        CurrentTurn = CurrentTurn == PieceSide.Player ? PieceSide.Enemy : PieceSide.Player;
    }
}

public enum PieceSide
{
    Player,
    Enemy
}
}