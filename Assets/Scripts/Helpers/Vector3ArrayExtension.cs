using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Extents the Array with Vectors
/// </summary>
public static class Vector3ArrayExtentions
{

    public static Vector3 Sample(Vector3[] NodesArray, float t)
    {
        int count = NodesArray.Length;
        if (count == 0)
        {
            Debug.LogError("Can not sample float array - it has no elements");
            return Vector3.zero;
        }
        if (count == 1)
            return NodesArray[0];
        float iNode = t * (count - 1);
        int idLower = Mathf.FloorToInt(iNode % count);
        int idUpper = Mathf.FloorToInt((iNode + 1) % count);
        if (idUpper > count)
            return NodesArray[count - 1];
        if (idLower < 0)
            return NodesArray[0];
        return Vector3.Lerp(NodesArray[idLower], NodesArray[idUpper], iNode - idLower);
    }
}
public static class Vector3FloatExtentions
{

    public static float Sample(float[] NodesArray, float t)
    {
        int count = NodesArray.Length;
        if (count == 0)
        {
            Debug.LogError("Can not sample float array - it has no elements");
            return 0;
        }
        if (count == 1)
            return NodesArray[0];
        float iNode = t * (count - 1);
        int idLower = Mathf.FloorToInt(iNode % count);
        int idUpper = Mathf.FloorToInt((iNode + 1) % count);
        if (idUpper > count)
            return NodesArray[count - 1];
        if (idLower < 0)
            return NodesArray[0];
        return Mathf.Lerp(NodesArray[idLower], NodesArray[idUpper], iNode - idLower);    
    }
}
