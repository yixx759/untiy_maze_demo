using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class ArrayReplace : MonoBehaviour
{
    [SerializeField] int fieldsize;
    private Vector2[,] field;
    private NativeArray<Vector2> odfield;
    private double tim;
    
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
                     
                        odfield[j + (i - 1) * fieldsize] = odfield[j + (i) * fieldsize];

                    }


                }

                for (int i = 0; i < fieldsize; i++)
                {
                   
                    odfield[i + (fieldsize - 1) * fieldsize] = new Vector2(-1 , -1);
                }
                
                break;
            
            case ScrollDir.down:
                for (int i = fieldsize-1; i > 0 ; i--)
                {
                    for (int j = 0; j < fieldsize; j++)
                    {
                     
                        odfield[j + (i ) * fieldsize] = odfield[j + (i- 1) * fieldsize];

                    }


                }

                for (int i = 0; i < fieldsize; i++)
                {
                   
                    odfield[i ] = new Vector2(-1 , -1);
                }
                
                break;
            
            
            case ScrollDir.left:
                for (int i = fieldsize-1; i > 0 ; i--)
                {
                    for (int j = 0; j < fieldsize; j++)
                    {
                   
                        odfield[j * fieldsize + i] = odfield[j * fieldsize + (i - 1)];


                    }


                }

                for (int i = 0; i < fieldsize; i++)
                {
                  
                    odfield[ fieldsize * i] = new Vector2(-1 , -1);
//optimize combine with replace
                }

                
                break;
            
            
            case ScrollDir.right:
                for (int i = 1; i < fieldsize ; i++)
                {
                    for (int j = 0; j < fieldsize; j++)
                    {
                     
                        odfield[ (fieldsize * j) + i-1] = odfield[ (fieldsize * j) + i] ;

                    }


                }

                for (int i = 0; i < fieldsize; i++)
                {
                  
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
             
                nustart = odfield[ (fieldsize) ]  ;
        
                for (int i = fieldsize-1; i >= 0; i--)
                {
            
                    odfield[i] = new Vector2(odfield[fieldsize+i].x,odfield[fieldsize+i].y -1 );


                }
                
                break;
            
            case ScrollDir.up:
            
                 nustart = odfield[(fieldsize*(fieldsize-1)-1)]  ;
               
                for (int i = 0; i < fieldsize; i++)
                {
                    
                    odfield[i + (fieldsize * (fieldsize - 1))] =new Vector2(odfield[i + (fieldsize * (fieldsize - 2))].x,nustart.y+1);
                 

                }
                
                break;
            case ScrollDir.left:
                //start = field[fieldsize - 2, fieldsize-1]  ;
        
                for (int i = 0; i < fieldsize; i++)
                {
                   
                    odfield[i*fieldsize] = new Vector2(odfield[i*fieldsize+1].x -1,odfield[i*fieldsize+1].y);
                   // ++start;

                }
                
                break;
            case ScrollDir.right:
                //start = field[fieldsize - 2, fieldsize-1]  ;
        
                for (int i = 0; i < fieldsize; i++)
                {
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
        tim = Time.realtimeSinceStartupAsDouble;
      
        odfield = new NativeArray<Vector2>(fieldsize * fieldsize, Allocator.Persistent);
        
        for(int i =0; i < fieldsize ; i++){
            for (int j = 0; j < fieldsize; j++)
            {
               
                odfield[j + (i * fieldsize)] =  new Vector2(j  ,i);

            }

        }

        printarray(fieldsize);
        //odfield = new NativeArray<Vector2>((9 * 9), Allocator.Persistent)

    }

    private void OnDestroy()
    {
        odfield.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        
        bool moved = false;
        ScrollDir direction = ScrollDir.down;
        if (Input.GetKeyDown(KeyCode.W))
        {
            direction = ScrollDir.up;
          moved = true;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            moved = true;
            direction = ScrollDir.down;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            moved = true;
            direction = ScrollDir.right;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            moved = true;
            direction = ScrollDir.left;
            
        }

        double nutim = 0;
       // move(direction);
        //replace(direction);
        if (moved)
        {
            double timTotal = 0;
            double timLTotal = 0;
            for (int i = 0; i < 500; i++)
            {
                tim = Time.realtimeSinceStartupAsDouble;
                move(direction);
                replace(direction);
                nutim = Time.realtimeSinceStartupAsDouble;

                timLTotal += (nutim - tim);
                
                tim = Time.realtimeSinceStartupAsDouble;
                ArrMover mov = new ArrMover() { fieldsize = fieldsize, odfield = odfield, d = direction};
                JobHandle jb = mov.Schedule(fieldsize, 128);
                jb.Complete();
                nutim = Time.realtimeSinceStartupAsDouble; 
                timTotal += (nutim - tim);
            }

            
            print("Lame Loser Time: "+ timLTotal/500);
            
            
            print("Cool Awsome Multithreading Time: "+ timTotal/500);
            
            //printarray(fieldsize);
        }

        
    }

//need intializer

    [BurstCompile]
    struct ArrMover : IJobParallelFor
    {
        //redo for diffrent axis
        public int fieldsize;
        [NativeDisableParallelForRestriction] public NativeArray<Vector2> odfield;
        public ScrollDir d;
        
        void move(ScrollDir dir, int index)
    {
        switch (dir)
        {
            
            case ScrollDir.up:
                for (int i = 1; i < fieldsize ; i++)
                {
                    
                      
                        odfield[index + (i - 1) * fieldsize] = odfield[index + (i) * fieldsize];

                    


                }

              
                 
                    odfield[index + (fieldsize - 1) * fieldsize] = new Vector2(-1 , -1);
                
                
                break;
            
            case ScrollDir.down:
                for (int i = fieldsize-1; i > 0 ; i--)
                {
                   
                      
                        odfield[index + (i ) * fieldsize] = odfield[index + (i- 1) * fieldsize];



                }

              
              
                    odfield[index ] = new Vector2(-1 , -1);
               
                
                break;
            
            
            case ScrollDir.left:
                for (int i = fieldsize-1; i > 0 ; i--)
                {
                   
                    
                        odfield[index * fieldsize + i] = odfield[index * fieldsize + (i - 1)];


                    


                }

                
                 
                    odfield[ fieldsize * index] = new Vector2(-1 , -1);
//optimize combine with replace
               

                
                break;
            
            
            case ScrollDir.right:
                for (int i = 1; i < fieldsize ; i++)
                {
                   
                      
                        odfield[ (fieldsize * index) + i-1] = odfield[ (fieldsize * index) + i] ;

                   


                }

           
                  
                odfield[(fieldsize * index) + fieldsize - 1] = new Vector2(-1 , -1);
                

                break;
            
                
            
            
            
            
            
            
        }

        
        

       






    }
    
     void replace(ScrollDir dir, int index)
    {
      
        Vector2 nustart = Vector2.zero;
        switch (dir)
        {
            case ScrollDir.down:
               
                nustart = odfield[ (fieldsize) ]  ;
        
               
                   
                    odfield[index] = new Vector2(odfield[fieldsize+index].x,odfield[fieldsize+index].y -1 );


                
                break;
            
            case ScrollDir.up:
              //do before threading
                 nustart = odfield[(fieldsize*(fieldsize-1)-1)]  ;
               
              
                    odfield[index + (fieldsize * (fieldsize - 1))] =new Vector2(odfield[index + (fieldsize * (fieldsize - 2))].x,odfield[index + (fieldsize * (fieldsize - 2))].y +1);
                 

                
                
                break;
            case ScrollDir.left:
                //start = field[fieldsize - 2, fieldsize-1]  ;
        
               
                   
                    odfield[index*fieldsize] = new Vector2(odfield[index*fieldsize+1].x -1,odfield[index*fieldsize+1].y);
                   // ++start;

                
                
                break;
            case ScrollDir.right:
                //start = field[fieldsize - 2, fieldsize-1]  ;
        
               
                    odfield[index*fieldsize + fieldsize-1] = new Vector2(odfield[index*fieldsize + fieldsize-2].x + 1,odfield[index*fieldsize + fieldsize-2].y );

               
                
                break;
            
            
            
        }
        
        
        
        
        
        
        
    }   
        public void Execute(int i)
        {
            
            move(d, i);
            replace(d,i);
            
            
        }
    
    
    
    
    }
    
    
    
}
