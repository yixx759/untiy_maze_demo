using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Anti : MonoBehaviour
{

    [SerializeField] private Material anti;
    [SerializeField, Range(0.0312f, 0.0833f)] private float FThresh;
    [SerializeField,Range(0.063f, 0.333f)] private float RThresh;
    [SerializeField,Range(0f, 1f)] private float filterMult;


    [SerializeField] private bool dig = false;

    private LocalKeyword key;
    private LocalKeyword akey;
    // Start is called before the first frame update
    void Start()
    {
        key = new LocalKeyword(anti.shader, "Diagonal");
        akey = new LocalKeyword(anti.shader, "No_Diagonal");
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        anti.SetFloat("FixThresh",FThresh);
        anti.SetFloat("RelThresh",RThresh);
        anti.SetFloat("filterMult",filterMult);
    

        if (dig)
        {
            anti.EnableKeyword(key);
            anti.DisableKeyword(akey);
        }
        else
        {
            anti.DisableKeyword(key);
            anti.EnableKeyword(akey);
        }

        Graphics.Blit(source, destination, anti);
    }
}
