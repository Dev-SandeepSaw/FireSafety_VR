using UnityAutomation; // For AllCondition
using System; // For Convert / Tuple
using UnityEngine; // For Debug
using System.Collections.Generic; // for List
public static class AllConditionExtension
{
    public static void SetIfElseConditionVariable(this AllCondition[] IfElseCondition)
    {
        if(IfElseCondition == null) return;
        for (int z = 0; z < IfElseCondition.Length; z++)
        {
            IfElseCondition[z].SetIfElseConditionVariable();
        }
    }
    public static void SetIfElseConditionVariable(this AllCondition IfElseCondition)
    {
        if(IfElseCondition == null) return;
        if(IfElseCondition.unityObj == null) return;
        //------------------------------For First Variable-------------------------
        ReflectionExtension.putSelectedValue(ref IfElseCondition.selectedMethodInfo1, 
                                            ref IfElseCondition.methodParamTypes1,
                                            ref IfElseCondition.returnType1,
                                            ref IfElseCondition.parameters1,
                                            IfElseCondition.unityObj, IfElseCondition.selectField);
        
        ReflectionExtension.putSelectedValue(ref IfElseCondition.selectedField1, IfElseCondition.unityObj, IfElseCondition.selectField);
        ReflectionExtension.putSelectedValue(ref IfElseCondition.selectedProperty1, IfElseCondition.unityObj, IfElseCondition.selectField);
        
        //------------------------------For Second Variable-------------------------
        if(IfElseCondition.compareWith == ValueOrGM.Value)
        {
            ReflectionExtension.putSelectedValue(ref IfElseCondition.valueCompare, IfElseCondition.selectedField1, IfElseCondition.selectedProperty1);
        }
        else
        {
            ReflectionExtension.putSelectedValue(ref IfElseCondition.selectedMethodInfo2, 
                                            ref IfElseCondition.methodParamTypes2,
                                            ref IfElseCondition.returnType2,
                                            ref IfElseCondition.parameters2,
                                            IfElseCondition.unityObj, IfElseCondition.selectField2);
                
            
            ReflectionExtension.putSelectedValue(ref IfElseCondition.selectedField2, IfElseCondition.unityObj2, IfElseCondition.selectField2);
            ReflectionExtension.putSelectedValue(ref IfElseCondition.selectedProperty2, IfElseCondition.unityObj2, IfElseCondition.selectField2);
        }
    }

    public static bool checkAllCondition(this AllCondition[] IfElseCondition)
    {
        List<bool> allBool = new List<bool>();
        List< Tuple <bool, MultipleCondition> > allCheckedBool = new List< Tuple <bool, MultipleCondition> >();
        bool? prevBool = null;

        allCheckedBool.Clear();

        // Store AllBool
        allBool.Clear();
        for (int i = 0; i < IfElseCondition.Length; i++)
        {
            allBool.Add( isConditionTrue(IfElseCondition[i]) );
        }

        bool openBracketFound = false;
        bool closeBracketFound = false;
        bool noneFound = false;
        // store all boolean operation
        for (int i = 0; i < IfElseCondition.Length; i++)
        {
            MultipleCondition andOr = IfElseCondition[i].AndOR;

            // if close bracket found
            if(andOr.isCloseFound())
            {
                closeBracketFound = true;
                openBracketFound = false;
                if( andOr.isORFound() ) allCheckedBool.Add( new Tuple<bool, MultipleCondition>( prevBool ?? false, MultipleCondition.OR) );
                if( andOr.isANDFound() ) allCheckedBool.Add( new Tuple<bool, MultipleCondition>( prevBool ?? false, MultipleCondition.AND ) );

                prevBool = null;
            } 

            // if none found or )&& or )||
            if(andOr == MultipleCondition.NULL || andOr == MultipleCondition.CloseBracketAND || andOr == MultipleCondition.CloseBracketOR) 
            {
                noneFound = true;
                prevBool = allBool[i];
            }

            // if Open bracket found
            if( andOr.isOpenFound() )
            {
                openBracketFound = true;
                closeBracketFound = false;
                if(noneFound)
                {
                    if( andOr.isORFound() ) allCheckedBool.Add( new Tuple<bool, MultipleCondition>( prevBool ?? false, MultipleCondition.OR) );
                    if( andOr.isANDFound() ) allCheckedBool.Add( new Tuple<bool, MultipleCondition>( prevBool ?? false, MultipleCondition.AND ) );
                    noneFound = false;
                }
                prevBool = allBool[i];
            } 

            // if [open close none] not found
            if(!openBracketFound && !closeBracketFound && !noneFound)
            {
                if( andOr.isORFound() ) allCheckedBool.Add( new Tuple<bool, MultipleCondition>( allBool[i], MultipleCondition.OR ) );
                if( andOr.isANDFound() ) allCheckedBool.Add( new Tuple<bool, MultipleCondition>( allBool[i], MultipleCondition.AND ) );
            }

            // Operation for inside bracket
            if(prevBool != null)
            {
                if(andOr.isANDFound())
                {
                    prevBool = (prevBool ?? false ) && allBool[i];
                }
                if(andOr.isORFound())
                {
                    prevBool = (prevBool ?? false ) || allBool[i];
                }
            }
            if(i == IfElseCondition.Length - 1){
                if(openBracketFound && !closeBracketFound || prevBool != null) 
                    allCheckedBool.Add( new Tuple<bool, MultipleCondition>( prevBool?? false, MultipleCondition.NULL ) );
                else
                    allCheckedBool.Add( new Tuple<bool, MultipleCondition>( allBool[i], MultipleCondition.NULL ) );
            }
        }
        // #if UNITY_EDITOR
        // foreach (var item in allCheckedBool)
        // {
        //     Debug.Log(item.Item1 + " " + item.Item2);
        // }
        // #endif
        bool? result = null;
        if(allCheckedBool.Count == 1) result = allCheckedBool[0].Item1;
        for (int i = 0; i < allCheckedBool.Count -1 ; i++)
        {
            if(result != null)
            {
                bool nextBool = allCheckedBool[i+1].Item1;
                if(allCheckedBool[i].Item2 == MultipleCondition.AND) result = (result ?? false && nextBool);
                if(allCheckedBool[i].Item2 == MultipleCondition.OR) result = (result ?? false || nextBool);
            }else
            {
                bool firstBool = allCheckedBool[i].Item1;
                
                bool secondBool = allCheckedBool[i+1].Item1;

                if(allCheckedBool[i].Item2 == MultipleCondition.AND) result = (firstBool && secondBool);
                if(allCheckedBool[i].Item2 == MultipleCondition.OR) result = (firstBool || secondBool);
            }
        }
        #if UNITY_EDITOR
        Debug.Log("Final Result => "+ result);
        #endif
        return result ?? false;
    }

