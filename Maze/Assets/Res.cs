using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Res : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private RenderTexture final;
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
        final.filterMode = FilterMode.Point;
        Graphics.Blit(final, destination);
    }
}
