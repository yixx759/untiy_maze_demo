using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;


public class PostProcess : MonoBehaviour
{
    
    [SerializeField] Material Post;
    [SerializeField] float a;
    [SerializeField] float b;
    [SerializeField, Range(0,1)] float test;
        
    //https://www.alanzucconi.com/2015/09/16/how-to-sample-from-a-gaussian-distribution/
    //https://en.wikipedia.org/wiki/Poisson_distribution#Generating_Poisson-distributed_random_variables


    public static float possionInvertedSearch(float lambda)
    {

        float x = 0;
        float p = (MathF.Exp( -lambda));
        float s = p;
        float u = Random.Range(-1f, 1.0f);
       // print(u);
        while (u > s)
        {
            x += 1;
            p = p * lambda / x;
            s += p;
        }

        return x;

    }

    public static float getGaussianNum()
    {
        float v1, v2, s;

        do
        {
            v1 = 2 *  Random.Range(-1f, 1.0f) - 1.0f;
            v2 = 2 *  Random.Range(-1f, 1.0f) - 1.0f;
            s = v1 * v1 + v2 * v2;
        } while (s >= 1.0f || s == 0);

        s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);
        return v1 * s;
        

    }

    public static float getGaussian(float mean, float sd, float max, float min)
    {
        float x;

       do
       {
            x = mean + getGaussianNum() * sd;
        } while (x < min || x > max);

        return x;


    }

    public static float CreateNoise(float lum,float a, float b, float max, float min)
    {
   
        float np =  getGaussian(lum,lum*a,max,min);
        print(np);
        float ng = getGaussian(0,b*b,max,min);
        print(ng);
       
        return np / (a+ 0.00000000001f) + ng;


    }

    private void Start()
    {
        this.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
       //  float np =possionInvertedSearch(11f*0.5f);
       // print(np);
       // float ng = getGaussian(0,0.01f*0.01f,1,-1);
       //
       // print(possionInvertedSearch(11f*0.5f));
   
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (Post != null)
        {
            print(test);
            // float aa = (float)(Mathf.Sin(Vector2.Dot(new Vector2(0.5f, 0.5f), new Vector2(12.2345f, 64.22343f))) * 32546.222);
            float aa = (float)(Mathf.Sin(Vector2.Dot(new Vector2((float)test, test), new Vector2(12.2345f, 64.22343f))) * 32546.222);
            aa -= (float)Math.Truncate(aa);
            if(aa < 0.5)
                aa = Mathf.Sqrt((float)(-2.0 * Mathf.Log((float)aa))) * Mathf.Sin((float)(2.0 * Mathf.PI * aa));
            else
                aa = Mathf.Sqrt((float)(-2.0 * Mathf.Log((float)aa))) * Mathf.Cos((float)(2.0 * Mathf.PI * aa));
            
            print(2435 + aa *2354 );
           // Post.SetFloat("offsety", Random.value);
            Post.SetFloat("offsety",0);
           // Post.SetFloat("offsetx", Random.value);
            Post.SetFloat("offsetx",0);
            
           // Post.SetFloat("seed", Random.Range(0,8324765f));
            Post.SetFloat("seed2", Random.Range(0,8324765f));
            Post.SetFloat("a", a);
            Post.SetFloat("b", b);
            //add shorhsift 64 star to shader
           // print(CreateNoise(test,a,b,1,-1));
       
          Graphics.Blit(source, destination);
           // Graphics.Blit(source, destination);
        }
        else
        {
            Graphics.Blit(source, destination);
        }

    }
}