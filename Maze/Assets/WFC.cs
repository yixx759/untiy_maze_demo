using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneTemplate;
using UnityEngine;
using Random = UnityEngine.Random;


public class WFC : MonoBehaviour
{
    private Material mat;
    [SerializeField]private GameObject Plane;

    [SerializeField] private Texture Tex;
    [SerializeField] private float offset;
    
    //keep equal
    [SerializeField] private int totalx = 6;
    [SerializeField] private int totaly = 6;
    
    
    private TileInfo[,] MasterTiles;

    private int[,] rules;
    private int[,] rulenum;
    private bool alltrue = false;
    [SerializeField] static private Texture[] Tiles;
    [SerializeField]  private Texture[] TTemp;
    [SerializeField]private float Timer= 2;
    private float TempTimer= 2;
    
    const  long m1  = 0x5555555555555555; //binary: 0101...
    const  long m2  = 0x3333333333333333; //binary: 00110011..
    const  long m4  = 0x0f0f0f0f0f0f0f0f; //binary:  4 zeros,  4 ones ...
    const  long m8  = 0x00ff00ff00ff00ff; //binary:  8 zeros,  8 ones ...
    const  long m16 = 0x0000ffff0000ffff; //binary: 16 zeros, 16 ones ...
    const  long m32 = 0x00000000ffffffff; //binary: 32 zeros, 32 ones
    const long h01 = 0x0101010101010101; //the sum of 256 to the power of 0,1,2,3...

    [Flags]
    enum TileType
    {
        Blank  =1,
        Down =2,
        Left =4,
        Right = 8,
        Up = 16
        

    }

    
    [Flags]
    enum Direction
    {
        
        Down =0,
        Left =1,
        Right = 2,
        Up = 3
        

    }
    private static int TPowerReverse(TileType a)
    {
        int b = (int)a;
        switch (b)
        {
            case 1:
                return 0;
            case 2:
                return 1;
            case 4:
                return 2;
            case 8:
                return 3;
            case 16:
                return 4;
            default:
                print("broke");
                return 500;
            
            
        }

       

    }

    struct TileInfo
    {
        public long possibility ;
        public int Entropy;
        public bool Known;

        public GameObject plane;

        private Material mat;
        
        
        //just add tyle type
        // have log ver and non log
        //do their version and compare with this one
        
        //create test tile

        public void Inst( GameObject Plane)
        {
            possibility = (long) (TileType.Blank | TileType.Down | TileType.Left | TileType.Right | TileType.Up);
           // print( possibility);
            Entropy = 5;
            Known = false;
            plane = Plane;
            mat = plane.GetComponent<Renderer>().material;
//            print(mat);

        }

        public void TileTypetoImage(TileType t)
        {
            mat.mainTexture = Tiles[TPowerReverse(t)];


        }
        public void IntetoImage(int i)
        {
            mat.mainTexture = Tiles[i];


        }



    }


