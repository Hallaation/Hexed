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
	// Use this for initialization
	public override void Initialise()
    {
        shieldObject = transform.FindChild("ShieldSprite").gameObject;
        shieldObject.SetActive(false);
	}
	
    public override void UseSpecialAbility(bool UsingAbility)
    {
        if (UsingAbility)
        {
            if (currentMana >= m_fMaximumMana)
            {
                m_bPoweredUp = true;
            }
            RegenMana = false;
            currentMana -= repeatedManaCost * Time.deltaTime;
            shieldObject.SetActive(true);
        }
        else
        {
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
    }

    public void TakeBullet(GameObject bullet)
    {
        if (m_bPoweredUp)
        {
            Debug.Log("taking bullet");
            bullet.transform.position += this.transform.up * 1.2f;
            bullet.transform.rotation = Quaternion.Inverse(bullet.transform.rotation);

            Vector3 velocity = bullet.GetComponent<Rigidbody2D>().velocity;
           // velocity.x = -velocity.x
            bullet.GetComponent<Rigidbody2D>().velocity = -velocity;
            //bullet.GetComponent<Rigidbody2D>().velocity = -velocity;
            //deflect the shield
        }
        else
        {
            Destroy(bullet);
            //delete the bullet
        }
        
    }


}

