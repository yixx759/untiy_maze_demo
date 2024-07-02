using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message_SPW : MonoBehaviour
{

    //avoid spawning on blank
    
    [SerializeField] private GameObject Message;
    [SerializeField] private Vector2 location;
    [SerializeField] private Vector2 Dir;
    [SerializeField] private GameObject wfcObjectInstance;
     private WFC wfcInstance;
    [SerializeField] private Vector3[] MessagePos;
    [SerializeField] private Vector3[] MessageRot;
    [SerializeField] private float DirectionDefine;
    [SerializeField] private Vector2[] posarray;
    [SerializeField] private GameObject[] Messages;
    [SerializeField] private bool[] inOut;
    private bool changed= false;

    private const int msgnum = 12;
    
    
    private int indexMes = 0;
    private Vector3 startloc;
    private bool dirSet = false;

    private float definedirvalue => DirectionDefine * DirectionDefine;
    private Vector3 curpos => new Vector3(tt.position.x,0 ,tt.position.z);
    
    

    private Transform tt;
    // Start is called before the first frame update
    void Start()
    {
        posarray = new Vector2[msgnum];
        Messages = new GameObject[msgnum];
        inOut = new bool[msgnum];
        tt = Movement.t;
        wfcInstance = wfcObjectInstance.GetComponent<WFC>();
        startloc = tt.position;
        startloc.y = 0;

        //once dir make message adn save to approach and regenerate
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((startloc - curpos).sqrMagnitude > definedirvalue && !dirSet)
        {
            dirSet = true;
            print("Here");
            Dir = new Vector2((curpos - startloc).normalized.x,(curpos - startloc).normalized.z) ;
            Vector2 nulocation = location;
            for (int i = 0; i < msgnum; i++)
            {
               
                nulocation +=  Dir * Random.Range(6, 12);
                //add to array
                nulocation = new Vector2(Mathf.Floor(nulocation.x), Mathf.Floor(nulocation.y));
                posarray[indexMes++] = nulocation; 
                
                
            }

            indexMes = 0;
            
            print(curpos - startloc);
            print(Dir);
            foreach (var VARIABLE in posarray)
            {
                print(VARIABLE);
            }
          


        }

        if (dirSet && wfcInstance.hasmoved && WFC.alltrue)
        {
            //for each
            for (int i = 0; i < msgnum; i++)
            {

                if (!(posarray[i].x >= wfcInstance.MazeEnd.x || posarray[i].y >= wfcInstance.MazeEnd.y ||
                    posarray[i].y < wfcInstance.MazeStart.y ||
                    posarray[i].x < wfcInstance.MazeStart.x))
                {
                    if (inOut[i] == false)
                    {
                        if (System.Object.ReferenceEquals(Messages[i],null))
                        {
                            CreateMessage((int)posarray[i].x,(int)posarray[i].y );
                        }
                        else
                        {
                            ReRotate(i);
                        
                        
                        
                        }
                   
                        inOut[i] = true;

                    }




                }
                else
                {
                    if (inOut[i] == true)
                    {
                  
                        inOut[i] = false;

                    }

                }

                wfcInstance.hasmoved = false;

            }

          
        }

        // if (WFC.alltrue && Input.GetKeyDown(KeyCode.Space))
        // {
        //     
        //     CreateMessage(Random.Range(0,wfcInstance.totalx),Random.Range(0,wfcInstance.totaly));
        //
        //     
        //     
        // }

        // if (Input.GetKeyDown(KeyCode.LeftControl))
        // {
        //     wfcInstance.hasmoved = true;
        // }


    }
    //cheack min and max then creat after intial projection
    //genreate more as movie along
    // and have back track change
    
    void CreateMessage(int x, int y)
    {
        
        //choose random for now and spawn decal and rotate based on 
        //what tile type.
        //array of diff stories
        print("x: "+x);
        print("y: "+x);
        if (x >= wfcInstance.MazeEnd.x || y >= wfcInstance.MazeEnd.y || y < wfcInstance.MazeStart.y  || x < wfcInstance.MazeStart.x)
        {
           
            print("Wrong loser");
            print(location);
            return;
        }
        //need to respawn
        location = new Vector2(x,y);
        x = x - (int)wfcInstance.MazeStart.x;
        y = y - (int)wfcInstance.MazeStart.y;
        
        print("startx: "+wfcInstance.MazeStart.x);
        print("x: "+x);
        print("y: "+x);
        print(wfcInstance.MasterTiles[13, 13].possibility);
        print(TPowerReverseInt(wfcInstance.MasterTiles[x, y].possibility));
        UnityEngine.Vector3 pos = wfcInstance.MasterTiles[x, y].plane.transform.position + MessagePos[TPowerReverseInt(wfcInstance.MasterTiles[x, y].possibility)];
        Quaternion rot = Quaternion.Euler(MessageRot[TPowerReverseInt(wfcInstance.MasterTiles[x, y].possibility)]);
        Messages[indexMes++] = Instantiate(Message, pos, rot);
        print("Nuloc: ");
        print(location);
        print(Dir);



    }


    void ReRotate(int i)

    {
        //subtract xy
        Vector2 xy = posarray[i];
        print("REROTATE");
        print("x: "+(int)xy.x+ " Y: "+(int)xy.y);
        xy.x -=  (int)wfcInstance.MazeStart.x;
        xy.y -= (int)wfcInstance.MazeStart.y;
        print("x: "+(int)xy.x+ " Y: "+(int)xy.y);
        print(wfcInstance.MasterTiles[(int)xy.x, (int)xy.y].possibility);
        print(TPowerReverseInt(wfcInstance.MasterTiles[(int)xy.x, (int)xy.y].possibility));
        Messages[i].transform.position = wfcInstance.MasterTiles[(int)xy.x, (int)xy.y].plane.transform.position + MessagePos[TPowerReverseInt(wfcInstance.MasterTiles[(int)xy.x, (int)xy.y].possibility)];
        Messages[i].transform.rotation = Quaternion.Euler(MessageRot[TPowerReverseInt(wfcInstance.MasterTiles[(int)xy.x, (int)xy.y].possibility)]);
    }

    private static int TPowerReverseInt(int a)
    {

        switch (a)
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
    
}
