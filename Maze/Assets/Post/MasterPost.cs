using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MasterPost : MonoBehaviour
{
      
  
    [SerializeField] Material Post;
    [SerializeField , Range(0,1)] float start;
    [SerializeField , Range(0,1)] float end =1;
    [SerializeField ] float sharpness =1;
    [SerializeField ] float exposure =1;
    [SerializeField ] Color whiteBal ;
    [SerializeField] private RenderTexture final;
    
    
    
    // [SerializeField] private Material Foci;
    // [SerializeField] private float focus;
    // [SerializeField] private float _vignette;
    // [SerializeField, Range(0,1)] private float _zoom;
    //
    // [SerializeField] private GameObject MSGObjectInstance;
    // private Message_SPW MsgInstance;
    
    
    
    
    
    
    
    // [SerializeField] float a;
    // [SerializeField] float b;
    // [SerializeField] int test;
    // [SerializeField] int detailp = 100;
    // [SerializeField] int detailp2 = 100;
    // [SerializeField] float blurp = 2;
    // [SerializeField] float blurp2 = 2;

    // [SerializeField] private Texture[] nn;
    private int noise = 0;
    RenderTexture tmp ;



    private void Start()
    {
        RenderTexture.active = final;
    //    MsgInstance = MSGObjectInstance.GetComponent<Message_SPW>();
        
        this.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
       tmp = new RenderTexture(final.width, final.height, 0);

        
    }
    
    // void Update()
    // {
    //     Foci.SetFloat("_curve", focus);
    //     Foci.SetFloat("_vignette", _vignette);
    //     Foci.SetFloat("_zoom", _zoom);
    //
    //
    //     if (MsgInstance.dirSet && MsgInstance.indexMes >-1 )
    //     {
    //         float dotToMessage = Vector3.Dot(Movement.t.forward,
    //             (MsgInstance.Messages[MsgInstance.indexMes].transform.position - Movement.t.position ).normalized);
    //         dotToMessage = dotToMessage * 0.5f + 0.5f;
    //         print(dotToMessage);
    //     
    //             
    //         Foci.SetFloat("_lookDir", dotToMessage);
    //     
    //     
    //     }
    //     
    //     
    //     
    //     //use sin to make shake
    //     //do dot here
    // }
    //
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (Post != null && !EditorApplication.isPaused  )
        {
           RenderTexture.active = final;  
           final.filterMode = FilterMode.Point;
            // Post.SetFloat("offsetx", Random.value);
            // Post.SetFloat("offsety", Random.value);
            // Post.SetTexture("_Col", nn[(noise++)%nn.Length]);
            //   Post.SetFloat("a", a);
            //   Post.SetFloat("b", b);
            
            Post.SetFloat("start", start);
            Post.SetFloat("end", end);
            Post.SetFloat("sharpness", sharpness);
            Post.SetFloat("exposure", exposure);
            Post.SetVector("whiteBal", whiteBal);
            Graphics.Blit(final, tmp,Post);
            Graphics.Blit(tmp, final);
            
       
       
          
        }
        else
        {
            Graphics.Blit(final, tmp);
            Graphics.Blit(tmp, final);
        }

    
        
        //
        // if (MsgInstance.dirSet && MsgInstance.indexMes > -1)
        // {
        //     Graphics.Blit(source, destination, Foci);
        // }
    }
}
