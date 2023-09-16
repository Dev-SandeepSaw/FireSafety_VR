using UnityEngine;
public static class GameObjectExt
{
    public  static  Component  GetComponentInChildren ( this  GameObject  self , string  type , bool  includeInactive )
    {
        var com = self.GetComponent( type );

        if ( com != null ) return com;

        foreach ( var n in self.GetComponentsInChildren<Transform>( includeInactive ) )
        {
            com = n.gameObject.GetComponent( type );

            if ( com != null ) return com;
        }

        return null;
    }
}