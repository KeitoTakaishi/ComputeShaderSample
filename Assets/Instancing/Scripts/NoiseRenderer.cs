using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseRenderer : MonoBehaviour
{
    public Material NoiseMaterial;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, NoiseMaterial);
    }
}
