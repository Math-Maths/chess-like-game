using UnityEditor;
using UnityEngine;
using ChessGame;
using UnityEngine.UI;

[CustomEditor(typeof(BoardCreator))]
public class BoardEditor : Editor
{
    SerializedProperty selectedBoardIndexProp;
    SerializedProperty boardsProp;
    SerializedProperty tilePrefabProp;
    SerializedProperty holderNameProp;
    SerializedProperty pieceHolderNameProp;

    private bool showBoardSettings = true;

    private void OnEnable()
    {
        selectedBoardIndexProp = serializedObject.FindProperty("selectedBoardIndex");
        boardsProp = serializedObject.FindProperty("boards");
        tilePrefabProp = serializedObject.FindProperty("tilePrefab");
        holderNameProp = serializedObject.FindProperty("holderName"); 
        pieceHolderNameProp = serializedObject.FindProperty("pieceHolderName");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
    
        //EditorGUILayout.PropertyField(tilePrefabProp);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(holderNameProp);
        EditorGUILayout.PropertyField(pieceHolderNameProp);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(selectedBoardIndexProp);
        EditorGUILayout.PropertyField(boardsProp, true);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space(10);

        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 24,
            normal = { textColor = Color.white}
        };
        GUIStyle background = new GUIStyle();
        background.normal.background = Texture2D.grayTexture;
        EditorGUILayout.BeginVertical(background);

        string buttonText = showBoardSettings ? "Hide Board Settings" : "Show Board Settings";

        if(GUILayout.Button(buttonText, headerStyle))
        {
           showBoardSettings = !showBoardSettings;
        }
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(10);

        if(showBoardSettings)
        {
            int index = selectedBoardIndexProp.intValue;

            if (index >= 0 && index < boardsProp.arraySize)
            {
                SerializedProperty element = boardsProp.GetArrayElementAtIndex(index);

                if (element.objectReferenceValue != null)
                {


                    SerializedObject so = new SerializedObject(element.objectReferenceValue);
                    so.Update();

                    SerializedProperty prop = so.GetIterator();
                    prop.NextVisible(true);

                    while (prop.NextVisible(false))
                    {
                        EditorGUILayout.PropertyField(prop, true);
                    }

                    so.ApplyModifiedProperties();
                }
            }
        }

        serializedObject.ApplyModifiedProperties();

        BoardCreator board = (BoardCreator)target;

        if (GUI.changed)
        {
            board.GenerateBoard();
        }
    }
}
