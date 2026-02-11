using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;


public class PostProcess : MonoBehaviour
{
    
    [SerializeField] Material Post;
     [SerializeField , Range(0,1)] float start;
     [SerializeField , Range(0,1)] float end =1;
     [SerializeField ] float sharpness =1;
     [SerializeField ] float exposure =1;
     [SerializeField ] Color whiteBal ;

    private int noise = 0;
    
    private void Start()
    {
        this.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (Post != null)
        {
            Post.SetFloat("start", start);
            Post.SetFloat("end", end);
            Post.SetFloat("sharpness", sharpness);
            Post.SetFloat("exposure", exposure);
            Post.SetVector("whiteBal", whiteBal);
          Graphics.Blit(source, destination,Post);
          
        }
        else
        {
            Graphics.Blit(source, destination);
        }

    }
}