using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostFocus : MonoBehaviour
{
    [SerializeField] private Material Foci;
    [SerializeField] private float focus;
    [SerializeField] private float _vignette;
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Foci.SetFloat("_curve", focus);
        Foci.SetFloat("_vignette", _vignette);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, Foci);
    }
}