    public static bool isConditionTrue(AllCondition IfElseCondition)
    {
        if(IfElseCondition.selectedField1 == null && IfElseCondition.selectedProperty1 == null && IfElseCondition.selectedMethodInfo1 == null)
        {
            IfElseCondition.SetIfElseConditionVariable();
        }
        
        System.Object firstValue,secondValue;
        firstValue = ReflectionExtension.GetValue(IfElseCondition.selectedField1,
                                                    IfElseCondition.selectedProperty1,
                                                    IfElseCondition.selectedMethodInfo1,
                                                    IfElseCondition.methodParamTypes1,
                                                    IfElseCondition.parameters1,
                                                    IfElseCondition.unityObj);
        if(IfElseCondition.compareWith == ValueOrGM.Value)
        {
            secondValue = IfElseCondition.valueCompare.Value();
        }else{
            secondValue = ReflectionExtension.GetValue(IfElseCondition.selectedField2,
                                                    IfElseCondition.selectedProperty2,
                                                    IfElseCondition.selectedMethodInfo2,
                                                    IfElseCondition.methodParamTypes2,
                                                    IfElseCondition.parameters2,
                                                    IfElseCondition.unityObj2);
        }
        secondValue = Convert.ChangeType(secondValue, firstValue.GetType());
        
        // IComparable comparableSecondValue = (IComparable) secondValue;
        // IComparable comparableFirstValue = (IComparable) firstValue;

        // Debug.Log( comparableFirstValue.CompareTo(secondValue) );

        bool isBothNumeric = ( IsValueNumeric(firstValue) && IsValueNumeric(secondValue) ) ? true:false;

        long firstLong = 0,secondLong = 0;
        if(isBothNumeric)
        {
            firstLong = Convert.ToInt64(firstValue);
            secondLong = Convert.ToInt64(secondValue);
        }

        int operatorEnum = (int) IfElseCondition.@operator;
        // #if UNITY_EDITOR
        // string[] operatorArray = new string[] { "==", "!=", ">", ">=", "<", "<=" };
        // Debug.Log(firstValue + " " + operatorArray[operatorEnum] + " " + secondValue);
        // #endif
        switch(operatorEnum)
        {
            //Equal To
            case 0:
                if(firstValue.GetHashCode() == secondValue.GetHashCode()) return true;
                else return false;
                // break;
            
            //Not Equal To
            case 1:
                if(firstValue.GetHashCode() != secondValue.GetHashCode()) return true;
                else return false;
                // break;
            
            //Greater Than
            case 2:
                if(!isBothNumeric) return false;
                if(firstLong > secondLong) return true;
                    else return false;
                // break;
            
            //Greater Than Equal To
            case 3:
                if(!isBothNumeric) return false;
                if(firstLong >= secondLong) return true;
                    else return false;
                // break;
            
            //Less Than
            case 4:
                if(!isBothNumeric) return false;
                if(firstLong < secondLong) return true;
                    else return false;
                // break;
            
            //Less Than Equal To
            case 5:
                if(!isBothNumeric) return false;
                if(firstLong <= secondLong) return true;
                    else return false;
                // break;
        }
        return false;
    }
    public static bool IsValueNumeric( System.Object value )
    {
        Type type = value.GetType();
        if (type == null)
        {
            return false;
        }

        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Byte:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.SByte:
            case TypeCode.Single:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
                return true;
            // case TypeCode.Object:
            //     if ( type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            //     {
            //         return IsNumericType(Nullable.GetUnderlyingType(type));
            //     }
            //     return false;
        }
        return false;
    }
}
