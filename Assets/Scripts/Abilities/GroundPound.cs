﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using XInputDotNetPure;
using System;
public class GroundPound : BaseAbility
{
    private bool ButtonHasBeenUp = true;
    PlayerStatus Slamer;
    CircleCollider2D GroundPoundCollider;
    Rigidbody2D _rigidBody;
    ControllerSetter m_controller;
    Move MoveScript;
    [Header("GroundPound Variables")]
    public float RadiusOfHitBox = 10f;
    public float WidthOfRipple;
    public float SpeedOfGrowthPerFrame;
    public float Delay;
    private bool Corotuine = true;
    // Use this for initialization
    public override void Initialise()
    {
        GroundPoundCollider = transform.Find("SlamCollider").GetComponent<CircleCollider2D>();
        MoveScript = GetComponent<Move>();
        GroundPoundCollider.radius = SpeedOfGrowthPerFrame;
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
        currentMana -= ManaCost;
        MoveScript.SetActive(false);
        _rigidBody.velocity = Vector2.zero;
        // Debug.Break();
        ButtonHasBeenUp = false;
        yield return new WaitForSeconds(Delay);
        if (!Slamer.IsDead && !Slamer.IsStunned)
        {
            while (Corotuine)
            {
                ButtonHasBeenUp = false;
                if (GroundPoundCollider.IsTouchingLayers(1 << LayerMask.NameToLayer("Player")))
                {
                    Collider2D[] nearBy = Physics2D.OverlapCircleAll(transform.position, GroundPoundCollider.radius, 1 << LayerMask.NameToLayer("Player"));
                    for (int k = 0; k < nearBy.Length; ++k)
                    {
                        if (nearBy[k].transform != this.transform)
                        {
                            if (Vector2.Distance(nearBy[k].transform.position, transform.position) > GroundPoundCollider.radius - WidthOfRipple)
                            {
                                PlayerStatus tempStatus = nearBy[k].GetComponentInParent<PlayerStatus>();
                                if (tempStatus.IsStunned == false)
                                {
                                   // Vector3 Temp = nearBy[k].GetComponent<Transform>().position;
                                    //Vector3 Tempa = Vector3.RotateTowards(Temp,transform.position,Mathf.Infinity,Mathf.Infinity);
                                    tempStatus.StunPlayer(Vector3.zero);
                                }
                            }
                        }
                    }
                    if (GroundPoundCollider.radius >= RadiusOfHitBox)
                        Corotuine = false;
                    else
                    {
                        GroundPoundCollider.radius += SpeedOfGrowthPerFrame;
                        if (GroundPoundCollider.radius > RadiusOfHitBox)
                            GroundPoundCollider.radius = RadiusOfHitBox;
                        yield return new WaitForEndOfFrame();
                    }
                }
                else
                {
                    if (GroundPoundCollider.radius >= RadiusOfHitBox)
                        Corotuine = false;
                    else
                    {
                        GroundPoundCollider.radius += SpeedOfGrowthPerFrame;
                        if (GroundPoundCollider.radius > RadiusOfHitBox)
                            GroundPoundCollider.radius = RadiusOfHitBox;
                        yield return new WaitForEndOfFrame();
                    }
                }
            }
        }

        //? !
        //    if (GroundPoundCollider.IsTouchingLayers(1 << LayerMask.NameToLayer("Player")))
        //{
        //    Collider2D[] nearBy = Physics2D.OverlapCircleAll(transform.position, RadiusOfHitBox, 1 << LayerMask.NameToLayer("Player"));
        //    for (int k = 0; k < nearBy.Length; ++k)
        //    {
        //        if (nearBy[k].transform != this.transform)
        //        {

        //            PlayerStatus tempStatus = nearBy[k].GetComponent<PlayerStatus>();
        //            if (tempStatus.IsStunned == false)
        //            {
        //                tempStatus.StunPlayer();
        //            }
        //        }
        //    }

        //    //GroundPoundCollider.OverlapCollider(1 << LayerMask.NameToLayer("Player"), ColliderArray);
        //}

        //? !
        // Collider[] Collider = Physics.c(transform.position, i, 1 << LayerMask.NameToLayer("Player")); //? Currently doesnt collide.
        MoveScript.enabled = true;
        MoveScript.SetActive(true);
        ButtonHasBeenUp = false;
           
        GroundPoundCollider.radius = SpeedOfGrowthPerFrame;
        Corotuine = true;

    }
    // Update is called once per frame
    void Update () {
		
	}
}
