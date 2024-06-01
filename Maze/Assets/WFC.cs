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
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

public class WFC : MonoBehaviour
{
    private Material mat;
    [SerializeField]private GameObject Plane;
    [SerializeField] private static Texture tex;
    [SerializeField] private  Texture stex;
    [SerializeField] private Texture Tex;
    [SerializeField] private float offset;
    private int moveTracker = 0;
    enum ScrollDir
    {
        up,
        down,
        left,
        right

    }
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

    private static Vector2 blockLength = new Vector2(16.8f, 16.8f);
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
//this is 12 bit bro fix it
    
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

    
    
    private static int TPowerReverseBigInt(BigInteger a)
    {
       
        switch ((int)a)
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
        public Vector2 xy;
        
        
        //just add tyle type
        // have log ver and non log
        //do their version and compare with this one
        
        //create test tile

        public void Inst( Vector2 i_xy)
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
           plane = Instantiate(MazePart[i] , new UnityEngine.Vector3(xy.x*blockLength.x,0,xy.y*blockLength.y), Quaternion.Euler(new UnityEngine.Vector3(-89.98f,-90,0)) );
            //plane.GetComponent<Renderer>().material.mainTexture = tex;
//-89.98
        }

        public void DelObj()
        {
            
            Destroy(plane);
            plane = null;
            


        }


    }


    BigInteger[,] initrules()
    {
        BigInteger[,] a = new  BigInteger[totalTiles,4];
       //remove blank
       //add blank

        a[TPowerReverse(TileType.Blank), (int)Direction.Up] = (int) (TileType.Blank | TileType.Up |TileType.Down | TileType.Left | TileType.Right |TileType.Ll | TileType.Lup | TileType.Up |TileType.CBL | TileType.CBR | TileType.CTL |TileType.CTR | TileType.Cross);
     
        a[TPowerReverse(TileType.Blank), (int)Direction.Left] =(int) (TileType.Blank | TileType.Up |TileType.Down | TileType.Left | TileType.Right |TileType.Ll | TileType.Lup | TileType.Up |TileType.CBL | TileType.CBR | TileType.CTL |TileType.CTR | TileType.Cross);

        a[TPowerReverse(TileType.Blank), (int)Direction.Down] = (int) (TileType.Blank | TileType.Up |TileType.Down | TileType.Left | TileType.Right |TileType.Ll | TileType.Lup | TileType.Up |TileType.CBL | TileType.CBR | TileType.CTL |TileType.CTR | TileType.Cross);

        a[TPowerReverse(TileType.Blank), (int)Direction.Right] = (int) (TileType.Blank | TileType.Up |TileType.Down | TileType.Left | TileType.Right |TileType.Ll | TileType.Lup | TileType.Up |TileType.CBL | TileType.CBR | TileType.CTL |TileType.CTR | TileType.Cross);

        //
        a[TPowerReverse(TileType.Down), (int)Direction.Up] = (int) (TileType.Blank |TileType.Ll | TileType.Up | TileType.CBL | TileType.CBR );
     
        a[TPowerReverse(TileType.Down), (int)Direction.Left] = (int) (TileType.Blank |TileType.Right | TileType.Up | TileType.Down | TileType.CTR | TileType.CBR | TileType.Down | TileType.Ll| TileType.Cross );
      
        a[TPowerReverse(TileType.Down), (int)Direction.Down] = (int) (TileType.Blank |TileType.Lup | TileType.Up | TileType.CBL | TileType.CBR | TileType.Left | TileType.Right| TileType.Cross );
      
        a[TPowerReverse(TileType.Down), (int)Direction.Right] = (int) (TileType.Blank |TileType.Left | TileType.Up | TileType.Down | TileType.CTL |TileType.CBL | TileType.Ll | TileType.Cross);
       //
       
        a[TPowerReverse(TileType.Right), (int)Direction.Up] = (int) (TileType.Blank |TileType.Left | TileType.Down | TileType.Right | TileType.Lup | TileType.CTL | TileType.CTR | TileType.Cross);
     
        a[TPowerReverse(TileType.Right), (int)Direction.Left] = (int) (TileType.Blank |TileType.Blank | TileType.Left | TileType.Lup | TileType.CTL | TileType.CBL);
       
        a[TPowerReverse(TileType.Right), (int)Direction.Down] = (int) (TileType.Blank |TileType.Left | TileType.Up | TileType.Right | TileType.Lup | TileType.CBR | TileType.CBL | TileType.Cross);
       
        a[TPowerReverse(TileType.Right), (int)Direction.Right] = (int) (TileType.Blank | TileType.Down | TileType.Up | TileType.Left | TileType.Ll  | TileType.CTL | TileType.CBL | TileType.Cross);
        //
        a[TPowerReverse(TileType.Left), (int)Direction.Up] = (int) (TileType.Blank |TileType.Left | TileType.Down | TileType.Right | TileType.Lup | TileType.CTL | TileType.CTR | TileType.Cross);
     
        a[TPowerReverse(TileType.Left), (int)Direction.Left] = (int) (TileType.Blank |TileType.Up | TileType.Down | TileType.Right | TileType.Ll | TileType.CTR | TileType.CBR | TileType.Cross);
      
        a[TPowerReverse(TileType.Left), (int)Direction.Down] = (int) (TileType.Blank |TileType.Left | TileType.Right | TileType.Up | TileType.Lup | TileType.CBR | TileType.CBL | TileType.Cross);
      
        a[TPowerReverse(TileType.Left), (int)Direction.Right] = (int) (TileType.Blank |TileType.Blank |TileType.Blank | TileType.Right | TileType.CTR | TileType.CBR | TileType.Lup);
        //
        
        a[TPowerReverse(TileType.Up), (int)Direction.Up] = (int) (TileType.Blank |TileType.Left | TileType.Down | TileType.Right | TileType.Lup | TileType.CTL | TileType.CTR | TileType.Cross );
   
        a[TPowerReverse(TileType.Up), (int)Direction.Left] = (int) (TileType.Blank |TileType.Up | TileType.Down | TileType.Right | TileType.Ll | TileType.CTR | TileType.CBR | TileType.Cross);
     
        a[TPowerReverse(TileType.Up), (int)Direction.Down] = (int) (TileType.Blank |TileType.Blank | TileType.Down | TileType.Ll | TileType.CTL | TileType.CTR);
     
        a[TPowerReverse(TileType.Up), (int)Direction.Right] = (int) (TileType.Blank |TileType.Up | TileType.Down | TileType.Left | TileType.Ll | TileType.CTL | TileType.CBL | TileType.Cross);
     
        //
        a[TPowerReverse(TileType.Lup), (int)Direction.Up] = (int) (TileType.Blank |TileType.Blank |TileType.Left | TileType.Down | TileType.Right | TileType.CTL | TileType.CTR | TileType.Lup | TileType.Cross);
   
        a[TPowerReverse(TileType.Lup), (int)Direction.Left] = (int) ( TileType.Blank |TileType.Left | TileType.CBL | TileType.CTL | TileType.Lup);
     
        a[TPowerReverse(TileType.Lup), (int)Direction.Down] = (int) (TileType.Blank |TileType.Blank|TileType.Up | TileType.Left | TileType.Right | TileType.CBR | TileType.CBL | TileType.Lup | TileType.Cross);
     
        a[TPowerReverse(TileType.Lup), (int)Direction.Right] = (int) (TileType.Blank |TileType.Right | TileType.CBR | TileType.CTR | TileType.Lup);

        //
        
        a[TPowerReverse(TileType.Ll), (int)Direction.Up] = (int) (TileType.Blank |TileType.Up | TileType.Ll | TileType.CBL | TileType.CBR);
   
        a[TPowerReverse(TileType.Ll), (int)Direction.Left] = (int) (TileType.Blank |TileType.Blank |TileType.Up | TileType.Down | TileType.Right | TileType.Ll | TileType.CTR | TileType.CBR  | TileType.Cross);
     
        a[TPowerReverse(TileType.Ll), (int)Direction.Down] = (int) (TileType.Blank |TileType.Down | TileType.Ll | TileType.CTL | TileType.CTR);
     
        a[TPowerReverse(TileType.Ll), (int)Direction.Right] = (int) (TileType.Blank |TileType.Blank |TileType.Blank |TileType.Up | TileType.Down | TileType.Left | TileType.Ll | TileType.CTL | TileType.CBL | TileType.Cross);

        //
        
        a[TPowerReverse(TileType.CBL), (int)Direction.Up] = (int) (TileType.Blank |TileType.Left | TileType.Down | TileType.Right | TileType.Lup | TileType.CTR | TileType.CTL | TileType.Cross );
   
        a[TPowerReverse(TileType.CBL), (int)Direction.Left] = (int) (TileType.Blank |TileType.Right | TileType.Down | TileType.Up | TileType.Ll | TileType.CTR | TileType.CBR | TileType.Cross);
     
        a[TPowerReverse(TileType.CBL), (int)Direction.Down] = (int) ( TileType.Blank |TileType.Down | TileType.Ll | TileType.CTR | TileType.CTL);
     
        a[TPowerReverse(TileType.CBL), (int)Direction.Right] = (int) (TileType.Blank |TileType.Right |  TileType.Lup | TileType.CBR | TileType.CTR);

        //
      
        a[TPowerReverse(TileType.CBR), (int)Direction.Up] = (int) (TileType.Blank |TileType.Left | TileType.Down | TileType.Right | TileType.Lup | TileType.CTR | TileType.CTL | TileType.Cross);
   
        a[TPowerReverse(TileType.CBR), (int)Direction.Left] = (int) (TileType.Blank |TileType.Right | TileType.Down | TileType.Up | TileType.Ll | TileType.CBL | TileType.CTL);
     
        a[TPowerReverse(TileType.CBR), (int)Direction.Down] = (int) (TileType.Blank |TileType.Down | TileType.Ll | TileType.CTR | TileType.CTL);
     
        a[TPowerReverse(TileType.CBR), (int)Direction.Right] = (int) (TileType.Blank |TileType.Left |  TileType.Up  |  TileType.Down| TileType.Ll | TileType.CBL | TileType.CTL | TileType.Cross);

        
        //
        
        a[TPowerReverse(TileType.CTL), (int)Direction.Up] = (int) ( TileType.Blank |TileType.Up | TileType.Ll | TileType.CBR | TileType.CBL);
   
        a[TPowerReverse(TileType.CTL), (int)Direction.Left] = (int) (TileType.Blank |TileType.Right | TileType.Down | TileType.Up | TileType.Ll | TileType.CTR | TileType.CBR | TileType.Cross);
     
        a[TPowerReverse(TileType.CTL), (int)Direction.Down] = (int) (TileType.Blank |TileType.Up | TileType.Left | TileType.Right | TileType.Lup | TileType.CBR | TileType.CBL | TileType.Cross);
     
        a[TPowerReverse(TileType.CTL), (int)Direction.Right] = (int) (TileType.Blank |TileType.Right | TileType.Lup | TileType.CTR | TileType.CBR);

        //
        
        a[TPowerReverse(TileType.CTR), (int)Direction.Up] = (int) (TileType.Blank |TileType.Up  | TileType.Ll | TileType.CBR | TileType.CBL);
   
        a[TPowerReverse(TileType.CTR), (int)Direction.Left] = (int) (TileType.Blank |TileType.Left | TileType.Lup | TileType.CTL | TileType.CBL);
     
        a[TPowerReverse(TileType.CTR), (int)Direction.Down] = (int) (TileType.Blank |TileType.Up | TileType.Left | TileType.Right | TileType.Lup | TileType.CBR | TileType.CBL | TileType.Cross);
     
        a[TPowerReverse(TileType.CTR), (int)Direction.Right] = (int) (TileType.Blank |TileType.Left | TileType.Down | TileType.Up | TileType.Ll | TileType.CTL | TileType.CBL | TileType.Cross);

        
        //
        a[TPowerReverse(TileType.Cross), (int)Direction.Up] = (int) (TileType.Blank |TileType.Down | TileType.Left | TileType.Right  | TileType.Lup | TileType.CTR | TileType.CTL | TileType.Cross);
   
        a[TPowerReverse(TileType.Cross), (int)Direction.Left] = (int) (TileType.Blank |TileType.Right | TileType.Down | TileType.Up |TileType.Ll | TileType.Cross | TileType.CBR | TileType.CTR);
     
        a[TPowerReverse(TileType.Cross), (int)Direction.Down] = (int) (TileType.Blank |TileType.Up | TileType.Left | TileType.Right  | TileType.Lup | TileType.CBR | TileType.CBL | TileType.Cross);
     
        a[TPowerReverse(TileType.Cross), (int)Direction.Right] = (int) (TileType.Blank |TileType.Left | TileType.Down | TileType.Up |TileType.Ll | TileType.Cross | TileType.CBL | TileType.CTL | TileType.Cross);

        
        //////////////////////////////
        ///
        ///a[TPowerReverse(TileType.Blank), (int)Direction.Up] = (int) (TileType.Blank | TileType.Up |TileType.Down | TileType.Left | TileType.Right |TileType.Ll | TileType.Lup | TileType.Up |TileType.CBL | TileType.CBR | TileType.CTL |TileType.CTR | TileType.Cross);
       //
       //  a[TPowerReverse(TileType.Blank), (int)Direction.Left] =(int) (TileType.Blank | TileType.Up |TileType.Down | TileType.Left | TileType.Right |TileType.Ll | TileType.Lup | TileType.Up |TileType.CBL | TileType.CBR | TileType.CTL |TileType.CTR | TileType.Cross);
       //
       //  a[TPowerReverse(TileType.Blank), (int)Direction.Down] = (int) (TileType.Blank | TileType.Up |TileType.Down | TileType.Left | TileType.Right |TileType.Ll | TileType.Lup | TileType.Up |TileType.CBL | TileType.CBR | TileType.CTL |TileType.CTR | TileType.Cross);
       //
       //  a[TPowerReverse(TileType.Blank), (int)Direction.Right] = (int) (TileType.Blank | TileType.Up |TileType.Down | TileType.Left | TileType.Right |TileType.Ll | TileType.Lup | TileType.Up |TileType.CBL | TileType.CBR | TileType.CTL |TileType.CTR | TileType.Cross);
       //
       //  //
       //  a[TPowerReverse(TileType.Down), (int)Direction.Up] = (int) (TileType.Blank |TileType.Ll | TileType.Up | TileType.CBL | TileType.CBR );
       //
       //  a[TPowerReverse(TileType.Down), (int)Direction.Left] = (int) (TileType.Right | TileType.Up | TileType.Down | TileType.CTR | TileType.CBR | TileType.Down | TileType.Ll| TileType.Cross );
       //
       //  a[TPowerReverse(TileType.Down), (int)Direction.Down] = (int) (TileType.Lup | TileType.Up | TileType.CBL | TileType.CBR | TileType.Left | TileType.Right| TileType.Cross );
       //
       //  a[TPowerReverse(TileType.Down), (int)Direction.Right] = (int) (TileType.Left | TileType.Up | TileType.Down | TileType.CTL |TileType.CBL | TileType.Ll | TileType.Cross);
       // //
       //
       //  a[TPowerReverse(TileType.Right), (int)Direction.Up] = (int) (TileType.Left | TileType.Down | TileType.Right | TileType.Lup | TileType.CTL | TileType.CTR | TileType.Cross);
       //
       //  a[TPowerReverse(TileType.Right), (int)Direction.Left] = (int) (TileType.Blank | TileType.Left | TileType.Lup | TileType.CTL | TileType.CBL);
       //
       //  a[TPowerReverse(TileType.Right), (int)Direction.Down] = (int) (TileType.Left | TileType.Up | TileType.Right | TileType.Lup | TileType.CBR | TileType.CBL | TileType.Cross);
       //
       //  a[TPowerReverse(TileType.Right), (int)Direction.Right] = (int) ( TileType.Down | TileType.Up | TileType.Left | TileType.Ll  | TileType.CTL | TileType.CBL | TileType.Cross);
       //  //
       //  a[TPowerReverse(TileType.Left), (int)Direction.Up] = (int) (TileType.Left | TileType.Down | TileType.Right | TileType.Lup | TileType.CTL | TileType.CTR | TileType.Cross);
       //
       //  a[TPowerReverse(TileType.Left), (int)Direction.Left] = (int) (TileType.Up | TileType.Down | TileType.Right | TileType.Ll | TileType.CTR | TileType.CBR | TileType.Cross);
       //
       //  a[TPowerReverse(TileType.Left), (int)Direction.Down] = (int) (TileType.Left | TileType.Right | TileType.Up | TileType.Lup | TileType.CBR | TileType.CBL | TileType.Cross);
       //
       //  a[TPowerReverse(TileType.Left), (int)Direction.Right] = (int) (TileType.Blank |TileType.Blank | TileType.Right | TileType.CTR | TileType.CBR | TileType.Lup);
       //  //
       //  
       //  a[TPowerReverse(TileType.Up), (int)Direction.Up] = (int) (TileType.Left | TileType.Down | TileType.Right | TileType.Lup | TileType.CTL | TileType.CTR | TileType.Cross );
       //
       //  a[TPowerReverse(TileType.Up), (int)Direction.Left] = (int) (TileType.Up | TileType.Down | TileType.Right | TileType.Ll | TileType.CTR | TileType.CBR | TileType.Cross);
       //
       //  a[TPowerReverse(TileType.Up), (int)Direction.Down] = (int) (TileType.Blank | TileType.Down | TileType.Ll | TileType.CTL | TileType.CTR);
       //
       //  a[TPowerReverse(TileType.Up), (int)Direction.Right] = (int) (TileType.Up | TileType.Down | TileType.Left | TileType.Ll | TileType.CTL | TileType.CBL | TileType.Cross);
       //
       //  //
       //  a[TPowerReverse(TileType.Lup), (int)Direction.Up] = (int) (TileType.Blank |TileType.Left | TileType.Down | TileType.Right | TileType.CTL | TileType.CTR | TileType.Lup | TileType.Cross);
       //
       //  a[TPowerReverse(TileType.Lup), (int)Direction.Left] = (int) ( TileType.Left | TileType.CBL | TileType.CTL | TileType.Lup);
       //
       //  a[TPowerReverse(TileType.Lup), (int)Direction.Down] = (int) (TileType.Blank|TileType.Up | TileType.Left | TileType.Right | TileType.CBR | TileType.CBL | TileType.Lup | TileType.Cross);
       //
       //  a[TPowerReverse(TileType.Lup), (int)Direction.Right] = (int) (TileType.Right | TileType.CBR | TileType.CTR | TileType.Lup);
       //
       //  //
       //  
       //  a[TPowerReverse(TileType.Ll), (int)Direction.Up] = (int) (TileType.Up | TileType.Ll | TileType.CBL | TileType.CBR);
       //
       //  a[TPowerReverse(TileType.Ll), (int)Direction.Left] = (int) (TileType.Blank |TileType.Up | TileType.Down | TileType.Right | TileType.Ll | TileType.CTR | TileType.CBR  | TileType.Cross);
       //
       //  a[TPowerReverse(TileType.Ll), (int)Direction.Down] = (int) (TileType.Down | TileType.Ll | TileType.CTL | TileType.CTR);
       //
       //  a[TPowerReverse(TileType.Ll), (int)Direction.Right] = (int) (TileType.Blank |TileType.Blank |TileType.Up | TileType.Down | TileType.Left | TileType.Ll | TileType.CTL | TileType.CBL | TileType.Cross);
       //
       //  //
       //  
       //  a[TPowerReverse(TileType.CBL), (int)Direction.Up] = (int) (TileType.Left | TileType.Down | TileType.Right | TileType.Lup | TileType.CTR | TileType.CTL | TileType.Cross );
       //
       //  a[TPowerReverse(TileType.CBL), (int)Direction.Left] = (int) (TileType.Right | TileType.Down | TileType.Up | TileType.Ll | TileType.CTR | TileType.CBR | TileType.Cross);
       //
       //  a[TPowerReverse(TileType.CBL), (int)Direction.Down] = (int) ( TileType.Down | TileType.Ll | TileType.CTR | TileType.CTL);
       //
       //  a[TPowerReverse(TileType.CBL), (int)Direction.Right] = (int) (TileType.Right |  TileType.Lup | TileType.CBR | TileType.CTR);
       //
       //  //
       //
       //  a[TPowerReverse(TileType.CBR), (int)Direction.Up] = (int) (TileType.Left | TileType.Down | TileType.Right | TileType.Lup | TileType.CTR | TileType.CTL | TileType.Cross);
       //
       //  a[TPowerReverse(TileType.CBR), (int)Direction.Left] = (int) (TileType.Right | TileType.Down | TileType.Up | TileType.Ll | TileType.CBL | TileType.CTL);
       //
       //  a[TPowerReverse(TileType.CBR), (int)Direction.Down] = (int) (TileType.Down | TileType.Ll | TileType.CTR | TileType.CTL);
       //
       //  a[TPowerReverse(TileType.CBR), (int)Direction.Right] = (int) (TileType.Left |  TileType.Up  |  TileType.Down| TileType.Ll | TileType.CBL | TileType.CTL | TileType.Cross);
       //
       //  
       //  //
       //  
       //  a[TPowerReverse(TileType.CTL), (int)Direction.Up] = (int) ( TileType.Up | TileType.Ll | TileType.CBR | TileType.CBL);
       //
       //  a[TPowerReverse(TileType.CTL), (int)Direction.Left] = (int) (TileType.Right | TileType.Down | TileType.Up | TileType.Ll | TileType.CTR | TileType.CBR | TileType.Cross);
       //
       //  a[TPowerReverse(TileType.CTL), (int)Direction.Down] = (int) (TileType.Up | TileType.Left | TileType.Right | TileType.Lup | TileType.CBR | TileType.CBL | TileType.Cross);
       //
       //  a[TPowerReverse(TileType.CTL), (int)Direction.Right] = (int) (TileType.Right | TileType.Lup | TileType.CTR | TileType.CBR);
       //
       //  //
       //  
       //  a[TPowerReverse(TileType.CTR), (int)Direction.Up] = (int) (TileType.Up  | TileType.Ll | TileType.CBR | TileType.CBL);
       //
       //  a[TPowerReverse(TileType.CTR), (int)Direction.Left] = (int) (TileType.Left | TileType.Lup | TileType.CTL | TileType.CBL);
       //
       //  a[TPowerReverse(TileType.CTR), (int)Direction.Down] = (int) (TileType.Up | TileType.Left | TileType.Right | TileType.Lup | TileType.CBR | TileType.CBL | TileType.Cross);
       //
       //  a[TPowerReverse(TileType.CTR), (int)Direction.Right] = (int) (TileType.Left | TileType.Down | TileType.Up | TileType.Ll | TileType.CTL | TileType.CBL | TileType.Cross);
       //
       //  
       //  //
       //  a[TPowerReverse(TileType.Cross), (int)Direction.Up] = (int) (TileType.Down | TileType.Left | TileType.Right  | TileType.Lup | TileType.CTR | TileType.CTL | TileType.Cross);
       //
       //  a[TPowerReverse(TileType.Cross), (int)Direction.Left] = (int) (TileType.Right | TileType.Down | TileType.Up |TileType.Ll | TileType.Cross | TileType.CBR | TileType.CTR);
       //
       //  a[TPowerReverse(TileType.Cross), (int)Direction.Down] = (int) (TileType.Up | TileType.Left | TileType.Right  | TileType.Lup | TileType.CBR | TileType.CBL | TileType.Cross);
       //
       //  a[TPowerReverse(TileType.Cross), (int)Direction.Right] = (int) (TileType.Left | TileType.Down | TileType.Up |TileType.Ll | TileType.Cross | TileType.CBL | TileType.CTL | TileType.Cross);
       //
       //  
       //  
        
        
        
        
        
        return a;
    }

    // Start is called before the first frame update
    void Start()
    {
//replace array mover with acessing xy stuct
        
        tex = stex;
        Tiles = TTemp;
        MazePart = TMaze;
        rules = initrules();


        totaly = totalx;

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
               MasterTiles[i,j].Inst( new Vector2(i,j) );
               
                
                
            }
            
        }

        int sx = Random.Range(0, totalx);
        int sy = Random.Range(0, totaly);
        int TT = Random.Range(0, totalTiles);
        MasterTiles[sx, sy].Known = true;
        MasterTiles[sx, sy].Entropy = 1;
        MasterTiles[sx, sy].possibility = (MasterTiles[sx, sy].possibility & (int)1<<TT);
        
        print("StartBlock is" +(TileType)(int) MasterTiles[sx, sy].possibility );
        print((sx,sy));
        //MasterTiles[sx,sy].IntetoImage(TT);
        MasterTiles[sx,sy].AddNewBlock(TT);
       
       UpdateNeighbour(1, 0, Direction.Right, (sx,sy), TT);
       UpdateNeighbour(-1, 0, Direction.Left, (sx,sy), TT);
       UpdateNeighbour(0, 1, Direction.Up, (sx,sy), TT);
       UpdateNeighbour(0, -1, Direction.Down, (sx,sy), TT);
       //printarray(totalx);
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

        for (int i = 0; i < totaly; i++)
        {
            for (int j = 0; j < totalx; j++)
            {
              //  print("i: "+i+" j: "+j+" "+MasterTiles[i, j].Entropy);
                if (MasterTiles[j, i].Entropy < target && !MasterTiles[j, i].Known )
                {
                    target = MasterTiles[j, i].Entropy;
                  
                    targetcord = (j, i);
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

        if (count >= 99)
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
//            print((TileType)(int)MasterTiles[coord.Item1+ offx,coord.Item2+offy].possibility);
            //fix entroy do count thing
           
           
            //print("pre pos: " + (MasterTiles[coord.Item1+ offx,coord.Item2+offy].possibility + " Ander "+rules[TT,(int)Dir]));
           
           // MasterTiles[coord.Item1+ offx,coord.Item2+offy].possibility = (MasterTiles[coord.Item1+ offx,coord.Item2+offy].possibility & rules[TT,(int)Diru ]);
          
          // print(TT);
            MasterTiles[coord.Item1+ offx,coord.Item2+offy].possibility = (MasterTiles[coord.Item1+ offx,coord.Item2+offy].possibility & rules[TT,(int)Diru ]);
        

            
            
            
            
            
          
           // print("A i: x+ (coord.Item1+ offx) + " j: "+(coord.Item2+offy) + " Pos: "+MasterTiles[coord.Item1+ offx,coord.Item2+offy].possibility);
            MasterTiles[coord.Item1+ offx,coord.Item2+offy].Entropy =  nuentropy(coord.Item1+ offx, coord.Item2+offy);
           

        }
        else
        {
            

           
        }
                   







    }

    
    
    
    
     void move(ScrollDir dir)
    {
        switch (dir)
        {
            
            case ScrollDir.right:
                
                for (int i = 0; i < totalx; i++)
                {
                   
                    MasterTiles[0,i ].DelObj();
                }
                
                
                for (int i = 1; i < totalx ; i++)
                {
                    for (int j = 0; j < totalx; j++)
                    {
                     
                        MasterTiles[ (i - 1),j  ] = MasterTiles[ (i),j ];

                    }


                }

                for (int i = 0; i < totalx; i++)
                {
                   
                    MasterTiles[(totalx - 1),i ].xy = new Vector2(-1 , -1);
                }
                
                break;
            
            case ScrollDir.left:
                
                for (int i = 0; i < totalx; i++)
                {
                   
                    MasterTiles[(totalx - 1),i ].DelObj();
                }
                
                
                
                for (int i = totalx-1; i > 0 ; i--)
                {
                    for (int j = 0; j < totalx; j++)
                    {
                     
                        MasterTiles[i,j] = MasterTiles[(i- 1),j ];

                    }


                }

                for (int i = 0; i < totalx; i++)
                {
                   
                    MasterTiles[0,i ].xy = new Vector2(-1 , -1);
                }
                
                break;
            
            
            case ScrollDir.down:
                
                for (int i = 0; i < totalx; i++)
                {
                  
                    MasterTiles[ i , totalx - 1].DelObj();
                }
                
                
                for (int i = totalx-1; i > 0 ; i--)
                {
                    for (int j = 0; j < totalx; j++)
                    {
                   
                        MasterTiles[j,i] = MasterTiles[j, i-1];


                    }


                }

                for (int i = 0; i < totalx; i++)
                {
                  
                    MasterTiles[ i,0].xy = new Vector2(-1 , -1);
//optimize combine with replace
                }

                
                break;
            
            
            case ScrollDir.up:
                
                for (int i = 0; i < totalx; i++)
                {
                  
                    MasterTiles[ i,0].DelObj();
//optimize combine with replace
                }
                
                
                for (int i = 1; i < totalx ; i++)
                {
                    for (int j = 0; j < totalx; j++)
                    {
                     
                        MasterTiles[ (j) , i-1] = MasterTiles[ ( j) , i] ;

                    }


                }

                for (int i = 0; i < totalx; i++)
                {
                  
                    MasterTiles[ i , totalx - 1].xy = new Vector2(-1 , -1);
                }

                break;
            
                
            
            
            
            
            
            
        }

        
        

       






    }

    void replace(ScrollDir dir)
    {
        
        //cache the y instead of clal from stored
        
        
        Vector2 start = Vector2.zero;
        Vector2 nustart = Vector2.zero;
        switch (dir)
        {
            case ScrollDir.left:
             
              
        
                for (int i = totalx-1; i >= 0; i--)
                {
                    
                   
                    MasterTiles[0,i].Inst( new Vector2(MasterTiles[1,i].xy.x-1,MasterTiles[1,i].xy.y  ));
                 
                  
                    //please amke tuple vector 2
                    (int, int) xy = ((int)1, i);
                    UpdateNeighbour(-1, 0, Direction.Left, xy, TPowerReverseBigInt(MasterTiles[1, i].possibility));
                    
                    
                    
                }
             
                break;
            
            case ScrollDir.right:
            
                print("right");
               // print("big start");
                for (int i = 0; i < totalx; i++)
                { if (i == totalx - 1)
                    {
                        print((MasterTiles[((totalx - 2)), i].possibility));
                    }
                    
                    nustart = MasterTiles[(totalx-2),i].xy  ;
                    MasterTiles[ ( (totalx - 1)) ,i].Inst(new Vector2(MasterTiles[ ( (totalx - 2)),i].xy.x+1,nustart.y));
                  
                    (int, int) xy = ((int) ( (totalx - 2)), (int)i);
                   
                    //fix right thing look at workignthign
                    
                   UpdateNeighbour(1, 0, Direction.Right, xy, TPowerReverseBigInt(MasterTiles[( (totalx - 2)),i].possibility));
                    //print((xy.Item1+1)+", "+xy.Item2);
                    //print(MasterTiles[(totalx-1),i].possibility);
                }
               // print("out a here");
                break;
            case ScrollDir.down:
               
        
                for (int i = 0; i < totalx; i++)
                {
                    
                   MasterTiles[i,0].Inst(new Vector2(MasterTiles[i,1].xy.x ,MasterTiles[i,1].xy.y-1));
              
                   
                   (int, int) xy = ((int)i, 1);
                   UpdateNeighbour(0, -1, Direction.Down, xy, TPowerReverseBigInt(MasterTiles[i,1].possibility));
                  
                
                }
                
                break;
            case ScrollDir.up:
             print("up");
               
                for (int i = 0; i < totalx; i++)
                {
                    if (i == totalx - 1)
                    {
                        print(MasterTiles[i,  totalx-2].possibility);
                    }

                   
                    MasterTiles[i, totalx-1].Inst( new Vector2(MasterTiles[i,  totalx-2].xy.x ,MasterTiles[i,  totalx-2].xy.y+1 ));
                   
                    
                    (int, int) xy = (i,   totalx-2);
                    UpdateNeighbour(0, 1, Direction.Up, xy, TPowerReverseBigInt(MasterTiles[i,  totalx-2].possibility));
                   
                }
              
                break;
            
               
            
        }
       // printarray(totalx);
        
        
        
        
        
        
    }

    
    
    
    void printarray(int fsize)
    {
        string a = "";
        string b = "";

        for(int i =fsize-1; i >= 0 ; i--){
            for (int j = 0; j < fsize; j++)
            {
                
                b += MasterTiles[i,j].xy.ToString();
                b += " ";

            }
                
            a += "\n";
            b += "\n";
        }

        //print(a);
       print(b);
    }
    
    
    
    
    
    // Update is called once per frame
    void Update()
    {

       
        //do thos in start
        //while not done
        
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
                // UpdateNeighbour(1, 0, Direction.Right, tind, TT);
                // UpdateNeighbour(-1, 0, Direction.Left, tind, TT);
                // UpdateNeighbour(0, 1, Direction.Up, tind, TT);
                // UpdateNeighbour(0, -1, Direction.Down, tind, TT);
                UpdateNeighbour(1, 0, Direction.Right, tind, TT);
                UpdateNeighbour(-1, 0, Direction.Left, tind, TT);
                UpdateNeighbour(0, 1, Direction.Up, tind, TT);
                UpdateNeighbour(0, -1, Direction.Down, tind, TT);
                
                
                
                
                
                
                




alltrue = done();
                TempTimer = Timer;
              

            }

           TempTimer -= Time.deltaTime;


         
           bool moved = false;
           ScrollDir direction = ScrollDir.down;
           if (Input.GetKeyDown(KeyCode.W))
           {
               print(ScrollDir.up);
               direction = ScrollDir.up;
               moved = true;
           }
           else if (Input.GetKeyDown(KeyCode.S))
           {
               print(ScrollDir.down);
               moved = true;
               direction = ScrollDir.down;
           }
           else if (Input.GetKeyDown(KeyCode.D))
           {
               print(ScrollDir.right);
               moved = true;
               direction = ScrollDir.right;
           }
           else if (Input.GetKeyDown(KeyCode.A))
           {
               print(ScrollDir.left);
               moved = true;
               direction = ScrollDir.left;
            
           }

           double nutim = 0;
           // move(direction);
           //replace(direction);
           
           
//do compute shader

           if (moved)
           {
               print("Moved : ");
             print(direction);
               move(direction);
               replace(direction);
//                print("Were Here");
                alltrue = false;

                // ArrMover mov = new ArrMover() { fieldsize = totaly, odfield = MasterTiles, d = direction };
                // JobHandle jb = mov.Schedule(fieldsize, 128);
                // jb.Complete();
                print(moveTracker);
                moveTracker++;

           }




    }
    
    
