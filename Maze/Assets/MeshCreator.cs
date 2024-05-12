using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCreator : MonoBehaviour
{

    [SerializeField] private int sx;
    [SerializeField] private int sy;
    private Vector3[] verticies;
    private Mesh mesh;
    private int[] tris;
    
    // Start is called before the first frame update


    IEnumerator delayer()
    {
        
        verticies = new Vector3[(sx + 1) * (sy + 1)];


        for (int j = 0, count =0 ; j < sy + 1; j++)
        {
            for (int i = 0; i < sx + 1; i++, count++)
            {

                verticies[count] = new Vector3(i, j);
                

            }
            
            
            
        }
        
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh =mesh;
       
       

        tris= new int[6];

        mesh.name = "YAYAYAYAY";
        mesh.vertices = verticies;
        
        
        tris[0] = 0;
        tris[1] =  sx+1;
        tris[2] =1;
        
        mesh.triangles = tris;
        yield return new WaitForSeconds(0.08f);
        tris[3] = 1;
        tris[4] =  sx+1;
        tris[5] =sx+2;
        mesh.triangles = tris;
        yield return null;



    }

    private void Awake()
    {
        StartCoroutine(delayer());
    }

    void Start()
    {
        
       



    }

    private void OnDrawGizmos()
    {
        if (verticies == null)
        {
            return;
        }
        for (int i = 0; i < verticies.Length; i++)
        {
            
            Gizmos.DrawSphere(transform.position+verticies[i],0.04f);

            
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        
        
        
        
    }
}
