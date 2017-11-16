using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;
using XInputDotNetPure;
public class PlayerStatus : MonoBehaviour, IHitByMelee
{
    //TODO Cleanup

    private Move m_MoveClass;
    private int m_iMaxHealth;
    public int m_iHealth = 3; //health completely useless right now
    int m_iTimesPunched = 0;
    int m_iPreviousTimesPunched = 0;

    bool m_bDead = false;
    public bool m_bStunned = false;
    public bool m_bMiniStun;
    public float StunedSlide = 400;
    public int m_iScore;
    public bool m_bInvincible = false;
    public float m_fInvincibleTime = 3.0f;
    public float m_fTimeBetweenColourSwapping = 0.5f;
    public float m_fStunBarOffset = 1.5f;
    public float m_fHealthBarOffset = 1.5f;
    public float m_fKillBarOffset = 1.5f;
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

    private BaseAbility m_Ability;

    public GameObject killMePrompt = null;
    public GameObject killMeArea = null;
    public Move Choker = null;

    private GameObject _PlayerCanvas;
    private GameObject _HealthMask;
    private GameObject HealthContainer;

    [SerializeField]
    private Image HealthLost;
    [SerializeField]
    private float m_fShowHealthMaxTime = 2.0f;
    private Timer healthLossTimer;
    private Timer ShowHealthChangeTimer;
    private bool m_bShowHealthLoss = false;
    private bool m_bShowHealthChange = false;
    private bool m_bLeftStun = false;
    [HideInInspector]
    public bool m_bKilledBySmash;
    Rigidbody2D _rigidbody;
    [HideInInspector]
    public int spawnIndex;
    public int mIEarnedPoints;
    public int mILostPoints;
    public Image stunBar;
    public Image stunMask;
    private GameObject stunBarContainer;
    private AudioSource m_MeleeHitAudioSource;

    private GameObject killbarContainer;

    public Sprite HeadSmashDeathSprite;
    public Sprite[] DeadSprites;
    public Sprite[] StunnedSprites;
    private bool DeathSpriteChanged = false;
    private bool StunSpriteChanged = false;
    private SpriteRenderer m_SpriteRenderer;
    private CameraControl _cameraControlInstance;


    [Range(0, 0.22f)]
    public float fill;
    //if the player is dead, the renderer will change their Color to gray, and all physics simulation of the player's rigidbody will be turned off.
    void Start()
    {
        m_MoveClass = this.GetComponent<Move>();
        ShowHealthChangeTimer = new Timer(m_fShowHealthMaxTime);
        healthLossTimer = new Timer(0.9f);

        _cameraControlInstance = CameraControl.mInstance;
        m_SpriteRenderer = this.transform.Find("Sprites").GetChild(0).GetComponent<SpriteRenderer>();

        m_MeleeHitAudioSource = this.gameObject.AddComponent<AudioSource>();
        m_MeleeHitAudioSource.outputAudioMixerGroup = AudioManager.RequestMixerGroup(SourceType.SFX);
        //m_MeleeHitAudioSource.outputAudioMixerGroup = (Resources.Load("AudioMixer/SFXAudio") as  AudioSource).outputAudioMixerGroup;
        m_MeleeHitAudioSource.playOnAwake = false;
        m_MeleeHitAudioSource.spatialBlend = 0.8f;
        m_Ability = this.GetComponent<BaseAbility>();
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

        LoadUIBars();

    }

    void Awake()
    {
        m_bInvincible = true; //when awake I am Invincible.
    }

