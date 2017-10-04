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
    [HideInInspector]
    public float currentMana = 0.0f; //TODO change back to protectd
    protected bool RegenMana;

    [HideInInspector]
    public float m_fMaximumMana = 100;
    [HideInInspector]
    public float PassiveManaRegeneration = 10.0f;

    [HideInInspector]
    public float ManaCost = 10.0f;
    [HideInInspector]
    public bool RepeatedUsage = false;
    [HideInInspector]
    public float repeatedManaCost = 0.5f;
    [HideInInspector]
    public float m_fMinimumManaRequired = 40;
    [HideInInspector]
    public float m_fMovementSpeedSlowDown = 2.0f;


    public float m_fAbilityCoolDown = 12;
    public int m_iMaxCharges = 2;
    [SerializeField]
    protected int m_iCurrentCharges;
    protected Timer m_CoolDownTimer;
    public bool findUI = true;

    //TODO if the shield is charged up to 100% of mana, the shield can deflect bullets, knock over players when run over with it
    //TODO otherwise all the shield is block bullets and knock over players when run over with it.

    protected Text _AbilityTypeText;
    protected Move m_MoveOwner;
    public GameObject manaBar;

    void Start()
    {
        m_CoolDownTimer = new Timer(m_fAbilityCoolDown);

        m_MoveOwner = this.GetComponent<Move>();

        // mana = GameObject.Find("Mana").GetComponent<UnityEngine.UI.Text>();
        Initialise();

        GetUIElements();
    }

    // Update is called once per frame
    void Update()
    {
        //! Fuck the mana shit, time to go to cooldowns.
        //if (manaBar == null)
        //{
        //    //GetUIElements();
        //}
        //
        //if (manaBar)
        //{
        //    //figure out the x offset
        //    //- .25 to 0
        //    float xOffset = -0.25f;
        //    xOffset = currentMana / m_fMaximumMana * -0.25f;
        //    // B1 - A1 / abs (A1)
        //    manaBar.GetComponent<Image>().material.SetTextureOffset("_MainTex" , new Vector2(xOffset , 0));
        //}
        //
        //if (RegenMana)
        //{
        //    currentMana += PassiveManaRegeneration * Time.deltaTime;
        //    if (currentMana >= m_fMaximumMana)
        //        currentMana = m_fMaximumMana;
        //}

        if (m_iCurrentCharges != m_iMaxCharges)
        {
            if (m_CoolDownTimer.Tick(Time.deltaTime))
            {
                m_iCurrentCharges++;
            }
        }
        else
        {
            m_CoolDownTimer.CurrentTime = 0;
        }
        AdditionalLogic();
    }

    public virtual void UseSpecialAbility(bool UsingAbility = false) { }
    public virtual void AdditionalLogic() { }
    public virtual void Initialise() { }

    public void GetUIElements()
    {
        //if (findUI)
        //{
        //    PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].gameObject.SetActive(true);
        //    manaBar = PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_manaBarMask;
        //    PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_SpecialScrollingIcon.GetComponent<Image>().sprite = AbilityIcon;
        //    PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_SpecialScrollingIcon.GetComponent<Image>().material.SetColor("_Color" , //GetComponent<PlayerStatus>()._playerColor);
        //}
    }
}

