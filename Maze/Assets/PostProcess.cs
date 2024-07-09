using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
            Post.SetFloat("seed", Random.value);
            Graphics.Blit(source, destination, Post);
        }
        else
        {
            Graphics.Blit(source, destination);
        }

    }
}