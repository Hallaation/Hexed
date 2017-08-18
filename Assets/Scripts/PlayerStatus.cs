using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PlayerStatus : MonoBehaviour
{
    private float m_iMaxHealth;
    public float m_iHealth = 3; //health completely useless right now
    int m_iTimesPunched = 0;
    int m_iPreviousTimesPunched = 0;
    bool m_bDead = false;
    bool m_bStunned = false;
    public float StunedSlide = 400;
    public int m_iScore;
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

    
    public GameObject killMePrompt = null;
    
    public GameObject killMeArea = null;

    private GameObject _HealthMask;
    private Text _ScoreText;
    Rigidbody2D _rigidbody;
    [HideInInspector]
    public int spawnIndex;
    //if the player is dead, the renderer will change their colour to gray, and all physics simulation of the player's rigidbody will be turned off.
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        SceneManager.sceneLoaded += OnSceneLoaded;
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
        Debug.Log(GetComponent<ControllerSetter>().m_playerNumber);
        _HealthMask = PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_HealthBarMask;
        _ScoreText = PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_ScoreText.GetComponent<Text>();
        PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_healthScrolllingIcon.GetComponent<Image>().material.SetColor("_Color" , _playerColour);
        PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_StaticObjectMaterial.SetColor("_Color" , _playerColour);

        foreach (var item in PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_objects)
        {
            item.SetActive(true);
        }

    }

    void Update()
    {
        //update my score
        m_iScore = GameManagerc.Instance.PlayerWins[this.GetComponent<ControllerSetter>().m_playerNumber];
        //Debug.LogError(GetComponent<ControllerSetter>().m_playerNumber);
        if (_ScoreText != null)
        {
            _ScoreText.text = m_iScore.ToString();
        }
        //if i've been punched once, start the timer, once the timer has reached the end, reset the amount of times punched.
        if (m_iTimesPunched >= 1)
        {
            if(m_iTimesPunched != m_iPreviousTimesPunched)
            {
                resetStunTimer.SetTimer(0);
                m_iPreviousTimesPunched = m_iTimesPunched;
            }
            if (resetStunTimer.Tick(Time.deltaTime))
            {
              //  m_iTimesPunched = 0;
               // m_iPreviousTimesPunched = 0;
            }
        }

        if (_HealthMask)
        {
            float xOffset = m_iHealth * -0.0791f;
            _HealthMask.GetComponent<Image>().material.SetTextureOffset("_MainTex" , new Vector2(0 + xOffset , 0));
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
            if (this.transform.GetChild(1).tag == "Stunned")
            {
                this.transform.GetChild(1).gameObject.GetComponent<PolygonCollider2D>().enabled = true;
            }
            else
            {
                this.transform.GetChild(2).gameObject.GetComponent<PolygonCollider2D>().enabled = true;
            }
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

            if (this.transform.GetChild(0).tag == "Stunned")
            {
                Debug.Log(this.transform.GetChild(0).tag);
                this.transform.GetChild(0).gameObject.GetComponent<PolygonCollider2D>().enabled = false;
            }
            else
            {
                this.transform.GetChild(1).gameObject.GetComponent<PolygonCollider2D>().enabled = false;
            }
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
         //   StunPlayer();
          //  GetComponent<Move>().StatusApplied();
            //m_iTimesPunched = 0;
        }

    }
    
    public void StunPlayer(Vector3 ThrownItemVelocity)
    {
        //stun the player called outside of class
        //Vector3 a = ThrownItemVelocity.normalized;
        // _rigidbody.velocity = (a * StunedSlide);


        _rigidbody.velocity = ThrownItemVelocity;
        m_bStunned = true;
        m_iTimesPunched = 0;
    }

    public void KillPlayer()
    {
        //kill the player, called outside of class (mostly used for downed kills)
        m_iHealth = 0;
        m_bDead = true;
    }

    public void ResetPlayer()
    {
        m_iHealth = 3;
        m_bDead = false;
        m_bStunned = false;
        m_iTimesPunched = 0;
        this.GetComponent<Rigidbody2D>().simulated = true;

        this.transform.position = ControllerManager.Instance.spawnPoints[spawnIndex].position;
        GetComponent<Move>().ThrowMyWeapon(Vector2.zero , Vector2.up , false);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //time to re activate all the UI stuff
        this.GetComponent<BaseAbility>().GetUIElements();
        
        _HealthMask = PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_HealthBarMask;
        _ScoreText = PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_ScoreText.GetComponent<Text>();
        PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_healthScrolllingIcon.GetComponent<Image>().material.SetColor("_Color" , _playerColour);
        PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_StaticObjectMaterial.SetColor("_Color" , _playerColour);

    }
}
