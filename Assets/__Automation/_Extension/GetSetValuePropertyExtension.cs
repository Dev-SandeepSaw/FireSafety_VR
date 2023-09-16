using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public static class GetSetValuePropertyExtension
{
    /// <summary>
    /// Set Any Property of a Component
    /// </summary>
    /// <param name="comp">Component</param>
    /// <param name="propertyName">Which Property To Change</param>
    /// <param name="value">Change Value</param>
    public static void SetProperty(this Component comp, string propertyName, object value)
    {
        if (comp == null) return;
        PropertyInfo propertyInfo = comp.GetType().GetProperty(propertyName);
        if(propertyInfo != null)
            propertyInfo.SetValue(comp,value);
    }
    /// <summary>
    /// Set Any Property of a Array Component
    /// </summary>
    /// <param name="comp">Component Array</param>
    /// <param name="propertyName">Which Property To Change</param>
    /// <param name="value">Change Value</param>
    public static void SetProperty(this Component[] comp, string propertyName, object value)
    {
        if (comp == null) return;
        int count = comp.Length;
        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            if (comp[i] == null) return;
            PropertyInfo propertyInfo = comp[i].GetType().GetProperty(propertyName);
            if (propertyInfo != null)
                propertyInfo.SetValue(comp[i], value);
        }
        
    }
    /// <summary>
    /// Set Any Property of a List Component
    /// </summary>
    /// <param name="comp">Component List</param>
    /// <param name="propertyName">Which Property To Change</param>
    /// <param name="value">Change Value</param>
    public static void SetProperty(this List<Component> comp, string propertyName, object value)
    {
        if (comp == null) return;
        int count = comp.Count;
        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            if (comp[i] == null) return;
            PropertyInfo propertyInfo = comp[i].GetType().GetProperty(propertyName);
            if (propertyInfo != null)
                propertyInfo.SetValue(comp[i], value);
        }

    }
    /// <summary>
    /// Get Any Property of a Component
    /// </summary>
    /// <param name="comp">Component</param>
    /// <param name="propertyName">Which Property To Get</param>
    public static System.Object GetProperty(this Component comp, string propertyName)
    {
        if (comp == null) return null;
        PropertyInfo propertyInfo = comp.GetType().GetProperty(propertyName);
        if(propertyInfo == null) return null;
        return propertyInfo.GetValue(comp);
    }
}
