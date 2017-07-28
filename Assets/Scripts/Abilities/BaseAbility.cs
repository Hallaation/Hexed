using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseAbility : MonoBehaviour
{

    //abilities are going to cost mana.
    public float currentMana = 0.0f; //TODO change back to protectd
    protected bool RegenMana;
    public float m_fMaximumMana = 100;
    public float PassiveManaRegeneration = 10.0f;

    public float ManaCost = 10.0f;
    public bool RepeatedUsage = false;
    public float repeatedManaCost = 0.5f;
    public float m_fMinimumManaRequired = 50;
    public float m_fMovementSpeedSlowDown = 2.0f;

    public bool findUI = true;
    //TODO if the shield is charged up to 100% of mana, the shield can deflect bullets, knock over players when run over with it
    //TODO otherwise all the shield is block bullets and knock over players when run over with it.

    protected Text _ManaText;
    protected Text _AbilityTypeText;
    //WTF Use this for initialization
    void Start()
    {
        // mana = GameObject.Find("Mana").GetComponent<UnityEngine.UI.Text>();
        Initialise();

        
        if (findUI)
        {
            GameObject UIElements = GameObject.FindGameObjectWithTag("PlayerUI");
            _ManaText = UIElements.GetComponent<PlayerUIArray>().playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_ManaText.GetComponent<Text>();
            _AbilityTypeText = UIElements.GetComponent<PlayerUIArray>().playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_AbilityType.GetComponent<Text>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_ManaText)
        {
            _ManaText.text = currentMana.ToString("0.00") + " / " + m_fMaximumMana.ToString();

        }
        if (RegenMana)
        {
            currentMana += PassiveManaRegeneration * Time.deltaTime;
            if (currentMana >= m_fMaximumMana)
                currentMana = m_fMaximumMana;
        }

        AdditionalLogic();
    }

    public virtual void UseSpecialAbility(bool UsingAbility = false) { }
    public virtual void AdditionalLogic() { }
    public virtual void Initialise() { }
}

