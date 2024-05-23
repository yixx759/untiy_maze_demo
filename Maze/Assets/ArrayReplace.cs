using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayReplace : MonoBehaviour
{
    [SerializeField] int fieldsize;
    private int[,] field;
    
    private int count;
    //keeping track of what index where


    enum ScrollDir
    {
        up,
        down,
        left,
        right

    }

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


                    }


                }

                for (int i = 0; i < fieldsize; i++)
                {
                    field[fieldsize - 1, i] = -1;

                }
                
                break;
            
            case ScrollDir.down:
                for (int i = fieldsize-1; i > 0 ; i--)
                {
                    for (int j = 0; j < fieldsize; j++)
                    {
                        field[i , j] = field[i-1 , j];


                    }


                }

                for (int i = 0; i < fieldsize; i++)
                {
                    field[0, i] = -1;

                }
                
                break;
            
            
            case ScrollDir.left:
                for (int i = 1; i < fieldsize ; i++)
                {
                    for (int j = 0; j < fieldsize; j++)
                    {
                        field[i - 1, j] = field[i , j];


                    }


                }

                for (int i = 0; i < fieldsize; i++)
                {
                    field[fieldsize - 1, i] = -1;

                }
                
                break;
            
            
            case ScrollDir.right:
                for (int i = 1; i < fieldsize ; i++)
                {
                    for (int j = 0; j < fieldsize; j++)
                    {
                        field[i - 1, j] = field[i , j];


                    }


                }

                for (int i = 0; i < fieldsize; i++)
                {
                    field[fieldsize - 1, i] = -1;

                }
                
                break;
            
                
            
            
            
            
            
            
        }

        
        

       






    }

    void replace(ScrollDir dir)
    {
        int start = 0;
        switch (dir)
        {
            case ScrollDir.down:
                start = field[1, fieldsize-1]  ;
        
                for (int i = fieldsize-1; i > 0; i--)
                {
                    field[0, i] =  --start;
                    

                }
                
                break;
            
            case ScrollDir.up:
                start = field[fieldsize - 2, fieldsize-1]  ;
        
                for (int i = 0; i < fieldsize; i++)
                {
                    field[fieldsize - 1, i] =  start+1;
                    ++start;

                }
                
                break;
            case ScrollDir.left:
                start = field[fieldsize - 2, fieldsize-1]  ;
        
                for (int i = 0; i < fieldsize; i++)
                {
                    field[fieldsize - 1, i] =  start+1;
                    ++start;

                }
                
                break;
            case ScrollDir.right:
                start = field[fieldsize - 2, fieldsize-1]  ;
        
                for (int i = 0; i < fieldsize; i++)
                {
                    field[fieldsize - 1, i] =  start+1;
                    ++start;

                }
                
                break;
            
            
            
        }
        
        
        
        
        
        
        
    }


    void printarray(int fsize)
    {
        string a = "";

        for(int i =fieldsize-1; i >= 0 ; i--){
            for (int j = 0; j < fieldsize; j++)
            {
                a += field[i, j].ToString();
                a += " ";

            }
                
                a += "\n";
        }

            print(a);
    }

    // Start is called before the first frame update
    void Start()
    {

        field = new int[fieldsize,fieldsize];
        
        for(int i =0; i < fieldsize ; i++){
            for (int j = 0; j < fieldsize; j++)
            {
                field[i, j] = j + (fieldsize * i);

            }

        }

        printarray(fieldsize);


    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.W))
        {
            move(ScrollDir.down);
            replace(ScrollDir.down);
            printarray(fieldsize);
        }
        
    }
}
