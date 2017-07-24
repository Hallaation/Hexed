using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseAbility : MonoBehaviour
{

    //abilities are going to cost mana.
    private float currentMana = 0.0f;
    public float m_fMaximumMana = 100;
    public float PassiveManaRegeneration = 1.0f;

    public float ManaCost = 10.0f;
    public bool RepeatedUsage = false;
    public float repeatedManaCost = 0.5f;
    public float m_fMinimumManaRequired = 50;
    public float m_fMovementSpeedSlowDown = 2.0f;
    //TODO if the shield is charged up to 100% of mana, the shield can deflect bullets, knock over players when run over with it
    //TODO otherwise all the shield is block bullets and knock over players when run over with it.
    
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public virtual void UseSpecialAbility() { } 
}

