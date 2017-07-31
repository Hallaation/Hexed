using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using XInputDotNetPure;
public class GroundPound : BaseAbility
{
    private bool ButtonHasBeenUp = true;
    PlayerStatus Slamer;
    CircleCollider2D GroundPoundCollider;
    Rigidbody2D _rigidBody;
    ControllerSetter m_controller;
    [Header("GroundPound Variables")]
    public float WidthOfHitbox = 10f;
    // Use this for initialization
    public override void Initialise()
    {
        GroundPoundCollider = GetComponent<CircleCollider2D>();
     
        Slamer = GetComponent<PlayerStatus>();
        ManaCost = 50f;
        m_controller = GetComponent<ControllerSetter>();
        _rigidBody = GetComponent<Rigidbody2D>();
        RegenMana = true;
    }
    public override void UseSpecialAbility(bool UsedAbility)
    {
        ////XCI.GetButtonDown(XboxButton.DPadUp, m_controller.mXboxController)
        if (currentMana >= ManaCost && ButtonHasBeenUp == true && UsedAbility == true)
        {
            
            StartCoroutine(GroundPoundAbility());
            
        }
        if (XCI.GetAxis(XboxAxis.LeftTrigger, m_controller.mXboxController) < 0.1)
        {
            ButtonHasBeenUp = true;

        }

    }

    IEnumerator GroundPoundAbility()
    {
        float i = 99;
        
        if (GroundPoundCollider.IsTouchingLayers(1 << LayerMask.NameToLayer("Player")))
            {
                Collider2D[] nearBy = Physics2D.OverlapCircleAll(transform.position, WidthOfHitbox, 1 << LayerMask.NameToLayer("Player"));
               for(int k = 0; k < nearBy.Length; ++k)
                {
                    if (nearBy[k].transform != this.transform)
                    {
                        
                        PlayerStatus tempStatus = nearBy[k].GetComponent<PlayerStatus>();
                        if (tempStatus.IsStunned == false)
                        {
                            tempStatus.StunPlayer();
                        }
                    }
                }

                //GroundPoundCollider.OverlapCollider(1 << LayerMask.NameToLayer("Player"), ColliderArray);
            }                                                                           //TODO Use Collider.OverlapCollider
         // Collider[] Collider = Physics.c(transform.position, i, 1 << LayerMask.NameToLayer("Player")); //? Currently doesnt collide.
            int j = 0;

            ++i;
            ButtonHasBeenUp = false;
            currentMana -= ManaCost;
     
        yield return null;

    }
    // Update is called once per frame
    void Update () {
		
	}
}
