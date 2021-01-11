
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCube : MonoBehaviour
{
    /// <summary>
    /// UI遮挡不了3D物体
    /// </summary>
    private void OnMouseDown()
    {
        Debug.Log(gameObject.name + "  " + Time.time);
    }
}
