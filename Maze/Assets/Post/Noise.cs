using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Noise : MonoBehaviour
{
    
    [SerializeField] private RenderTexture final;
    [SerializeField] private Material noise;
    [SerializeField] private float a;
    [SerializeField] private float b;
  
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderTexture tmp = RenderTexture.GetTemporary(final.width, final.height, 0);
        
        noise.SetFloat("a", a);
        noise.SetFloat("b", b);
        noise.SetFloat("offsetx", Random.value);
    
        noise.SetFloat("offsety", Random.value);
        
        Graphics.Blit(final, tmp,noise);
        Graphics.Blit(tmp, final);
        RenderTexture.ReleaseTemporary(tmp);
    }
}
