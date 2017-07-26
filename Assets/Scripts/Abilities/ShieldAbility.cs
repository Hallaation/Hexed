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
            Debug.Log("Special Ability");
            RegenMana = false;
            currentMana -= repeatedManaCost * Time.deltaTime;
            shieldObject.SetActive(true);
        }
        else
        {
            shieldObject.SetActive(false);
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
}

