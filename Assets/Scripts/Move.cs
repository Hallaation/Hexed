using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XboxCtrlrInput;
using XInputDotNetPure;
using UnityEngine.SceneManagement;
//////////////////////
//                  //
//   Louis Nguyen   //
//   21/07/2017     //
//                  //
//////////////////////

[RequireComponent(typeof(ControllerSetter))]
[RequireComponent(typeof(PlayerStatus))]
public class Move : MonoBehaviour
{
    ControllerSetter m_controller;
    // CharacterController _characterController;
    bool PlayerIsActive = true; public bool getActive() { return PlayerIsActive; }
    public void SetActive(bool a_PlayerActive) { PlayerIsActive = a_PlayerActive; }
    PlayerStatus m_status;
    [HideInInspector]
    public GameObject heldWeapon = null;
    Rigidbody2D _rigidBody;
    private bool m_bTriggerReleased;
    [HideInInspector]
    public bool m_bStopStickRotation = false;
    bool m_bHoldingWeapon = false;
    bool runningAnimation = false;
    //[HideInInspector]
    public GameObject crosshair;
    protected Animator FeetAnimator; public Animator GetFeetAnimator() { return FeetAnimator; }
    protected Animator BodyAnimator; public Animator GetBodyAnimator() { return BodyAnimator; }
    public Transform weapon1HandedMount;
    public Transform weapon2HandedMount;
    public Transform Melee1HandedMount;
    public Transform Melee2HandedMount;
    public GameObject fistObject;
    public float movementSpeed = 10.0f;
    public float throwingForce = 100.0f;
    private float StoredMoveSpeed;

    [Header("Head Smashing Variables")]
    public float m_fChokedTimeIncrement = 0.1f; //Everytime the choking is hpapening, increment the time.
    public float m_fChokeKillTime = 0.8f; //Time it takes for the kill to happen
    private bool m_bInChokeMode = false; //Determine if this guy is choking someone
    private bool m_bChoked; //Used to check if the head has been smashed in the entire animation so it only happens once.
    private GameObject chokingPlayer = null; //the player this guy is choking
    private int OriginalSortingOrder; //Used to move the player back to their sorting layer so everything renders properly.
    private Timer m_ChokingTimer;

    private Image killMask;
    private GameObject KillBarContainer;

    //public bool m_b2DMode = true;
    EmptyHand defaultWeapon;
    [HideInInspector]
    public Vector2 vibrationValue;
    [HideInInspector]
    public GameObject playerSpirte;
    //float MoveDelayTimer;
    [Space]
    public float StartMoveDelay = 3;
    public Vector3 m_LeftStickRotation;
    public float StickDeadZone = 0.12f;
    public string ColorDatabaseKey = "Player1";
    public AudioClip quack;
    private Database ColorDatabase;
    private Dictionary<string, Color> colorDictionary;
    //  private Text _AmmoText;

    private GameObject previousWeapon = null;
    private Collider2D m_NoHandsCollider;
    private Collider2D m_OneHandCollider;
    private Collider2D m_TwoHandedCollider;

    private AudioSource[] m_audioSource;
    private GameObject AudioSourcePool;
    //Vector3 movement;
    // Use this for initialization
    void Awake()
    {
        m_ChokingTimer = new Timer(m_fChokeKillTime);
        // movement = Vector3.zero;
        //pool of audiosources
        m_audioSource = new AudioSource[16];
        AudioSourcePool = new GameObject("AudioSources");
        AudioSourcePool.transform.SetParent(this.transform);
        AudioSourcePool.transform.localPosition = Vector3.zero;

        for (int i = 0; i < m_audioSource.Length; ++i)
        {
            m_audioSource[i] = AudioSourcePool.AddComponent<AudioSource>();
            m_audioSource[i].outputAudioMixerGroup = (Resources.Load("AudioMixer/SFXAudio") as GameObject).GetComponent<AudioSource>().outputAudioMixerGroup;
            m_audioSource[i].playOnAwake = false;
            m_audioSource[i].clip = quack;
            m_audioSource[i].spatialBlend = 1;
        }

        if (!ColorDatabase)
        {
            ColorDatabase = Resources.Load("Database") as Database;

            colorDictionary = new Dictionary<string, Color>();
            for (int i = 0; i < ColorDatabase.colors.Length; i++)
            {
                if (!colorDictionary.ContainsKey(ColorDatabase.colors[i].PlayerType))
                    colorDictionary.Add(ColorDatabase.colors[i].PlayerType, ColorDatabase.colors[i].playerColor);
            }
        }


        weapon1HandedMount = transform.Find("1HandedSpot");
        weapon2HandedMount = transform.Find("2HandedSpot");


        if (transform.Find("Sprites"))
        {
            Melee1HandedMount = transform.Find("Sprites").GetChild(0).Find("1HandedMeleeSpot");
            Melee2HandedMount = transform.Find("Sprites").GetChild(0).Find("2HandedMeleeSpot");
            if (transform.Find("Sprites").transform.Find("Character001_Feet"))
            {
                FeetAnimator = transform.Find("Sprites").transform.Find("Character001_Feet").GetComponent<Animator>();
            }
            if (transform.Find("Sprites").transform.Find("Character001_Body"))
            {
                BodyAnimator = transform.Find("Sprites").transform.Find("Character001_Body").GetComponent<Animator>();
            }
        }
        if (crosshair)
            crosshair.AddComponent<CrosshairClamp>();

        vibrationValue = Vector2.zero;
        //setting up any references to other classes needed.
        m_NoHandsCollider = transform.Find("Colliders").Find("NoHands").GetComponent<Collider2D>();
        m_OneHandCollider = transform.Find("Colliders").Find("1Hand").GetComponent<Collider2D>();
        m_TwoHandedCollider = transform.Find("Colliders").Find("2Hands").GetComponent<Collider2D>();
        m_NoHandsCollider.enabled = true;
        m_controller = GetComponent<ControllerSetter>();
        m_status = GetComponent<PlayerStatus>();
        _rigidBody = GetComponent<Rigidbody2D>();
        if (transform.Find("Sprite/Character001_Body") != null)
            playerSpirte = transform.Find("Sprites/Character001_Body").gameObject;
        else if (transform.Find("Sprites/PlayerSprite"))
            playerSpirte = transform.Find("Sprites/PlayerSprite").gameObject;
        //change my colour depending on what player I am
        Renderer temp;
        if (GetComponent<Renderer>())
        {
            temp = GetComponent<Renderer>();
        }
        else
        {
            if (transform.Find("Sprite/Character001_Body") != null)
                temp = transform.Find("Sprites").Find("Character001_Body").GetComponent<Renderer>();
            else if (transform.Find("Sprites/PlayerSprite"))
                temp = transform.Find("Sprites").Find("PlayerSprite").GetComponent<Renderer>();
            else temp = null;
        }
        if (colorDictionary.ContainsKey(ColorDatabaseKey))
        {
            temp.material.color = colorDictionary[ColorDatabaseKey];
        }
        else
        {
            temp.material.color = Color.red;
        }

        //temp.material.color = PlayerColor;
        //switch (m_controller.mPlayerIndex)
        //{
        //    case PlayerIndex.One:
        //        temp.material.color = Color.red;
        //        break;
        //    case PlayerIndex.Two:
        //        temp.material.color = Color.blue;
        //        break;
        //    case PlayerIndex.Three:
        //        temp.material.color = Color.magenta;
        //        break;
        //    case PlayerIndex.Four:
        //        temp.material.color = Color.yellow;
        //        break;
        //}

        OriginalSortingOrder = BodyAnimator.GetComponent<SpriteRenderer>().sortingOrder;
        defaultWeapon = GetComponent<EmptyHand>();
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Delay
        // MoveDelayTimer = 0;
        StoredMoveSpeed = movementSpeed;
        StartCoroutine(DelayMovement());
    }

