#if UNITY_EDITOR
using UnityEngine;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

public static class SerializedPropertyExt
{
    public static object GetTargetObjectOfProperty(this SerializedProperty prop, string propertyName = null , bool returnObject = false, bool insideObject = false)
	{
		string path = prop.propertyPath.Replace(".Array.data[", "[");
		// Debug.Log("Path => " + path);
		object obj = prop.serializedObject.targetObject;
 		List<string> elements = new List<string>(path.Split('.'));

        if(propertyName != null && insideObject == false ) elements[elements.Count - 1 ] = propertyName;
        if(propertyName != null && insideObject == true ) elements.Add( propertyName );

        if(returnObject && propertyName == null)
        {
            elements[elements.Count - 1] = null;
        }

		foreach (string element in elements)
		{
            if(element == null) continue;
			if (element.Contains("["))
			{
				string elementName = element.Substring(0, element.IndexOf("["));
				int index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
				obj = GetValue_Imp(obj, elementName, index);

				// Debug.Log("Array Element => " + elementName + " | index => " + index);
			}
			else
			{
				// Debug.Log("Element1 => " + obj + " => " + element);
				obj = GetValue_Imp(obj, element);
				// Debug.Log("Element2 => " + obj);
			}
		}
		return obj;
	}
	public static UnityEngine.Object GetTargetUnityObjectOfProperty(this SerializedProperty prop, string propertyName = null , bool returnObject = false, bool insideObject = false)
	{
		string path = prop.propertyPath.Replace(".Array.data[", "[");
		// Debug.Log("Path => " + path);
		UnityEngine.Object obj = prop.serializedObject.targetObject;
 		List<string> elements = new List<string>(path.Split('.'));

        if(propertyName != null && insideObject == false ) elements[elements.Count - 1 ] = propertyName;
        if(propertyName != null && insideObject == true ) elements.Add( propertyName );

        if(returnObject && propertyName == null)
        {
            elements[elements.Count - 1] = null;
        }

		foreach (string element in elements)
		{
            if(element == null) continue;
			if (element.Contains("["))
			{
				string elementName = element.Substring(0, element.IndexOf("["));
				int index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
				obj = (UnityEngine.Object)  GetValue_Imp(obj, elementName, index);

				// Debug.Log("Array Element => " + elementName + " | index => " + index);
			}
			else
			{
				// Debug.Log("Element => " + element);
				obj = (UnityEngine.Object) GetValue_Imp(obj, element);
			}
		}
		return obj;
	}
	public static void SetTargetObjectOfProperty(this SerializedProperty prop, string propertyName = null , bool returnObject = false, bool insideObject = false , System.Object value = null)
	{
		string path = prop.propertyPath.Replace(".Array.data[", "[");
        // Debug.Log("Path => " + path);
        object obj = prop.serializedObject.targetObject;
        // System.TypedReference myRef = __makeref(refObj);
        List<string> elements = new List<string>(path.Split('.'));

        if(propertyName != null && insideObject == false ) elements[elements.Count - 1 ] = propertyName;
        if(propertyName != null && insideObject == true ) elements.Add( propertyName );

        if(returnObject && propertyName == null)
        {
            elements[elements.Count - 1] = null;
        }
        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i] == null) continue;
            if (elements[i].Contains("["))
            {
                string elementName = elements[i].Substring(0, elements[i].IndexOf("["));
                int index = System.Convert.ToInt32(elements[i].Substring(elements[i].IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue_Imp(obj, elementName, index);

                //Debug.Log("Array Element => " + elementName + " | index => " + index);
            }
            else
            {
                // Debug.Log($"Element => {elements[i]} | {i}");
                // Debug.Log(obj);
                // SetValue_Imp(obj, elements[i], value);

                if (obj == null)
                    return;
                var type = obj.GetType();
                while (type != null)
                {
                    var f = type.GetField(elements[elements.Count - 1], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (f != null)
                    {
                        // Debug.Log($"{f} {elements[elements.Count - 1]}");
                        // Debug.Log(prop.serializedObject.targetObject);
                        // if(type.Equals(value))

                        //New Trick
                        // prop.serializedObject.targetObject.GetType().GetField(
                        // 	elements[elements.Count - 1], 
                        // 	BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).SetValue(obj,value);
                        // f.SetValueDirect(myRef, value);
                        f.SetValue(obj, value);
                        //UnityAutomationVariable.SelectedValue transValue = (UnityAutomationVariable.SelectedValue) f.GetValue(obj);
                        //                  Debug.Log(transValue.uniObject);
                        // Debug.Log(f + "====> " + obj.GetType() + " | " + value.GetType());

                        break;
                    }
                    var p = type.GetProperty(elements[elements.Count - 1], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (p != null){
                        p.SetValue(obj, value, null);
                        break;
                    }

                    type = type.BaseType;
                }
                // Debug.Log(obj + "<=>" + elements[i]);
                //var type = source.GetType();

                //while (type != null)
                //{
                //    var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                //    if (f != null)
                //        return f.GetValue(source);

                //    var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                //    if (p != null)
                //        return p.GetValue(source, null);

                //    type = type.BaseType;
                //}
                //return null;

                obj = GetValue_Imp(obj,elements[i]);
                // myRef = __makeref(obj);
            }
        }
		//foreach (string element in elements)
		//{
  //          if(element == null) continue;
		//	if (element.Contains("["))
		//	{
		//		string elementName = element.Substring(0, element.IndexOf("["));
		//		int index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
		//		obj = GetValue_Imp(obj, elementName, index);

		//		// Debug.Log("Array Element => " + elementName + " | index => " + index);
		//	}
		//	else
		//	{
  //              Debug.Log($"Element => {element}");
  //              // Debug.Log(obj);
  //              // SetValue_Imp(obj, element, value);

  //              if (obj == null)
		//			return;
		//		var type = obj.GetType();

		//		while (type != null)
		//		{
		//			var f = type.GetField(element, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
		//			if (f != null)
  //                  {
  //                      Debug.Log(f + "==> " + obj.GetType() + " | " + value.GetType());
  //                      //try
  //                      //{
  //                      f.SetValue(obj, value);
  //                      //}
  //                      //catch (System.Exception)
  //                      //{
  //                      Debug.Log(f + "====> " + obj.GetType() + " | " + value.GetType());
  //                      //}

  //                  }


  //                  var p = type.GetProperty(element, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
		//			if (p != null)
		//				p.SetValue(obj, value, null);

		//			type = type.BaseType;
		//		}
		//	}
		//}
		// return obj;
	}


    private static void SetValue_Imp(object source, string name, System.Object value)
    {
        if (source == null)
            return;
        var type = source.GetType();

        while (type != null)
        {
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f != null)
                f.SetValue(source, value);

            var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p != null)
                p.SetValue(source, value, null);

            type = type.BaseType;
        }
        // return null;
    }


    private static object GetValue_Imp(object source, string name)
	{
		if (source == null)
			return null;
		var type = source.GetType();

		while (type != null)
		{
			var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if (f != null)
				return f.GetValue(source);

			var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
			if (p != null)
				return p.GetValue(source, null);

			type = type.BaseType;
		}
		return null;
	}
	private static object GetValue_Imp(object source, string name, int index)
	{
		var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
		if (enumerable == null) return null;
		var enm = enumerable.GetEnumerator();
		//while (index-- >= 0)
		//    enm.MoveNext();
		//return enm.Current;

		for (int i = 0; i <= index; i++)
		{
            
			if (!enm.MoveNext()) return null;
		}
		return enm.Current;
	}
}
#endif