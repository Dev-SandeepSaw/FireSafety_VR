using UnityAutomation;
public static class MultipleConditionExtension
{
    public static bool isANDFound(this MultipleCondition andOr)
    {
        if(andOr == MultipleCondition.AND || andOr == MultipleCondition.CloseBracketAND || andOr == MultipleCondition.CloseBracketANDOpenBracket || andOr == MultipleCondition.ANDOpenBracket)
        {
            return true;
        }else
        {
            return false;
        }
    }
    public static bool isORFound(this MultipleCondition andOr)
    {
        if(andOr == MultipleCondition.OR || andOr == MultipleCondition.CloseBracketOR || andOr == MultipleCondition.CloseBracketOROpenBracket || andOr == MultipleCondition.OROpenBracket)
        {
            return true;
        }else
        {
            return false;
        }
    }
    public static bool isCloseFound(this MultipleCondition andOr)
    {
        if(andOr == MultipleCondition.CloseBracket || 
                andOr == MultipleCondition.CloseBracketOR || 
                andOr == MultipleCondition.CloseBracketOROpenBracket || 
                andOr == MultipleCondition.CloseBracketAND || 
                andOr == MultipleCondition.CloseBracketANDOpenBracket)
        {
            return true;
        }else
        {
            return false;
        }
    }
    public static bool isOpenFound(this MultipleCondition andOr)
    {
        if(andOr == MultipleCondition.OpenBracket ||
                andOr == MultipleCondition.CloseBracketOROpenBracket ||
                andOr == MultipleCondition.CloseBracketANDOpenBracket ||
                andOr == MultipleCondition.ANDOpenBracket ||
                andOr == MultipleCondition.OROpenBracket)
        {
            return true;
        }else
        {
            return false;
        }
    }
}