    // Update is called once per frame
    void Update()
    {
        //If game isn't paused
        if (!GameManagerc.Instance.Paused)
        {
            //If I'm not dead, or stunned
            if (!m_status.IsDead && !m_status.IsStunned)
            {
                //if the walking animation isnt running, do everything else
                if (!GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("WalkingMan") && !runningAnimation)
                {
                    if (PlayerIsActive)
                    {
                        if (CheckForDownedKill())
                            return;
                        Quack();
                        CalculateMovement();
                        CheckForPickup();
                        Attack(TriggerReleaseCheck());

                        Special();
                    }

                    //! an ammo text changing for UI, move this to another function then change to sprites/masking later
                    if (heldWeapon)
                    {
                        if (heldWeapon.GetType() == typeof(Gun))
                        {
                            //Debug.Log("test");
                            //_AmmoText.text = heldWeapon.GetComponent<Gun>().m_iAmmo.ToString();
                        }
                        else
                        {
                            //_AmmoText.text = "Infinite Ammo";
                        }
                    }
                    else
                    {
                        //_AmmoText.text = "you punch";
                    }

                }
                else //otherwise set the iskilling to false so it can return the animation to idle
                {
                    runningAnimation = false;
                    //   _rigidBody.velocity = Vector2.zero;
                    GetComponentInChildren<Animator>().SetBool("IsKilling", false);
                }

            }
            else
            {
                //  _rigidBody.velocity = Vector2.zero;
            }
        }
    }
    IEnumerator DelayMovement()
    {
        movementSpeed = 0;
        yield return new WaitForSeconds(StartMoveDelay);
        movementSpeed = StoredMoveSpeed;
    }
    //used for semi auto fire
    bool TriggerReleaseCheck()
    {
        //IF the right trigger is being pressed down
        if (XCI.GetAxis(XboxAxis.RightTrigger, m_controller.mXboxController) > 0)
        {
            return m_bTriggerReleased;  //return trigger released (im assuming this is false by default);
        }
        else
            return m_bTriggerReleased = true; //otherwise return true;

    }


    void FixedUpdate()
    {
        if (!GameManagerc.Instance.Paused)
        {
            //Buggy with XBone controller with high frame rates.
            //GamePad.SetVibration(m_controller.mPlayerIndex , XCI.GetAxis(XboxAxis.LeftTrigger , m_controller.mXboxController) , XCI.GetAxis(XboxAxis.RightTrigger , m_controller.mXboxController));
            //vibrationValue = new Vector2(XCI.GetAxis(XboxAxis.LeftTrigger , m_controller.mXboxController) , XCI.GetAxis(XboxAxis.RightTrigger , m_controller.mXboxController));
            // GamePad.SetVibration(m_controller.mPlayerIndex , vibrationValue.x , vibrationValue.y);

            //  vibrationValue *= 0.99f; //magic numbers.

            if (vibrationValue.magnitude < 0.4f)
            {
                vibrationValue = Vector2.zero;
            }

            //if (_characterController)
            //{
            //    Vector3 gravity = new Vector3(0 , -9.8f , 0);
            //    _characterController.Move(gravity * (1 - Time.fixedDeltaTime * 0.5f));
            //}
        }
    }

