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
        Up = 2
        

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
                return 2;
            case 16:
                return 4;
            default:
                print("broke");
                return 500;
            
            
        }

       

    }

    struct TileInfo
    {
        public int possibility ;
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
            possibility = (int) (TileType.Blank | TileType.Down | TileType.Left | TileType.Right | TileType.Up);
            print( possibility);
            Entropy = 5;
            Known = false;
            plane = Plane;
            mat = plane.GetComponent<Renderer>().material;
            print(mat);

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
        rulenume[TPowerReverse(TileType.Blank), (int)Direction.Up] = 2;
        a[TPowerReverse(TileType.Blank), (int)Direction.Left] = (int) (TileType.Blank | TileType.Left);
        rulenume[TPowerReverse(TileType.Blank), (int)Direction.Left] = 2;
        a[TPowerReverse(TileType.Blank), (int)Direction.Down] = (int) (TileType.Blank | TileType.Down);
        rulenume[TPowerReverse(TileType.Blank), (int)Direction.Down] = 2;
        a[TPowerReverse(TileType.Blank), (int)Direction.Right] = (int) (TileType.Blank | TileType.Right);
        rulenume[TPowerReverse(TileType.Blank), (int)Direction.Right] = 2;
        a[TPowerReverse(TileType.Down), (int)Direction.Up] = (int) (TileType.Blank | TileType.Up );
        rulenume[TPowerReverse(TileType.Down), (int)Direction.Up] = 2;
        a[TPowerReverse(TileType.Down), (int)Direction.Left] = (int) (TileType.Right | TileType.Up | TileType.Down);
        rulenume[TPowerReverse(TileType.Down), (int)Direction.Left] = 3;
        a[TPowerReverse(TileType.Down), (int)Direction.Down] = (int) (TileType.Left | TileType.Right | TileType.Up);
        rulenume[TPowerReverse(TileType.Down), (int)Direction.Down] = 3;
        a[TPowerReverse(TileType.Down), (int)Direction.Right] = (int) (TileType.Left | TileType.Up | TileType.Down);
        rulenume[TPowerReverse(TileType.Down), (int)Direction.Right] = 3;
        a[TPowerReverse(TileType.Right), (int)Direction.Up] = (int) (TileType.Left | TileType.Down | TileType.Right);
        rulenume[TPowerReverse(TileType.Right), (int)Direction.Up] = 3;
        a[TPowerReverse(TileType.Right), (int)Direction.Left] = (int) (TileType.Blank | TileType.Left);
        rulenume[TPowerReverse(TileType.Right), (int)Direction.Left] = 2;
        a[TPowerReverse(TileType.Right), (int)Direction.Down] = (int) (TileType.Left | TileType.Up | TileType.Right);
        rulenume[TPowerReverse(TileType.Right), (int)Direction.Down] = 3;
        a[TPowerReverse(TileType.Right), (int)Direction.Right] = (int) ( TileType.Down | TileType.Up | TileType.Left);
        rulenume[TPowerReverse(TileType.Right), (int)Direction.Right] = 3;
        a[TPowerReverse(TileType.Left), (int)Direction.Up] = (int) (TileType.Left | TileType.Down | TileType.Right);
        rulenume[TPowerReverse(TileType.Left), (int)Direction.Up] = 3;
        a[TPowerReverse(TileType.Left), (int)Direction.Left] = (int) (TileType.Up | TileType.Down | TileType.Right);
        rulenume[TPowerReverse(TileType.Left), (int)Direction.Left] = 3;
        a[TPowerReverse(TileType.Left), (int)Direction.Down] = (int) (TileType.Left | TileType.Right | TileType.Up);
        rulenume[TPowerReverse(TileType.Left), (int)Direction.Down] = 3;
        a[TPowerReverse(TileType.Left), (int)Direction.Right] = (int) (TileType.Blank | TileType.Right);
        rulenume[TPowerReverse(TileType.Left), (int)Direction.Right]  = 2;
        a[TPowerReverse(TileType.Up), (int)Direction.Up] = (int) (TileType.Left | TileType.Down | TileType.Right);
        rulenume[TPowerReverse(TileType.Up), (int)Direction.Up] = 3;
        a[TPowerReverse(TileType.Up), (int)Direction.Left] = (int) (TileType.Up | TileType.Down | TileType.Right);
        rulenume[TPowerReverse(TileType.Up), (int)Direction.Left] = 3;
        a[TPowerReverse(TileType.Up), (int)Direction.Down] = (int) (TileType.Blank | TileType.Down);
        rulenume[TPowerReverse(TileType.Up), (int)Direction.Down] = 2;
        a[TPowerReverse(TileType.Up), (int)Direction.Right] = (int) (TileType.Up | TileType.Down | TileType.Left);
        rulenume[TPowerReverse(TileType.Up), (int)Direction.Right] = 3;
        return a;
    }

    // Start is called before the first frame update
    void Start()
    {

        print("HERERERE"+
            (int)(TileType.Blank | TileType.Down | TileType.Left | TileType.Right | TileType.Up));
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

        MasterTiles[2, 2].Known = true;
        MasterTiles[2, 2].Entropy = 1;
        MasterTiles[2, 2].possibility = (MasterTiles[2, 2].possibility & (int)TileType.Right);
        MasterTiles[2,2].TileTypetoImage(TileType.Right);

        
        UpdateNeighbour(1, 0, Direction.Right, (2,2), 0);
        UpdateNeighbour(-1, 0, Direction.Left, (2,2), 0);
        UpdateNeighbour(0, 1, Direction.Up, (2,2), 0);
        UpdateNeighbour(0, -1, Direction.Down, (2,2), 0);
       
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
        int num = MasterTiles[x, y].possibility;
        for (int i = 0; i < 4; i++)
        {
            if ((num & 1) == 1)
            {
                count++;

            }
            num = num >> 1;

        }

        return count;

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
            tt = Random.Range(0, 4);
            print("Heeeeeee");
            print("t"+tt);
            print("binar"+ (1 << tt));
            print("pos"+MasterTiles[coord.Item1, coord.Item2].possibility);
        } while (((1<<tt) & MasterTiles[coord.Item1, coord.Item2].possibility) == 0 && count > 100);

        print(count);
        return tt;


    }

    void UpdateNeighbour(int offx, int offy, Direction Dir, (int, int) coord , int TT)
    {
        if (coord.Item1 + offx < totalx && coord.Item2 + offy < totaly && coord.Item1 + offx >= 0 &&
            coord.Item2 + offy >= 0 &&   MasterTiles[coord.Item1+ offx,coord.Item2+offy].Entropy != 0 && MasterTiles[coord.Item1+ offx,coord.Item2+offy].Known == false       )
        {
            //fix entroy do count thing
            //print("HERERERE" + MasterTiles[3, 2].possibility);
            //print("pre pos: " + (MasterTiles[coord.Item1+ offx,coord.Item2+offy].possibility + " Ander "+rules[TT,(int)Dir]));
            MasterTiles[coord.Item1+ offx,coord.Item2+offy].possibility = (MasterTiles[coord.Item1+ offx,coord.Item2+offy].possibility & rules[TT,(int)Dir ]);
          
           // print("A i: "+ (coord.Item1+ offx) + " j: "+(coord.Item2+offy) + " Pos: "+MasterTiles[coord.Item1+ offx,coord.Item2+offy].possibility);
            MasterTiles[coord.Item1+ offx,coord.Item2+offy].Entropy =  nuentropy(coord.Item1+ offx, coord.Item2+offy);
            //print("ENT: "+MasterTiles[coord.Item1+ offx,coord.Item2+offy].Entropy);
        }

       
        
        
        
        
        
    }

    // Update is called once per frame
    void Update()
    {

        
        
        
        
        //do space then if


        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("step");
            
            if (!alltrue)
            {
                (int, int) tind = findTarget();
                print("tind"+tind);

                int TT = targetIndex(tind);
            
                MasterTiles[tind.Item1,tind.Item2 ].Known = true;
                MasterTiles[tind.Item1,tind.Item2].Entropy = 1;
                MasterTiles[tind.Item1,tind.Item2].possibility = (MasterTiles[tind.Item1,tind.Item2].possibility & 1 << TT);
                MasterTiles[tind.Item1,tind.Item2].IntetoImage(TT);
                print("what img: "+ (TileType) MasterTiles[tind.Item1,tind.Item2].possibility);

                UpdateNeighbour(1, 0, Direction.Right, tind, TT);
                UpdateNeighbour(-1, 0, Direction.Left, tind, TT);
                UpdateNeighbour(0, 1, Direction.Up, tind, TT);
                UpdateNeighbour(0, -1, Direction.Down, tind, TT);
            
            
            

                alltrue = done();

            }
            
        }

      
        
        
        
        
    }
}
