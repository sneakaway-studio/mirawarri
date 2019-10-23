using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;

public class PrintAllGOs : MonoBehaviour
{


    [MenuItem("Editor/PrintAllGOs")]
    static void PrintAllGO()
    {
        PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (Object unityObject in GameObject.FindObjectsOfType<Object>())
        {
            SerializedObject serializedObject = new SerializedObject(unityObject);
            inspectorModeInfo.SetValue(serializedObject, InspectorMode.Debug, null);

            SerializedProperty localIdProp = serializedObject.FindProperty("m_LocalIdentfierInFile");

            int localId = localIdProp.intValue;
            Debug.Log(localId + " -> " + unityObject.name);
        }
    }
}