using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message_SPW : MonoBehaviour
{

    [SerializeField] private GameObject Message;
    [SerializeField] private Vector2 location;
    [SerializeField] private Vector2 Dir;
    [SerializeField] private GameObject wfcObjectInstance;
     private WFC wfcInstance;
    [SerializeField] private Vector3[] MessagePos;
    [SerializeField] private Vector3[] MessageRot;
    [SerializeField] private float DirectionDefine;
    [SerializeField] private Vector3[] posarray;

    private Vector3 startloc;
    // Start is called before the first frame update
    void Start()
    {
        wfcInstance = wfcObjectInstance.GetComponent<WFC>();
        startloc = Movement.t.position;
        startloc.y = 0;

    }

    // Update is called once per frame
    void Update()
    {



        if (WFC.alltrue && Input.GetKeyDown(KeyCode.Space))
        {
            
            CreateMessage(Random.Range(0,wfcInstance.totalx),Random.Range(0,wfcInstance.totaly));

            
            
        }



    }
    
    
    void CreateMessage(int x, int y)
    {
        
        //choose random for now and spawn decal and rotate based on 
        //what tile type.
        //array of diff stories
        Vector2 nulocation = location + new Vector2(Dir.x * Random.Range(1, 6),Dir.y * Random.Range(1, 6));
        nulocation = new Vector2(Mathf.Floor(nulocation.x), Mathf.Floor(nulocation.y));
        x = (int)nulocation.x;
        y = (int)nulocation.y;
        if (x >= wfcInstance.MazeEnd.x || y >= wfcInstance.MazeEnd.y || y < wfcInstance.MazeStart.y  || x < wfcInstance.MazeStart.x)
        {
           
            print("Wrong loser");
            print(location);
            return;
        }
        //need to respawn
        location = nulocation;
        x = x - (int)wfcInstance.MazeStart.x;
        y = y - (int)wfcInstance.MazeStart.y;
        
        print("startx: "+wfcInstance.MazeStart.x);
        print("x: "+x);
        print("y: "+x);
        print("length: "+ wfcInstance.MasterTiles.Length);
        print(wfcInstance.MasterTiles[0, 0].possibility);
        print(wfcInstance.MasterTiles[x, y].plane.transform.position);
        print(wfcInstance.MasterTiles[x, y].possibility);
        print(TPowerReverseInt(wfcInstance.MasterTiles[x, y].possibility));
        UnityEngine.Vector3 pos = wfcInstance.MasterTiles[x, y].plane.transform.position + MessagePos[TPowerReverseInt(wfcInstance.MasterTiles[x, y].possibility)];
        Quaternion rot = Quaternion.Euler(MessageRot[TPowerReverseInt(wfcInstance.MasterTiles[x, y].possibility)]);
        Instantiate(Message, pos, rot);
        print("Nuloc: ");
        print(location);
        print(Dir);



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
