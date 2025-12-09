using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PiecePlacer))]
public class PiecePlacerEditor : Editor
{
    const int cellSize = 50;
    private int placeX;
    private int placeY;

    public override void OnInspectorGUI()
    {
        PiecePlacer placer = (PiecePlacer)target;

        DrawDefaultInspector();

        EditorGUILayout.Space(12);
        EditorGUILayout.LabelField("Grid Tools", new GUIStyle(EditorStyles.boldLabel) { fontSize = 20 });
        EditorGUILayout.LabelField("Left click to place, right to remove", EditorStyles.boldLabel);

        if (placer.board == null || placer.board.currentDefinition == null)
        {
            EditorGUILayout.HelpBox("Assign Board in PiecePlacer to preview grid.", MessageType.Warning);
            return;
        }

        EditorGUILayout.Space(8);
        DrawGridPreview(placer);

        /*
        EditorGUILayout.Space(12);
        EditorGUILayout.LabelField("Direct Placement", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        placeX = EditorGUILayout.IntField("X", placeX);
        placeY = EditorGUILayout.IntField("Y", placeY);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(6);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Place Piece"))
        {
            placer.PlacePieceAt(new Vector2Int(placeX, placeY));
            EditorUtility.SetDirty(placer);
        }

        if (GUILayout.Button("Remove Piece"))
        {
            placer.RemovePieceAt(new Vector2Int(placeX, placeY));
            EditorUtility.SetDirty(placer);
        }
        EditorGUILayout.EndHorizontal();
        */

        EditorGUILayout.Space(8);
        
        var buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 14 };
        var originalColor = GUI.backgroundColor;
        GUI.backgroundColor = new Color(0.8f, 0.4f, 0.4f);
        
        if (GUILayout.Button("Clear All Pieces", GUILayout.Height(40)))
        {
            placer.ClearAllPieces();
            EditorUtility.SetDirty(placer);
        }
        
        GUI.backgroundColor = originalColor;

        EditorGUILayout.Space(12);
        EditorGUILayout.LabelField("Current Pieces", EditorStyles.boldLabel);

        var def = placer.board.currentDefinition;
        int width = def.width;
        int height = def.height;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var p = GetPlacedPieceAt(placer, new Vector2Int(x, y));
                if (p != null)
                    EditorGUILayout.LabelField($"Piece at ({x}, {y}): {p.definition.name}");
            }
        }

        if (GUI.changed)
            EditorUtility.SetDirty(placer);
    }

    void DrawGridPreview(PiecePlacer placer)
    {
        var def = placer.board.currentDefinition;
        int width = def.width;
        int height = def.height;

        EditorGUILayout.LabelField("Grid Preview", EditorStyles.boldLabel);

        Rect rect = GUILayoutUtility.GetRect(
            width * cellSize,
            height * cellSize,
            GUILayout.ExpandWidth(false)
        );
        
        rect.x = (EditorGUIUtility.currentViewWidth - (width * cellSize)) / 2;

        Event e = Event.current;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Rect cell = new Rect(
                    rect.x + x * cellSize,
                    rect.y + ((height - 1 - y) * cellSize),
                    cellSize,
                    cellSize
                );

                bool hasPiece = GetPlacedPieceAt(placer, new Vector2Int(x, y)) != null;

                EditorGUI.DrawRect(cell, hasPiece ? new Color(0.4f, 0.8f, 0.4f) : new Color(0.22f, 0.22f, 0.22f));
                GUI.Box(cell, GUIContent.none);

                if (e.type == EventType.MouseDown && cell.Contains(e.mousePosition))
                {
                    if (e.button == 0)
                    {
                        placer.PlacePieceAt(new Vector2Int(x, y));
                        EditorUtility.SetDirty(placer);
                    }
                    else if (e.button == 1)
                    {
                        placer.RemovePieceAt(new Vector2Int(x, y));
                        EditorUtility.SetDirty(placer);
                    }
                    e.Use();
                }
            }
        }
    }

    BasePiece GetPlacedPieceAt(PiecePlacer placer, Vector2Int pos)
    {
        foreach (var piece in placer.placedPieces)
        {
            if (piece.gridPos == pos)
                return piece;
        }
        return null;
    }

}
