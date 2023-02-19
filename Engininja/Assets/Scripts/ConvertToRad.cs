using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertToRad : MonoBehaviour
{
    public static float Convert(float num)
    {
        return num * (Mathf.PI / 180);
    }
}
