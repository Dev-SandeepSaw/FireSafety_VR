using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class OnOffAttribute: PropertyAttribute
{
    public string variableName;
   public OnOffAttribute(string variableName){
       this.variableName = variableName;
   }
}

#if UNITY_EDITOR
[CustomPropertyDrawer( typeof( TypeDetails ) )]
public class OnOffAttributeDrawer : PropertyDrawer
{
    int selectedIndex = 0;
    OnOffAttribute OnOffAttribClass { get {return (OnOffAttribute)attribute;} }
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        var fieldPos = position;
        fieldPos.width = Mathf.Lerp(0f, position.width, 0.97f);

        label = EditorGUI.BeginProperty( position, label, property );

        // System.Object[] allAttribute =  property.GetType().GetCustomAttributes(true);

        // foreach (var item in allAttribute)
        // {
        //     Debug.Log(item);
        // }

        List<string> allTypesList = (List<string>) property.GetTargetObjectOfProperty(propertyName:"ChooseTypes");

        if(allTypesList.Count <= 0 ){
            property.SetTargetObjectOfProperty("chooseType",insideObject:true,value:null);
            return;
        } 
        
        List<string> allTypesList2 = new List<string>();
        for (int i = 0; i < allTypesList.Count; i++)
        {
            allTypesList2.Add(allTypesList[i].Replace("UnityEngine.",""));
        }
        // allTypesList2 = allTypesList;

        // Currently Selected Name
        string currentFieldName = (string) property.GetTargetObjectOfProperty("chooseType",insideObject:true);

        // Index Of Selected Name
        selectedIndex = allTypesList.IndexOf(currentFieldName);
        if(selectedIndex > allTypesList.Count || selectedIndex < 0) selectedIndex = 0;

         // Create Popup list
        selectedIndex = EditorGUI.Popup(fieldPos, "", selectedIndex, allTypesList2.ToArray());

        // Change Selected Value
        property.SetTargetObjectOfProperty("chooseType",insideObject:true,value:allTypesList[selectedIndex]);// = allTypesList[selectedIndex];


        // Currently Selected Value
        fieldPos.x += Mathf.Lerp(0f, position.width, 0.98f); 
        fieldPos.width = Mathf.Lerp(0f, position.width, 0.2f);
        bool currentBoolValue = (bool) property.GetTargetObjectOfProperty("enableDisable",insideObject:true);
        currentBoolValue = EditorGUI.Toggle(fieldPos,currentBoolValue); 
        property.SetTargetObjectOfProperty("enableDisable", insideObject:true, value:currentBoolValue);

        // Debug.Log(allTypesList.Count);
        // List<string> allTypesList = (List<string>) property.GetTargetObjectOfProperty(OnOffAttribClass.variableName);

        // foreach (var item in allTypesList)
        // {
        //     Debug.Log(item);
        // }


        EditorGUI . EndProperty ();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // if(condition)
            return base.GetPropertyHeight(property,label);
        // else return 0;
    }
}
#endif