    int[,] initrules( out int[,] rulenume )
    {
        int[,] a = new int[5,4];
        rulenume = new int[5,4];

        a[TPowerReverse(TileType.Blank), (int)Direction.Up] = (int) (TileType.Blank | TileType.Up);
     
        a[TPowerReverse(TileType.Blank), (int)Direction.Left] = (int) (TileType.Blank | TileType.Left);
       
        a[TPowerReverse(TileType.Blank), (int)Direction.Down] = (int) (TileType.Blank | TileType.Down);
       
        a[TPowerReverse(TileType.Blank), (int)Direction.Right] = (int) (TileType.Blank | TileType.Right);
        
        a[TPowerReverse(TileType.Down), (int)Direction.Up] = (int) (TileType.Blank | TileType.Up );
     
        a[TPowerReverse(TileType.Down), (int)Direction.Left] = (int) (TileType.Right | TileType.Up | TileType.Down);
      
        a[TPowerReverse(TileType.Down), (int)Direction.Down] = (int) (TileType.Left | TileType.Right | TileType.Up);
      
        a[TPowerReverse(TileType.Down), (int)Direction.Right] = (int) (TileType.Left | TileType.Up | TileType.Down);
       
        a[TPowerReverse(TileType.Right), (int)Direction.Up] = (int) (TileType.Left | TileType.Down | TileType.Right);
     
        a[TPowerReverse(TileType.Right), (int)Direction.Left] = (int) (TileType.Blank | TileType.Left);
       
        a[TPowerReverse(TileType.Right), (int)Direction.Down] = (int) (TileType.Left | TileType.Up | TileType.Right);
       
        a[TPowerReverse(TileType.Right), (int)Direction.Right] = (int) ( TileType.Down | TileType.Up | TileType.Left);
        
        a[TPowerReverse(TileType.Left), (int)Direction.Up] = (int) (TileType.Left | TileType.Down | TileType.Right);
     
        a[TPowerReverse(TileType.Left), (int)Direction.Left] = (int) (TileType.Up | TileType.Down | TileType.Right);
      
        a[TPowerReverse(TileType.Left), (int)Direction.Down] = (int) (TileType.Left | TileType.Right | TileType.Up);
      
        a[TPowerReverse(TileType.Left), (int)Direction.Right] = (int) (TileType.Blank | TileType.Right);
        
        a[TPowerReverse(TileType.Up), (int)Direction.Up] = (int) (TileType.Left | TileType.Down | TileType.Right);
   
        a[TPowerReverse(TileType.Up), (int)Direction.Left] = (int) (TileType.Up | TileType.Down | TileType.Right);
     
        a[TPowerReverse(TileType.Up), (int)Direction.Down] = (int) (TileType.Blank | TileType.Down);
     
        a[TPowerReverse(TileType.Up), (int)Direction.Right] = (int) (TileType.Up | TileType.Down | TileType.Left);
     
        return a;
    }

    // Start is called before the first frame update
    void Start()
    {

        
       // print("HERERERE"+
           // (int)(TileType.Blank | TileType.Down | TileType.Left | TileType.Right | TileType.Up));
        Tiles = TTemp;
        rules = initrules(out rulenum);
        
        


        MasterTiles = new TileInfo[totalx , totaly];
        
        Vector2 bnds = Plane.GetComponent<Renderer>().bounds.size;
        mat = Plane.GetComponent<Renderer>().sharedMaterial;
        mat.mainTexture = Tex;

        Vector2 start = (Vector2) transform.position + (Vector2.left*bnds.x+ Vector2.down*bnds.y) ;
        print(bnds);
        
      // Vector2 start = transform.position -  new Vector2(2, 0,0) ;

        for (int i = 0; i < totalx; i++)
        {
            for (int j = 0; j < totaly; j++)
            {
               
               MasterTiles[i,j].Inst( Instantiate(Plane, start+((Vector2.right*((bnds.x*i)+offset*i))+ Vector2.up*((bnds.y*j)+offset*j)),Plane.transform.rotation));
               
                
                
            }
            
        }

        int sx = Random.Range(0, totalx);
        int sy = Random.Range(0, totaly);
        int TT = Random.Range(0, 5);
        MasterTiles[sx, sy].Known = true;
        MasterTiles[sx, sy].Entropy = 1;
        MasterTiles[sx, sy].possibility = (MasterTiles[sx, sy].possibility & (int)1<<TT);
        MasterTiles[sx,sy].IntetoImage(TT);

       
        UpdateNeighbour(1, 0, Direction.Right, (sx,sy), TT);
        UpdateNeighbour(-1, 0, Direction.Left, (sx,sy), TT);
        UpdateNeighbour(0, 1, Direction.Up, (sx,sy), TT);
        UpdateNeighbour(0, -1, Direction.Down, (sx,sy), TT);
       
    }


    bool done()
    {

        for (int i = 0; i < totalx; i++)
        {
            for (int j = 0; j < totaly; j++)
            {

                if (MasterTiles[i, j].Known == false)
                {
                    return false;
                }

            }
        }

        return true;

    }

