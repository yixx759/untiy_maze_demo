using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class Store : MonoBehaviour
{
    private const string fileLoc = "D:/Game projects/untiy_Maze/Maze/Assets/SaveFile/yaya.bin";
//have xy then jump to next of same sighn eg 3,1 4 the next 4,1 will be in 4 jumps, over write when writing next pos or neg
// 2 files one with data and an index file that points to where it is in real file
//have index be able to skip like from 1 pos to next. or skip halfway if val is buig enough
    void AddtoFile(int loc)
    {
        using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(fileLoc, FileMode.OpenOrCreate)))
        {
            for (int i = 0; i < loc; i++)
            {
                int r = (int)Random.Range(0, Int32.MaxValue);
                binaryWriter.Write(r);
                
            }

            
            
            print("Wrotten");
            
        }


    }
//parraeleize test please
    void AppendtoFile(int loc)
    {
        // int[] rturn = new int[loc];
        // using (BinaryReader binaryReader = new BinaryReader(File.Open(fileLoc, FileMode.OpenOrCreate)))
        // {
        //     for (int i = 0; i < loc; i++)
        //     {
        //
        //         rturn[i] = binaryReader.ReadInt32();
        //     }
        //
        //     print("readen");
        //     
        // }
        //
        //
        // using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(fileLoc, FileMode.OpenOrCreate)))
        // {
        //     
        //     binaryWriter.Write(1920);
        //   //  Span<byte> bytes = MemoryMarshal.Cast<int, byte>(nurturn.AsSpan());
        //   for (int i = 0; i < loc; i++)
        //   {
        //       binaryWriter.Write(rturn[i]);    
        //   }
        //     
        //             
        //     
        //    
        //
        //     
        //     
        //     print("Wrotten");
        //     
        // }


        using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(fileLoc, FileMode.OpenOrCreate)))
        {
            using (BinaryWriter binaryReader = new BinaryWriter(File.Open(fileLoc, FileMode.OpenOrCreate)))
            {
                for (int i = 0; i < loc; i++)
                {
                    binaryWriter.Seek(-loc * 4, SeekOrigin.End);
                    

                }

                print("readen");
            }
        }


       
        


    }
    int[] readFromFile(int loc)
    {
        
        int[] rturn = new int[loc];
        using (BinaryReader binaryReader = new BinaryReader(File.Open(fileLoc, FileMode.OpenOrCreate)))
        {
            for (int i = 0; i < loc; i++)
            {

                rturn[i] = binaryReader.ReadInt32();
            }

            print("readen");
            
        }

        return rturn;
    }



    // Start is called before the first frame update
    void Start()
    {
        AddtoFile(5);
        int[] a = (readFromFile(5));


        foreach (var VARIABLE in a)
        {
            print(VARIABLE);
        }
      AppendtoFile(5);
         a = (readFromFile(6));


        foreach (var VARIABLE in a)
        {
            print(VARIABLE);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
