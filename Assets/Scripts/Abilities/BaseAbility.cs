using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseAbility : MonoBehaviour
{

    //abilities are going to cost mana.
    public UnityEngine.UI.Text mana;
    public float currentMana = 0.0f;
    protected bool RegenMana;
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
    void Start()
    {
        mana = GameObject.Find("Mana").GetComponent<UnityEngine.UI.Text>();
        
        Debug.Log("Please not like this");
        Initialise();
    }

    // Update is called once per frame
    void Update()
    {
        if (mana)
        {
            mana.text = currentMana.ToString() + " / " + m_fMaximumMana.ToString();
        }
        if (RegenMana)
        {
            currentMana += PassiveManaRegeneration * Time.deltaTime;
        }

        AdditionalLogic();
        
    }

    public virtual void UseSpecialAbility(bool UsingAbility = false) { }
    public virtual void AdditionalLogic() { }
    public virtual void Initialise() { }
}

