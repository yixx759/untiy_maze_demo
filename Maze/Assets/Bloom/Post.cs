using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Post : MonoBehaviour
{
    [SerializeField] private Material mat;
    [SerializeField, Range(0,1)] private float thresh;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        mat.SetFloat("thresh", thresh);
        Graphics.Blit(source, destination,mat );
        
    }
}
