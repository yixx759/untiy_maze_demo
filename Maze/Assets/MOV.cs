using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOV : MonoBehaviour
{

    private Rigidbody r;

    private Vector3 init;

    private Vector3 intiDir = Vector3.zero;

    [SerializeField] private float accel;

    [SerializeField] private float mag;
    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        intiDir.x = Input.GetAxis("Horizontal");
        intiDir.z = Input.GetAxis("Vertical");


        intiDir = intiDir.normalized * mag;


    }

    private void FixedUpdate()
    {
        init = r.velocity;

        Vector3 nuinit = Vector3.zero;

        nuinit.x = Mathf.MoveTowards(init.x, intiDir.x, accel);
        nuinit.z = Mathf.MoveTowards(init.z, intiDir.z, accel);

        nuinit.y = init.y;

        r.velocity = nuinit;



    }
}
