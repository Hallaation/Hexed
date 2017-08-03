using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShieldAbility : BaseAbility
{
    [Space]
    [Header("Shield Variables")]
    public float testVariable;
    [SerializeField]
    public GameObject shieldObject;

    bool m_bPoweredUp = false;
    bool PowerReset = false;
    // Use this for initialization
    public override void Initialise()
    {
        if (findUI)
        {
            shieldObject = transform.Find("ShieldSprite").gameObject;
            shieldObject.SetActive(false);
        }
    }

    public override void UseSpecialAbility(bool UsingAbility)
    {
        //if the input calls for a power usage and the power hasnt been reset yet (trigger hasnt been let go yet)
        //once I want to use the ability, power reset is true
        if (UsingAbility)
        {
            PowerReset = (currentMana >= m_fMinimumManaRequired);
        }

        if (UsingAbility && PowerReset)
        {
            m_bPoweredUp = !(currentMana >= m_fMaximumMana);
            RegenMana = false;
            currentMana -= repeatedManaCost * Time.deltaTime;
            this.GetComponent<Move>().HideWeapon(true);
            shieldObject.SetActive(true);
        }
        //once the left trigger is at value 0, power reset is fals;e
        else
        {
            this.GetComponent<Move>().HideWeapon(false);
            PowerReset = false;
            shieldObject.SetActive(false);
            m_bPoweredUp = false;
        }

        //TODO Shield logic goes here.
    }

    public override void AdditionalLogic()
    {
        if (shieldObject)
        {
            if (!shieldObject.activeInHierarchy)
            {
                RegenMana = true;
            }
        }

        if (currentMana <= 0)
        {
            PowerReset = false;
        }
        if (_AbilityTypeText)
        {
            _AbilityTypeText.text = "Ability : Shield";
        }
    }

    public void TakeBullet(GameObject bullet , RaycastHit2D rayHit)
    {
        if (m_bPoweredUp)
        {
            bullet.transform.position += this.transform.up * 1.2f;
            bullet.transform.rotation = Quaternion.Inverse(bullet.transform.rotation);

            Vector3 velocity = bullet.GetComponent<Rigidbody2D>().velocity;
            bullet.GetComponent<Rigidbody2D>().velocity = Vector3.Reflect(bullet.transform.up + velocity * 0.5f , rayHit.transform.up);
            bullet.GetComponent<Bullet>().bulletOwner = null;
        }
        else
        {
            Destroy(bullet);
            //delete the bullet
        }

    }


}

