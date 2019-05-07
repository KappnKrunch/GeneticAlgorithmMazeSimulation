using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector2Int Vector2IntClamp(Vector2Int vector2, int min, int max)
    {
        Vector2Int newVector2 = new Vector2Int 
        {
            x = Mathf.Clamp(vector2.x, min, max),
            y = Mathf.Clamp(vector2.y, min, max)
        };

        return newVector2;
    }
}
