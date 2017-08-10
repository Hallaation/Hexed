using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseAbility : MonoBehaviour
{
    public Sprite AbilityIcon;
    public Sprite[] SelectionSprites;

    [Space]
    //abilities are going to cost mana.
    public float currentMana = 0.0f; //TODO change back to protectd
    protected bool RegenMana;
    public float m_fMaximumMana = 100;
    public float PassiveManaRegeneration = 10.0f;

    public float ManaCost = 10.0f;
    public bool RepeatedUsage = false;
    public float repeatedManaCost = 0.5f;
    public float m_fMinimumManaRequired = 40;
    public float m_fMovementSpeedSlowDown = 2.0f;

    public bool findUI = true;

    //TODO if the shield is charged up to 100% of mana, the shield can deflect bullets, knock over players when run over with it
    //TODO otherwise all the shield is block bullets and knock over players when run over with it.

    protected Text _AbilityTypeText;
    protected GameObject manaBar;

    void Start()
    {
        
        // mana = GameObject.Find("Mana").GetComponent<UnityEngine.UI.Text>();
        Initialise();

        GetUIElements();
    }

    // Update is called once per frame
    void Update()
    {
        if (manaBar)
        {
            //figure out the x offset
            //- .25 to 0
            float xOffset = -0.25f;
            xOffset = currentMana / m_fMaximumMana * -0.25f;
            // B1 - A1 / abs (A1)
            manaBar.GetComponent<Image>().material.SetTextureOffset("_MainTex" , new Vector2(xOffset , 0));
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

    public void GetUIElements()
    {
        if (findUI)
        {
            PlayerUIArray.instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].gameObject.SetActive(true);
            manaBar = PlayerUIArray.instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_manaBarMask;
            PlayerUIArray.instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_SpecialScrollingIcon.GetComponent<Image>().sprite = AbilityIcon;
        }
    }
}

