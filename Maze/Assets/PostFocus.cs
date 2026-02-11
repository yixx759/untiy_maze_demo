using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class PostFocus : MonoBehaviour
{
    [SerializeField] private Material Foci;
    [SerializeField] private float focus;
    [SerializeField] private float _vignette;
    [SerializeField, Range(0,1)] private float _zoom;
    
    [SerializeField] private GameObject MSGObjectInstance;
    private Message_SPW MsgInstance;
    [SerializeField] private RenderTexture final;
    RenderTexture tmp ;
    private float timeMult = 0f;


 
    // Start is called before the first frame update
    void Start()
    {
        MsgInstance = MSGObjectInstance.GetComponent<Message_SPW>();
        tmp = RenderTexture.GetTemporary(final.width, final.height, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Foci.SetFloat("_curve", focus);
        Foci.SetFloat("_vignette", _vignette);
        Foci.SetFloat("_zoom", _zoom);


        if (MsgInstance.dirSet && MsgInstance.indexMes >-1 )
        {
            float dotToMessage = Vector3.Dot(Movement.t.forward,
                (MsgInstance.Messages[MsgInstance.indexMes].transform.position - Movement.t.position ).normalized);
            dotToMessage = dotToMessage * 0.5f + 0.5f;
         
    
        
                     
            Foci.SetFloat("_lookDir", dotToMessage*timeMult);
        
        
        }


        if (timeMult < 1f&&MsgInstance.dirSet)
        {
            timeMult += Time.deltaTime*0.5f;
            print(timeMult);
        }

        //use sin to make shake
        //do dot here
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      

        if (MsgInstance.dirSet && MsgInstance.indexMes > -1)
        {
            Graphics.Blit(final, tmp,Foci);
            Graphics.Blit(tmp, final );
        
        }
        else
        {
            
            Graphics.Blit(final, final);
        }
       
    }
}
