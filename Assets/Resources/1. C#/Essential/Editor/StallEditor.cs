using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Stall))]
public class StallEditor : Editor
{
    SerializedProperty hasHanako;
    SerializedProperty hanakoObj;
    SerializedProperty hanakoSpawnPoint;
    SerializedProperty evtManager;

    void OnEnable(){
        hasHanako = serializedObject.FindProperty("hasHanako");
        hanakoObj = serializedObject.FindProperty("hanakoObj");
        hanakoSpawnPoint = serializedObject.FindProperty("hanakoSpawnPoint");
        evtManager = serializedObject.FindProperty("evtManager");
    }

    public override void OnInspectorGUI(){
        serializedObject.Update();

        EditorGUILayout.PropertyField(hasHanako);

        if(hasHanako.boolValue){
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(hanakoObj);
            EditorGUILayout.PropertyField(hanakoSpawnPoint);
            EditorGUILayout.PropertyField(evtManager);
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}