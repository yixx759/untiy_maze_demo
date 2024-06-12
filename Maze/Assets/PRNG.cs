using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PRNG : MonoBehaviour
{
    static private long seed = 5141568;
    private long LCG( long mod, long a, long c)
    {

        while (true)
        {
            seed = (seed * a + c) % mod;
            return seed;
        }

       


    }

    // Start is called before the first frame update
    void Start()
    {

        for (int i = 0; i < 500; i++)
        {
            
            print((LCG(500,63,23)));
            
            
            
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
