using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour
{
    private float m_iMaxHealth;
    public float m_iHealth = 3; //health completely useless right now
    int m_iTimesPunched = 0;
    bool m_bDead = false;
    bool m_bStunned = false;
    public bool IsDead { get { return m_bDead; } set { m_bDead = value; } }
    public bool IsStunned { get { return m_bStunned; } set { m_bStunned = value; } }
    public int TimesPunched { get { return m_iTimesPunched; } set { m_iTimesPunched = value; } }

    public float m_fStunTime = 1;
    public float m_fMaximumStunWait = 2;
    Timer stunTimer;
    Timer resetStunTimer;
    [HideInInspector]
    public Color _playerColour;
    private Renderer PlayerSprite;

    [HideInInspector]
    public GameObject killMePrompt = null;
    [HideInInspector]
    public GameObject killMeArea = null;

    private UnityEngine.UI.Text _healthText;
    private GameObject _HealthMask;
   
    //if the player is dead, the renderer will change their colour to gray, and all physics simulation of the player's rigidbody will be turned off.
    void Start()
    {
        m_iMaxHealth = m_iHealth;
        //initialize my timer and get the player's colour to return to.
        stunTimer = new Timer(m_fStunTime);
        resetStunTimer = new Timer(m_fMaximumStunWait);
        //_playerColour = GetComponent<Renderer>().material.color;
        if (GetComponent<Renderer>())
        {
            PlayerSprite = GetComponent<Renderer>();
            _playerColour = GetComponent<Renderer>().material.color;
        }
        else
        {
            PlayerSprite = transform.Find("Sprites").Find("PlayerSprite").GetComponent<Renderer>();
            _playerColour = transform.Find("Sprites").Find("PlayerSprite").GetComponent<Renderer>().material.color;
        }
        killMePrompt.SetActive(false);

        GameObject UIElements = GameObject.FindGameObjectWithTag("PlayerUI");
        _healthText = UIElements.GetComponent<PlayerUIArray>().playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_HealthText.GetComponent<Text>();
        _HealthMask = UIElements.GetComponent<PlayerUIArray>().playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_HealthBarMask;
    }
    void Update()
    {
        //if i've been punched once, start the timer, once the timer has reached the end, reset the amount of times punched.
        if (m_iTimesPunched >= 1)
        {
            if (resetStunTimer.Tick(Time.deltaTime))
            {
                m_iTimesPunched = 0;
            }
        }

        if (_healthText)
        {
            float xOffset = (m_iHealth / m_iMaxHealth) * 0.235f;
            _healthText.text = (m_iHealth > 0) ? m_iHealth.ToString() : "You're dead";
            _HealthMask.GetComponent<Image>().material.SetTextureOffset("_MainTex" , new Vector2(xOffset , 0));
        }

        //if im dead, set my colour to gray, turn of all physics simulations and exit the function
        if (m_bDead)
        {
            //this.GetComponent<Renderer>().material.color = Color.grey;
            PlayerSprite.material.color = Color.grey;
            this.GetComponent<Rigidbody2D>().simulated = false;
            killMePrompt.SetActive(false);
            killMeArea.SetActive(false);
            return;
        }
        //if im stunned, make me cyan and show any kill prompts (X button and kill radius);
        if (m_bStunned)
        {
            killMeArea.SetActive(true);
            PlayerSprite.material.color = Color.cyan;
            //this.GetComponent<Renderer>().material.color = Color.cyan;
            this.GetComponent<Collider2D>().isTrigger = true;
            if (stunTimer.Tick(Time.deltaTime))
            {
                m_bStunned = false;
            }
        }
        //if not stunned dont kill me
        else
        {
            this.GetComponent<Collider2D>().isTrigger = false;
            killMeArea.SetActive(false);
            killMePrompt.SetActive(false);
            if (GetComponent<Renderer>())
            {
                GetComponent<Renderer>().material.color = _playerColour;
            }
            else
            {
                transform.Find("Sprites").Find("PlayerSprite").GetComponent<Renderer>().material.color = _playerColour;
            }
        }
        //? should probably set a timer to reset these?
        if (m_iTimesPunched >= 2)
        {
            StunPlayer();
            GetComponent<Move>().StatusApplied();
            m_iTimesPunched = 0;
        }

    }

    public void StunPlayer()
    {
        //stun the player called outside of class
        m_bStunned = true;
        m_iTimesPunched = 0;
    }

    public void KillPlayer()
    {
        //kill the player, called outside of class (mostly used for downed kills)
        m_iHealth = 0;
        m_bDead = true;
    }
}
