using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using XInputDotNetPure;

public class Teleport : BaseAbility
{
    [Space]
    public float m_TeleportForce;
    [Header("Teleport Variables")]
    public float test;
   
    Rigidbody2D _rigidBody;
    ControllerSetter m_controller;

    // Use this for initialization
    void Start()
    {
        m_TeleportForce = 10;
        m_fMaximumMana = 50f;
        PassiveManaRegeneration = 1;
        ManaCost = 50f;
        m_controller = GetComponent<ControllerSetter>();
        _rigidBody = GetComponent<Rigidbody2D>();
    }
    
    // Update is called once per frame
   
    public override void AdditionalLogic()
    {
        if (XCI.GetButton(XboxButton.DPadUp, m_controller.mXboxController))
        {
            _rigidBody.position += new Vector2(transform.up.x * m_TeleportForce, transform.up.y * m_TeleportForce);
            _rigidBody.AddForce(transform.up * 10);
        }
    }

    void UseSpecialAbility()
    {
      
    }
}

