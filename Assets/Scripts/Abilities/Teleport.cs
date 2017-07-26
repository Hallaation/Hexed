using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using XInputDotNetPure;

public class Teleport : BaseAbility
{
    private bool ButtonHasBeenUp = true;
    [Space]
    public float m_TeleportForce;
    [Header("Teleport Variables")]
    public float test;

    Rigidbody2D _rigidBody;
    ControllerSetter m_controller;

    // Use this for initialization
    //void Start()
    //{
    //    m_TeleportForce = 10;
    //    m_fMaximumMana = 50f;
    //    PassiveManaRegeneration = 1;
    //    ManaCost = 50f;
    //    m_controller = GetComponent<ControllerSetter>();
    //    _rigidBody = GetComponent<Rigidbody2D>();
    //}

    public override void Initialise()
    {
        m_TeleportForce = 10;
        m_fMaximumMana = 50f;
        PassiveManaRegeneration = 1;
        ManaCost = 50f;
        m_controller = GetComponent<ControllerSetter>();
        _rigidBody = GetComponent<Rigidbody2D>();
        RegenMana = true;
    }

    // Update is called once per frame

    public override void AdditionalLogic()
    {

        //if (XCI.GetButtonDown(XboxButton.DPadUp, m_controller.mXboxController) && currentMana >= ManaCost && ButtonHasBeenUp == true)
        //{
        //    _rigidBody.position += new Vector2(transform.up.x * m_TeleportForce, transform.up.y * m_TeleportForce);
        //    ButtonHasBeenUp = false;
        //    currentMana -= ManaCost;
        //}
        //if (XCI.GetButtonUp(XboxButton.DPadUp, m_controller.mXboxController))
        //    ButtonHasBeenUp = true;

    }

    public override void UseSpecialAbility(bool ThisBoolIsUseless)
    {
        ////XCI.GetButtonDown(XboxButton.DPadUp, m_controller.mXboxController)
        if (currentMana >= ManaCost && ButtonHasBeenUp == true)
        {
            _rigidBody.position += new Vector2(transform.up.x * m_TeleportForce, transform.up.y * m_TeleportForce);
            ButtonHasBeenUp = false;
            currentMana -= ManaCost;
        }
        if (XCI.GetAxis(XboxAxis.LeftTrigger, m_controller.mXboxController) < 0.1)
        {
            ButtonHasBeenUp = true;
            Debug.Log("Reset");


        }
        Debug.Log(XCI.GetAxis(XboxAxis.LeftTrigger, m_controller.mXboxController));
    }
}

