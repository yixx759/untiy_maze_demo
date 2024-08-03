using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Res : MonoBehaviour
{
    enum Quality
    {
        Low,
        Medium,
        High
    }


    // Start is called before the first frame update
    [SerializeField] private RenderTexture final;
    private RenderTexture tmp;


    [SerializeField] private Material anti;
    [SerializeField, Range(0.0312f, 0.0833f)] private float FThresh;
    [SerializeField,Range(0.063f, 0.333f)] private float RThresh;
    [SerializeField,Range(0f, 1f)] private float filterMult;
    [SerializeField] private Quality Q;


    [SerializeField] private bool dig = false;
    [SerializeField] private bool antiEnable = false;
    [SerializeField] private bool en = false;
    
    private LocalKeyword key;
    private LocalKeyword akey;
    
    private LocalKeyword LowQ;
    private LocalKeyword MidQ;
    void Start()
    {
        key = new LocalKeyword(anti.shader, "Diagonal");
        akey = new LocalKeyword(anti.shader, "No_Diagonal");
        LowQ = new LocalKeyword(anti.shader, "LOW");
        MidQ = new LocalKeyword(anti.shader, "MID");
    }

    // Update is called once per frame
    void Update()
    {
        
     
    }

    void Quali()
    {
        if (Q == Quality.Low)
        {
            anti.EnableKeyword(LowQ);
            anti.DisableKeyword(MidQ);
            
        }
        else if (Q == Quality.Medium)
        {
            anti.DisableKeyword(LowQ);
            anti.EnableKeyword(MidQ);
        }
        else
        {
            anti.DisableKeyword(LowQ);
            anti.DisableKeyword(MidQ);
        }
        
        
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
        anti.SetInteger("enab",en ? 1 : 0);


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
        
        
        Quali();
        
        
        
        

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
