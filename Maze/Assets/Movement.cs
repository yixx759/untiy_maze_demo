using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
   [SerializeField] private Camera cam;
   [SerializeField] private float RateofChange;
   [SerializeField] private float speed;
   [SerializeField] private float Riteheight;
   [SerializeField] private float RiteDamper;

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

        
       vel = r.velocity;
        
        inputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        inputDir = inputDir.normalized * speed;
        inputDir = t.rotation * inputDir;
        
        rotx += Input.GetAxis("Mouse X");
        roty -= Input.GetAxis("Mouse Y");
        roty = Mathf.Clamp(roty, -90, 90);

        t.rotation = Quaternion.Euler(0,rotx,0);
        cam.transform.rotation = Quaternion.Euler(roty,rotx,0);



    }

    private void FixedUpdate()
    {
        vel.x = Mathf.MoveTowards(vel.x, inputDir.x, RateofChange);
        vel.z = Mathf.MoveTowards(vel.z, inputDir.z, RateofChange);

        
        
        r.velocity = vel;
        RaycastHit ray;
        if (Physics.Raycast(new Ray(transform.position, -transform.up), out ray, 50f))
        {
            
                float x = (ray.distance -Riteheight) - (Vector3.Dot(vel,-transform.up ) * RiteDamper)  ;
                r.AddForce(-transform.up * x);
            
            
        
        }




    }
}
