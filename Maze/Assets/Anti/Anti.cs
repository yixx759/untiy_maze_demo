using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anti : MonoBehaviour
{

    [SerializeField] private Material anti;
    [SerializeField, Range(0.0312f, 0.0833f)] private float FThresh;
    [SerializeField,Range(0.063f, 0.333f)] private float RThresh;
    [SerializeField,Range(0f, 1f)] private float filterMult;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        anti.SetFloat("FixThresh",FThresh);
        anti.SetFloat("RelThresh",RThresh);
        anti.SetFloat("filterMult",filterMult);
        Graphics.Blit(source, destination, anti);
    }
}