    void Update()
    {
        ShowHealthChangeTimer.mfTimeToWait = m_fShowHealthMaxTime;
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (m_bDead)
            {
                ResetPlayer();
            }
        }
#endif
        if (!GameManagerc.Instance.Paused)
        {
            if (stunBarContainer.activeSelf)
            {
                float sunOffset = stunTimer.CurrentTime / stunTimer.mfTimeToWait * 0.24f;
                stunMask.material.SetTextureOffset("_MainTex", new Vector2(-0.24f + sunOffset, 0));
            }

            if (m_iHealth <= 0)
                m_iHealth = 0;

            StartCoroutine(InvinciblityTime());
            //update my score
            if (GameManagerc.Instance.PlayerWins.ContainsKey(this))
            {
                m_iScore = GameManagerc.Instance.PlayerWins[this];
            }
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
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
                    m_iTimesPunched = 0;
                    m_iPreviousTimesPunched = 0;
                }
            }

            //if im dead, set my Color to gray, turn of all physics simulations and exit the function
            if (m_bDead)
            {
                m_SpriteRenderer.sortingOrder = -4;
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0.35f);
                m_MoveClass.StopChoke();
                if (m_MoveClass.heldWeapon)
                {
                    m_MoveClass.ThrowWeapon(Vector2.zero, Vector2.zero, false);
                }
                SetAllAnimatorsFalse(false);
                PlayerSprite.material.color = Color.grey;

                //this.GetComponent<Rigidbody2D>().simulated = false;
                this.transform.Find("Colliders").GetChild(0).GetComponent<Collider2D>().enabled = false;
                killMePrompt.SetActive(false);
                killMeArea.SetActive(false);
                stunBarContainer.SetActive(false);

                m_MoveClass.GetBodyAnimator().enabled = false;
                m_MoveClass.GetFeetAnimator().enabled = false;

                if (DeadSprites.Length > 0 && !DeathSpriteChanged)
                {
                    DeathSpriteChanged = true;
                    if (!m_bKilledBySmash)
                        m_SpriteRenderer.sprite = DeadSprites[Random.Range(0, DeadSprites.Length)];
                    else
                        m_SpriteRenderer.sprite = HeadSmashDeathSprite;
                }

                return;

            }

            //if im stunned, make me cyan and show any kill prompts (X button and kill radius);
            if (m_bStunned)
            {
                if (StunnedSprites.Length > 0 && !StunSpriteChanged)
                {
                    StunSpriteChanged = true;
                    m_SpriteRenderer.sprite = StunnedSprites[Random.Range(0, StunnedSprites.Length)];
                    m_MoveClass.GetBodyAnimator().SetBool("Stunned", true);

                }

                m_SpriteRenderer.sortingOrder = -4;
                if (Choker != null && Choker.chokingPlayer != null)
                {
                    if (Choker.chokingPlayer.transform.root != this.gameObject.transform.root)
                    {
                        Choker = null;
                    }
                }
                if (m_MoveClass.heldWeapon) //if holding weapon;
                {
                    m_MoveClass.StatusApplied();
                }
                stunTimer.mfTimeToWait = m_fStunTime;

                m_Ability.m_ChargeIndicator.SetActive(false);
                m_Ability.ChargeCoolDown = false;

                //Changes the sprite if stunned.


                SetAllAnimatorsFalse(true);
                killMeArea.SetActive(true);
                PlayerSprite.material.color = Color.cyan;
                stunBarContainer.SetActive(true);

                CheckForButtonMash();
                //this.GetComponent<Renderer>().material.color = Color.cyan;
                //Find the collision colliders and turn them on
                this.transform.Find("Colliders").gameObject.GetComponent<Collider2D>().enabled = true;
                //this.transform.Find("Colliders").gameObject.GetComponent<Collider2D>().isTrigger = true;


                m_MoveClass.MakeCollidersTriggers(true);
                if (stunTimer.Tick(Time.deltaTime))
                {
                    m_bStunned = false;
                }
                m_bLeftStun = false; //set to true as it hasn't left stun yet.
            }
            //When not stunned
            else
            {
                m_SpriteRenderer.sortingOrder = 4;
                Choker = null;
                if (!m_bLeftStun)
                {
                    m_MoveClass.GetBodyAnimator().SetBool("CancelHeadSmash", true);
                    m_MoveClass.GetBodyAnimator().SetBool("Stunned", false);
                    m_bLeftStun = true;
                }
                //GetComponent<Move>().GetBodyAnimator().enabled = true; //Get the animator(s) from the Move script and enable them
                //GetComponent<Move>().GetFeetAnimator().enabled = true; //Get the animator(s) from the Move script and enable them
                m_Ability.m_ChargeIndicator.SetActive(true); //Turn the ability charge indicator back on
                m_Ability.ChargeCoolDown = true; //Continue to tick the timer for more Ability charges
                StunSpriteChanged = false;
                m_MoveClass.MakeCollidersTriggers(false);
                stunBarContainer.SetActive(false);

                if (this.transform.GetChild(1).tag == "Stunned")
                {

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
    }

    //Snap all the health/stun bars to the player's position.
    public void LateUpdate()
    {
        if (/*GameManagerc.Instance.Paused || Application.isEditor*/ true)
        {
            //Check for a health mask
            if (_HealthMask)
            {
                float xOffset = m_iHealth * -0.0791f;
                _HealthMask.GetComponent<Image>().material.SetTextureOffset("_MainTex", new Vector2(0 + xOffset, 0));

                if (m_bShowHealthLoss)
                {
                    if (healthLossTimer.Tick(Time.deltaTime))
                    {
                        HealthLost.fillAmount = m_iHealth / 3;
                        m_bShowHealthLoss = false;
                    }
                }
            }

            //If the previous frames health isnt the current frames health, show the changed health.
            //_PlayerCanvas.transform.localScale = new Vector3(Camera.main.orthographicSize, Camera.main.orthographicSize, Camera.main.orthographicSize) * 0.003f;
            _PlayerCanvas.transform.position = this.transform.position - Vector3.forward * 8;
            _PlayerCanvas.transform.rotation = Quaternion.identity; //This should fix it, though not sure
            //HealthContainer.transform.position = this.transform.position + Vector3.up * m_fHealthBarOffset - Vector3.forward * 8 ;
            //stunBarContainer.transform.position = this.transform.position + Vector3.up * m_fStunBarOffset - Vector3.forward * 8;
            //killbarContainer.transform.position = this.transform.position - Vector3.up * 1.5f;
            //Showing health change is when the health bar shows up. health loss is seperate.
            if (m_bShowHealthChange)
            {
                HealthContainer.SetActive(true);
                if (stunBarContainer.activeSelf)
                {
                    HealthContainer.transform.localPosition = new Vector3(0, 150, 0);
                }
                else
                {
                    HealthContainer.transform.localPosition = new Vector3(0, 100, 0);
                }

                //HealthContainer.transform.position = -this.transform.up * 0.5f;
                if (ShowHealthChangeTimer.Tick(Time.deltaTime))
                {
                    HealthContainer.SetActive(false);
                    m_bShowHealthChange = false;
                }
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

        StartCoroutine(MiniStun(StunTime));

    }

    public IEnumerator MiniStun(float StunTime)
    {

        yield return new WaitForSeconds(StunTime);
        m_bMiniStun = false;
        yield return null;

    }

    /// <summary>
    /// Used for combining a stun effect with a knock back. If no stun required use "Knockback()"
    /// </summary>
    /// <param name="ThrownItemVelocity"></param>

    public void StunPlayer(Vector3 ThrownItemVelocity)
    {
        float angle = Mathf.Atan2(ThrownItemVelocity.normalized.x, -ThrownItemVelocity.normalized.y);
        this.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
        //stun the player called outside of class
        //Vector3 a = ThrownItemVelocity.normalized;
        // _rigidbody.velocity = (a * StunedSlide);
        m_MoveClass.StopChoke();
        this.GetComponent<Move>().StatusApplied();
        SetAllAnimatorsFalse(true);                                         //! This was a problem for the animator, edited to take in a if stunned bool.
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

    //Resets the player, called whenever the map is reset.
    public void ResetPlayer()
    {
        m_iHealth = 3;
        m_bDead = false;
        m_bStunned = false;
        m_iTimesPunched = 0;
        stunTimer.CurrentTime = 0;
        DeathSpriteChanged = false;
        this.GetComponent<Rigidbody2D>().simulated = true;
        m_Ability.AbilityCharges = 0;
        float xOffset = m_iHealth * -0.0791f;
        _HealthMask.GetComponent<Image>().material.SetTextureOffset("_MainTex", new Vector2(0 + xOffset, 0));
        //ControllerManager.Instance.FindSpawns();
        this.transform.position = ControllerManager.Instance.spawnPoints[spawnIndex].position;
        //this.transform.position = Vector3.zero;
        GetComponent<Move>().ThrowWeapon(Vector2.zero, Vector2.up, false);
        _PlayerCanvas.transform.SetParent(this.transform.Find("Sprites"));
        // this.GetComponent<Collider2D>().isTrigger = true;
        m_bInvincible = true;
        m_MoveClass.GetBodyAnimator().enabled = true;
        m_MoveClass.GetFeetAnimator().enabled = true;
        //Reset the body animator
        foreach (AnimatorControllerParameter item in m_MoveClass.GetBodyAnimator().parameters)
        {
            if (item.type == AnimatorControllerParameterType.Bool)
            {
                m_MoveClass.GetBodyAnimator().SetBool(item.name, false);
            }
            else if (item.type == AnimatorControllerParameterType.Trigger)
            {
                m_MoveClass.GetBodyAnimator().ResetTrigger(item.name);
            }
        }
        //Reset feet animator
        foreach (AnimatorControllerParameter item in m_MoveClass.GetFeetAnimator().parameters)
        {
            if (item.type == AnimatorControllerParameterType.Bool)
            {
                m_MoveClass.GetFeetAnimator().SetBool(item.name, false);
            }
            else if (item.type == AnimatorControllerParameterType.Trigger)
            {
                m_MoveClass.GetFeetAnimator().ResetTrigger(item.name);
            }
        }
        //Find all colliders and turn them on
        this.transform.Find("Colliders").GetChild(0).GetComponent<Collider2D>().enabled = true;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Load UI stuff.
        LoadUIBars();

        if (scene.buildIndex == 0)
        {
            Destroy(this.gameObject);
            return;
        }
        //time to re activate all the UI stuff
        this.GetComponent<BaseAbility>().GetUIElements();

        //foreach (var item in PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_objects)
        //{
        //    item.SetActive(true);
        //}

    }
    //Kills the player
    public void KillPlayer(PlayerStatus killer)
    {
        //kill the player, called outside of class (mostly used for downed kills)
        if (/*!m_bInvincible*/true)
        {
            if (killer)
            {
                if (GameManagerc.Instance.m_gameMode == Gamemode_type.HEAD_HUNTERS)
                {
                    if (killer != this)
                    {
                        killer.mIEarnedPoints++;
                    }
                    else
                    {
                        if (GameManagerc.Instance.PlayerWins[this] < GameManagerc.Instance.m_iPointsNeeded)
                        {
                            killer.mILostPoints++;
                        }
                    }
                }
                GameManagerc.Instance.lastPlayerToEarnPoints = killer;
            }
            SetAllAnimatorsFalse(false);
            m_iHealth = 0;
            m_bDead = true;
            m_bStunned = false;
            //if (GameManagerc.Instance.m_gameMode == Gamemode_type.DEATHMATCH_POINTS)
            //    GameManagerc.Instance.PlayerWins[killer]++;
        }

    }
    //handles all logic for a bullet hitting the player
    public void HitPlayer(Bullet aBullet, bool abGiveIFrames = false)
    {
        if (!m_bInvincible)
        {
            healthLossTimer.CurrentTime = 0;
            m_bShowHealthLoss = true;
            ShowHealthChangeTimer.CurrentTime = 0;
            m_bShowHealthChange = (aBullet.m_iDamage > 0);
            m_iHealth -= aBullet.m_iDamage;
            m_MoveClass.vibrationValue.x = 5;
            //If the game mode is either the timed deathmatch or scores appointed on kills deathmatch, then give them points
            if (m_iHealth <= 0 /*&& (GameManagerc.Instance.m_gameMode == Gamemode_type.DEATHMATCH_POINTS *//*|| GameManagerc.Instance.m_gameMode == Gamemode_type.DEATHMATCH_TIMED*/)
            {
                m_MoveClass.ThrowWeapon(Vector2.zero, Vector2.zero, false);
                //update the bullet owner's score
                //GameManagerc.Instance.PlayerWins[aBullet.bulletOwner]++;
            }
        }
        if (abGiveIFrames)
        {
            m_bInvincible = true;
        }
    }
    //Does all the logic for when a player is hit by a weapon.
    public void HitPlayer(Weapon a_weapon, bool abGiveIFrames = false)
    {
        if (!m_bInvincible)
        {
            healthLossTimer.CurrentTime = 0;
            m_bShowHealthLoss = true;
            ShowHealthChangeTimer.CurrentTime = 0;
            m_bShowHealthChange = (a_weapon.m_iDamage > 0);
            m_iHealth -= a_weapon.m_iDamage;
            m_MoveClass.vibrationValue.x = 5;
            //If the game mode is either the timed deathmatch or scores appointed on kills deathmatch, then give them points
            if (m_iHealth <= 0 /*&& (GameManagerc.Instance.m_gameMode == Gamemode_type.DEATHMATCH_POINTS*/ /*|| GameManagerc.Instance.m_gameMode == Gamemode_type.DEATHMATCH_TIMED*/)
            {
                m_MoveClass.ThrowWeapon(Vector2.zero, Vector2.zero, false);
                //update the bullet owner's score
                // GameManagerc.Instance.PlayerWins[a_weapon.transform.root.GetComponent<PlayerStatus>()]++;
            }
        }
        if (abGiveIFrames)
        {
            m_bInvincible = true;
        }
    }

    public void HitPlayer(int a_Damage, PlayerStatus a_Status, bool abGiveIFrames = false) // maybe change to int
    {
        if (!m_bInvincible)
        {
            healthLossTimer.CurrentTime = 0;
            m_bShowHealthLoss = true;
            ShowHealthChangeTimer.CurrentTime = 0;
            m_bShowHealthChange = (a_Damage > 0);
            m_iHealth -= a_Damage;
            m_MoveClass.vibrationValue.x = 5;
            //If the game mode is either the timed deathmatch or scores appointed on kills deathmatch, then give them points
            if (m_iHealth <= 0 /*&& (GameManagerc.Instance.m_gameMode == Gamemode_type.DEATHMATCH_POINTS*/ /*|| GameManagerc.Instance.m_gameMode == Gamemode_type.DEATHMATCH_TIMED*/)
            {
                m_MoveClass.ThrowWeapon(Vector2.zero, Vector2.zero, false);
                //update the bullet owner's score
                //GameManagerc.Instance.PlayerWins[a_Status]++;
            }
        }
        if (abGiveIFrames)
        {
            m_bInvincible = true;
        }
    }

    //Give I frames to the player, the flashing is broken but whatever.
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
            yield return new WaitForSecondsRealtime(m_fTimeBetweenColourSwapping);
            PlayerSprite.GetComponent<Renderer>().material = m;
            PlayerSprite.GetComponent<Renderer>().material.color = _playerColor;
        }

        yield return null;
    }

    //Clear removes the onsceneloaded from the scenelaoded delegate
    public void Clear()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //Checks for a button mash to reduce the stun time.
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

    //Turn all animations off in the animator;
    void SetAllAnimatorsFalse(bool a_Stunned)
    {
        Animator Body = GetComponent<Move>().GetBodyAnimator();
        Animator Feet = GetComponent<Move>().GetFeetAnimator();

        //If the body animator is found, go through each parameter, and set it to false
        if (Body)
        {
            foreach (AnimatorControllerParameter parameter in Body.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Bool) //snaity check
                {
                    if (a_Stunned == true && parameter.name != "BeingSmashed")
                        Body.SetBool(parameter.name, false);
                }
            }
        }
        //same with feet
        if (Feet)
        {
            foreach (AnimatorControllerParameter parameter in Feet.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Bool) //sanity check
                    Feet.SetBool(parameter.name, false);
            }
        }
        if (a_Stunned == true)
        {
            Body.SetBool("Stunned", true);
        }
    }

    public void HitByMelee(Weapon meleeWeapon, AudioClip soundEffect, float Volume = 1, float Pitch = 1)
    {
        m_MeleeHitAudioSource.clip = soundEffect;
        m_MeleeHitAudioSource.volume = Volume;
        m_MeleeHitAudioSource.pitch = Pitch;
        m_MeleeHitAudioSource.Play();
        //GetComponent<AudioSource>().PlayOneShot(soundEffect , Volume); 
    }


    void LoadUIBars()
    {

        //Find the canvas that holds all the player UI bars
        _PlayerCanvas = this.transform.Find("Sprites").Find("PlayerCanvas").gameObject;
        //Player bars and shit

        //Find the container for the stun bar and the stun mask
        stunBarContainer = this.transform.Find("Sprites").Find("PlayerCanvas").GetChild(1).gameObject;
        stunMask = stunBarContainer.transform.GetChild(0).GetComponent<Image>();

        //Find the killcontainers
        Image killMask;
        killbarContainer = this.transform.Find("Sprites").Find("PlayerCanvas").GetChild(2).gameObject;
        killMask = killbarContainer.transform.GetChild(0).GetComponent<Image>();

        //Find all the bars and containers related to health.
        _HealthMask = this.transform.Find("Sprites").Find("PlayerCanvas").GetChild(0).GetChild(0).gameObject;
        Material temp = new Material(_HealthMask.GetComponent<Image>().material.shader);
        _HealthMask.GetComponent<Image>().material = temp;
        HealthContainer = this.transform.Find("Sprites").Find("PlayerCanvas").GetChild(0).gameObject;
        HealthLost = this.transform.Find("Sprites").Find("PlayerCanvas").GetChild(0).GetChild(1).GetComponent<Image>();

        //Set health bar colours to my player colour
        foreach (var item in HealthContainer.GetComponentsInChildren<Image>())
        {
            Material oldMat = item.GetComponent<Image>().material;

            Material tempMaterial = new Material(item.GetComponent<Image>().material.shader);
            item.GetComponent<Image>().material = tempMaterial;
            if (item.GetComponent<Image>().material.HasProperty("_Color") && !item.CompareTag("White"))
                item.GetComponent<Image>().material.color = _playerColor;
        }
        //Set the stun bar container colours
        foreach (var item in stunBarContainer.GetComponentsInChildren<Image>())
        {
            Material oldMat = item.GetComponent<Image>().material;

            Material tempMaterial = new Material(item.GetComponent<Image>().material.shader);
            item.GetComponent<Image>().material = tempMaterial;
            if (item.GetComponent<Image>().material.HasProperty("_Color") && !item.CompareTag("White"))
                item.GetComponent<Image>().material.color = _playerColor;
        }

        //Set the killcontainer's colour;
        foreach (var item in killbarContainer.GetComponentsInChildren<Image>())
        {
            Material oldMat = item.GetComponent<Image>().material;

            Material tempMaterial = new Material(item.GetComponent<Image>().material.shader);
            item.GetComponent<Image>().material = tempMaterial;
            if (item.GetComponent<Image>().material.HasProperty("_Color") && !item.CompareTag("White"))
                item.GetComponent<Image>().material.color = _playerColor;
        }

        m_MoveClass.SetUIBars(killMask, killbarContainer); //Set the UI bars for the move class to work
        //Change the health loss colour to yellow so it stands out. (kind of)
        HealthLost.color = Colors.Yellow;
        _PlayerCanvas.transform.SetParent(null); //Set its parent to null so it can properly follow the player's positon.
        HealthContainer.SetActive(false); //turn it off.
        killbarContainer.SetActive(false);
    }

    public void KilledAPlayer(PlayerStatus killedPlayer)
    {
        Debug.Log("kiled a player");
        //if (!GameManagerc.Instance.m_bWinnerFound)
        //{
        if (killedPlayer != this)
        {
            mIEarnedPoints += 1;
        }
        else
        {
            if (GameManagerc.Instance.PlayerWins[this] < GameManagerc.Instance.m_iPointsNeeded)
            {
                mILostPoints += 1;
            }
        }
        //}
    }

}







