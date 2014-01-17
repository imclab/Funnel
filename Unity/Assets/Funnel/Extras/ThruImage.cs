using UnityEngine;
using System.Collections;

public class ThruImage : MonoBehaviour
{
    public Material material;

    void OnRenderImage (RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit (src, dest, material);
    }
}
