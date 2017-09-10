using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinToWin : BaseAbility
{

    private GameObject spinSprite;
    public float RotationSpeed = 20.0f;
    public float MovementSpeedSlow = 10.0f;


    public float SpinRadius = 2.3f;
    private bool PowerReset = false;
    public override void AdditionalLogic()
    {
        if (!spinSprite.activeInHierarchy)
        {
            RegenMana = true;
        }

        if (currentMana <= 0)
        {
            PowerReset = false;
        }
        if (_AbilityTypeText)
        {

        }
    }
    

    public override void Initialise()
    {
        //find the gameobject containing the sprite, and set its parent to be the player's sprite (pure laziness)
        spinSprite = this.transform.Find("SpinSprite").gameObject;
        if (!m_MoveOwner.playerSpirte)
        {
            m_MoveOwner.FindSprite();
        }
        spinSprite.transform.SetParent(m_MoveOwner.playerSpirte.transform);
        spinSprite.SetActive(false);
    }

    public override void UseSpecialAbility(bool UsingAbility = false)
    {
        //do ability logic on a seperate thread
        StartCoroutine(RunAbility(UsingAbility));
    }


    IEnumerator RunAbility(bool UsingAbility)
    {
        if (UsingAbility)
        {
            if (currentMana >= m_fMinimumManaRequired)
            {
                PowerReset = true;
            }
        }

        if (UsingAbility && PowerReset)
        {

            RegenMana = false;
            currentMana -= repeatedManaCost * Time.deltaTime;
            m_MoveOwner.HideWeapon(true);
            //should use an animation instead of hacking it like this, but whatever.
            m_MoveOwner.playerSpirte.transform.rotation = Quaternion.Euler(new Vector3(0 , 0 , m_MoveOwner.playerSpirte.transform.rotation.eulerAngles.z + RotationSpeed));
            //disable stick rotation and turn on the radius
            m_MoveOwner.m_bStopStickRotation = true;
            spinSprite.SetActive(true);

            //overlap circle against player layer
            Collider2D[] hitCollider = Physics2D.OverlapCircleAll(this.transform.position , SpinRadius , 1 << 8);

            foreach (Collider2D collided in hitCollider)
            {
                if (collided.transform != this.transform)
                {
                    collided.GetComponent<PlayerStatus>().KillPlayer(this.GetComponent<PlayerStatus>());
                }
            }
        }
        else
        {
            m_MoveOwner.HideWeapon(false);
            PowerReset = false;
            //let the player rotate again, turn the spin radius indicator off and reset the rotation and allow the player to rotate with their sticks
            m_MoveOwner.m_bStopStickRotation = false;
            spinSprite.SetActive(false);
            m_MoveOwner.playerSpirte.transform.rotation = this.transform.rotation;
        }
        yield return null;
    }
}
