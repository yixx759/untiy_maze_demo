using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chroma : MonoBehaviour
{
    
    
    [SerializeField] Material chromatic;
    [SerializeField] Vector2 redoff;
    [SerializeField] Vector2 blueoff;
    [SerializeField] Vector2 greenoff;
    [SerializeField] private RenderTexture final;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
       RenderTexture.active = final;  
         chromatic.SetVector("_RedOffset", redoff);
         chromatic.SetVector("_GreenOffset", greenoff);
         chromatic.SetVector("_BlueOffset", blueoff);
 
        Graphics.Blit(final, destination,chromatic);
    }
}