    //do better
    int nuentropy(int x, int y)
    {
        // int count = 0;
        // int num = MasterTiles[x, y].possibility;
        // //automate this
        // for (int i = 0; i < 5; i++)
        // {
        //     if ((num & 1) == 1)
        //     {
        //         count++;
        //
        //     }
        //     num = num >> 1;
        //
        // }
        //
        // return count;
         long xi = MasterTiles[x, y].possibility;
        
        
        
        // https://en.wikipedia.org/wiki/Hamming_weight#:~:text=The%20Hamming%20weight%20of%20a,string%20of%20the%20same%20length.
        
            xi = (xi & m1 ) + ((xi >>  1) & m1 ); //put count of each  2 bits into those  2 bits 
            xi = (xi & m2 ) + ((xi >>  2) & m2 ); //put count of each  4 bits into those  4 bits 
            xi = (xi & m4 ) + ((xi >>  4) & m4 ); //put count of each  8 bits into those  8 bits 
            xi = (xi & m8 ) + ((xi >>  8) & m8 ); //put count of each 16 bits into those 16 bits 
            xi = (xi & m16) + ((xi >> 16) & m16); //put count of each 32 bits into those 32 bits 
            xi = (xi & m32) + ((xi >> 32) & m32); //put count of each 64 bits into those 64 bits 
            return (int)xi;
        
        
        
        

    }

    (int, int) findTarget()
    {

        (int, int) targetcord = (0,0) ;
        int target = Int32.MaxValue;

        for (int i = 0; i < totalx; i++)
        {
            for (int j = 0; j < totaly; j++)
            {
              //  print("i: "+i+" j: "+j+" "+MasterTiles[i, j].Entropy);
                if (MasterTiles[i, j].Entropy < target && !MasterTiles[i, j].Known )
                {
                    target = MasterTiles[i, j].Entropy;
                  
                    targetcord = (i, j);
                }

            }
        }

        return targetcord;


    }
    //
    int targetIndex((int,int) coord)
    {
        int tt ;
        int count = 0;
        
        //fix loop
        
        do
        {
            ++count;
            tt = Random.Range(0, 5);
           // print("Heeeeeee");
           // print("t"+tt);
            //print("binar"+ (1 << tt));
           // print("pos"+MasterTiles[coord.Item1, coord.Item2].possibility);
        } while (((1<<tt) & MasterTiles[coord.Item1, coord.Item2].possibility) == 0 && count < 100);

       // print(count);
        return tt;


    }

    void UpdateNeighbour(int offx, int offy, Direction Diru, (int, int) coord , int TT)
    {
        if (coord.Item1 + offx < totalx && coord.Item2 + offy < totaly && coord.Item1 + offx >= 0 &&
            coord.Item2 + offy >= 0 &&   MasterTiles[coord.Item1+ offx,coord.Item2+offy].Entropy != 0 && MasterTiles[coord.Item1+ offx,coord.Item2+offy].Known == false       )
        {
            //fix entroy do count thing
           
            //print("pre pos: " + (MasterTiles[coord.Item1+ offx,coord.Item2+offy].possibility + " Ander "+rules[TT,(int)Dir]));
            MasterTiles[coord.Item1+ offx,coord.Item2+offy].possibility = (MasterTiles[coord.Item1+ offx,coord.Item2+offy].possibility & rules[TT,(int)Diru ]);
          
           // print("A i: "+ (coord.Item1+ offx) + " j: "+(coord.Item2+offy) + " Pos: "+MasterTiles[coord.Item1+ offx,coord.Item2+offy].possibility);
            MasterTiles[coord.Item1+ offx,coord.Item2+offy].Entropy =  nuentropy(coord.Item1+ offx, coord.Item2+offy);
          

        }

     

       
        
        
        
        
    }

    // Update is called once per frame
    void Update()
    {

       
        
        
        //make new tiles multiple





//optimze when finhsed generation


//for large scale maze do parralel on opposite ends then once magnitude between points
//low enough treat as one.


        if (!alltrue && TempTimer <= 0)
            {
                (int, int) tind = findTarget();
              

                int TT = targetIndex(tind);
               
                MasterTiles[tind.Item1, tind.Item2].Known = true;
                MasterTiles[tind.Item1, tind.Item2].Entropy = 1;
                MasterTiles[tind.Item1, tind.Item2].possibility = 
                    (MasterTiles[tind.Item1, tind.Item2].possibility & 1 << TT);
                
                MasterTiles[tind.Item1, tind.Item2].IntetoImage(TT);
      

                UpdateNeighbour(1, 0, Direction.Right, tind, TT);
                UpdateNeighbour(-1, 0, Direction.Left, tind, TT);
                UpdateNeighbour(0, 1, Direction.Up, tind, TT);
                UpdateNeighbour(0, -1, Direction.Down, tind, TT);




                alltrue = done();
                TempTimer = Timer;

            }

           TempTimer -= Time.deltaTime;



        




    }
}
