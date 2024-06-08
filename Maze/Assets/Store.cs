using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public class Store : MonoBehaviour
{
    private const string fileLoc = "D:/Game projects/untiy_Maze/Maze/Assets/SaveFile/yaya.bin";

    void AddtoFile(int loc)
    {
        using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(fileLoc, FileMode.OpenOrCreate)))
        {
            for (int i = 0; i < loc; i++)
            {
                int r = (int)Random.Range(0, Int32.MaxValue);
                binaryWriter.Write(r);
                
            }

            binaryWriter.BaseStream.Seek(4, SeekOrigin.Begin);
            
            binaryWriter.Write(1902);
            
            print("Wrotten");
            
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
