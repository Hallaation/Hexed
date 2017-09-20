using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;
using XInputDotNetPure;
public class PlayerStatus : MonoBehaviour, IHitByMelee
{
    private float m_iMaxHealth;
    public float m_iHealth = 3; //health completely useless right now
    int m_iTimesPunched = 0;
    int m_iPreviousTimesPunched = 0;
    bool m_bDead = false;
    public bool m_bStunned = false;
    public bool m_bMiniStun;
    public float StunedSlide = 400;
    public int m_iScore;
    public bool m_bInvincible = false;
    public float m_fInvincibleTime = 3.0f;
    public float test = 0.5f;
    private Timer m_InvincibilityTimer;
    public bool IsDead { get { return m_bDead; } set { m_bDead = value; } }
    public bool IsStunned { get { return m_bStunned; } set { m_bStunned = value; } }
    public int TimesPunched { get { return m_iTimesPunched; } set { m_iTimesPunched = value; } }

    public float m_fStunTime = 1;
    public float m_fMaximumStunWait = 2;
    public float m_fStunTimerReduction = 0.5f;
    Timer stunTimer;
    Timer resetStunTimer;
   
    [HideInInspector]
    public Color _playerColor;
    private Renderer PlayerSprite;


    public GameObject killMePrompt = null;

    public GameObject killMeArea = null;

    private GameObject _HealthMask;
    private Text _ScoreText;
    Rigidbody2D _rigidbody;
    [HideInInspector]
    public int spawnIndex;

