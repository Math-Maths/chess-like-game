using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
public class ConditionalFieldDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (ShouldShow(property))
            return EditorGUI.GetPropertyHeight(property, label);

        return 0f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (ShouldShow(property))
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    private bool ShouldShow(SerializedProperty property)
    {
        ConditionalFieldAttribute condH = attribute as ConditionalFieldAttribute;
        SerializedProperty comparedProp = property.serializedObject.FindProperty(condH.ComparedPropertyName);

        if (comparedProp == null)
        {
            Debug.LogWarning($"ConditionalField: Property '{condH.ComparedPropertyName}' not found.");
            return true;
        }

        return comparedProp.boolValue;
    }
}
