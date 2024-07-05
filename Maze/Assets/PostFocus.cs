using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostFocus : MonoBehaviour
{
    [SerializeField] private Material Foci;
    [SerializeField] private float focus;
    [SerializeField] private float _vignette;
    
    [SerializeField] private GameObject MSGObjectInstance;
    private Message_SPW MsgInstance;
    
    
    // Start is called before the first frame update
    void Start()
    {
        MsgInstance = MSGObjectInstance.GetComponent<Message_SPW>();
    }

    // Update is called once per frame
    void Update()
    {
        Foci.SetFloat("_curve", focus);
        Foci.SetFloat("_vignette", _vignette);


        if (MsgInstance.dirSet && MsgInstance.indexMes >-1 )
        {
            float dotToMessage = Vector3.Dot(Movement.t.forward,
                (MsgInstance.Messages[MsgInstance.indexMes].transform.position - Movement.t.position ).normalized);
            dotToMessage = dotToMessage * 0.5f + 0.5f;
            print(dotToMessage);
        
                
            Foci.SetFloat("_lookDir", dotToMessage);
        
        
        }
        
        
        
        //use sin to make shake
        //do dot here
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (MsgInstance.dirSet && MsgInstance.indexMes > -1)
        {
            Graphics.Blit(source, destination, Foci);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