//     
//     [BurstCompile]
//     struct ArrMover : IJobParallelFor
//     {
//         //redo for diffrent axis
//         public int fieldsize;
//         [NativeDisableParallelForRestriction] public NativeArray<TileInfo> odfield;
//         public ScrollDir d;
//         
//         void move(ScrollDir dir, int index)
//     {
//         TileInfo tmp;
//         switch (dir)
//         {
//                
//             case ScrollDir.up:
//                 for (int i = 1; i < fieldsize ; i++)
//                 {
//                     
//                       
//                         odfield[index + (i - 1) * fieldsize] =  odfield[index + (i) * fieldsize];
//
//                     
//
//
//                 }
//
//                  tmp = odfield[index + (fieldsize - 1) * fieldsize];
//                 tmp.xy =new Vector2(-1 , -1);
//
//                 odfield[index + (fieldsize - 1) * fieldsize] = tmp;
//                 
//                 
//                 break;
//             
//             case ScrollDir.down:
//                 for (int i = fieldsize-1; i > 0 ; i--)
//                 {
//                    
//                       
//                         odfield[index + (i ) * fieldsize] = odfield[index + (i- 1) * fieldsize];
//
//
//
//                 }
//
//                 
//                
//
//                 tmp = odfield[index] ;
//                 tmp.xy =new Vector2(-1 , -1);
//                 odfield[index ] = tmp;
//                 
//           
//                 
//                 break;
//             
//             
//             case ScrollDir.left:
//                 for (int i = fieldsize-1; i > 0 ; i--)
//                 {
//                    
//                     
//                         odfield[index * fieldsize + i] = odfield[index * fieldsize + (i - 1)];
//
//
//                     
//
//
//                 }
//
//                 tmp = odfield[ fieldsize * index] ;
//                 tmp.xy =new Vector2(-1 , -1);
//                 odfield[ fieldsize * index ] = tmp;
//                  
//                    // odfield[ fieldsize * index] = new Vector2(-1 , -1);
// //optimize combine with replace
//                
//
//                 
//                 break;
//             
//             
//             case ScrollDir.right:
//                 for (int i = 1; i < fieldsize ; i++)
//                 {
//                    
//                       
//                         odfield[ (fieldsize * index) + i-1] = odfield[ (fieldsize * index) + i] ;
//
//                    
//
//
//                 }
//
//            
//                 tmp = odfield[(fieldsize * index) + fieldsize - 1] ;
//                 tmp.xy =new Vector2(-1 , -1);
//                 odfield[ (fieldsize * index) + fieldsize - 1 ] = tmp;
//                     // odfield[(fieldsize * index) + fieldsize - 1] = new Vector2(-1 , -1);
//                 
//
//                 break;
//             
//                 
//             
//             
//             
//             
//             
//             
//         }
//
//         
//         
//
//        
//
//
//
//
//
//
//     }
//     
//      void replace(ScrollDir dir, int index)
//     {
//         TileInfo tmp;
//         Vector2 nustart = Vector2.zero;
//         switch (dir)
//         {
//             case ScrollDir.down:
//                
//              
//                 tmp = new TileInfo();
//                 tmp.Inst(  new  Vector2(odfield[fieldsize+index].xy.x,odfield[fieldsize+index].xy.y -1 ));
//                // tmp.xy =new  Vector2(odfield[fieldsize+index].xy.x,odfield[fieldsize+index].xy.y -1 );
//                 odfield[ index ] = tmp;
//
//                    
//                   
//
//                 
//                 break;
//             
//             case ScrollDir.up:
//               //do before threading
//                
//               tmp = new TileInfo();
//               tmp.Inst(  new Vector2(odfield[index + (fieldsize * (fieldsize - 2))].xy.x,odfield[index + (fieldsize * (fieldsize - 2))].xy.y +1));
//               // tmp.xy =new  Vector2(odfield[fieldsize+index].xy.x,odfield[fieldsize+index].xy.y -1 );
//               odfield[ index + (fieldsize * (fieldsize - 1)) ] = tmp;
//               
//                 
//                  
//
//                 
//                 
//                 break;
//             case ScrollDir.left:
//                 //start = field[fieldsize - 2, fieldsize-1]  ;
//         
//                 tmp = new TileInfo();
//                 tmp.Inst(  new Vector2(odfield[index*fieldsize+1].xy.x -1,odfield[index*fieldsize+1].xy.y));
//                 // tmp.xy =new  Vector2(odfield[fieldsize+index].xy.x,odfield[fieldsize+index].xy.y -1 );
//                 odfield[ index*fieldsize ] = tmp;
//                    
//                   
//                    // ++start;
//
//                 
//                 
//                 break;
//             case ScrollDir.right:
//                 //start = field[fieldsize - 2, fieldsize-1]  ;
//         
//                 tmp = new TileInfo();
//                 tmp.Inst(  new Vector2(odfield[index*fieldsize + fieldsize-2].xy.x + 1,odfield[index*fieldsize + fieldsize-2].xy.y ));
//                 odfield[index*fieldsize + fieldsize-1 ] = tmp;
//                    
//                
//                 
//                 break;
//             
//             
//             
//         }
//         
//         
//         
//         
//         
//         
//         
//     }   
//         public void Execute(int i)
//         {
//             
//             move(d, i);
//             replace(d,i);
//             
//             
//         }
//     
//     
//     
//     
//     }
//     
//
//
//     
//     
//     
//     
//     
//     
    
    
    
    
    
}































