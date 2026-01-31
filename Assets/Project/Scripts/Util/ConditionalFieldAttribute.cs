using UnityEngine;

public class ConditionalFieldAttribute : PropertyAttribute
{
    public string ComparedPropertyName { get; private set; }

    public ConditionalFieldAttribute(string comparedPropertyName)
    {
        ComparedPropertyName = comparedPropertyName;
    }
}
