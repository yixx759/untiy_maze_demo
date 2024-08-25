using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
   [SerializeField] private Camera cam;
   [SerializeField] private float RateofChange;
   [SerializeField] private float speed;

   public static Transform t;

   private Rigidbody r;
   private Vector3 inputDir;
   private Vector3 vel ;
   
   private float rotx = 0;
   private float roty  = 0;
 
 
    // Start is called before the first frame update
    void Start()
    {
        t = this.GetComponent<Transform>();
        r = this.GetComponent<Rigidbody>();
        
        
        r.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (WFC.alltrue)
        {
            r.useGravity = true;
        }
       vel = r.velocity;
        
        inputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        inputDir = inputDir.normalized * speed;
        inputDir = t.rotation * inputDir;
        rotx += Input.GetAxis("Mouse X");
        float nu = roty - Input.GetAxis("Mouse Y");
        if ( nu < 90 && nu > -90)
        {
            roty = nu;
        }


        t.rotation = Quaternion.Euler(0,rotx,0);
        cam.transform.rotation = Quaternion.Euler(roty,rotx,0);



    }

    private void FixedUpdate()
    {
        vel.x = Mathf.MoveTowards(vel.x, inputDir.x, RateofChange);
        vel.z = Mathf.MoveTowards(vel.z, inputDir.z, RateofChange);

        r.velocity = vel;
    }
}
