using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using XInputDotNetPure;
public class GroundPound : BaseAbility
{
    private bool ButtonHasBeenUp = true;
    PlayerStatus Slamer;

    Rigidbody2D _rigidBody;
    ControllerSetter m_controller;
    [Header("GroundPound Variables")]
    float WidthOfHitbox = 10f;
    // Use this for initialization
    public override void Initialise()
    {
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
        while (i < 100)
        {
          Collider[] Collider = Physics.OverlapSphere(transform.position, i, 1 << LayerMask.NameToLayer("Player")); //? Currently doesnt collide.
            int j = 0;
            while (j < Collider.Length)
            {
                if (Collider[j].transform.GetComponent<PlayerStatus>().IsStunned && Collider[j].transform.GetComponent<PlayerStatus>() != Slamer)
                {
                    Collider[j].transform.GetComponent<PlayerStatus>().StunPlayer();
                }
            }
            ++i;
            ButtonHasBeenUp = false;
            currentMana -= ManaCost;
            yield return null;
        }
    }
    // Update is called once per frame
    void Update () {
		
	}
}
