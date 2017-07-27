using System.Collections;
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
    // Use this for initialization
    void Start()
    {
        timer = 0f;
        MyRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (DoorHinge.jointAngle > DoorHinge.limits.max - DoorAngleOfBounce && timer > 0)
        {
            MyRigidBody.AddTorque(DoorBounceForce, ForceMode2D.Force);
            Debug.Log("MAX");
            HasBounced = true;
            timer = 0;
        }
        else if (DoorHinge.jointAngle < DoorHinge.limits.min + DoorAngleOfBounce && timer > 0)
        {
            MyRigidBody.AddTorque(-DoorBounceForce, ForceMode2D.Force);
            Debug.Log("yes");
            HasBounced = true;
            timer = 0;
        }
        else
        {
            HasBounced = false;
            timer += Time.deltaTime;

        }
    }


}