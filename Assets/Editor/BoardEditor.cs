using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoardCreator))]
public class BoardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BoardCreator creator = (BoardCreator)target;

        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();

        if (EditorGUI.EndChangeCheck())
        {
            if (GUI.changed)
            {
                creator.GenerateBoard();
            }
        }

        if (GUILayout.Button("Generate Board (Manual)"))
        {
            creator.GenerateBoard();
        }
    }
}
