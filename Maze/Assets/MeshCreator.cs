using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCreator : MonoBehaviour
{

    [SerializeField] private int sx;
    [SerializeField] private int sy;
    [SerializeField] private int sz;
    [SerializeField] private int roundness;
    private Vector3[] verticies;
    private Vector3[] normals;
    private Mesh mesh;
    private int[] tris1;
    private int[] tris2;
    private int[] tris3;
   
    private int counttotal = 0;

    // Start is called before the first frame update

    int createQuad(int[] triang, int i, int BL, int BR, int TL, int TR)
    {
        triang[i] = BL;
        triang[i+1] = TL;
        triang[i+2] = BR;
        triang[i+3] = TL;
        triang[i+4] = TR;
        triang[i+5] = BR;
       


        return i + 6;
    }

    IEnumerator delayer()
    {
        int corner = 8;
        int edges =  (sx + sy + sz - 3)*4 ;
        int faces = ((sx - 1) * (sy - 1) +
                     (sx - 1) * (sz - 1) +
                     (sy - 1) * (sz - 1)) * 2 ;
        
        verticies = new Vector3[corner + edges+ faces];
        normals = new Vector3[verticies.Length];
        Vector2[] uv = new Vector2[verticies.Length];
        Vector4[] tangent = new Vector4[verticies.Length];

        int count = 0;

        void SetVertex(int i, float x, float y, float z)
        {
            Vector3 inner = new Vector3(x, y, z);

            if (inner.x < roundness)
            {
                inner.x = roundness;

            }

            else if (inner.x > sx - roundness)
            {
                inner.x = sx - roundness;
            }
            
            
            if (inner.y < roundness)
            {
                inner.y = roundness;

            }

            else if (inner.y > sy - roundness)
            {
                inner.y = sy - roundness;
            }
            
            if (inner.z < roundness)
            {
                inner.z = roundness;

            }

            else if (inner.z > sz - roundness)
            {
                inner.z = sz - roundness;
            }
            
            normals[i] = (new Vector3(x, y, z)-inner).normalized;
            verticies[i] = inner + normals[i]*roundness ;
            
        }








        for (int y = 0; y <= sy; y++)
        {
            for (int x = 0; x <= sx ; x++)
            {

               
                //verticies[count++] = new Vector3(x,y,0);
                SetVertex(count++, x, y, 0);

            }
        
            
            for (int i = 1; i <= sz; i++)
            {
               
               // verticies[count++] = new Vector3(sx,y,i);
                SetVertex(count++, sx, y, i);
           
            }

            for (int i = sx-1; i >= 0; i--)
            {
               
               // verticies[count++] = new Vector3(i,y,sz);
                SetVertex(count++, i, y, sz);
            
            }
            for (int i = sz -1 ; i > 0; i--)
            {
               
                //verticies[count++] = new Vector3(0,y,i);
                SetVertex(count++, 0, y, i);
            }
            
            
        }
        for (int z = 1; z < sz; z++)
        {
            for (int x = 1; x < sx; x++)
            {
               
                //verticies[count++] = new Vector3(x,sy,z);
                SetVertex(count++, x, sy, z);
            }
            
        }
        for (int z = 1; z < sz; z++)
        {
            for (int x = 1; x < sx; x++)
            {
               
                //verticies[count++] = new Vector3(x,0,z);
                SetVertex(count++, x, 0, z);
            }
            
        }
        
       

        counttotal = count;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;


       
        
        

        mesh.name = "YAYAYAYAY";
        mesh.vertices = verticies;
        mesh.normals = normals;
        
       // int quads = (sx * sz + sx * sy + sz * sy) * 2;
        tris1 = new int[(sx*sy)*12];
        tris2 = new int[(sy*sz)*12];
        tris3 = new int[(sz*sx)*12];


        int ring = ((sx + sz) * 2);
        int v = 0 , tx =0, ty =0, tz =0, index =0;

        for (int y = 0; y < sy; y++, index++)
        {
            for (int i = 0; i < sx; i++, index++)
            {
                tz = createQuad(tris1, tz, index, 1+index, index+ring, index+ring + 1);

            }
            
            for (int i = 0; i < sz; i++, index++)
            {
                tx = createQuad(tris2, tx, index, 1+index, index+ring, index+ring + 1);

            }
            for (int i = 0; i < sx; i++, index++)
            {
                tz = createQuad(tris1, tz, index, 1+index, index+ring, index+ring + 1);

            }
            for (int i = 0; i < sz-1; i++, index++)
            {
                tx = createQuad(tris2, tx, index, 1+index, index+ring, index+ring + 1);

            }
            tx = createQuad(tris2, tx, index, index - ring+1, index + ring, index+1);

        }

        
        //bottom
        int nuring = ring * sy;
        for (int top = 0; top < sx - 1; top++, nuring++)
        {
            ty = createQuad(tris3, ty, nuring, nuring + 1, nuring + ring - 1, nuring + ring);


        }
        ty = createQuad(tris3, ty, nuring, nuring + 1, nuring + ring - 1, nuring + 2);

        int vMin = ring * (sy + 1) - 1;
        int vMid = vMin + 1;
        
        
        int vMax = nuring + 2;

        for (int j = 1; j < sz-1; j++, vMin--, vMid++, vMax++)
        {
            ty = createQuad(tris3, ty, vMin, vMid, vMin - 1, vMid + sx - 1);

            for (int i = 1; i < sx - 1; i++, vMid++)
            {

                ty = createQuad(tris3, ty, vMid, vMid + 1, vMid + sx - 1, vMid + sx);

            }

            ty = createQuad(tris3, ty, vMid, vMax, vMid + sx - 1, vMax + 1);
        }

        int vtop = vMin - 2;
        ty = createQuad(tris3, ty, vMin, vMid , vtop+1, vtop );

        for (int i = 1; i < sx - 1; i++, vtop--, vMid++)
        {

            ty = createQuad(tris3, ty, vMid, vMid + 1, vtop, vtop - 1);

        }
        ty = createQuad(tris3, ty, vMid, vtop-2 , vtop, vtop-1 );


        //bottom
        int luring = 1; 
        vMid = verticies.Length - (sx - 1) * (sz - 1);
        ty = createQuad(tris3, ty, ring-1, vMid , 0, 1 );


        for (int i = 1; i < sx - 1; i++, luring++, vMid++)
        {
            
            ty = createQuad(tris3, ty, vMid, vMid+1 , luring, luring+1 );

            
            
        }

        ty = createQuad(tris3, ty, vMid, luring+2 , luring, luring+1 );

        vMin = ring - 2;
        vMid -= sx - 2;
        vMax = luring + 2;
        for (int j = 1; j < sz-1; j++, vMin--, vMid++, vMax++)
        {
            ty = createQuad(tris3, ty, vMin, vMid+sx-1, vMin + 1, vMid);

            for (int i = 1; i < sx - 1; i++, vMid++)
            {

                ty = createQuad(tris3, ty, vMid+sx-1, vMid + sx, vMid , vMid + 1);

            }

            ty = createQuad(tris3, ty, vMid+sx-1, vMax+1, vMid  , vMax );
        }

        vtop = vMin - 1;

        
        ty = createQuad(tris3, ty, vtop+1, vtop , vtop+2, vMid );


        for (int i = 1; i < sx - 1; i++, vtop--, vMid++)
        {
            
            ty = createQuad(tris3, ty, vtop, vtop-1 , vMid, vMid+1 );

            
            
        }

        ty = createQuad(tris3, ty, vtop, vtop-1 , vMid, vtop-2 );

        mesh.subMeshCount = 3;
        mesh.SetTriangles(tris1,0);
        mesh.SetTriangles(tris2,1);
        mesh.SetTriangles(tris3,2);
        
        
        // int counter = 0;
        // //seperate x and y
        // for (int i = 0, vi = 0; i < sy; i++, vi++)
        // {
        //     for (int j = 0; j < sx; j++ , vi++)
        //     {
        //         tris[counter++] = vi;
        //         tris[counter++] =  sx+1+vi;
        //         tris[counter++] =1+vi;
        //
        //         mesh.triangles = tris;
        //         yield return new WaitForSeconds(0.06f);
        //         tris[counter++] = 1+vi;
        //         tris[counter++] =  sx+1+vi;
        //         tris[counter++] =sx+2+vi;
        //         mesh.triangles = tris;
        //         yield return new WaitForSeconds(0.06f);
        //     }
        //     
        // }
        //
        // mesh.RecalculateNormals();
        // mesh.uv = uv;
        // mesh.tangents = tangent;
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
        for (int i = 0; i < counttotal; i++)
        {
            
            Gizmos.DrawSphere(transform.position+verticies[i],0.04f);

            
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        
        
        
        
    }
}