    void Special()
    {

        if (XCI.GetAxis(XboxAxis.LeftTrigger, m_controller.mXboxController) > 0.4f)
        {
            GetComponent<BaseAbility>().UseSpecialAbility(true);
        }
        else if (XCI.GetAxis(XboxAxis.LeftTrigger, m_controller.mXboxController) <= 0)
        {
            GetComponent<BaseAbility>().UseSpecialAbility(false);
        }
    }

    Vector3 CheckDeadZone(Vector3 controllerInput, float deadzone)
    {
        //if any of the numbers are below a certain deadzone, they get zeroed.
        Vector3 temp = controllerInput;
        if (temp.magnitude < deadzone)
        {
            temp = Vector3.zero;
        }
        return temp;
    }

    void Quack()
    {
        //AudioSourcePool.transform.localRotation = Quaternion.identity;
        //If Y button down
        if (XCI.GetButtonDown(XboxButton.Y, m_controller.mXboxController))
        {
            foreach (AudioSource item in m_audioSource)
            {
                //look for an audiosource from the pool that isn't active
                if (!item.isPlaying)
                {
                    //set the audiosource to play with the pitch determined by left trigger, break out of the loop.
                    item.pitch = 1 + XCI.GetAxis(XboxAxis.LeftTrigger, m_controller.mXboxController);
                    item.Play();
                    break;
                }
            }
        }

    }

