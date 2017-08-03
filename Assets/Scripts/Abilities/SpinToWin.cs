using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinToWin : BaseAbility
{

    private GameObject spinSprite;
    private float m_fRotation;
    public float RotationSpeed = 20.0f;
    public float MovementSpeedSlow = 10.0f;


    public float SpinRadius = 2.3f;
    private bool PowerReset = false;
    public override void AdditionalLogic()
    {

    }

    public override void Initialise()
    {
        m_fRotation = 0;
        //find the gameobject containing the sprite, and set its parent to be the player's sprite (pure laziness)
        spinSprite = this.transform.Find("SpinSprite").gameObject;
        if (!GetComponent<Move>().playerSpirte)
        {
            GetComponent<Move>().FindSprite();
        }
        spinSprite.transform.SetParent(GetComponent<Move>().playerSpirte.transform);
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
            PowerReset = (currentMana >= m_fMinimumManaRequired);
        }

        if (UsingAbility && PowerReset)
        {
                RegenMana = !UsingAbility;
                currentMana -= repeatedManaCost * Time.deltaTime;
                GetComponent<Move>().HideWeapon(true);
                //should use an animation instead of hacking it like this, but whatever.
                GetComponent<Move>().playerSpirte.transform.rotation = Quaternion.Euler(new Vector3(0 , 0 , GetComponent<Move>().playerSpirte.transform.rotation.eulerAngles.z + RotationSpeed));
                //disable stick rotation and turn on the radius
                GetComponent<Move>().m_bStopStickRotation = true;
                spinSprite.SetActive(true);

                //overlap circle against player layer
                Collider2D[] hitCollider = Physics2D.OverlapCircleAll(this.transform.position , SpinRadius , 1 << 8);

                foreach (Collider2D collided in hitCollider)
                {
                    if (collided.transform != this.transform)
                    {
                        collided.GetComponent<PlayerStatus>().KillPlayer();
                    }
                }

            
        }
        else
        {
            GetComponent<Move>().HideWeapon(false);
            PowerReset = false;
            //let the player rotate again, turn the spin radius indicator off and reset the rotation and allow the player to rotate with their sticks
            GetComponent<Move>().m_bStopStickRotation = false;
            spinSprite.SetActive(false);
            GetComponent<Move>().playerSpirte.transform.rotation = this.transform.rotation;
            RegenMana = !UsingAbility;
        }
        yield return null;
    }
}
