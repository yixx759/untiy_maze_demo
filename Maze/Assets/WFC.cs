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
    
    
     private ulong seed = 37653;
     private ulong seeda = 642362;
     private ulong seedb = 2266742;
    
    struct TwistConst
    {
        public const int n = 624;
        public const ulong f = 1812433253UL;
        public const int w = 32;
        public const ulong a= 0x9908b0dfUL;
        public const ulong b =0x9d2c5680UL;
        public const ulong c =0xefc60000UL;
    
        public const int m = 397;
        public const int u = 11;
    
        public const int r = 31;
    
        public const ulong LMASK = (0xffffffffUL >> (w - r));
        public const  ulong UMASK =(0xffffffffUL << r);
    }
    
    
    struct StateTracker
    {
        public ulong[] states;
        public int curstate;


    }

    
    
    void initilize_State(ref StateTracker s)
    {
       
        s.states = new ulong[TwistConst.n];
        ulong useed = (seed*(ulong)DateTime.Now.Millisecond);
        s.states[0] = useed ;
    
        for (int i = 1; i < TwistConst.n; i++)
        {
            useed = (uint)(TwistConst.f * (useed ^ (useed >> (TwistConst.w - 2))));
            
            s.states[i] = useed;
    
    
        }
    
        s.curstate = 0;
    
    
    }
    
    ulong MTwist(ref StateTracker s)
    {
        int k = s.curstate;
    
        int j = k - (TwistConst.n - 1);
        if (j < 0) {j += TwistConst.n;}
    
        ulong x = (ulong)((s.states[k] & TwistConst.UMASK) | (s.states[j] & TwistConst.LMASK));
        ulong xA = x >> 1;
    
        if ((x & 1) == 1)
        {
            xA= (uint)(xA^TwistConst.a);
    
        }
    
        j = k - (TwistConst.n - TwistConst.m);
        if (j < 0){ j += TwistConst.n;}
    
    
        x = s.states[j] ^ xA;
    
        s.states[k++] = x;
        if (k >= TwistConst.n)
        {
            k = 0;
        }
    
        s.curstate = k;
    
        ulong y = x ^ (x >> TwistConst.u);
    
        y = (uint)((y ^ ((y << 7) & TwistConst.b)));
        y = (uint)((y ^ ((y << 15) & TwistConst.c)));
        ulong z = y ^ (y >> 1);
    
        return z;
    
    }

    
    private ulong nuLCG( ulong mod, ulong a, ulong c)
    {
        unchecked
        {
            ulong tmp = (seed * a + c) % mod;
            return tmp;
        }
        
       
      

       


    }
    
    
    
    
    
    
    
    
    
    
    private Material mat;
    [SerializeField] private GameObject Plane;
    [SerializeField] private static Texture tex;
    [SerializeField] private Texture stex;
    [SerializeField] private Texture Tex;
    [SerializeField] private float offset;
    private int moveTracker = 0;

    
    //think abvout using async load 
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

    private int[,] rules;
    private int[,] rulenum;
    private bool alltrue = false;
    [SerializeField] static private Texture[] Tiles;
    [SerializeField] static private GameObject[] MazePart;
    [SerializeField] private GameObject[] TMaze;
    [SerializeField] private Texture[] TTemp;
    [SerializeField] private float Timer = 2;
    private float TempTimer = 2;
    [SerializeField] private int lesschanceofblank = 0;

    private static Vector2 blockLength = new Vector2(16.8f, 16.8f);
    //add colluider
