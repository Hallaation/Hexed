using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseAbility : MonoBehaviour
{
    public Sprite AbilityIcon;
    public Sprite[] SelectionSprites;
    public Sprite m_CharacterPortrait = null;
    [Space]
    //abilities are going to cost mana.
    [HideInInspector]
    protected float currentMana = 0.0f;
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
    private PlayerStatus m_PlayerStatus;
    public int AbilityCharges { get { return m_iCurrentCharges; } set { m_iCurrentCharges = value; } }

    protected Text _AbilityTypeText;
    public Move m_MoveOwner;

    public GameObject m_ChargeIndicator;
    public Sprite[] ChargeReadySprites;
    public Sprite[] EmptyChargeSprites;
    public GameObject[] ChargeHighlighters;
    protected SpriteRenderer m_IndicatorRenderer;
    ParticleSystem.MainModule m_indicatorModule;
    protected float m_fIndicatorScale = 0;
    public bool ChargeCoolDown = true;

    void Start()
    {
        m_CoolDownTimer = new Timer(m_fAbilityCoolDown);

        m_MoveOwner = this.GetComponent<Move>();
        m_PlayerStatus = this.GetComponent<PlayerStatus>();
        m_ChargeIndicator = this.transform.Find("Sprites").Find("TeleportIndicator").GetChild(0).gameObject;
        ChargeHighlighters = new GameObject[2];

        ChargeHighlighters[0] = this.transform.Find("Sprites").Find("TeleportIndicator").GetChild(1).gameObject;
        ChargeHighlighters[1] = this.transform.Find("Sprites").Find("TeleportIndicator").GetChild(2).gameObject;
        m_IndicatorRenderer = m_ChargeIndicator.GetComponentInChildren<SpriteRenderer>();
        m_indicatorModule = m_ChargeIndicator.GetComponent<ParticleSystem>().main;
        // mana = GameObject.Find("Mana").GetComponent<UnityEngine.UI.Text>();
        Initialise();

        GetUIElements();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_PlayerStatus.IsDead)
        {
            m_ChargeIndicator.SetActive(false);
            ChargeHighlighters[0].SetActive(false);
            ChargeHighlighters[1].SetActive(false);
        }
        else
        {
            m_ChargeIndicator.SetActive(true);
            ChargeHighlighters[0].SetActive(true);
            ChargeHighlighters[1].SetActive(true);
        }
        if (!GameManagerc.Instance.Paused)
        {
            switch (m_iCurrentCharges)
            {
                case 1: //When there is 1 Charge
                    //Turn on highlight for object 0
                    ChargeHighlighters[0].GetComponent<SpriteRenderer>().sprite = ChargeReadySprites[0];
                    //turn off highlight for object 1
                    ChargeHighlighters[1].GetComponent<SpriteRenderer>().sprite = EmptyChargeSprites[1];
                    break;
                case 2: //When there is 2 charges

                    //turn on highlight for obj 0
                    //turn on highlight for obj 1
                    ChargeHighlighters[0].GetComponent<SpriteRenderer>().sprite = ChargeReadySprites[0];
                    ChargeHighlighters[1].GetComponent<SpriteRenderer>().sprite = ChargeReadySprites[1];
                    break;
                default: //When none of the above apply (usually 0)
                    //turn off highlight for obj 0 and 1
                    ChargeHighlighters[0].GetComponent<SpriteRenderer>().sprite = EmptyChargeSprites[0];
                    ChargeHighlighters[1].GetComponent<SpriteRenderer>().sprite = EmptyChargeSprites[1];
                    break;
            }
            // float GlowScale = 1 ;
            //m_fIndicatorScale = (m_CoolDownTimer.CurrentTime / m_CoolDownTimer.mfTimeToWait + m_iCurrentCharges )  * 0.5f;
            //m_ChargeIndicator.transform.localScale = new Vector2(m_fIndicatorScale, m_fIndicatorScale);
            if (!m_IndicatorRenderer)
            {
                m_indicatorModule.startSizeMultiplier = (m_CoolDownTimer.CurrentTime / m_CoolDownTimer.mfTimeToWait + m_iCurrentCharges + 0.4f);
            }
            else
            {
                m_fIndicatorScale = (m_CoolDownTimer.CurrentTime / m_CoolDownTimer.mfTimeToWait + m_iCurrentCharges )  * 0.5f;
                m_ChargeIndicator.transform.localScale = new Vector2(m_fIndicatorScale, m_fIndicatorScale);
            }
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
            m_CoolDownTimer.mfTimeToWait = m_fAbilityCoolDown;
            if (m_iCurrentCharges != m_iMaxCharges && ChargeCoolDown && GameManagerc.Instance.RoundReady)
            {
                if (m_CoolDownTimer.Tick(Time.deltaTime))
                {
                    m_iCurrentCharges++;
                }
            }
            else if (!ChargeCoolDown)
            {

            }
            else
            {
                m_CoolDownTimer.CurrentTime = 0;
            }
            AdditionalLogic();
        }
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
    void Reset()
    {
        m_iCurrentCharges = 0;
    }
}

