using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcess : MonoBehaviour
{
    
    [SerializeField] Material Post;

    private void Start()
    {
        this.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (Post != null)
        {
            Graphics.Blit(source, destination, Post);
        }
        else
        {
            Graphics.Blit(source, destination);
        }

    }
}