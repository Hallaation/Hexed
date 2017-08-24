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

            //otherwise if it is clear, allow the player to teleport
  
                //makes a quaternion
                Quaternion LeftStickRotation = new Quaternion();
                LeftStickRotation = Quaternion.Euler(0 , 0 , Mathf.Atan2(-GetComponent<Move>().m_LeftStickRotation.x , GetComponent<Move>().m_LeftStickRotation.y) * Mathf.Rad2Deg); // This works
                Vector3 rotation = LeftStickRotation * Vector3.up;
                
                //makes a rotation vector from the left stick's rotation

                //if there is any rotation from the left stick, the player will teleport the direction of the left stick, otherwise they will teleport the way they are looking
                if (GetComponent<Move>().m_LeftStickRotation.magnitude > 0)
                {
                    
                    Vector2 V2rotation = new Vector2(rotation.x, rotation.y);
                    RaycastHit2D hitLeftStick = Physics2D.Raycast(transform.position, V2rotation, m_TeleportForce, 1 << LayerMask.NameToLayer("Wall"));
                    if(hitLeftStick.collider != null)   //! If a raycast sent along the direction of the left stick collides with a wall. Put the player at the collision
                    {
                        float Xdistance = ((hitLeftStick.point.x) - (transform.position.x));
                        float Ydistance = ((hitLeftStick.point.y) - (transform.position.y));
                        _rigidBody.position = new Vector2(_rigidBody.position.x + Xdistance, _rigidBody.position.y + Ydistance);
                        Debug.Log("WallPrevention");
                    }
                    else
                    _rigidBody.position += new Vector2(rotation.x * m_TeleportForce, rotation.y * m_TeleportForce);
                }
                else
                {
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, m_TeleportForce, 1 << LayerMask.NameToLayer("Wall"));

                //Doe sa raycast to see if it has hit a wall, if it has, dont teleport.
                if (hit.collider != null) //! If a raycast sent along the direction the player is facing collides with a wall. Put the player at the collision
                {
                    float Xdistance = ((hit.point.x) - (transform.position.x));
                    float Ydistance = ((hit.point.y) - (transform.position.y));
                    _rigidBody.position = new Vector2(_rigidBody.position.x + Xdistance, _rigidBody.position.y + Ydistance);
                    Debug.Log("WallPrevention");
                }
                else
                    _rigidBody.position += new Vector2(this.transform.up.x * m_TeleportForce, this.transform.up.y * m_TeleportForce);  // Teleport full distance
                }
                
                ButtonHasBeenUp = false;
                currentMana -= ManaCost;
            
        }
        if (XCI.GetAxis(XboxAxis.LeftTrigger, m_controller.mXboxController) < 0.1)
        {
            ButtonHasBeenUp = true;
        }
    }
}

