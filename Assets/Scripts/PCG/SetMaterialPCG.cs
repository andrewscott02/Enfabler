using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMaterialPCG : MonoBehaviour
{
    public bool changeTheme = false;

    public void Setup(ThemeData theme)
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();

        if (renderer != null && theme.genericMaterial != null)
        {
            renderer.material = theme.genericMaterial;
        }
    }
}
