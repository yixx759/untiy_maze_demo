using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PRNG : MonoBehaviour
{
    static private long seed = 10;
    private const int n = 624;
    private const ulong f = 1812433253UL;
    private const int w = 32;
    private const ulong a= 0x9908b0dfUL;
    private const ulong b =0x9d2c5680UL;
    private const ulong c =0xefc60000UL;

    private const int m = 397;
    private const int u = 11;

    private const int r = 31;

    private const ulong LMASK = (0xffffffffUL >> (w - r));
    private const ulong UMASK =(0xffffffffUL << r);
    
    
    
    struct StateTracker
    {
        public uint[] states;
        public int curstate;


    }



    void initilize_State(ref StateTracker s)
    {
        s.states = new uint[n];
        uint useed = (uint)seed;
        s.states[0] = useed ;

        for (int i = 1; i < n; i++)
        {
            useed = (uint)(f * (useed ^ (useed >> (w - 2))));
            
            s.states[i] = useed;


        }

        s.curstate = 0;


    }

    uint MTwist(ref StateTracker s)
    {
        int k = s.curstate;

        int j = k - (n - 1);
        if (j < 0) {j += n;}

        uint x = (uint)((s.states[k] & UMASK) | (s.states[j] & LMASK));
        uint xA = x >> 1;

        if ((x & 1) == 1)
        {
            xA= (uint)(xA^a);

        }

        j = k - (n - m);
        if (j < 0){ j += n;}


        x = s.states[j] ^ xA;

        s.states[k++] = x;
        if (k >= n)
        {
            k = 0;
        }

        s.curstate = k;

        uint y = x ^ (x >> u);

        y = (uint)((y ^ ((y << 7) & b)));
        y = (uint)((y ^ ((y << 15) & c)));
        uint z = y ^ (y >> 1);

        return z;

    }



    private long LCG( long mod, long a, long c)
    {

            seed = (seed * a + c) % mod;
            return seed;
      

       


    }

    long Middle_Square(int n)
    {
        seed *= seed;
      
        int dig = (int)Mathf.Floor(Mathf.Log10(seed))+1;
     

       seed = seed * (int)((Mathf.Pow(10 ,Mathf.Max(2*n-dig,0) ))) ;
     
       seed /= (int)Mathf.Pow(10,(n / 2) );
     
       seed %= (int)Mathf.Pow(10 , n) - 1;
     
       return seed;
    }

    
    


    // Start is called before the first frame update
    void Start()
    {

       // StateTracker nus = new StateTracker();
        //initilize_State(ref nus);
        

        for (int i = 0; i < 500; i++)
        {
            
           // print((LCG(500,63,23)));
          //  print(Middle_Square(8));
          //  print(MTwist(ref nus));
            
            
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