    Vector3 CheckKeyboardInput()
    {
        Vector3 temp = Vector3.zero;
        if (m_controller.mXboxController == XboxController.First)
        {
            if (Input.GetKey(KeyCode.W))
            {
                temp.y = 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                temp.y = -1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                temp.x = -1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                temp.x = 1;
            }
        }
        return temp;
    }
    void CalculateMovement()
    {

        //Quack.

        Vector3 movement = Vector3.zero;
        Vector3 KeyboardMovement = Vector3.zero;
        //Gets the input from the left stick to determine the movement
        movement = new Vector3(XCI.GetAxis(XboxAxis.LeftStickX, m_controller.mXboxController), XCI.GetAxis(XboxAxis.LeftStickY, m_controller.mXboxController));
        KeyboardMovement = CheckKeyboardInput();
        //Vrotation used to determine what way the character to rotate
        Vector3 vrotation = Vector3.zero;
        Vector3 LeftStickRotation = Vector3.zero; // consistently left stick rotation
        m_LeftStickRotation = new Vector2(-GamePad.GetState(m_controller.mPlayerIndex).ThumbSticks.Left.X, GamePad.GetState(m_controller.mPlayerIndex).ThumbSticks.Left.Y);

        if (!m_bStopStickRotation) //IF dont stop the stick rotation, populate the rotation vectors.
        {
            //vrotation = new Vector2(-GamePad.GetState(m_controller.mPlayerIndex).ThumbSticks.Right.X, GamePad.GetState(m_controller.mPlayerIndex).ThumbSticks.Right.Y);

            vrotation = new Vector2(-XCI.GetAxisRaw(XboxAxis.RightStickX, m_controller.mXboxController), XCI.GetAxisRaw(XboxAxis.RightStickY, m_controller.mXboxController));

            LeftStickRotation = new Vector2(-GamePad.GetState(m_controller.mPlayerIndex).ThumbSticks.Left.X, GamePad.GetState(m_controller.mPlayerIndex).ThumbSticks.Left.Y);
        }

        //if im not getting any input from the right stick, make my rotation from the left stick instead
        //if rotation is none and stick rotation is allowed
        if (vrotation == Vector3.zero && !m_bStopStickRotation) //If stopping the stick rotation and normal rotation (right stick) is zeroed;
        {
            //turn off the crosshair
            crosshair.SetActive(false);
            //vrotation = new Vector2(-GamePad.GetState(m_controller.mPlayerIndex).ThumbSticks.Left.X, GamePad.GetState(m_controller.mPlayerIndex).ThumbSticks.Left.Y);

            vrotation = new Vector2(-XCI.GetAxisRaw(XboxAxis.LeftStickX, m_controller.mXboxController), XCI.GetAxisRaw(XboxAxis.LeftStickY, m_controller.mXboxController));

        }
        else
        {
            //If not stopping right stick rotation, turn the crosshair on.

            crosshair.SetActive(!m_bStopStickRotation);

        }

        //Do deadzone calculations on rotation vectors
        vrotation = CheckDeadZone(vrotation, StickDeadZone);
        LeftStickRotation = CheckDeadZone(LeftStickRotation, StickDeadZone);

        //After deadzone checks, if there is still input, and set the character rotation to the vectors direction
        if (vrotation != Vector3.zero)
        {
            //Set the rotation to the Stick rotation
            this.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(vrotation.x, vrotation.y) * Mathf.Rad2Deg);

            //! This makes the feet face the left sticks direction. Quaternions are wierd.
            if (LeftStickRotation.magnitude != 0 && FeetAnimator)
            {
                transform.Find("Sprites").transform.Find("Character001_Feet").transform.rotation = transform.Find("Sprites").transform.Find("Character001_Feet").transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(LeftStickRotation.x, LeftStickRotation.y) * Mathf.Rad2Deg);
                transform.Find("Sprites").transform.Find("Character001_Feet").transform.rotation *= Quaternion.Euler(0, 0, 90);
            }
        }
        else if (KeyboardMovement != Vector3.zero)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(-KeyboardMovement.x, KeyboardMovement.y) * Mathf.Rad2Deg);
        }

        //If im not stunned, add movement to my rigid body's velocity, allowing it to move, scaling it by a movement speed.
        if (!m_status.IsStunned)
        {
            _rigidBody.velocity = (movement + KeyboardMovement) * movementSpeed;
        }

        //animation checks go here
        if (FeetAnimator != null)
        {
            if (movement.magnitude > 0)
            {
                SetMovingAnimators(true); // Sets Body and Feet animator movement bools to true
            }
            else
            {
                SetMovingAnimators(false); // Sets Body and Feet animator movement bools to false
            }
        }
    }

    public void ThrowWeapon(Vector2 movement, Vector2 throwDirection, bool tossWeapon)
    {
        //if holding a weapon and the weapon is active
        if (heldWeapon && heldWeapon.GetComponent<Weapon>().m_bActive)
        {
            BodyAnimator.SetBool("ReverseAnimator", false);
            BodyAnimator.SetFloat("Speed", 1);
            Vector3 GunMountPosition = Vector3.one;
            //drop the weapon 
            heldWeapon.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 0; //? Puts gun layer behinde player layer when it's dropped. 
            //Determine what type of weapon im holding
            if (heldWeapon.GetComponent<Gun>())
            {
                GunMountPosition = (!heldWeapon.GetComponent<Gun>().m_b2Handed) ?/*True*/ weapon1HandedMount.position : /*False*/weapon2HandedMount.position;
            }
            else if (heldWeapon.GetComponent<Melee>())
            {
                GunMountPosition = (!heldWeapon.GetComponent<Melee>().m_b2Handed) ?/*True*/ weapon1HandedMount.position : /*False*/weapon2HandedMount.position;
                heldWeapon.GetComponent<Melee>().m_bAttacking = false;                             //TODO Update this for melee to stop throw through walls
                heldWeapon.GetComponent<Melee>().SetAnimator(null);
            }

            if (/*movement.magnitude == 0 ||*/ !tossWeapon) //if drop weapon
            {
                //Raycast from me to the gun mount position + an arbitrary number. IF I hit something, snap the gun to behind the wall
                RaycastHit2D hit = Physics2D.Raycast(this.transform.position, throwDirection, (this.transform.position - GunMountPosition).magnitude + 0.3f, 1 << LayerMask.NameToLayer("Wall"));
                if (hit)
                {
                    heldWeapon.transform.position = hit.point + (hit.normal * 0.4f);
                }
                Debug.DrawRay(this.transform.position, throwDirection * (this.transform.position - GunMountPosition).magnitude, Color.yellow, 5);

                heldWeapon.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                heldWeapon.transform.Find("Sprite").GetComponent<Collider2D>().enabled = true;
                //drop the weapon. magic number 2.
                heldWeapon.GetComponent<Weapon>().ThrowWeapon(throwDirection * 2);
                heldWeapon.GetComponent<Weapon>().previousOwner = this.gameObject;
                heldWeapon.GetComponent<Weapon>().weaponThrower = this.gameObject;
                //heldWeapon.transform.Find("Sprite").GetComponent<Collider2D>().enabled = false;
                m_bHoldingWeapon = false;

                heldWeapon = null;
                if (BodyAnimator != null)
                    SetHoldingGun(0);

            }
            else //else throw weapon
            {
                //toss it away with force
                RaycastHit2D hit = Physics2D.Raycast(this.transform.position, throwDirection, (this.transform.position - GunMountPosition).magnitude, 1 << LayerMask.NameToLayer("Wall"));
                if (hit)
                {
                    heldWeapon.transform.localPosition = -this.transform.up * 0.2f;
                }
                heldWeapon.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                heldWeapon.transform.Find("Sprite").GetComponent<Collider2D>().enabled = true;
                heldWeapon.GetComponent<Weapon>().ThrowWeapon(throwDirection * throwingForce);
                heldWeapon.GetComponent<Weapon>().previousOwner = this.gameObject;
                heldWeapon.GetComponent<Weapon>().weaponThrower = this.gameObject;
                //heldWeapon.transform.Find("Sprite").GetComponent<Collider2D>().enabled = false;
                m_bHoldingWeapon = false;
                heldWeapon = null;
                if (BodyAnimator != null)
                    SetHoldingGun(0);

            }
            StartCoroutine(WeaponPickUpDelay(.3f));
        }
    }
    void CheckForPickup()
    {
        //both the LB and B button will be used to pickup weapons, pressing these again will determine how the weapon being held will be thrown away.
        //Pressing B button will do a small throw and land just in front of where the player threw it away.
        if (XCI.GetButtonDown(XboxButton.B, m_controller.mXboxController) || Input.GetMouseButtonDown(1))
        {
            Vector2 movement = new Vector3(XCI.GetAxis(XboxAxis.LeftStickX, m_controller.mXboxController), XCI.GetAxis(XboxAxis.LeftStickY, m_controller.mXboxController));
            Vector2 throwDirection = new Vector2(this.transform.up.x, this.transform.up.y);

            PickUpWeaponCheck(movement, throwDirection, false);
        }
        //pressing LB will throw the weapon away at a higher velocity, essentially making a projectile, this throw will be used to stun players.
        if (XCI.GetButtonDown(XboxButton.LeftBumper, m_controller.mXboxController) || (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(1)))
        {
            Vector2 movement = new Vector3(XCI.GetAxis(XboxAxis.LeftStickX, m_controller.mXboxController), XCI.GetAxis(XboxAxis.LeftStickY, m_controller.mXboxController));
            Vector2 throwDirection = new Vector2(this.transform.up.x, this.transform.up.y);

            PickUpWeaponCheck(movement, throwDirection, true);
        }
    }


    bool PickUpWeaponCheck(Vector2 stickMovement, Vector2 throwingDirection, bool tossWeapon)
    {

        Collider2D weaponToPickUp = null;

        Collider2D[] hitCollider = Physics2D.OverlapCircleAll(this.transform.position, 1.0f, /*Pickup layer*/(1 << 12));
        foreach (Collider2D WeaponCollider in hitCollider)
        {
            if (WeaponCollider.GetComponentInParent<Weapon>())//If I find a weapon in the parent
            {
                if (WeaponCollider.GetComponentInParent<Weapon>().gameObject != previousWeapon) //If the wepaon found isn't the previous weapon
                {
                    //if (WeaponCollider.transform.parent.tag == "2hMelee")
                    //{
                    //    weaponToPickUp = WeaponCollider;
                    //    break;
                    //}


                    weaponToPickUp = WeaponCollider;
                    break;

                }
            }
            else if (WeaponCollider.transform.parent.GetComponentInParent<Weapon>())
            {
                if (WeaponCollider.transform.parent.GetComponentInParent<Weapon>().gameObject != previousWeapon)
                {
                    weaponToPickUp = WeaponCollider;
                    break;

                }
            }

            else if (WeaponCollider.GetComponentInChildren<Weapon>())//! Null Check
            {
                if (WeaponCollider.GetComponentInChildren<Weapon>().gameObject != previousWeapon)

                {
                    weaponToPickUp = WeaponCollider;
                    break;
                }
            }
            else if (hitCollider.Length == 1)
            {
                weaponToPickUp = WeaponCollider;
            }
        }

        //If there is a weapon being held, the weapon will be thrown away.
        if (heldWeapon)
        {

            ThrowWeapon(stickMovement, throwingDirection, tossWeapon);                                                 //? ??????????????????

        }

        //if the overlap circle found something, pickup the weapon
        if (weaponToPickUp)
        {
            //Raycast from me
            Vector3 pos = transform.position;
            Vector3 Dir = weaponToPickUp.gameObject.transform.position - transform.position;

            //Quaternion temp = Quaternion.Euler(0,0,Vector3.Angle(transform.position , weaponToPickUp.transform.position));
            //Dir = temp * transform.up;

            RaycastHit2D hitPoint = Physics2D.Raycast(pos, Dir, 1, (1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("FloorGun")));
            //Debug.DrawRay(this.transform.position , Dir , Color.magenta , 5);

            if (hitPoint.transform == null)
            {
                if (weaponToPickUp.GetComponentInParent<Weapon>().transform.root.GetComponent<PlayerStatus>() == null)
                {
                    PickupWeapon(weaponToPickUp);                                                                        //? ???????????????????
                    return true;
                }
                else if (weaponToPickUp.transform.parent.GetComponentInParent<Weapon>().transform.root.GetComponent<PlayerStatus>() == null)
                {
                    PickupWeapon(weaponToPickUp);
                    return true;
                }
            }
            else if (hitPoint.transform.GetComponentInParent<Weapon>())
            {
                if (weaponToPickUp.GetComponentInParent<Weapon>().transform.root.GetComponent<PlayerStatus>() == null)
                {
                    PickupWeapon(weaponToPickUp);
                    return true;
                }
                else if (weaponToPickUp.transform.parent.GetComponentInParent<Weapon>().transform.root.GetComponent<PlayerStatus>() == null)
                {
                    PickupWeapon(weaponToPickUp);
                    return true;
                }
            }
            else
            {
                // Debug.Log("WallBlock");
                //Debug.DrawLine(pos , pos + Dir , Color.red , Mathf.Infinity);
            }

        }
        return false;
    }

    void PickupWeapon(Collider2D hitCollider)
    {
        if (hitCollider.GetComponentInParent<Weapon>() && (hitCollider.transform.parent.tag == "2hMelee" || hitCollider.transform.parent.tag == "1hMelee"))
        {
            if (hitCollider.GetComponentInParent<Weapon>().previousOwner != this.gameObject)
            {
                heldWeapon = hitCollider.transform.parent.GetComponentInParent<Weapon>().gameObject;
                heldWeapon.transform.Find("Sprite").GetComponent<SpriteRenderer>().sortingOrder = 5; //? Puts gun layer infront of player layer when picked up. 
                heldWeapon.transform.Find("Sprite").transform.localPosition = new Vector3(0, 0, 0); //! Resets Shadow on pickup.
                heldWeapon.GetComponent<Weapon>().PlayPickup();
                if (heldWeapon.tag == "2hMelee")
                { //! Sets weapon to spot based on tag.
                    heldWeapon.transform.SetParent(this.gameObject.transform.Find("Sprites").GetChild(0).Find("2HandedMeleeSpot"));
                    hitCollider.gameObject.transform.parent.position = Melee2HandedMount.position; //set position to the weapon mount spot
                    hitCollider.gameObject.transform.parent.rotation = Melee2HandedMount.rotation; //set its rotation

                }
                else
                {       //! if the weapon isn't a 2 handed weapon, mount it to the 1 handed location
                    heldWeapon.transform.SetParent(this.gameObject.transform.Find("Sprites").GetChild(0).Find("1HandedMeleeSpot"));
                    hitCollider.gameObject.transform.parent.position = Melee1HandedMount.position; //set position to the weapon mount spot
                    hitCollider.gameObject.transform.parent.rotation = Melee1HandedMount.rotation; //set its rotation
                }
                heldWeapon.GetComponent<Melee>().SetAnimator(BodyAnimator); // Sets animator for bat to know when swinging.



                Rigidbody2D weaponRigidBody = hitCollider.GetComponentInParent<Rigidbody2D>(); //find its rigidbody in its 

                //weaponRigidBody.simulated = false; 
                //turn off any of its simulation
                weaponRigidBody.bodyType = RigidbodyType2D.Kinematic;
                weaponRigidBody.transform.Find("Sprite").GetComponent<Collider2D>().enabled = false;
                //weaponRigidBody.simulated = false;
                weaponRigidBody.velocity = Vector2.zero; //set any velocity to nothing
                weaponRigidBody.angularVelocity = 0.0f; //set any angular velocity to nothing
                weaponRigidBody.transform.Find("Sprite").GetComponent<Collider2D>().enabled = false;
                previousWeapon = heldWeapon;
                m_bHoldingWeapon = true;
                SetHoldingGun(0); //? Probably un-necessary
                if (BodyAnimator != null)  //! Could make this a function
                {
                    switch (heldWeapon.tag)
                    {
                        case "1hMelee":
                            SetHoldingMelee(1);
                            break;
                        case "2hMelee":
                            SetHoldingMelee(2);
                            break;
                        case "OneHanded":
                            SetHoldingGun(1);
                            break;
                        default:
                            SetHoldingGun(2);
                            break;
                    }
                }
                vibrationValue.y = 0.5f; //vibrate controller for haptic feedback
            }

        }
        else if (hitCollider.GetComponentInParent<Weapon>())
        {
            if (hitCollider.GetComponentInParent<Weapon>().previousOwner != this.gameObject)
            {
                heldWeapon = hitCollider.GetComponentInParent<Weapon>().gameObject;
                heldWeapon.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 5; //? Puts gun layer infront of player layer when picked up. 
                heldWeapon.transform.GetChild(0).transform.localPosition = new Vector3(0, 0, 0); //! Resets Shadow on pickup.
                heldWeapon.GetComponent<Weapon>().PlayPickup();
                heldWeapon.transform.SetParent(this.gameObject.transform);
                //! if the weapon isn't a 2 handed weapon, mount it to the 1 handed location
                if (!hitCollider.transform.parent.gameObject.GetComponent<Gun>().m_b2Handed)
                {
                    hitCollider.gameObject.transform.parent.position = weapon1HandedMount.position; //set position to the weapon mount spot
                    hitCollider.gameObject.transform.parent.rotation = weapon1HandedMount.rotation; //set its rotation
                }
                else //! mount it to the 2handed mounting position
                {
                    hitCollider.gameObject.transform.parent.position = weapon2HandedMount.position; //set position to the weapon mount spot
                    hitCollider.gameObject.transform.parent.rotation = weapon2HandedMount.rotation; //set its rotation
                }
                Rigidbody2D weaponRigidBody = hitCollider.GetComponentInParent<Rigidbody2D>(); //find its rigidbody in its 

                //weaponRigidBody.simulated = false; 
                //turn off any of its simulation
                weaponRigidBody.bodyType = RigidbodyType2D.Kinematic;
                weaponRigidBody.transform.Find("Sprite").GetComponent<Collider2D>().enabled = false;
                //weaponRigidBody.simulated = false;
                weaponRigidBody.velocity = Vector2.zero; //set any velocity to nothing
                weaponRigidBody.angularVelocity = 0.0f; //set any angular velocity to nothing
                weaponRigidBody.transform.Find("Sprite").GetComponent<Collider2D>().enabled = false;
                previousWeapon = heldWeapon;
                m_bHoldingWeapon = true;
                SetHoldingGun(0); //? Probably un-necessary
                if (BodyAnimator != null)
                {
                    switch (heldWeapon.tag)
                    {
                        case "1hMelee":
                            SetHoldingMelee(1);
                            break;
                        case "2hMelee":
                            SetHoldingMelee(2);
                            break;
                        case "OneHanded":
                            SetHoldingGun(1);
                            break;
                        default:
                            SetHoldingGun(2);
                            break;
                    }
                }
                vibrationValue.y = 0.5f; //vibrate controller for haptic feedback
            }
        }
    }
    void Attack(bool TriggerCheck)
    {
        //attacks with weapon in hand, if no weapon, they do a melee punch instead.
        if (XCI.GetAxis(XboxAxis.RightTrigger, m_controller.mXboxController) > 0 || (Input.GetMouseButton(0) && m_controller.mXboxController == XboxController.First))
        {
            if (m_bHoldingWeapon)
            {
                if (BodyAnimator != null) //This check is only if there is a melee weapon.
                {
                    BodyAnimator.SetBool("UnarmedAttack", false);
                    BodyAnimator.SetBool("Attack", true);
                }
                //attack using the weapon im holding. if an attack was done, set a vibration on my controller.
                // Ray2D ray = new Ray2D(this.transform.position, this.transform.up);
                RaycastHit2D hit = Physics2D.Raycast(this.transform.position, this.transform.up, 1f, (1 << 10 | 1 << 11 | 1 << 14));
                if (!hit)
                {
                    if (heldWeapon.GetComponent<Weapon>().Attack(TriggerCheck))
                    {
                        //CameraShake.Instance.ShakeCamera();
                        vibrationValue.x = 0.45f;
                    }
                }
                m_bTriggerReleased = false;
            }
            else
            {
                vibrationValue.x = 0.1f;
                //GamePad.SetVibration(m_controller.mPlayerIndex , vibrationValue.x , vibrationValue.y);
                defaultWeapon.Attack(TriggerCheck);
                if (BodyAnimator != null)
                {
                    BodyAnimator.SetBool("UnarmedAttack", true);
                    BodyAnimator.SetBool("Attack", false);
                }
                //currently doesnt actually do melee attacks. using controller vibration for testing purposes
            }
        }
        else if (BodyAnimator != null)
        {
            BodyAnimator.SetBool("UnarmedAttack", false);
            BodyAnimator.SetBool("Attack", false);
        }
    }

    public void StatusApplied()
    {
        //called outside
        //whenever a status is applied to player (stunned / killed) they drop their weapon
        //
        if (BodyAnimator)
        {
            BodyAnimator.SetBool("UnarmedAttack", false);
            BodyAnimator.SetBool("Moving", false);
            BodyAnimator.SetBool("IsKilling", false);

        }

        if (transform.Find("Colliders"))
            //transform.Find("Colliders").GetComponent<PolygonCollider2D>().enabled = false;
            if (heldWeapon)
            {
                ThrowWeapon(Vector2.zero, this.transform.up, false);
            }
    }

    void OnTriggerStay2D(Collider2D a_collider)
    {
        //if im standing on a stunned player, show a prompt (x) to kill the stunned player
        if (a_collider.tag == "Player")
        {
            PlayerStatus other = a_collider.GetComponentInParent<PlayerStatus>();
            if (other.IsStunned)
            {
                //other.killMePrompt.SetActive(true);
                BodyAnimator.GetComponent<SpriteRenderer>().sortingOrder = 99;
                GameObject killmeprompt = other.killMePrompt;
                Transform[] transforms = new Transform[2];
                transforms[0] = this.transform;
                transforms[1] = other.transform;
                killmeprompt.transform.position = GetAveragePos(transforms);
            }
        }
    }
    void OnTriggerExit2D(Collider2D a_collider)
    {
        //turn off the kill prompt as I am no longer in rang.e
        if (a_collider.tag == "Player")
        {
            a_collider.GetComponentInParent<PlayerStatus>().killMePrompt.SetActive(false);
            BodyAnimator.GetComponent<SpriteRenderer>().sortingOrder = OriginalSortingOrder;
        }

    }
    bool CheckForDownedKill()
    {
        //look for controller input x
        if (XCI.GetButtonDown(XboxButton.X, m_controller.mXboxController) && !m_bInChokeMode)
        {
            //look for any colliders around me (will also hit myself)
            Collider2D[] hitCollider = Physics2D.OverlapCircleAll(this.transform.position, 1.0f, 1 << 16); // Layer 16 == stunned.

            //for every other collider found in the circle, kill them.
            foreach (Collider2D collidersFound in hitCollider)
            {
                if (collidersFound.gameObject != this.gameObject)
                {
                    //Null check
                    if (collidersFound.tag == "Stunned")           //? Never Passes.
                    {
                        if (collidersFound.gameObject.transform.parent.GetComponent<PlayerStatus>().IsStunned)
                        {
                            runningAnimation = true;
                            _rigidBody.velocity = Vector2.zero;
                            BodyAnimator.SetTrigger("HeadSmashPullUp");
                            m_bInChokeMode = true;
                            chokingPlayer = collidersFound.gameObject;
                            //this.GetComponentInChildren<Animator>().SetBool("IsKilling", true);
                            //collidersFound.transform.parent.GetComponent<PlayerStatus>().KillPlayer(this.GetComponent<PlayerStatus>());
                        }
                    }
                }
            }
        }
        else if (m_bInChokeMode) //If in choke mode
        {
            // #Head Smash, #Smash Head, #Choking, #smash,
            m_ChokingTimer.mfTimeToWait = m_fChokeKillTime; //set the time to wait
            PlayerStatus chokingPlayerStatus = chokingPlayer.transform.root.GetComponent<PlayerStatus>(); //get the player status of choking player
            if (chokingPlayerStatus.IsStunned) //if the choking player is still stunned
            {
                this.transform.position = chokingPlayer.transform.root.position; //set my position to their position
                float ZRotation = chokingPlayer.transform.root.rotation.eulerAngles.z - 180;
                this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, ZRotation));
                if (XCI.GetButtonDown(XboxButton.X, m_controller.mXboxController)) //look for X button down
                {
                    BodyAnimator.SetTrigger("HeadSmashSmash"); //Set trigger to do smash
                }
                //Check Animator State
                if (BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("HeadSmash")) //if in head smash state
                {
                    if (!m_bChoked) //check if I applied choking logic already
                    {
                        m_bChoked = true; //set the applied logic to true
                        m_ChokingTimer.CurrentTime += m_fChokedTimeIncrement; //increase the timer
                    }
                }
                else
                {
                    m_bChoked = false; //Logic not applied
                }

                if (m_ChokingTimer.Tick(Time.deltaTime)) //If timer over, kill player
                {
                    chokingPlayerStatus.KillPlayer(this.GetComponent<PlayerStatus>());
                }

                //Do choking timer here.
                if (KillBarContainer.activeSelf)
                {
                    float killOffset = m_ChokingTimer.CurrentTime / m_ChokingTimer.mfTimeToWait * 0.23f;
                    killMask.material.SetTextureOffset("_MainTex", new Vector2(0 - killOffset, 0));
                }
                else
                    KillBarContainer.SetActive(true);
            }
            else //Otherwise if not stunned
            {
                //get out of choke mode, choking player reference is now null
                BodyAnimator.SetTrigger("CancelHeadSmash");
                m_bInChokeMode = false;
                chokingPlayer = null;
            }
            return true;
        }
        else
        {
            KillBarContainer.SetActive(false);
            m_ChokingTimer.CurrentTime = 0;
        }
        return false;
    }

    Vector2 GetAveragePos(Transform[] transforms)
    {
        Vector3 averagePos = Vector2.zero;
        for (int i = 0; i < transforms.Length; ++i)
        {
            averagePos += transforms[i].position;
        }
        return averagePos / transforms.Length;
    }

    public void HideWeapon(bool aHideWeapon)
    {
        if (aHideWeapon)
        {
            if (heldWeapon)
            {
                heldWeapon.GetComponent<Weapon>().m_bActive = false;
            }
        }
        else
        {
            if (heldWeapon)
            {
                heldWeapon.GetComponent<Weapon>().m_bActive = true;
            }
        }
    }

    public void FindSprite()
    {
        if (transform.Find("Sprite/Character001_Body") != null)
            playerSpirte = transform.Find("Sprites").Find("Character001_Body").gameObject;
        else if (transform.Find("Sprites/PlayerSprite"))
            playerSpirte = transform.Find("Sprites").Find("PlayerSprite").gameObject;
    }

    private void SetMovingAnimators(bool Moving)
    {
        FeetAnimator.SetBool("Moving", Moving);
        BodyAnimator.SetBool("Moving", Moving);
    }
    ///<summary>
    ///Amount of hands required to hold GUN. if no weapon, 0. Sets Animators. Not Melee.
    ///</summary>
    private void SetHoldingGun(int HandsOccupied)  // Input the amount of hands needed to hold weapon. If no weapon input 0 for 0 hands.
    {
        if (BodyAnimator)
        {
            switch (HandsOccupied)
            {
                case 0:
                    BodyAnimator.SetBool("HoldingOneHandedGun", false); // Sets animators
                    BodyAnimator.SetBool("HoldingTwoHandedGun", false);
                    BodyAnimator.SetBool("HoldingOneHandedMelee", false);
                    BodyAnimator.SetBool("HoldingTwoHandedMelee", false);
                    m_NoHandsCollider.enabled = true;
                    m_OneHandCollider.enabled = false;
                    m_TwoHandedCollider.enabled = false;
                    break;
                case 1:
                    BodyAnimator.SetBool("HoldingOneHandedGun", true);
                    BodyAnimator.SetBool("HoldingTwoHandedGun", false);
                    BodyAnimator.SetBool("HoldingOneHandedMelee", false);
                    BodyAnimator.SetBool("HoldingTwoHandedMelee", false);
                    m_NoHandsCollider.enabled = false;
                    m_OneHandCollider.enabled = true;
                    m_TwoHandedCollider.enabled = false;
                    break;
                case 2:
                    BodyAnimator.SetBool("HoldingOneHandedGun", false);
                    BodyAnimator.SetBool("HoldingTwoHandedGun", true);
                    BodyAnimator.SetBool("HoldingOneHandedMelee", false);
                    BodyAnimator.SetBool("HoldingTwoHandedMelee", false);
                    m_NoHandsCollider.enabled = false;
                    m_OneHandCollider.enabled = false;
                    m_TwoHandedCollider.enabled = true;
                    break;
                default:
                    break;
            }
        }
    }

    private void SetHoldingMelee(int HandsOccupied)
    {
        if (BodyAnimator)
        {
            switch (HandsOccupied)
            {
                case 0:
                    BodyAnimator.SetBool("HoldingOneHandedGun", false); // Sets animators
                    BodyAnimator.SetBool("HoldingTwoHandedGun", false);
                    BodyAnimator.SetBool("HoldingOneHandedMelee", false);
                    BodyAnimator.SetBool("HoldingTwoHandedMelee", false);
                    m_NoHandsCollider.enabled = true;
                    m_OneHandCollider.enabled = false;                                          //TODO Update Accordingly MELEE
                    m_TwoHandedCollider.enabled = false;
                    break;
                case 1:
                    BodyAnimator.SetBool("HoldingOneHandedGun", false);
                    BodyAnimator.SetBool("HoldingTwoHandedGun", false);
                    BodyAnimator.SetBool("HoldingOneHandedMelee", true);
                    BodyAnimator.SetBool("HoldingTwoHandedMelee", false);
                    m_NoHandsCollider.enabled = true;                                           //TODO Update Accordingly MELEE
                    m_OneHandCollider.enabled = false;
                    m_TwoHandedCollider.enabled = false;
                    break;
                case 2:
                    BodyAnimator.SetBool("HoldingOneHandedGun", false);
                    BodyAnimator.SetBool("HoldingTwoHandedGun", false);
                    BodyAnimator.SetBool("HoldingOneHandedMelee", false);
                    BodyAnimator.SetBool("HoldingTwoHandedMelee", true);
                    m_NoHandsCollider.enabled = true;
                    m_OneHandCollider.enabled = false;                                          //TODO Update Accordingly MELEE
                    m_TwoHandedCollider.enabled = false;
                    break;
                default:
                    break;
            }
        }
    }

    public void MakeCollidersTriggers(bool Trigger)
    {
        if (Trigger == true)
        {
            m_NoHandsCollider.isTrigger = true;
            m_OneHandCollider.isTrigger = true;
            m_TwoHandedCollider.isTrigger = true;
        }
        else
        {
            m_NoHandsCollider.isTrigger = false;
            m_OneHandCollider.isTrigger = false;
            m_TwoHandedCollider.isTrigger = false;
        }
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //_AmmoText = PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_AmmoText.GetComponent<Text>();
    }

    IEnumerator WeaponPickUpDelay(float WaitTime)
    {
        yield return new WaitForSeconds(WaitTime);
        previousWeapon = null;
        yield return null;
    }

    public void SetUIBars(Image a_Mask, GameObject a_barContainer)
    {
        killMask = a_Mask;
        KillBarContainer = a_barContainer;
    }
}
