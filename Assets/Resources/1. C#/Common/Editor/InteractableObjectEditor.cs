using UnityEngine;
using UnityEditor;

// UNUSED!
[CustomEditor(typeof(InteractableComponent))]
public class InteractableObjectEditor : Editor
{
    SerializedProperty interactionPrompt;
    SerializedProperty oneTimeInteraction;
    SerializedProperty interactionRange;
    SerializedProperty isMultiInteractable;
    SerializedProperty firstHasDialogue;
    SerializedProperty firstOneTime;
    SerializedProperty secondHasDialogue;
    SerializedProperty secondOneTime;

    void OnEnable(){
        interactionPrompt = serializedObject.FindProperty("interactionPrompt");
        oneTimeInteraction = serializedObject.FindProperty("oneTimeInteraction");
        interactionRange = serializedObject.FindProperty("interactionRange");
        isMultiInteractable = serializedObject.FindProperty("isMultiInteractable");
        firstHasDialogue = serializedObject.FindProperty("firstHasDialogue");
        firstOneTime = serializedObject.FindProperty("firstOneTime");
        secondHasDialogue = serializedObject.FindProperty("secondHasDialogue");
        secondOneTime = serializedObject.FindProperty("secondOneTime");
    }

    // public override void OnInspectorGUI(){
    //     serializedObject.Update();

    //     EditorGUILayout.PropertyField(interactionPrompt);
    //     EditorGUILayout.PropertyField(oneTimeInteraction);
    //     EditorGUILayout.PropertyField(interactionRange);
        
    //     EditorGUILayout.Space(10);
        
    //     EditorGUILayout.PropertyField(isMultiInteractable);
        
    //     if(isMultiInteractable.boolValue){
    //         EditorGUI.indentLevel++;
            
    //         EditorGUILayout.Space(5);
    //         EditorGUILayout.LabelField("FIRST INTERACTION", EditorStyles.boldLabel);
    //         EditorGUILayout.PropertyField(firstHasDialogue);
    //         EditorGUILayout.PropertyField(firstOneTime);
            
    //         EditorGUILayout.Space(5);
    //         EditorGUILayout.LabelField("SECOND INTERACTION", EditorStyles.boldLabel);
    //         EditorGUILayout.PropertyField(secondHasDialogue);
    //         EditorGUILayout.PropertyField(secondOneTime);
            
    //         EditorGUI.indentLevel--;
    //     }
    //     else{
    //         EditorGUI.indentLevel++;
    //         EditorGUILayout.LabelField("SINGLE INTERACTION", EditorStyles.boldLabel);
    //         EditorGUILayout.PropertyField(firstHasDialogue);
    //         EditorGUI.indentLevel--;
    //     }

    //     serializedObject.ApplyModifiedProperties();
    // }
}