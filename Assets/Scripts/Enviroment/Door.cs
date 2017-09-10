using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IHitByBullet
{
    public float DoorAngleOfBounce;
    public int DoorBounceForce;
    public HingeJoint2D DoorHinge;
    Rigidbody2D MyRigidBody;

    public void HitByBullet(Vector3 a_Vecocity, Vector3 HitPoint)
    {
        this.GetComponent<Rigidbody2D>().AddForceAtPosition(a_Vecocity, HitPoint); //hah
    }

    // bool HasBounced = false;
    // float timer;
    // float freezeTimer;
    // Use this for initialization
    void Start()
    {
        //freezeTimer = 0;
        //timer = 0f;
        MyRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

            if (DoorHinge.jointAngle > DoorHinge.limits.max - DoorAngleOfBounce)
            {
                MyRigidBody.AddTorque(DoorBounceForce, ForceMode2D.Force);

               // HasBounced = true;
               // timer = 0;
            }
            else if (DoorHinge.jointAngle < DoorHinge.limits.min + DoorAngleOfBounce)
            {
                MyRigidBody.AddTorque(-DoorBounceForce, ForceMode2D.Force);
            
               // HasBounced = true;
               // timer = 0;
            }
            //else
            //{
            //   // HasBounced = false;
            //   // timer += Time.deltaTime;
            //}
    }


}