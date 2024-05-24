using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class ArrayReplace : MonoBehaviour
{
    [SerializeField] int fieldsize;
    private Vector2[,] field;
    private Vector2[] odfield;
    
    private int count;
    //keeping track of what index where


    enum ScrollDir
    {
        up,
        down,
        left,
        right

    }
//for left right need to identify but cant ut will have repated num
    void move(ScrollDir dir)
    {
        switch (dir)
        {
            
            case ScrollDir.up:
                for (int i = 1; i < fieldsize ; i++)
                {
                    for (int j = 0; j < fieldsize; j++)
                    {
                        field[i - 1, j] = field[i , j];
                        odfield[j + (i - 1) * fieldsize] = odfield[j + (i) * fieldsize];

                    }


                }

                for (int i = 0; i < fieldsize; i++)
                {
                    field[fieldsize - 1, i] = new Vector2(-1 , -1);
                    odfield[i + (fieldsize - 1) * fieldsize] = new Vector2(-1 , -1);
                }
                
                break;
            
            case ScrollDir.down:
                for (int i = fieldsize-1; i > 0 ; i--)
                {
                    for (int j = 0; j < fieldsize; j++)
                    {
                        field[i , j] = field[i-1 , j];
                        odfield[j + (i ) * fieldsize] = odfield[j + (i- 1) * fieldsize];

                    }


                }

                for (int i = 0; i < fieldsize; i++)
                {
                    field[0, i] = new Vector2(-1 , -1);
                    odfield[i ] = new Vector2(-1 , -1);
                }
                
                break;
            
            
            case ScrollDir.left:
                for (int i = fieldsize-1; i > 0 ; i--)
                {
                    for (int j = 0; j < fieldsize; j++)
                    {
                        field[j, i] = field[j , i-1];
                        odfield[j * fieldsize + i] = odfield[j * fieldsize + (i - 1)];


                    }


                }

                for (int i = 0; i < fieldsize; i++)
                {
                    field[i,0 ] =new Vector2(-1 , -1);
                    odfield[ fieldsize * i] = new Vector2(-1 , -1);
//optimize combine with replace
                }

                
                break;
            
            
            case ScrollDir.right:
                for (int i = 1; i < fieldsize ; i++)
                {
                    for (int j = 0; j < fieldsize; j++)
                    {
                        field[j, i-1] = field[j , i];
                        odfield[ (fieldsize * j) + i-1] = odfield[ (fieldsize * j) + i] ;

                    }


                }

                for (int i = 0; i < fieldsize; i++)
                {
                    field[i, fieldsize-1] = new Vector2(-1 , -1);
                    odfield[(fieldsize * i) + fieldsize - 1] = new Vector2(-1 , -1);
                }

                break;
            
                
            
            
            
            
            
            
        }

        
        

       






    }

    void replace(ScrollDir dir)
    {
        Vector2 start = Vector2.zero;
        Vector2 nustart = Vector2.zero;
        switch (dir)
        {
            case ScrollDir.down:
                start = field[1, 0] ;
                nustart = odfield[ (fieldsize) ]  ;
        
                for (int i = fieldsize-1; i >= 0; i--)
                {
                    field[0, i] =  new Vector2(field[1, i].x, field[1, i].y-1 );
                    odfield[i] = new Vector2(odfield[fieldsize+i].x,odfield[fieldsize+i].y -1 );


                }
                
                break;
            
            case ScrollDir.up:
                start = field[fieldsize - 2, fieldsize-1]  ;
                 nustart = odfield[(fieldsize*(fieldsize-1)-1)]  ;
               
                for (int i = 0; i < fieldsize; i++)
                {
                    field[fieldsize - 1, i] =  new Vector2( field[fieldsize - 2, i].x, start.y+1);;
                    odfield[i + (fieldsize * (fieldsize - 1))] =new Vector2(odfield[i + (fieldsize * (fieldsize - 2))].x,nustart.y+1);
                 

                }
                
                break;
            case ScrollDir.left:
                //start = field[fieldsize - 2, fieldsize-1]  ;
        
                for (int i = 0; i < fieldsize; i++)
                {
                    field[i, 0] = new Vector2(  field[i, 1].x-1, field[i, 1].y);
                    odfield[i*fieldsize] = new Vector2(odfield[i*fieldsize+1].x -1,odfield[i*fieldsize+1].y);
                   // ++start;

                }
                
                break;
            case ScrollDir.right:
                //start = field[fieldsize - 2, fieldsize-1]  ;
        
                for (int i = 0; i < fieldsize; i++)
                {
                    field[i, fieldsize - 1] =  new Vector2(field[i, fieldsize - 2].x + 1,field[i, fieldsize - 2].y );
                    odfield[i*fieldsize + fieldsize-1] = new Vector2(odfield[i*fieldsize + fieldsize-2].x + 1,odfield[i*fieldsize + fieldsize-2].y );

                }
                
                break;
            
            
            
        }
        
        
        
        
        
        
        
    }


    void printarray(int fsize)
    {
        string a = "";
        string b = "";

        for(int i =fieldsize-1; i >= 0 ; i--){
            for (int j = 0; j < fieldsize; j++)
            {
                a += field[i, j].ToString();
                a += " ";
                b += odfield[j + i * fieldsize].ToString();
                b += " ";

            }
                
                a += "\n";
                b += "\n";
        }

           //print(a);
            print(b);
    }

    // Start is called before the first frame update
    void Start()
    {

        field = new Vector2[fieldsize,fieldsize];
        odfield = new Vector2[fieldsize * fieldsize];
        
        for(int i =0; i < fieldsize ; i++){
            for (int j = 0; j < fieldsize; j++)
            {
                field[i, j] = new Vector2(j  ,i);
                odfield[j + (i * fieldsize)] =  new Vector2(j  ,i);

            }

        }

        printarray(fieldsize);


    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.W))
        {
            move(ScrollDir.up);
           replace(ScrollDir.up);
            printarray(fieldsize);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            move(ScrollDir.down);
            replace(ScrollDir.down);
            printarray(fieldsize);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            move(ScrollDir.right);
            replace(ScrollDir.right);
            printarray(fieldsize);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            move(ScrollDir.left);
            replace(ScrollDir.left);
            printarray(fieldsize);
        }
    }



    // struct ArrMover : IJobParallelFor
    // {
    //
    //    
    //
    //
    //     public void execute(int i)
    //     {
    //         
    //         
    //         
    //         
    //     }
    //
    //
    //
    //
    // }
    
    
    
}