/*
 MAYBE LOOK INTO Later
 
 
 uint64_t v;          // Input value to find position with rank r.
  unsigned int r;      // Input: bit's desired rank [1-64].
  unsigned int s;      // Output: Resulting position of bit with rank r [1-64]
  uint64_t a, b, c, d; // Intermediate temporaries for bit count.
  unsigned int t;      // Bit count temporary.

  // Do a normal parallel bit count for a 64-bit integer,                     
  // but store all intermediate steps.                                        
  // a = (v & 0x5555...) + ((v >> 1) & 0x5555...);
  a =  v - ((v >> 1) & ~0UL/3);
  // b = (a & 0x3333...) + ((a >> 2) & 0x3333...);
  b = (a & ~0UL/5) + ((a >> 2) & ~0UL/5);
  // c = (b & 0x0f0f...) + ((b >> 4) & 0x0f0f...);
  c = (b + (b >> 4)) & ~0UL/0x11;
  // d = (c & 0x00ff...) + ((c >> 8) & 0x00ff...);
  d = (c + (c >> 8)) & ~0UL/0x101;
  t = (d >> 32) + (d >> 48);
  // Now do branchless select!                                                
  s  = 64;
  // if (r > t) {s -= 32; r -= t;}
  s -= ((t - r) & 256) >> 3; r -= (t & ((t - r) >> 8));
  t  = (d >> (s - 16)) & 0xff;
  // if (r > t) {s -= 16; r -= t;}
  s -= ((t - r) & 256) >> 4; r -= (t & ((t - r) >> 8));
  t  = (c >> (s - 8)) & 0xf;
  // if (r > t) {s -= 8; r -= t;}
  s -= ((t - r) & 256) >> 5; r -= (t & ((t - r) >> 8));
  t  = (b >> (s - 4)) & 0x7;
  // if (r > t) {s -= 4; r -= t;}
  s -= ((t - r) & 256) >> 6; r -= (t & ((t - r) >> 8));
  t  = (a >> (s - 2)) & 0x3;
  // if (r > t) {s -= 2; r -= t;}
  s -= ((t - r) & 256) >> 7; r -= (t & ((t - r) >> 8));
  t  = (v >> (s - 1)) & 0x1;
  // if (r > t) s--;
  s -= ((t - r) & 256) >> 8;
  s = 65 - s;*/