    public SpriteRenderer stunBar;
    public SpriteRenderer stunMask;
    private GameObject stunBarContainer;
    private AudioSource m_MeleeHitAudioSource;
    [Range(0, 0.22f)]
    public float fill;
    //if the player is dead, the renderer will change their Color to gray, and all physics simulation of the player's rigidbody will be turned off.
    void Start()
    {
        m_MeleeHitAudioSource = this.gameObject.AddComponent<AudioSource>();
        m_MeleeHitAudioSource.playOnAwake = false;
        m_MeleeHitAudioSource.spatialBlend = 1;

        stunBarContainer = new GameObject("StunBarContainer");

        stunBar = transform.Find("Sprites").Find("StunBarContainer").Find("StunBar").GetComponent<SpriteRenderer>();
        stunMask = transform.Find("Sprites").Find("StunBarContainer").Find("StunBarMask").GetComponent<SpriteRenderer>();

        stunBarContainer.transform.SetParent(transform.Find("Sprites"));

        stunBar.gameObject.transform.SetParent(stunBarContainer.transform);
        stunMask.gameObject.transform.SetParent(stunBarContainer.transform);

        stunBar.transform.localPosition = Vector3.zero;
        stunMask.transform.localPosition = Vector3.zero;

        stunBarContainer.transform.localPosition = Vector3.zero;
        stunBarContainer.transform.position += Vector3.up * 1.2f;
        stunBarContainer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));

        stunBarContainer.SetActive(false);

        if (stunBar && stunMask)
        {
            stunMask.material.shader = Shader.Find("Custom/MaskTest");
            stunMask.material.renderQueue = 3000;
            stunBar.material.shader = Shader.Find("Sprites/Default");
            stunBar.material.renderQueue = 3002;
        }

        m_InvincibilityTimer = new Timer(m_fInvincibleTime);
        _rigidbody = GetComponent<Rigidbody2D>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        m_iMaxHealth = m_iHealth;
        //initialize my timer and get the player's Color to return to.
        stunTimer = new Timer(m_fStunTime);
        resetStunTimer = new Timer(m_fMaximumStunWait);
        //_playerColor = GetComponent<Renderer>().material.color;
        if (GetComponent<Renderer>())
        {
            PlayerSprite = GetComponent<Renderer>();
            _playerColor = GetComponent<Renderer>().material.color;
        }
        else
        {
            PlayerSprite = transform.Find("Sprites").Find("PlayerSprite").GetComponent<Renderer>();
            _playerColor = transform.Find("Sprites").Find("PlayerSprite").GetComponent<Renderer>().material.color;
        }
        killMePrompt.SetActive(false);

        _HealthMask = PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_HealthBarMask;
        //_ScoreText = PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_ScoreText.GetComponent<Text>();
        PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_healthScrolllingIcon.GetComponent<Image>().material.SetColor("_Color", _playerColor);
        PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_StaticObjectMaterial.SetColor("_Color", _playerColor);

        foreach (var item in PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_objects)
        {
            item.SetActive(true);
        }

    }

    void Awake()
    {
        m_bInvincible = true;
    }
    void Update()
    {
        if (stunBarContainer.activeSelf)
        {
            float sunOffset = stunTimer.CurrentTime / stunTimer.mfTimeToWait * 0.25f;
            stunMask.material.SetTextureOffset("_MainTex", new Vector2(0.25f - sunOffset, 0));
        }

        if (m_iHealth <= 0)
            m_iHealth = 0;

        StartCoroutine(InvinciblityTime());
        //update my score
        if (GameManagerc.Instance.PlayerWins.ContainsKey(this))
        {
            m_iScore = GameManagerc.Instance.PlayerWins[this];
        }
        //Debug.LogError(GetComponent<ControllerSetter>().m_playerNumber);
        if (_ScoreText != null)
        {
            _ScoreText.text = m_iScore.ToString();
        }
        //if i've been punched once, start the timer, once the timer has reached the end, reset the amount of times punched.
        if (m_iTimesPunched >= 1)
        {
            if (m_iTimesPunched != m_iPreviousTimesPunched)
            {
                resetStunTimer.SetTimer(0);
                m_iPreviousTimesPunched = m_iTimesPunched;
            }
            if (resetStunTimer.Tick(Time.deltaTime))
            {
                //  m_iTimesPunched = 0;
                // m_iPreviousTimesPunched = 0; 
                //TODO Needs to be readded at some point.
            }
        }

        if (_HealthMask)
        {
            float xOffset = m_iHealth * -0.0791f;
            _HealthMask.GetComponent<Image>().material.SetTextureOffset("_MainTex", new Vector2(0 + xOffset, 0));
        }

        //if im dead, set my Color to gray, turn of all physics simulations and exit the function
        if (m_bDead)
        {
            //this.GetComponent<Renderer>().material.color = Color.grey;
            SetAllAnimatorsFalse();
            PlayerSprite.material.color = Color.grey;
            this.GetComponent<Rigidbody2D>().simulated = false;
            killMePrompt.SetActive(false);
            killMeArea.SetActive(false);
            stunBarContainer.SetActive(false);
            return;
        }

        //if im stunned, make me cyan and show any kill prompts (X button and kill radius);
        if (m_bStunned)
        {
            SetAllAnimatorsFalse();
            killMeArea.SetActive(true);
            PlayerSprite.material.color = Color.cyan;

            //set the stun bar location
            stunBarContainer.transform.localPosition = Vector3.zero;
            stunBarContainer.transform.position += Vector3.up * 1.2f;
            stunBar.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            stunMask.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            stunBarContainer.SetActive(true);
            CheckForButtonMash();
            //this.GetComponent<Renderer>().material.color = Color.cyan;
            if (this.transform.GetChild(1).tag == "Stunned")
            {
                this.transform.GetChild(1).gameObject.GetComponent<Collider2D>().enabled = true;
            }
            else
            {
                this.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = true;
            }
            this.GetComponent<Move>().MakeCollidersTriggers(true);
            if (stunTimer.Tick(Time.deltaTime))
            {
                m_bStunned = false;
            }
        }
        //if not stunned dont kill me
        else
        {
            this.GetComponent<Move>().MakeCollidersTriggers(false);
            stunBarContainer.SetActive(false);
            if (this.transform.GetChild(1).tag == "Stunned")
            {
                Debug.Log(this.transform.GetChild(0).tag);
                this.transform.GetChild(1).gameObject.GetComponent<Collider2D>().enabled = false;        //? child 0 is weaponSpot... 
            }
            else
            {
                this.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;
            }
            killMeArea.SetActive(false);
            killMePrompt.SetActive(false);
            //If I find a regular renderer
            if (GetComponent<Renderer>())
            {
                GetComponent<Renderer>().material.color = _playerColor;
            }
            else //if no rendere was found
            {
                if (!m_bInvincible)
                    PlayerSprite.GetComponent<Renderer>().material.color = _playerColor;
            }
        }

    }


    public void MiniStun(Vector3 ForceApplied, float StunTime)
    {
        m_bMiniStun = true;
        //TODO change to attacking animators = false maybe.
        //SetAllAnimatorsFalse();
        //        GetComponent<Move>().SetActive(false);
        _rigidbody.velocity = ForceApplied;
        Debug.Log("Corotuine should be here");
       StartCoroutine( MiniStun(StunTime));

    }
    public IEnumerator MiniStun(float StunTime)
    {
        //Debug.Log(StunTime);
        yield return new WaitForSeconds(StunTime);
        m_bMiniStun = false;
        yield return null;
        //Debug.Log("Done");
    }
    /// <summary>
    /// Used for combining a stun effect with a knock back. If no stun required use "Knockback()"
    /// </summary>
    /// <param name="ThrownItemVelocity"></param>

    public void StunPlayer(Vector3 ThrownItemVelocity)
    {
        //stun the player called outside of class
        //Vector3 a = ThrownItemVelocity.normalized;
        // _rigidbody.velocity = (a * StunedSlide);
        SetAllAnimatorsFalse();
        _rigidbody.velocity = ThrownItemVelocity;
        m_bStunned = true;
        m_iTimesPunched = 0;
    }
    /// <summary>
    /// Used for knocking a player back without stunning them.
    /// </summary>
    /// <param name="KnockBackVelocity"></param>
    public void KnockBack(Vector3 KnockBackVelocity) 
    {
        _rigidbody.velocity = KnockBackVelocity;
    }


    public void ResetPlayer()
    {
        m_iHealth = 3;
        m_bDead = false;
        m_bStunned = false;
        m_iTimesPunched = 0;
        this.GetComponent<Rigidbody2D>().simulated = true;

        float xOffset = m_iHealth * -0.0791f;
        _HealthMask.GetComponent<Image>().material.SetTextureOffset("_MainTex", new Vector2(0 + xOffset, 0));
        //this.transform.position = ControllerManager.Instance.spawnPoints[spawnIndex].position;
        this.transform.position = Vector3.zero;
        GetComponent<Move>().ThrowWeapon(Vector2.zero, Vector2.up, false);

       // this.GetComponent<Collider2D>().isTrigger = true;
        m_bInvincible = true;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            Destroy(this.gameObject);
            return;
        }
        //time to re activate all the UI stuff
        this.GetComponent<BaseAbility>().GetUIElements();

        _HealthMask = PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_HealthBarMask;
        _ScoreText = PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_ScoreText.GetComponent<Text>();
        PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_healthScrolllingIcon.GetComponent<Image>().material.SetColor("_Color", _playerColor);
        PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_StaticObjectMaterial.SetColor("_Color", _playerColor);

        foreach (var item in PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_objects)
        {
            item.SetActive(true);
        }
    }
    public void KillPlayer(PlayerStatus killer)
    {
        //kill the player, called outside of class (mostly used for downed kills)
        if (/*!m_bInvincible*/true)
        {
            SetAllAnimatorsFalse();
            m_iHealth = 0;
            m_bDead = true;
            if (GameManagerc.Instance.m_gameMode == Gamemode_type.DEATHMATCH_POINTS)
                GameManagerc.Instance.PlayerWins[killer]++;
        }

    }

    public void HitPlayer(Bullet aBullet, bool abGiveIFrames = false)
    {
        if (!m_bInvincible)
        {
            m_iHealth -= aBullet.m_iDamage;
            //If the game mode is either the timed deathmatch or scores appointed on kills deathmatch, then give them points
            if (m_iHealth <= 0 && (GameManagerc.Instance.m_gameMode == Gamemode_type.DEATHMATCH_POINTS /*|| GameManagerc.Instance.m_gameMode == Gamemode_type.DEATHMATCH_TIMED*/))
            {
                //update the bullet owner's score
                GameManagerc.Instance.PlayerWins[aBullet.bulletOwner]++;
            }
        }
        if (abGiveIFrames)
        {
            m_bInvincible = true;
        }
    }

    IEnumerator InvinciblityTime()
    {
        if (m_InvincibilityTimer.mfTimeToWait != m_fInvincibleTime)
        {
            m_InvincibilityTimer = new Timer(m_fInvincibleTime);
        }

        if (m_bInvincible)
        {
            if (m_InvincibilityTimer.Tick(Time.deltaTime))
            {
                m_bInvincible = false;
            }
            Material m = PlayerSprite.GetComponent<Renderer>().material;

            PlayerSprite.GetComponent<Renderer>().material = null;
            PlayerSprite.GetComponent<Renderer>().material.color = Color.white;
            yield return new WaitForSecondsRealtime(test);
            PlayerSprite.GetComponent<Renderer>().material = m;
            PlayerSprite.GetComponent<Renderer>().material.color = _playerColor;
        }

        yield return null;
    }

    public void Clear()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void CheckForButtonMash()
    {
        if (!m_bMiniStun)
        {
            if (XCI.GetButtonDown(XboxButton.X, GetComponent<ControllerSetter>().mXboxController))
            {
                stunTimer.CurrentTime += m_fStunTimerReduction;
            }
        }

    }

    void SetAllAnimatorsFalse()
    {
        Animator Body = GetComponent<Move>().GetBodyAnimator();
        Animator Feet = GetComponent<Move>().GetFeetAnimator();

        //If the body animator is found, go through each parameter, and set it to false
        if (Body)
        {
            foreach (AnimatorControllerParameter parameter in Body.parameters)
            {
                Body.SetBool(parameter.name, false);
            }
        }
        //same with feet
        if (Feet)
        {
            foreach (AnimatorControllerParameter parameter in Feet.parameters)
            {
                Feet.SetBool(parameter.name, false);
            }
        }
    }

    public void HitByMelee(Weapon meleeWeapon , AudioClip soundEffect , float Volume = 1, float Pitch = 1)
    {
        m_MeleeHitAudioSource.clip = soundEffect;
        m_MeleeHitAudioSource.volume = Volume;
        m_MeleeHitAudioSource.pitch = Pitch;
        m_MeleeHitAudioSource.Play();
        //GetComponent<AudioSource>().PlayOneShot(soundEffect , Volume); 
    }
}