//put in struct
    const long m1 = 0x5555555555555555; //binary: 0101...
    const long m2 = 0x3333333333333333; //binary: 00110011..
    const long m4 = 0x0f0f0f0f0f0f0f0f; //binary:  4 zeros,  4 ones ...
    const long m8 = 0x00ff00ff00ff00ff; //binary:  8 zeros,  8 ones ...
    const long m16 = 0x0000ffff0000ffff; //binary: 16 zeros, 16 ones ...
    const long m32 = 0x00000000ffffffff; //binary: 32 zeros, 32 ones


    const uint i1 = 0x55555555; //binary: 0101...
    const uint i2 = 0x33333333; //binary: 00110011..
    const uint i4 = 0x0f0f0f0f; //binary:  4 zeros,  4 ones ...
    const uint i8 = 0x00ff00ff; //binary:  8 zeros,  8 ones ...

    const uint i16 = 0x0000ffff; //binary: 16 zeros, 16 ones ...
    //const  int i32 = 0x00000000ffffffff; 

    [Flags]
    enum TileType
    {
        Blank = 1,
        Down = 2,
        Left = 4,
        Right = 8,
        Up = 16,
        Lup = 32,
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

        Down = 0,
        Left = 1,
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
                // print("broke");
                return 500;


        }



    }


    private uint selectbitNoBranch(uint v, int range, (int, int) coord)
    {

        // Input value to find position with rank r.

        //  v = 5024 ;
        //r = (uint)rank;

//       print(System.Convert.ToString(v,2));


        uint a = (v & i1) + ((v >> 1) & i1); //put count of each  2 bits into those  2 bits 
        uint b = (a & i2) + ((a >> 2) & i2); //put count of each  4 bits into those  4 bits 
        uint c = (b & i4) + ((b >> 4) & i4); //put count of each  8 bits into those  8 bits 
        uint t = (c & i8) + ((c >> 8) & i8); //put count of each 16 bits into those 16 bits 

        t = (t >> 16) & 0xffff;

        
        //account for neg numbers
        
       // uint r = (uint)Random.Range(1, range + 1);
       uint r = 1;
       if (range != 1)
       {
           r = (uint) nuLCG((ulong)range-1 , (seeda * (uint)coord.Item1), (seedb * (uint)coord.Item2)) + 1;

           
       }
      

      
        
        
        //test with entropy
        print("R: " + r);
        uint s = 32;

        int ia = 1;

        s = 32;


        //
        // if (r > t) {s -= 16; r -= t;}
        s -= ((t - r) & 256) >> 4;
        r -= (t & ((t - r) >> 8));

        //if (r > t) {s -= 8; r -= t;}


        t = (c >> (int)(s - 8)) & 0xf;

        // if (r > t) {s -= 8; r -= t;}
        s -= ((t - r) & 256) >> 5;
        r -= (t & ((t - r) >> 8));
        t = (b >> (int)(s - 4)) & 0x7;

        //if (r > t) {s -= 4; r -= t;}
        s -= ((t - r) & 256) >> 6;
        r -= (t & ((t - r) >> 8));
        t = (a >> (int)(s - 2)) & 0x3;

        // if (r > t) {s -= 2; r -= t;}
        s -= ((t - r) & 256) >> 7;
        r -= (t & ((t - r) >> 8));
        t = (v >> (int)(s - 1)) & 0x1;

        //if (r > t) s--;
        s -= ((t - r) & 256) >> 8;

        return s;







        // return s;
    }

    

    struct TileInfo
    {
        public int possibility;
        public int Entropy;
        public bool Known;

        public GameObject plane;

        private Material mat;
        public Vector2 xy;


        //just add tyle type
        // have log ver and non log
        //do their version and compare with this one

        //create test tile

        public void Inst(Vector2 i_xy)
        {
            possibility = (int)(TileType.Cross | TileType.Blank | TileType.Down | TileType.Left | TileType.Right |
                                TileType.Up | TileType.Ll | TileType.Lup | TileType.CBL | TileType.CBR | TileType.CTL |
                                TileType.CTR);
            // print( possibility);
            xy = i_xy;
            Entropy = totalTiles;
            Known = false;

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
            plane = Instantiate(MazePart[i], new UnityEngine.Vector3(xy.x * blockLength.x, 0, xy.y * blockLength.y),
                Quaternion.Euler(new UnityEngine.Vector3(-89.98f, -90, 0)));
            //plane.GetComponent<Renderer>().material.mainTexture = tex;
//-89.98
        }

        public void DelObj()
        {

            Destroy(plane);
            plane = null;



        }


    }


    int[,] initrules()
    {
        int[,] a = new int[totalTiles, 4];
        //remove blank
        //add blank

        a[TPowerReverse(TileType.Blank), (int)Direction.Up] = (int)(TileType.Blank | TileType.Up | TileType.Down |
                                                                    TileType.Left | TileType.Right | TileType.Ll |
                                                                    TileType.Lup | TileType.Up | TileType.CBL |
                                                                    TileType.CBR | TileType.CTL | TileType.CTR |
                                                                    TileType.Cross);

        a[TPowerReverse(TileType.Blank), (int)Direction.Left] = (int)(TileType.Blank | TileType.Up | TileType.Down |
                                                                      TileType.Left | TileType.Right | TileType.Ll |
                                                                      TileType.Lup | TileType.Up | TileType.CBL |
                                                                      TileType.CBR | TileType.CTL | TileType.CTR |
                                                                      TileType.Cross);

        a[TPowerReverse(TileType.Blank), (int)Direction.Down] = (int)(TileType.Blank | TileType.Up | TileType.Down |
                                                                      TileType.Left | TileType.Right | TileType.Ll |
                                                                      TileType.Lup | TileType.Up | TileType.CBL |
                                                                      TileType.CBR | TileType.CTL | TileType.CTR |
                                                                      TileType.Cross);

        a[TPowerReverse(TileType.Blank), (int)Direction.Right] = (int)(TileType.Blank | TileType.Up | TileType.Down |
                                                                       TileType.Left | TileType.Right | TileType.Ll |
                                                                       TileType.Lup | TileType.Up | TileType.CBL |
                                                                       TileType.CBR | TileType.CTL | TileType.CTR |
                                                                       TileType.Cross);

        //
        a[TPowerReverse(TileType.Down), (int)Direction.Up] =
            (int)(TileType.Blank | TileType.Ll | TileType.Up | TileType.CBL | TileType.CBR);

        a[TPowerReverse(TileType.Down), (int)Direction.Left] = (int)(TileType.Blank | TileType.Right | TileType.Up |
                                                                     TileType.Down | TileType.CTR | TileType.CBR |
                                                                     TileType.Down | TileType.Ll | TileType.Cross);

        a[TPowerReverse(TileType.Down), (int)Direction.Down] = (int)(TileType.Blank | TileType.Lup | TileType.Up |
                                                                     TileType.CBL | TileType.CBR | TileType.Left |
                                                                     TileType.Right | TileType.Cross);

        a[TPowerReverse(TileType.Down), (int)Direction.Right] = (int)(TileType.Blank | TileType.Left | TileType.Up |
                                                                      TileType.Down | TileType.CTL | TileType.CBL |
                                                                      TileType.Ll | TileType.Cross);
        //

        a[TPowerReverse(TileType.Right), (int)Direction.Up] = (int)(TileType.Blank | TileType.Left | TileType.Down |
                                                                    TileType.Right | TileType.Lup | TileType.CTL |
                                                                    TileType.CTR | TileType.Cross);

        a[TPowerReverse(TileType.Right), (int)Direction.Left] = (int)(TileType.Blank | TileType.Blank | TileType.Left |
                                                                      TileType.Lup | TileType.CTL | TileType.CBL);

        a[TPowerReverse(TileType.Right), (int)Direction.Down] = (int)(TileType.Blank | TileType.Left | TileType.Up |
                                                                      TileType.Right | TileType.Lup | TileType.CBR |
                                                                      TileType.CBL | TileType.Cross);

        a[TPowerReverse(TileType.Right), (int)Direction.Right] = (int)(TileType.Blank | TileType.Down | TileType.Up |
                                                                       TileType.Left | TileType.Ll | TileType.CTL |
                                                                       TileType.CBL | TileType.Cross);
        //
        a[TPowerReverse(TileType.Left), (int)Direction.Up] = (int)(TileType.Blank | TileType.Left | TileType.Down |
                                                                   TileType.Right | TileType.Lup | TileType.CTL |
                                                                   TileType.CTR | TileType.Cross);

        a[TPowerReverse(TileType.Left), (int)Direction.Left] = (int)(TileType.Blank | TileType.Up | TileType.Down |
                                                                     TileType.Right | TileType.Ll | TileType.CTR |
                                                                     TileType.CBR | TileType.Cross);

        a[TPowerReverse(TileType.Left), (int)Direction.Down] = (int)(TileType.Blank | TileType.Left | TileType.Right |
                                                                     TileType.Up | TileType.Lup | TileType.CBR |
                                                                     TileType.CBL | TileType.Cross);

        a[TPowerReverse(TileType.Left), (int)Direction.Right] = (int)(TileType.Blank | TileType.Blank | TileType.Blank |
                                                                      TileType.Right | TileType.CTR | TileType.CBR |
                                                                      TileType.Lup);
        //

        a[TPowerReverse(TileType.Up), (int)Direction.Up] = (int)(TileType.Blank | TileType.Left | TileType.Down |
                                                                 TileType.Right | TileType.Lup | TileType.CTL |
                                                                 TileType.CTR | TileType.Cross);

        a[TPowerReverse(TileType.Up), (int)Direction.Left] = (int)(TileType.Blank | TileType.Up | TileType.Down |
                                                                   TileType.Right | TileType.Ll | TileType.CTR |
                                                                   TileType.CBR | TileType.Cross);

        a[TPowerReverse(TileType.Up), (int)Direction.Down] = (int)(TileType.Blank | TileType.Blank | TileType.Down |
                                                                   TileType.Ll | TileType.CTL | TileType.CTR);

        a[TPowerReverse(TileType.Up), (int)Direction.Right] = (int)(TileType.Blank | TileType.Up | TileType.Down |
                                                                    TileType.Left | TileType.Ll | TileType.CTL |
                                                                    TileType.CBL | TileType.Cross);

        //
        a[TPowerReverse(TileType.Lup), (int)Direction.Up] = (int)(TileType.Blank | TileType.Blank | TileType.Left |
                                                                  TileType.Down | TileType.Right | TileType.CTL |
                                                                  TileType.CTR | TileType.Lup | TileType.Cross);

        a[TPowerReverse(TileType.Lup), (int)Direction.Left] =
            (int)(TileType.Blank | TileType.Left | TileType.CBL | TileType.CTL | TileType.Lup);

        a[TPowerReverse(TileType.Lup), (int)Direction.Down] = (int)(TileType.Blank | TileType.Blank | TileType.Up |
                                                                    TileType.Left | TileType.Right | TileType.CBR |
                                                                    TileType.CBL | TileType.Lup | TileType.Cross);

        a[TPowerReverse(TileType.Lup), (int)Direction.Right] =
            (int)(TileType.Blank | TileType.Right | TileType.CBR | TileType.CTR | TileType.Lup);

        //

        a[TPowerReverse(TileType.Ll), (int)Direction.Up] =
            (int)(TileType.Blank | TileType.Up | TileType.Ll | TileType.CBL | TileType.CBR);

        a[TPowerReverse(TileType.Ll), (int)Direction.Left] = (int)(TileType.Blank | TileType.Blank | TileType.Up |
                                                                   TileType.Down | TileType.Right | TileType.Ll |
                                                                   TileType.CTR | TileType.CBR | TileType.Cross);

        a[TPowerReverse(TileType.Ll), (int)Direction.Down] =
            (int)(TileType.Blank | TileType.Down | TileType.Ll | TileType.CTL | TileType.CTR);

        a[TPowerReverse(TileType.Ll), (int)Direction.Right] = (int)(TileType.Blank | TileType.Blank | TileType.Blank |
                                                                    TileType.Up | TileType.Down | TileType.Left |
                                                                    TileType.Ll | TileType.CTL | TileType.CBL |
                                                                    TileType.Cross);

        //

        a[TPowerReverse(TileType.CBL), (int)Direction.Up] = (int)(TileType.Blank | TileType.Left | TileType.Down |
                                                                  TileType.Right | TileType.Lup | TileType.CTR |
                                                                  TileType.CTL | TileType.Cross);

        a[TPowerReverse(TileType.CBL), (int)Direction.Left] = (int)(TileType.Blank | TileType.Right | TileType.Down |
                                                                    TileType.Up | TileType.Ll | TileType.CTR |
                                                                    TileType.CBR | TileType.Cross);

        a[TPowerReverse(TileType.CBL), (int)Direction.Down] =
            (int)(TileType.Blank | TileType.Down | TileType.Ll | TileType.CTR | TileType.CTL);

        a[TPowerReverse(TileType.CBL), (int)Direction.Right] =
            (int)(TileType.Blank | TileType.Right | TileType.Lup | TileType.CBR | TileType.CTR);

        //

        a[TPowerReverse(TileType.CBR), (int)Direction.Up] = (int)(TileType.Blank | TileType.Left | TileType.Down |
                                                                  TileType.Right | TileType.Lup | TileType.CTR |
                                                                  TileType.CTL | TileType.Cross);

        a[TPowerReverse(TileType.CBR), (int)Direction.Left] = (int)(TileType.Blank | TileType.Right | TileType.Down |
                                                                    TileType.Up | TileType.Ll | TileType.CBL |
                                                                    TileType.CTL);

        a[TPowerReverse(TileType.CBR), (int)Direction.Down] =
            (int)(TileType.Blank | TileType.Down | TileType.Ll | TileType.CTR | TileType.CTL);

        a[TPowerReverse(TileType.CBR), (int)Direction.Right] = (int)(TileType.Blank | TileType.Left | TileType.Up |
                                                                     TileType.Down | TileType.Ll | TileType.CBL |
                                                                     TileType.CTL | TileType.Cross);


        //

        a[TPowerReverse(TileType.CTL), (int)Direction.Up] =
            (int)(TileType.Blank | TileType.Up | TileType.Ll | TileType.CBR | TileType.CBL);

        a[TPowerReverse(TileType.CTL), (int)Direction.Left] = (int)(TileType.Blank | TileType.Right | TileType.Down |
                                                                    TileType.Up | TileType.Ll | TileType.CTR |
                                                                    TileType.CBR | TileType.Cross);

        a[TPowerReverse(TileType.CTL), (int)Direction.Down] = (int)(TileType.Blank | TileType.Up | TileType.Left |
                                                                    TileType.Right | TileType.Lup | TileType.CBR |
                                                                    TileType.CBL | TileType.Cross);

        a[TPowerReverse(TileType.CTL), (int)Direction.Right] =
            (int)(TileType.Blank | TileType.Right | TileType.Lup | TileType.CTR | TileType.CBR);

        //

        a[TPowerReverse(TileType.CTR), (int)Direction.Up] =
            (int)(TileType.Blank | TileType.Up | TileType.Ll | TileType.CBR | TileType.CBL);

        a[TPowerReverse(TileType.CTR), (int)Direction.Left] =
            (int)(TileType.Blank | TileType.Left | TileType.Lup | TileType.CTL | TileType.CBL);

        a[TPowerReverse(TileType.CTR), (int)Direction.Down] = (int)(TileType.Blank | TileType.Up | TileType.Left |
                                                                    TileType.Right | TileType.Lup | TileType.CBR |
                                                                    TileType.CBL | TileType.Cross);

        a[TPowerReverse(TileType.CTR), (int)Direction.Right] = (int)(TileType.Blank | TileType.Left | TileType.Down |
                                                                     TileType.Up | TileType.Ll | TileType.CTL |
                                                                     TileType.CBL | TileType.Cross);


        //
        a[TPowerReverse(TileType.Cross), (int)Direction.Up] = (int)(TileType.Blank | TileType.Down | TileType.Left |
                                                                    TileType.Right | TileType.Lup | TileType.CTR |
                                                                    TileType.CTL | TileType.Cross);

        a[TPowerReverse(TileType.Cross), (int)Direction.Left] = (int)(TileType.Blank | TileType.Right | TileType.Down |
                                                                      TileType.Up | TileType.Ll | TileType.Cross |
                                                                      TileType.CBR | TileType.CTR);

        a[TPowerReverse(TileType.Cross), (int)Direction.Down] = (int)(TileType.Blank | TileType.Up | TileType.Left |
                                                                      TileType.Right | TileType.Lup | TileType.CBR |
                                                                      TileType.CBL | TileType.Cross);

        a[TPowerReverse(TileType.Cross), (int)Direction.Right] = (int)(TileType.Blank | TileType.Left | TileType.Down |
                                                                       TileType.Up | TileType.Ll | TileType.Cross |
                                                                       TileType.CBL | TileType.CTL | TileType.Cross);


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
        StateTracker nus = new StateTracker();
        initilize_State(ref nus);
        seed = MTwist(ref nus);
         seeda = MTwist(ref nus);
         seedb = MTwist(ref nus);


        tex = stex;
        Tiles = TTemp;
        MazePart = TMaze;
        rules = initrules();


        totaly = totalx;

        MasterTiles = new TileInfo[totalx, totaly];

        Vector2 bnds = Plane.GetComponent<Renderer>().bounds.size;
        mat = Plane.GetComponent<Renderer>().sharedMaterial;
        mat.mainTexture = Tex;

        Vector2 start = (Vector2)transform.position + (Vector2.left * bnds.x + Vector2.down * bnds.y);

        for (int i = 0; i < totalx; i++)
        {
            for (int j = 0; j < totaly; j++)
            {
                MasterTiles[i, j].Inst(new Vector2(i, j));



            }

        }

        int sx = Random.Range(0, totalx);
        int sy = Random.Range(0, totaly);
        int TT = Random.Range(0, totalTiles);
        MasterTiles[sx, sy].Known = true;
        MasterTiles[sx, sy].Entropy = 1;
        MasterTiles[sx, sy].possibility = (MasterTiles[sx, sy].possibility & (int)1 << TT);

        MasterTiles[sx, sy].AddNewBlock(TT);

        UpdateNeighbour(1, 0, Direction.Right, (sx, sy), TT);
        UpdateNeighbour(-1, 0, Direction.Left, (sx, sy), TT);
        UpdateNeighbour(0, 1, Direction.Up, (sx, sy), TT);
        UpdateNeighbour(0, -1, Direction.Down, (sx, sy), TT);
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





//        print("NuWorked:");
        long xi = (long)MasterTiles[x, y].possibility;



        xi = (xi & m1) + ((xi >> 1) & m1); //put count of each  2 bits into those  2 bits 
        xi = (xi & m2) + ((xi >> 2) & m2); //put count of each  4 bits into those  4 bits 
        xi = (xi & m4) + ((xi >> 4) & m4); //put count of each  8 bits into those  8 bits 
        xi = (xi & m8) + ((xi >> 8) & m8); //put count of each 16 bits into those 16 bits 
        xi = (xi & m16) + ((xi >> 16) & m16); //put count of each 32 bits into those 32 bits 
        xi = (xi & m32) + ((xi >> 32) & m32); //put count of each 64 bits into those 64 bits 












        return (int)xi;

        // int count = 0;
        // int num = MasterTiles[x, y].possibility;
        // //automate this
        // for (int i = 0; i < totalTiles; i++)
        // {
        //     if ((num & 1) == 1)
        //     {
        //         count++;
        //
        //     }
        //     num = num >> 1;
        //
        // }




    }

    (int, int) findTarget()
    {

        (int, int) targetcord = (0, 0);
        int target = Int32.MaxValue;

        for (int i = 0; i < totaly; i++)
        {
            for (int j = 0; j < totalx; j++)
            {
                //  print("i: "+i+" j: "+j+" "+MasterTiles[i, j].Entropy);
                if (MasterTiles[j, i].Entropy < target && !MasterTiles[j, i].Known)
                {
                    target = MasterTiles[j, i].Entropy;

                    targetcord = (j, i);
                }

            }
        }

        return targetcord;


    }

    //
    int targetIndex((int, int) coord)
    {



        int g = Random.Range(0, lesschanceofblank + 1);
        uint num = (uint)MasterTiles[coord.Item1, coord.Item2].possibility;
        int ent = MasterTiles[coord.Item1, coord.Item2].Entropy;
        if (g % (lesschanceofblank + 1) != 0)
        {
            num = num ^ (1);
            ent--;

        }



        return (int)selectbitNoBranch(num, ent,coord) - 1;
        // return tt;


    }

    void UpdateNeighbour(int offx, int offy, Direction Diru, (int, int) coord, int TT)
    {


        if (coord.Item1 + offx < totalx && coord.Item2 + offy < totaly && coord.Item1 + offx >= 0 &&
            coord.Item2 + offy >= 0 && MasterTiles[coord.Item1 + offx, coord.Item2 + offy].Entropy != 0 &&
            MasterTiles[coord.Item1 + offx, coord.Item2 + offy].Known == false)
        {
//           
            MasterTiles[coord.Item1 + offx, coord.Item2 + offy].possibility =
                (MasterTiles[coord.Item1 + offx, coord.Item2 + offy].possibility & rules[TT, (int)Diru]);








            MasterTiles[coord.Item1 + offx, coord.Item2 + offy].Entropy =
                nuentropy(coord.Item1 + offx, coord.Item2 + offy);


        }









    }





    void move(ScrollDir dir)
    {
        switch (dir)
        {

            case ScrollDir.right:

                for (int i = 0; i < totalx; i++)
                {

                    MasterTiles[0, i].DelObj();
                }


                for (int i = 1; i < totalx; i++)
                {
                    for (int j = 0; j < totalx; j++)
                    {

                        MasterTiles[(i - 1), j] = MasterTiles[(i), j];

                    }


                }

                for (int i = 0; i < totalx; i++)
                {

                    MasterTiles[(totalx - 1), i].xy = new Vector2(-1, -1);
                }

                break;

            case ScrollDir.left:

                for (int i = 0; i < totalx; i++)
                {

                    MasterTiles[(totalx - 1), i].DelObj();
                }



                for (int i = totalx - 1; i > 0; i--)
                {
                    for (int j = 0; j < totalx; j++)
                    {

                        MasterTiles[i, j] = MasterTiles[(i - 1), j];

                    }


                }

                for (int i = 0; i < totalx; i++)
                {

                    MasterTiles[0, i].xy = new Vector2(-1, -1);
                }

                break;


            case ScrollDir.down:

                for (int i = 0; i < totalx; i++)
                {

                    MasterTiles[i, totalx - 1].DelObj();
                }


                for (int i = totalx - 1; i > 0; i--)
                {
                    for (int j = 0; j < totalx; j++)
                    {

                        MasterTiles[j, i] = MasterTiles[j, i - 1];


                    }


                }

                for (int i = 0; i < totalx; i++)
                {

                    MasterTiles[i, 0].xy = new Vector2(-1, -1);
//optimize combine with replace
                }


                break;


            case ScrollDir.up:

                for (int i = 0; i < totalx; i++)
                {

                    MasterTiles[i, 0].DelObj();
//optimize combine with replace
                }


                for (int i = 1; i < totalx; i++)
                {
                    for (int j = 0; j < totalx; j++)
                    {

                        MasterTiles[(j), i - 1] = MasterTiles[(j), i];

                    }


                }

                for (int i = 0; i < totalx; i++)
                {

                    MasterTiles[i, totalx - 1].xy = new Vector2(-1, -1);
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



                for (int i = totalx - 1; i >= 0; i--)
                {


                    MasterTiles[0, i].Inst(new Vector2(MasterTiles[1, i].xy.x - 1, MasterTiles[1, i].xy.y));


                    //please amke tuple vector 2
                    (int, int) xy = ((int)1, i);
                    UpdateNeighbour(-1, 0, Direction.Left, xy, TPowerReverseBigInt(MasterTiles[1, i].possibility));



                }

                break;

            case ScrollDir.right:

                //print("right");
                // print("big start");
                for (int i = 0; i < totalx; i++)
                {
                    if (i == totalx - 1)
                    {
                        // print((MasterTiles[((totalx - 2)), i].possibility));
                    }

                    nustart = MasterTiles[(totalx - 2), i].xy;
                    MasterTiles[((totalx - 1)), i]
                        .Inst(new Vector2(MasterTiles[((totalx - 2)), i].xy.x + 1, nustart.y));

                    (int, int) xy = ((int)((totalx - 2)), (int)i);

                    //fix right thing look at workignthign

                    UpdateNeighbour(1, 0, Direction.Right, xy,
                        TPowerReverseBigInt(MasterTiles[((totalx - 2)), i].possibility));
                    //print((xy.Item1+1)+", "+xy.Item2);
                    //print(MasterTiles[(totalx-1),i].possibility);
                }

                // print("out a here");
                break;
            case ScrollDir.down:


                for (int i = 0; i < totalx; i++)
                {

                    MasterTiles[i, 0].Inst(new Vector2(MasterTiles[i, 1].xy.x, MasterTiles[i, 1].xy.y - 1));


                    (int, int) xy = ((int)i, 1);
                    UpdateNeighbour(0, -1, Direction.Down, xy, TPowerReverseBigInt(MasterTiles[i, 1].possibility));


                }

                break;
            case ScrollDir.up:
                //print("up");

                for (int i = 0; i < totalx; i++)
                {
                    if (i == totalx - 1)
                    {
                        //print(MasterTiles[i,  totalx-2].possibility);
                    }


                    MasterTiles[i, totalx - 1].Inst(new Vector2(MasterTiles[i, totalx - 2].xy.x,
                        MasterTiles[i, totalx - 2].xy.y + 1));


                    (int, int) xy = (i, totalx - 2);
                    UpdateNeighbour(0, 1, Direction.Up, xy,
                        TPowerReverseBigInt(MasterTiles[i, totalx - 2].possibility));

                }

                break;



        }
        // printarray(totalx);






    }




    void printarray(int fsize)
    {
        string a = "";
        string b = "";

        for (int i = fsize - 1; i >= 0; i--)
        {
            for (int j = 0; j < fsize; j++)
            {

                b += MasterTiles[i, j].xy.ToString();
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
            //print("Moved : ");
            // print(direction);
            move(direction);
            replace(direction);
//                print("Were Here");
            alltrue = false;

            // ArrMover mov = new ArrMover() { fieldsize = totaly, odfield = MasterTiles, d = direction };
            // JobHandle jb = mov.Schedule(fieldsize, 128);
            // jb.Complete();
//                print(moveTracker);
            moveTracker++;

        }




    }


}