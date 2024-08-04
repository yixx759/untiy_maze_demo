using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Post : MonoBehaviour
{
    
    [SerializeField] private RenderTexture final;
  

    [SerializeField] private Material mat;
    [SerializeField] private Material box;
    [SerializeField] private int scale = 1;
    [SerializeField] private Vector2 downDelta ;
    [SerializeField] private Vector2 upDelta ;
    [SerializeField] private float intensity ;

    [SerializeField, Range(0, 1)] private float thresh;
    [SerializeField, Range(0, 1)] private float softthresh;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderTexture.active = final;
        RenderTexture tmp1 = RenderTexture.GetTemporary(final.width, final.height,0 );

        mat.SetFloat("thresh", thresh);
        mat.SetFloat("softthresh", softthresh);
        
        box.SetVector("upDelta", upDelta);
        box.SetVector("downDelta", downDelta);
      
        
        RenderTexture[] s = new RenderTexture[16];
//create tmp
       RenderTexture curdes=  s[0] = RenderTexture.GetTemporary(final.width, final.height);
        Graphics.Blit(final, curdes, mat);
        int height = source.height;
        int width = source.width;
        
        for (int i = 1; i <= scale; i++)
        {
            height /= 2;
            width /= 2;
            if (height < 2) {
                break;
            }
           // s[i] = new RenderTexture(1920 / i, 1080 / i, 0);
            s[i] = RenderTexture.GetTemporary(width, height);
            Graphics.Blit(curdes, s[i], box, 1);
            curdes = s[i];


        }
        
        RenderTexture tmp =curdes;
        
        for (int i = scale-1 ; i >= 0; i--)
        {
            curdes = s[i];
            s[i] = null;
            Graphics.Blit(tmp, curdes, box, 0);
            RenderTexture.ReleaseTemporary(tmp);
            tmp = curdes;


        }
        Graphics.Blit(final,tmp1 );
        Graphics.Blit(tmp1,final );
        box.SetFloat("intensity", intensity);
        Graphics.Blit(curdes , final,box, 2);
        
      
      

        RenderTexture.ReleaseTemporary(curdes);
        RenderTexture.ReleaseTemporary(tmp1);
    }
}
