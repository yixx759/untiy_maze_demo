using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor.SceneTemplate;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;


public class WFC : MonoBehaviour
{
    private Material mat;
    [SerializeField]private GameObject Plane;

    [SerializeField] private Texture Tex;
    [SerializeField] private float offset;
    
    //keep equal
    [SerializeField] private int totalx = 6;
    [SerializeField] private int totaly = 6;

    private const int totalTiles = 12; 
    private TileInfo[,] MasterTiles;

    private BigInteger[,] rules;
    private int[,] rulenum;
    private bool alltrue = false;
    [SerializeField] static private Texture[] Tiles;
    [SerializeField] static private GameObject[] MazePart;
    [SerializeField]  private GameObject[] TMaze;
    [SerializeField]  private Texture[] TTemp;
    [SerializeField]private float Timer= 2;
    private float TempTimer= 2;
    [SerializeField] private int lesschanceofblank = 0;

    private static Vector2 blockLength = new Vector2(17.5f, 17.5f);
    //add colluider
    
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
        Up = 16,
        Lup =32,
        Ll = 64,
        CTR = 128,
        CTL = 256,
        CBR = 512,
        CBL = 1024,
        Cross = 2048
        
        

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
            case 32:
                return 5;
            case 64:
                return 6;
            case 128:
                return 7;
            case 256:
                return 8;
            case 512:
                return 9;
            case 1024:
                return 10;
            case 2048:
                return 11;
            
            
            
            
            default:
                print("broke");
                return 500;
            
            
        }

       

    }

    struct TileInfo
    {
        public BigInteger possibility ;
        public int Entropy;
        public bool Known;

        public GameObject plane;

        private Material mat;
        private Vector2 xy;
        
        
        //just add tyle type
        // have log ver and non log
        //do their version and compare with this one
        
        //create test tile

        public void Inst( GameObject Plane, Vector2 i_xy)
        {
            possibility = (long) (TileType.Cross|TileType.Blank | TileType.Down | TileType.Left | TileType.Right | TileType.Up | TileType.Ll | TileType.Lup | TileType.CBL | TileType.CBR | TileType.CTL | TileType.CTR);
           // print( possibility);
           xy = i_xy;
            Entropy = totalTiles;
            Known = false;
           // plane = Plane;
          //  mat = plane.GetComponent<Renderer>().material;
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
        
        public void AddNewBlock(int i)
        {
            Instantiate(MazePart[i] , new UnityEngine.Vector3(xy.x*blockLength.x,0,xy.y*blockLength.y), Quaternion.Euler(new UnityEngine.Vector3(-89.98f,0,0)) );

//-89.98
        }


    }


    BigInteger[,] initrules()
    {
        BigInteger[,] a = new  BigInteger[totalTiles,4];
       //remove blank

        a[TPowerReverse(TileType.Blank), (int)Direction.Up] = (int) (TileType.Blank | TileType.Up |TileType.Down | TileType.Left | TileType.Right |TileType.Ll | TileType.Lup | TileType.Up |TileType.CBL | TileType.CBR | TileType.CTL |TileType.CTR | TileType.Cross);
     
        a[TPowerReverse(TileType.Blank), (int)Direction.Left] =(int) (TileType.Blank | TileType.Up |TileType.Down | TileType.Left | TileType.Right |TileType.Ll | TileType.Lup | TileType.Up |TileType.CBL | TileType.CBR | TileType.CTL |TileType.CTR | TileType.Cross);

        a[TPowerReverse(TileType.Blank), (int)Direction.Down] = (int) (TileType.Blank | TileType.Up |TileType.Down | TileType.Left | TileType.Right |TileType.Ll | TileType.Lup | TileType.Up |TileType.CBL | TileType.CBR | TileType.CTL |TileType.CTR | TileType.Cross);

        a[TPowerReverse(TileType.Blank), (int)Direction.Right] = (int) (TileType.Blank | TileType.Up |TileType.Down | TileType.Left | TileType.Right |TileType.Ll | TileType.Lup | TileType.Up |TileType.CBL | TileType.CBR | TileType.CTL |TileType.CTR | TileType.Cross);

        //
        a[TPowerReverse(TileType.Down), (int)Direction.Up] = (int) (TileType.Blank |TileType.Ll | TileType.Up | TileType.CBL | TileType.CBR );
     
        a[TPowerReverse(TileType.Down), (int)Direction.Left] = (int) (TileType.Right | TileType.Up | TileType.Down | TileType.CTR | TileType.CBR | TileType.Down | TileType.Ll| TileType.Cross );
      
        a[TPowerReverse(TileType.Down), (int)Direction.Down] = (int) (TileType.Lup | TileType.Up | TileType.CBL | TileType.CBR | TileType.Left | TileType.Right| TileType.Cross );
      
        a[TPowerReverse(TileType.Down), (int)Direction.Right] = (int) (TileType.Left | TileType.Up | TileType.Down | TileType.CTL |TileType.CBL | TileType.Ll | TileType.Cross);
       //
       
        a[TPowerReverse(TileType.Right), (int)Direction.Up] = (int) (TileType.Left | TileType.Down | TileType.Right | TileType.Lup | TileType.CTL | TileType.CTR | TileType.Cross);
     
        a[TPowerReverse(TileType.Right), (int)Direction.Left] = (int) (TileType.Blank | TileType.Left | TileType.Lup | TileType.CTL | TileType.CBL);
       
        a[TPowerReverse(TileType.Right), (int)Direction.Down] = (int) (TileType.Left | TileType.Up | TileType.Right | TileType.Lup | TileType.CBR | TileType.CBL | TileType.Cross);
       
        a[TPowerReverse(TileType.Right), (int)Direction.Right] = (int) ( TileType.Down | TileType.Up | TileType.Left | TileType.Ll  | TileType.CTL | TileType.CTR | TileType.Cross);
        //
        a[TPowerReverse(TileType.Left), (int)Direction.Up] = (int) (TileType.Left | TileType.Down | TileType.Right | TileType.Lup | TileType.CTL | TileType.CTR | TileType.Cross);
     
        a[TPowerReverse(TileType.Left), (int)Direction.Left] = (int) (TileType.Up | TileType.Down | TileType.Right | TileType.Ll | TileType.CTR | TileType.CBR | TileType.Cross);
      
        a[TPowerReverse(TileType.Left), (int)Direction.Down] = (int) (TileType.Left | TileType.Right | TileType.Up | TileType.Lup | TileType.CBR | TileType.CBL | TileType.Cross);
      
        a[TPowerReverse(TileType.Left), (int)Direction.Right] = (int) (TileType.Blank |TileType.Blank | TileType.Right | TileType.CTR | TileType.CBR | TileType.Lup);
        //
        
        a[TPowerReverse(TileType.Up), (int)Direction.Up] = (int) (TileType.Left | TileType.Down | TileType.Right | TileType.Lup | TileType.CTL | TileType.CTR | TileType.Cross );
   
        a[TPowerReverse(TileType.Up), (int)Direction.Left] = (int) (TileType.Up | TileType.Down | TileType.Right | TileType.Ll | TileType.CTR | TileType.CBR | TileType.Cross);
     
        a[TPowerReverse(TileType.Up), (int)Direction.Down] = (int) (TileType.Blank | TileType.Down | TileType.Ll | TileType.CTL | TileType.CTR);
     
        a[TPowerReverse(TileType.Up), (int)Direction.Right] = (int) (TileType.Up | TileType.Down | TileType.Left | TileType.Ll | TileType.CTL | TileType.CBL | TileType.Cross);
     
        //
        a[TPowerReverse(TileType.Lup), (int)Direction.Up] = (int) (TileType.Blank |TileType.Left | TileType.Down | TileType.Right | TileType.CTL | TileType.CTR | TileType.Lup | TileType.Cross);
   
        a[TPowerReverse(TileType.Lup), (int)Direction.Left] = (int) ( TileType.Left | TileType.CBL | TileType.CTL | TileType.Lup);
     
        a[TPowerReverse(TileType.Lup), (int)Direction.Down] = (int) (TileType.Blank|TileType.Up | TileType.Left | TileType.Right | TileType.CBR | TileType.CBL | TileType.Lup | TileType.Cross);
     
        a[TPowerReverse(TileType.Lup), (int)Direction.Right] = (int) (TileType.Right | TileType.CBR | TileType.CTR | TileType.Lup);

        //
        
        a[TPowerReverse(TileType.Ll), (int)Direction.Up] = (int) (TileType.Up | TileType.Ll | TileType.CBL | TileType.CBR);
   
        a[TPowerReverse(TileType.Ll), (int)Direction.Left] = (int) (TileType.Blank |TileType.Up | TileType.Down | TileType.Right | TileType.Ll | TileType.CTR | TileType.CBR  | TileType.Cross);
     
        a[TPowerReverse(TileType.Ll), (int)Direction.Down] = (int) (TileType.Down | TileType.Ll | TileType.CTL | TileType.CTR);
     
        a[TPowerReverse(TileType.Ll), (int)Direction.Right] = (int) (TileType.Blank |TileType.Blank |TileType.Up | TileType.Down | TileType.Left | TileType.Ll | TileType.CTL | TileType.CBL | TileType.Cross);

        //
        
        a[TPowerReverse(TileType.CBL), (int)Direction.Up] = (int) (TileType.Left | TileType.Down | TileType.Right | TileType.Lup | TileType.CTR | TileType.CTL | TileType.Cross );
   
        a[TPowerReverse(TileType.CBL), (int)Direction.Left] = (int) (TileType.Right | TileType.Down | TileType.Up | TileType.Ll | TileType.CTR | TileType.CBR | TileType.Cross);
     
        a[TPowerReverse(TileType.CBL), (int)Direction.Down] = (int) ( TileType.Down | TileType.Ll | TileType.CTR | TileType.CTL);
     
        a[TPowerReverse(TileType.CBL), (int)Direction.Right] = (int) (TileType.Right |  TileType.Up | TileType.CBR | TileType.CTR);

        //
        
        a[TPowerReverse(TileType.CBR), (int)Direction.Up] = (int) (TileType.Left | TileType.Down | TileType.Right | TileType.Lup | TileType.CTR | TileType.CTL | TileType.Cross);
   
        a[TPowerReverse(TileType.CBR), (int)Direction.Left] = (int) (TileType.Right | TileType.Down | TileType.Up | TileType.Ll | TileType.CTR | TileType.CBR);
     
        a[TPowerReverse(TileType.CBR), (int)Direction.Down] = (int) (TileType.Down | TileType.Ll | TileType.CTR | TileType.CTL);
     
        a[TPowerReverse(TileType.CBR), (int)Direction.Right] = (int) (TileType.Left |  TileType.Up  |  TileType.Down| TileType.Ll | TileType.CBL | TileType.CTL | TileType.Cross);

        
        //
        
        a[TPowerReverse(TileType.CTL), (int)Direction.Up] = (int) ( TileType.Up | TileType.Ll | TileType.CBR | TileType.CBL);
   
        a[TPowerReverse(TileType.CTL), (int)Direction.Left] = (int) (TileType.Right | TileType.Down | TileType.Up | TileType.Ll | TileType.CTR | TileType.CBR | TileType.Cross);
     
        a[TPowerReverse(TileType.CTL), (int)Direction.Down] = (int) (TileType.Up | TileType.Left | TileType.Right | TileType.Lup | TileType.CBR | TileType.CBL | TileType.Cross);
     
        a[TPowerReverse(TileType.CTL), (int)Direction.Right] = (int) (TileType.Right | TileType.Lup | TileType.CTR | TileType.CBR);

        //
        
        a[TPowerReverse(TileType.CTR), (int)Direction.Up] = (int) (TileType.Up  | TileType.Ll | TileType.CBR | TileType.CBL);
   
        a[TPowerReverse(TileType.CTR), (int)Direction.Left] = (int) (TileType.Left | TileType.Lup | TileType.CTL | TileType.CBL);
     
        a[TPowerReverse(TileType.CTR), (int)Direction.Down] = (int) (TileType.Up | TileType.Left | TileType.Right | TileType.Lup | TileType.CBR | TileType.CBL | TileType.Cross);
     
        a[TPowerReverse(TileType.CTR), (int)Direction.Right] = (int) (TileType.Left | TileType.Down | TileType.Up | TileType.Ll | TileType.CTL | TileType.CBL | TileType.Cross);

        
        //
        a[TPowerReverse(TileType.Cross), (int)Direction.Up] = (int) (TileType.Down | TileType.Left | TileType.Right  | TileType.Lup | TileType.CTR | TileType.CTL | TileType.Cross);
   
        a[TPowerReverse(TileType.Cross), (int)Direction.Left] = (int) (TileType.Right | TileType.Down | TileType.Up |TileType.Ll | TileType.Cross | TileType.CBR | TileType.CTR);
     
        a[TPowerReverse(TileType.Cross), (int)Direction.Down] = (int) (TileType.Up | TileType.Left | TileType.Right  | TileType.Lup | TileType.CBR | TileType.CBL | TileType.Cross);
     
        a[TPowerReverse(TileType.Cross), (int)Direction.Right] = (int) (TileType.Left | TileType.Down | TileType.Up |TileType.Ll | TileType.Cross | TileType.CBL | TileType.CTL | TileType.Cross);

        
        
        
        
        
        return a;
    }

    // Start is called before the first frame update
    void Start()
    {

        
       // print("HERERERE"+
           // (int)(TileType.Blank | TileType.Down | TileType.Left | TileType.Right | TileType.Up));
        Tiles = TTemp;
        MazePart = TMaze;
        rules = initrules();
        
        


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
               //Instantiate(Plane, start+((Vector2.right*((bnds.x*i)+offset*i))+ Vector2.up*((bnds.y*j)+offset*j)),Plane.transform.rotation)
               MasterTiles[i,j].Inst(null, new Vector2(i,j) );
               
                
                
            }
            
        }

        int sx = Random.Range(0, totalx);
        int sy = Random.Range(0, totaly);
        int TT = Random.Range(0, totalTiles);
        MasterTiles[sx, sy].Known = true;
        MasterTiles[sx, sy].Entropy = 1;
        MasterTiles[sx, sy].possibility = (MasterTiles[sx, sy].possibility & (int)1<<TT);
        //MasterTiles[sx,sy].IntetoImage(TT);
        MasterTiles[sx,sy].AddNewBlock(TT);
       
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
        int count = 0;
        BigInteger num = MasterTiles[x, y].possibility;
        //automate this
        for (int i = 0; i < totalTiles; i++)
        {
            if ((num & 1) == 1)
            {
                count++;
        
            }
            num = num >> 1;
        
        }
        
        return count;
        //  BigInteger xi = MasterTiles[x, y].possibility;
        //
        //
        //
        // // https://en.wikipedia.org/wiki/Hamming_weight#:~:text=The%20Hamming%20weight%20of%20a,string%20of%20the%20same%20length.
        //
        //     xi = (xi & m1 ) + ((xi >>  1) & m1 ); //put count of each  2 bits into those  2 bits 
        //     xi = (xi & m2 ) + ((xi >>  2) & m2 ); //put count of each  4 bits into those  4 bits 
        //     xi = (xi & m4 ) + ((xi >>  4) & m4 ); //put count of each  8 bits into those  8 bits 
        //     xi = (xi & m8 ) + ((xi >>  8) & m8 ); //put count of each 16 bits into those 16 bits 
        //     xi = (xi & m16) + ((xi >> 16) & m16); //put count of each 32 bits into those 32 bits 
        //     xi = (xi & m32) + ((xi >> 32) & m32); //put count of each 64 bits into those 64 bits 
        //     return (int)xi;
        
        
        
        

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
            tt = Random.Range(0, totalTiles);
            int g = Random.Range(0, lesschanceofblank);
            if (g != 0)
            {
                //simple imp improve later
                if (tt == 0)
                {
                    tt = (tt+ g)%totalTiles; 
                    
                }
                   
                
            }

            // print("Heeeeeee");
           // print("t"+tt);
            //print("binar"+ (1 << tt));
           // print("pos"+MasterTiles[coord.Item1, coord.Item2].possibility);
         
          
        } while (((BigInteger)(1<<tt) & MasterTiles[coord.Item1, coord.Item2].possibility) == 0 && count < 100);

        if (count == 100)
        {
            
            print("BROOOOOOOOOOKKKKKKKKKKKEEEEEEEEE");
            // print(count);
        }
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
//                print("TTTERRR: " + TT );
               
                MasterTiles[tind.Item1, tind.Item2].Known = true;
                MasterTiles[tind.Item1, tind.Item2].Entropy = 1;
                MasterTiles[tind.Item1, tind.Item2].possibility = 
                    (MasterTiles[tind.Item1, tind.Item2].possibility & 1 << TT);
                
               // MasterTiles[tind.Item1, tind.Item2].IntetoImage(TT);
               MasterTiles[tind.Item1, tind.Item2].AddNewBlock(TT);

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
