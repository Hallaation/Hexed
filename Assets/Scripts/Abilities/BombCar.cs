using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

using XInputDotNetPure;

public class BombCar : BaseAbility {
    [Header("Car Variables")]
    public float m_CarSpeed = 10;
    public GameObject RemoteCar;

    bool ButtonHasBeenUp = true;
    bool CarIsActive = false;
    Rigidbody2D _rigidBody;
    ControllerSetter m_controller;
    Move Movement;
    // Use this for initialization

    public override void Initialise()
    {
        ManaCost = 50f;
        m_controller = GetComponent<ControllerSetter>();
        _rigidBody = GetComponent<Rigidbody2D>();
        RegenMana = true;
        Movement = GetComponent<Move>();

    }
    public override void AdditionalLogic()
    {
        //if (CarIsActive)
        //{
        //    Movement.m_bStopStickRotation = true;   
        //    //TODO Car Movement
        //}

        if (_AbilityTypeText)
        {
            _AbilityTypeText.text = "Ability : BombCar";
        }
    }
    public override void UseSpecialAbility(bool UsedAbility)
    {
        ////XCI.GetButtonDown(XboxButton.DPadUp, m_controller.mXboxController)
        if (currentMana >= ManaCost && ButtonHasBeenUp == true && UsedAbility == true)
        {
            GameObject Car = Instantiate(RemoteCar, this.transform.position, this.transform.rotation);
            ButtonHasBeenUp = false;
        }
        if (XCI.GetAxis(XboxAxis.LeftTrigger, m_controller.mXboxController) < 0.1)
        {
            ButtonHasBeenUp = true;
        }
    }
            // Update is called once per frame

}
