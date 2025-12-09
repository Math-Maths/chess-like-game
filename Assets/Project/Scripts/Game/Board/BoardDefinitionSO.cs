using UnityEngine;

[CreateAssetMenu(menuName = "Board/Board Definition")]
public class BoardDefinitionSO : ScriptableObject
{
    public int width = 8;
    public int height = 8;

    [Header("Tile Sprites")]
    public Sprite tileA;
    public Sprite tileB;
}
