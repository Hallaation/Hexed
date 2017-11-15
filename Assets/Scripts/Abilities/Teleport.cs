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
    public float m_DashSpeed = 50;
    public float m_DurationOfDash = .15f;
    bool Dashing; public bool GetDashing() { return Dashing; }
    bool m_bDashTrails;
    Rigidbody2D _rigidBody;
    ControllerSetter m_controller;
    //float m_TeleportForce;
    
    GameObject[] m_DashTrails;
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
        m_DashTrails = new GameObject[2];
        m_DashTrails[0] = this.transform.Find("TrailSpot01").gameObject;
        m_DashTrails[1] = this.transform.Find("TrailSpot02").gameObject;
    }

    // Update is called once per frame
    public override void UseSpecialAbility(bool UsedAbility)
    {
        ////if (currentMana >= ManaCost && ButtonHasBeenUp == true && UsedAbility == true)
        //{
        if (m_iCurrentCharges != 0 && ButtonHasBeenUp == true && UsedAbility == true)
        {
            //otherwise if it is clear, allow the player to teleport
            Dashing = true;
            m_bDashTrails = true;
            ButtonHasBeenUp = false;
            m_iCurrentCharges--; //deduct from available charges.
            m_AudioSource.Play();
            m_AudioSource.pitch = Random.Range(0.9f, 1.2f);
            StartCoroutine(SetDashForDuration());
            //currentMana -= ManaCost;
        }

        if (XCI.GetAxis(XboxAxis.LeftTrigger, m_controller.mXboxController) < 0.1)
        {
            ButtonHasBeenUp = true;
        }

    }

    IEnumerator SetDashForDuration()
    {
        yield return new WaitForSeconds(m_DurationOfDash);
        Dashing = false;
        yield return new WaitForSeconds(0.2f);
        m_bDashTrails = false;
        yield return null;

    }

    public void TeleportActive(bool UsedAbility)
    {
        ////if (currentMana >= ManaCost && ButtonHasBeenUp == true && UsedAbility == true)
        //{
        if (m_iCurrentCharges != 0 && ButtonHasBeenUp == true && UsedAbility == true)
        {
            //otherwise if it is clear, allow the player to teleport

            //makes a quaternion
            Quaternion LeftStickRotation = new Quaternion();
            LeftStickRotation = Quaternion.Euler(0, 0, Mathf.Atan2(m_MoveOwner.m_LeftStickRotation.x, m_MoveOwner.m_LeftStickRotation.y) * Mathf.Rad2Deg); // This works
            Vector3 rotation = LeftStickRotation * Vector3.up;

            //makes a rotation vector from the left stick's rotation

            //if there is any rotation from the left stick, the player will teleport the direction of the left stick, otherwise they will teleport the way they are looking
            ButtonHasBeenUp = false;
            m_iCurrentCharges--; //deduct from available charges.
            //currentMana -= ManaCost;

        }
        if (XCI.GetAxis(XboxAxis.LeftTrigger, m_controller.mXboxController) < 0.1)
        {
            ButtonHasBeenUp = true;
        }
    }

    public override void AdditionalLogic()
    {
        m_DashTrails[0].SetActive(m_bDashTrails);
        m_DashTrails[1].SetActive(m_bDashTrails);
    }
}

