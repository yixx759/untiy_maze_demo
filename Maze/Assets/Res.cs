using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Res : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private RenderTexture final;
    private RenderTexture tmp;
    [SerializeField] private Material anti;
    [SerializeField, Range(0.0312f, 0.0833f)] private float FThresh;
    [SerializeField,Range(0.063f, 0.333f)] private float RThresh;
    [SerializeField,Range(0f, 1f)] private float filterMult;


    [SerializeField] private bool dig = false;
    [SerializeField] private bool antiEnable = false;

    private LocalKeyword key;
    private LocalKeyword akey;
    void Start()
    {
        key = new LocalKeyword(anti.shader, "Diagonal");
        akey = new LocalKeyword(anti.shader, "No_Diagonal");
    }

    // Update is called once per frame
    void Update()
    {
        
     
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderTexture.active = final;
        final.filterMode = FilterMode.Point;

        tmp = RenderTexture.GetTemporary(final.width, final.height,0 );
        tmp.filterMode = FilterMode.Point;


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

        if (antiEnable)
        {
            Graphics.Blit(final, tmp, anti);
            Graphics.Blit(tmp, destination);
        }
        else
        {
            Graphics.Blit(final, destination);
        }



        RenderTexture.ReleaseTemporary(tmp);



    }
}
