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
        Vector2[] uv = new Vector2[verticies.Length];
        Vector4[] tangent = new Vector4[verticies.Length];
        

        for (int j = 0, count =0 ; j < sy + 1; j++)
        {
            for (int i = 0; i < sx + 1; i++, count++)
            {

                verticies[count] = new Vector3(i, j);
                uv[count] = new Vector2((float)i / sx, (float)j / sy);
                tangent[count] = new Vector4(1.0f, 0, 0, -1);

            }
            
            
            
        }
        
        
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh =mesh;
       
       

        tris= new int[verticies.Length*6];

        mesh.name = "YAYAYAYAY";
        mesh.vertices = verticies;
        int counter = 0;
        //seperate x and y
        for (int i = 0, vi = 0; i < sy; i++, vi++)
        {
            for (int j = 0; j < sx; j++ , vi++)
            {
                tris[counter++] = vi;
                tris[counter++] =  sx+1+vi;
                tris[counter++] =1+vi;
        
                mesh.triangles = tris;
                yield return new WaitForSeconds(0.06f);
                tris[counter++] = 1+vi;
                tris[counter++] =  sx+1+vi;
                tris[counter++] =sx+2+vi;
                mesh.triangles = tris;
                yield return new WaitForSeconds(0.06f);
            }
            
        }
        
        mesh.RecalculateNormals();
        mesh.uv = uv;
        mesh.tangents = tangent;
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
