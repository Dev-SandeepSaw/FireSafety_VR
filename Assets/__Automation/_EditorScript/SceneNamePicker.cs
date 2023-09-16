using System;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif


[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class SceneNamePicker : PropertyAttribute
{
    public bool showPath = false;
    public SceneNamePicker() { }
    public SceneNamePicker(bool showPath)
    {
        this.showPath = showPath;
    }
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneNamePicker))]
public class SceneNamePickerPropertyDrawer : PropertyDrawer
{
    private int selectedIndex;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.serializedObject.Update();
        
        SceneNamePicker picker = (SceneNamePicker)attribute;
        
        string[] validSceneNames = GetValidSceneNames(picker.showPath);
        
        // set index of popup list to current position
        string currentSceneName = property.stringValue;

        if( ! validSceneNames.Contains(currentSceneName) ) currentSceneName = ""; 

        selectedIndex = (currentSceneName == "") ? 0 : validSceneNames.ToList().IndexOf(currentSceneName);
        
        // Create Popup list
        try
        {
            selectedIndex = EditorGUI.Popup(position, property.displayName, selectedIndex, validSceneNames);
            property.stringValue = validSceneNames[selectedIndex];
        }
        catch (System.Exception)
        {
            selectedIndex = EditorGUI.Popup(position, property.displayName, 0, validSceneNames);
            property.stringValue = validSceneNames[selectedIndex];
        }
        
        property.serializedObject.ApplyModifiedProperties();
    }

    private string[] GetValidSceneNames(bool includePath)
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        string[] sceneNames = EditorBuildSettingsScene.GetActiveSceneList(scenes);
        for (int i = 0; i < sceneNames.Length; i++)
        {
            if (includePath)
            {
                sceneNames[i] = sceneNames[i].Replace("Assets/", "");
                sceneNames[i] = sceneNames[i].Replace(".unity", "");
            }
            else
            {
                sceneNames[i] = AssetDatabase.LoadAssetAtPath<SceneAsset>																		(sceneNames[i]).name;
            }
        }
        return sceneNames;
    }
}
#endif
