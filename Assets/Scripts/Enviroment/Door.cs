﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float DoorAngleOfBounce;
    public int DoorBounceForce;
    public HingeJoint2D DoorHinge;
    Rigidbody2D MyRigidBody;
    bool HasBounced = false;
    float timer;
    float freezeTimer;
    // Use this for initialization
    void Start()
    {
        freezeTimer = 0;
        timer = 0f;
        MyRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (freezeTimer > 10)
        {
            if (DoorHinge.jointAngle > DoorHinge.limits.max - DoorAngleOfBounce && timer > 0)
            {
                MyRigidBody.AddTorque(DoorBounceForce, ForceMode2D.Force);
                Debug.Log("MAX"); //! MAX
                HasBounced = true;
                timer = 0;
            }
            else if (DoorHinge.jointAngle < DoorHinge.limits.min + DoorAngleOfBounce && timer > 0)
            {
                MyRigidBody.AddTorque(-DoorBounceForce, ForceMode2D.Force);
                Debug.Log("Min");
                HasBounced = true;
                timer = 0;
            }
            else
            {
                HasBounced = false;
                timer += Time.deltaTime;
            }
        }
        else
            MyRigidBody.velocity = new Vector3(0, 0, 0);
        MyRigidBody.angularVelocity = 0;
        freezeTimer += Time.deltaTime;
    }


}