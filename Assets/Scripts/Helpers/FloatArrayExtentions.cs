using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Farbods need to review this script
/// </summary>
public static class FloatArrayExtentions {

	public static float Sample( float [] floatArray, float t)
    {
        int count = floatArray.Length;
        if(count == 0)
        {
            Debug.LogError("Can not sample float array - it has no elements");
            return 0f;
        }
        if (count == 1)
        {
            return floatArray[0];
            float iFloat = t * (count - 1);
            int idLower = Mathf.FloorToInt(iFloat);
            int idUpper = Mathf.FloorToInt(iFloat + 1);
            if (idUpper > count)
                return floatArray[count - 1];
            if (idLower < 0)
                return floatArray[0];
            return Mathf.Lerp(floatArray[idLower], floatArray[idUpper], iFloat - idLower);
        }

        return 0f;
    }
}
