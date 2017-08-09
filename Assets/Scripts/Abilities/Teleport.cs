using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using XInputDotNetPure;

public class Teleport : BaseAbility
{
    private bool ButtonHasBeenUp = true;
    [Space]
   
    [Header("Teleport Variables")]
    public float m_TeleportForce = 10;

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
        if (_AbilityTypeText)
        {
            _AbilityTypeText.text = "Ability : Teleport";
        }
    }

    public override void UseSpecialAbility(bool UsedAbility)
    {
        ////XCI.GetButtonDown(XboxButton.DPadUp, m_controller.mXboxController)
        if (currentMana >= ManaCost && ButtonHasBeenUp == true && UsedAbility == true )
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, m_TeleportForce, 1 << LayerMask.NameToLayer("Wall"));
            if (hit.collider != null)
            {
                float Xdistance = ((hit.point.x) - (transform.position.x));
                float Ydistance = ((hit.point.y) - (transform.position.y));
                _rigidBody.position = new Vector2(_rigidBody.position.x + Xdistance, _rigidBody.position.y + Ydistance);
                Debug.Log("WallPrevention");
                ButtonHasBeenUp = false;
                currentMana -= ManaCost;

            }
            else
            {
                _rigidBody.position += new Vector2(transform.up.x * m_TeleportForce, transform.up.y * m_TeleportForce);
                ButtonHasBeenUp = false;
                currentMana -= ManaCost;
            }
        }
        if (XCI.GetAxis(XboxAxis.LeftTrigger, m_controller.mXboxController) < 0.1)
        {
            ButtonHasBeenUp = true;
        }
    }
}

