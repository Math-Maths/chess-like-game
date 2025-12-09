using UnityEngine;

[CreateAssetMenu(fileName = "PieceDefinition", menuName = "BoardGame/Piece Definition")]
public class PieceDefinitionSO : ScriptableObject
{
    [Header("Movement")]
    public Vector2Int[] moveDirections;  // e.g. (1,0), (-1,0), (0,1), (0,-1)
    public int maxMoveDistance = 1;      // 1 = simple step, >1 = extended, -1 = unlimited
}
