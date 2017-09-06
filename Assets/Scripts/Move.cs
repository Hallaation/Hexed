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
    CharacterController _characterController;
    bool PlayerIsActive = true; public bool getActive() { return PlayerIsActive; }
    public void setActive(bool Active) { PlayerIsActive = Active; }
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
    public GameObject weaponMount;
    public GameObject fistObject;
    public float movementSpeed = 10.0f;
    public float throwingForce = 100.0f;
    float StoredMoveSpeed;
    //public bool m_b2DMode = true;
    EmptyHand defaultWeapon;
    [HideInInspector]
    public Vector2 vibrationValue;
    [HideInInspector]
    public GameObject playerSpirte;
    float MoveDelayTimer;
    public float StartMoveDelay = 3;
    public Vector3 m_LeftStickRotation;
    public float StickDeadZone = 0.12f;
    private Text _AmmoText;
    // Use this for initialization
    void Awake()
    {
        if (transform.Find("Sprites"))
        {
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
        switch (m_controller.mPlayerIndex)
        {
            case PlayerIndex.One:
                temp.material.color = Color.red;
                break;
            case PlayerIndex.Two:
                temp.material.color = Color.blue;
                break;
            case PlayerIndex.Three:
                temp.material.color = Color.magenta;
                break;
            case PlayerIndex.Four:
                temp.material.color = Color.yellow;
                break;
        }

        defaultWeapon = GetComponent<EmptyHand>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        // Delay
        MoveDelayTimer = 0;
        StoredMoveSpeed = movementSpeed;
        StartCoroutine(DelayMovement());
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_status.IsDead && !m_status.IsStunned)
        {
            //if the walking animation isnt running, do everything else
            if (!GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("WalkingMan") && !runningAnimation)
            {
                if (PlayerIsActive)
                {
                    CalculateMovement();
                    CheckForPickup();
                    Attack(TriggerReleaseCheck());

                    CheckForDownedKill();
                    Special();
                }

                //! an ammo text changing for UI, move this to another function then change to sprites/masking later
                if (heldWeapon)
                {
                    if (heldWeapon.GetComponent<Gun>())
                    {
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

        //Buggy with XBone controller with high frame rates.
        //GamePad.SetVibration(m_controller.mPlayerIndex , XCI.GetAxis(XboxAxis.LeftTrigger , m_controller.mXboxController) , XCI.GetAxis(XboxAxis.RightTrigger , m_controller.mXboxController));
        //vibrationValue = new Vector2(XCI.GetAxis(XboxAxis.LeftTrigger , m_controller.mXboxController) , XCI.GetAxis(XboxAxis.RightTrigger , m_controller.mXboxController));
        // GamePad.SetVibration(m_controller.mPlayerIndex , vibrationValue.x , vibrationValue.y);

        //  vibrationValue *= 0.99f; //magic numbers.

        if (vibrationValue.magnitude < 0.4f)
        {
            vibrationValue = Vector2.zero;
        }

        if (_characterController)
        {
            Vector3 gravity = new Vector3(0, -9.8f, 0);
            _characterController.Move(gravity * (1 - Time.fixedDeltaTime * 0.5f));
        }
    }

    void Special()
    {

        if (XCI.GetAxis(XboxAxis.LeftTrigger, m_controller.mXboxController) > 0)
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

    void CalculateMovement()
    {
        Vector3 movement = Vector3.zero;
        
        //Gets the input from the left stick to determine the movement
        movement = new Vector3(XCI.GetAxis(XboxAxis.LeftStickX, m_controller.mXboxController), XCI.GetAxis(XboxAxis.LeftStickY, m_controller.mXboxController));
        //Vrotation used to determine what way the character to rotate
        Vector3 vrotation = Vector3.zero;
        Vector3 LeftStickRotation = Vector3.zero; // consistently left stick rotation
        m_LeftStickRotation = new Vector2(-GamePad.GetState(m_controller.mPlayerIndex).ThumbSticks.Left.X, GamePad.GetState(m_controller.mPlayerIndex).ThumbSticks.Left.Y);

        // FeetAnimator.transform.rotation = new Quaternion(0, 0, temp.z, temp.w);
        if (!m_bStopStickRotation)
        {
            //vrotation = new Vector2(-GamePad.GetState(m_controller.mPlayerIndex).ThumbSticks.Right.X, GamePad.GetState(m_controller.mPlayerIndex).ThumbSticks.Right.Y);

            vrotation = new Vector2(-XCI.GetAxisRaw(XboxAxis.RightStickX, m_controller.mXboxController), XCI.GetAxisRaw(XboxAxis.RightStickY, m_controller.mXboxController));

            LeftStickRotation = new Vector2(-GamePad.GetState(m_controller.mPlayerIndex).ThumbSticks.Left.X, GamePad.GetState(m_controller.mPlayerIndex).ThumbSticks.Left.Y);
        }
        //if im not getting any input from the right stick, make my rotation from the left stick instead
        //if rotation is none and stick rotation is allowed
        if (vrotation == Vector3.zero && !m_bStopStickRotation)
        {
            //turn off the crosshair
            crosshair.SetActive(false);
            //vrotation = new Vector2(-GamePad.GetState(m_controller.mPlayerIndex).ThumbSticks.Left.X, GamePad.GetState(m_controller.mPlayerIndex).ThumbSticks.Left.Y);

            vrotation = new Vector2(-XCI.GetAxisRaw(XboxAxis.LeftStickX, m_controller.mXboxController), XCI.GetAxisRaw(XboxAxis.LeftStickY, m_controller.mXboxController));
            
        }
        else
        {
            //turn on crosshair if im stopping
            crosshair.SetActive(!m_bStopStickRotation);
        }

        //Do deadzone calculations
        vrotation = CheckDeadZone(vrotation, StickDeadZone);
        LeftStickRotation = CheckDeadZone(LeftStickRotation, StickDeadZone);
        
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

        if (!_characterController && !m_status.IsStunned)
        {
            //this.transform.position += movement * movementSpeed * Time.deltaTime;
            //_rigidBody.AddForce(movement * movementSpeed * Time.deltaTime , ForceMode2D.Impulse);
            _rigidBody.velocity = movement * movementSpeed;

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

    public void ThrowMyWeapon(Vector2 movement, Vector2 throwDirection, bool tossWeapon)
    {
        if (heldWeapon && heldWeapon.GetComponent<Weapon>().m_bActive)
        {
            //throw the weapon away
            heldWeapon.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 0; //? Puts gun layer behinde player layer when it's dropped. 
            if (/*movement.magnitude == 0 ||*/ !tossWeapon)
            {

                //drop the weapon. magic number 2.
                heldWeapon.GetComponent<Weapon>().throwWeapon(throwDirection * 2);
                heldWeapon.GetComponent<Weapon>().previousOwner = this.gameObject;
                m_bHoldingWeapon = false;

                heldWeapon = null;
                if (BodyAnimator != null)
                    SetHoldingGun(0);

            }
            else
            {
                //toss it away with force
                heldWeapon.GetComponent<Weapon>().throwWeapon(throwDirection * throwingForce);
                heldWeapon.GetComponent<Weapon>().previousOwner = this.gameObject;
                m_bHoldingWeapon = false;
                heldWeapon = null;
                if (BodyAnimator != null)
                    SetHoldingGun(0);

            }
           
        }
    }
    void CheckForPickup()
    {
        //both the LB and B button will be used to pickup weapons, pressing these again will determine how the weapon being held will be thrown away.
        //Pressing B button will do a small throw and land just in front of where the player threw it away.
        if (XCI.GetButtonDown(XboxButton.B, m_controller.mXboxController))
        {
            Vector2 movement = new Vector3(XCI.GetAxis(XboxAxis.LeftStickX, m_controller.mXboxController), XCI.GetAxis(XboxAxis.LeftStickY, m_controller.mXboxController));
            Vector2 throwDirection = new Vector2(this.transform.up.x, this.transform.up.y);

            PickUpWeaponCheck(movement, throwDirection, false);
        }
        //pressing LB will throw the weapon away at a higher velocity, essentially making a projectile, this throw will be used to stun players.
        if (XCI.GetButtonDown(XboxButton.LeftBumper, m_controller.mXboxController))
        {
            Vector2 movement = new Vector3(XCI.GetAxis(XboxAxis.LeftStickX, m_controller.mXboxController), XCI.GetAxis(XboxAxis.LeftStickY, m_controller.mXboxController));
            Vector2 throwDirection = new Vector2(this.transform.up.x, this.transform.up.y);

            PickUpWeaponCheck(movement, throwDirection, true);
        }
    }


    bool PickUpWeaponCheck(Vector2 stickMovement, Vector2 throwingDirection, bool tossWeapon)
    {

        Collider2D hitCollider = Physics2D.OverlapCircle(this.transform.position, 1.0f, (1 << 12));

        //If there is a weapon being held, the weapon will be thrown away.
        if (heldWeapon)
        {
            
            ThrowMyWeapon(stickMovement, throwingDirection, tossWeapon);
            
        }
        //if the overlap circle found something, pickup the weapon
        if (hitCollider)
        {
            Vector3 pos = transform.position;
            Vector3 Dir = hitCollider.gameObject.transform.position - transform.position;


            RaycastHit2D hitPoint = Physics2D.Raycast(pos, Dir.normalized, 1, 1 << LayerMask.NameToLayer("Wall"));

            if (hitPoint.transform == null)
            {
                if (hitCollider.transform.parent.parent == null)

                {
                    PickUpWeaon(hitCollider);
                    return true;
                }
            }
            else if (hitCollider.transform.parent.parent == null)
            {
                Debug.Log("WallBlock");
                Debug.DrawLine(pos, pos + Dir, Color.red, Mathf.Infinity);
            }

        }
        return false;
    }

    void PickUpWeaon(Collider2D hitCollider)
    {
        heldWeapon = hitCollider.transform.parent.gameObject;
        heldWeapon.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 4; //? Puts gun layer infront of player layer when picked up. 
        hitCollider.gameObject.transform.parent.SetParent(this.transform);
        hitCollider.gameObject.transform.parent.position = weaponMount.transform.position; //set position to the weapon mount spot
        hitCollider.gameObject.transform.parent.rotation = weaponMount.transform.rotation; //set its rotation
        Rigidbody2D weaponRigidBody = hitCollider.transform.parent.GetComponent<Rigidbody2D>(); //find its rigidbody in its parent
        weaponRigidBody.simulated = false; //turn off any of its simulation
        weaponRigidBody.velocity = Vector2.zero; //set any velocity to nothing
        weaponRigidBody.angularVelocity = 0.0f; //set any angular velocity to nothing

        m_bHoldingWeapon = true;
        SetHoldingGun(0); //? Probably un-necessary
        if (BodyAnimator != null)
        {
            if (heldWeapon.tag == "OneHanded")
            {
                BodyAnimator.SetBool("HoldingOneHandedGun", true);
            }
            BodyAnimator.SetBool("HoldingTwoHandedGun", true);
        }
        vibrationValue.y = 0.5f; //vibrate controller for haptic feedback
    }
    void Attack(bool TriggerCheck)
    {
        //attacks with weapon in hand, if no weapon, they do a melee punch instead.
        if (XCI.GetAxis(XboxAxis.RightTrigger, m_controller.mXboxController) > 0)
        {
            if (m_bHoldingWeapon)
            {
                if (BodyAnimator != null)
                    BodyAnimator.SetBool("UnarmedAttack", false);
                //attack using the weapon im holding. if an attack was done, set a vibration on my controller.
                if (heldWeapon.GetComponent<Weapon>().Attack(TriggerCheck))
                {
                    vibrationValue.x = 0.45f;

                }

                m_bTriggerReleased = false;
            }
            else
            {
                vibrationValue.x = 0.1f;
                //GamePad.SetVibration(m_controller.mPlayerIndex , vibrationValue.x , vibrationValue.y);
                defaultWeapon.Attack(TriggerCheck);
                if (BodyAnimator != null)
                    BodyAnimator.SetBool("UnarmedAttack", true);
                //currently doesnt actually do melee attacks. using controller vibration for testing purposes
            }
        }
        else if (BodyAnimator != null)
            BodyAnimator.SetBool("UnarmedAttack", false);
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
        transform.Find("StunnedCollider").GetComponent<PolygonCollider2D>().enabled = false;
        if (heldWeapon)
        {
            ThrowMyWeapon(Vector2.zero, this.transform.up, false);
        }
    }

    void OnTriggerStay2D(Collider2D a_collider)
    {
        //if im standing on a stunned player, show a prompt (x) to kill the stunned player
        if (a_collider.tag == "Player")
        {
            PlayerStatus other = a_collider.GetComponent<PlayerStatus>();
            if (other.IsStunned)
            {
                other.killMePrompt.SetActive(true);
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
            a_collider.GetComponent<PlayerStatus>().killMePrompt.SetActive(false);

        }

    }
    void CheckForDownedKill()
    {
        //look for controller input x
        if (XCI.GetButtonDown(XboxButton.X, m_controller.mXboxController))
        {
            //look for any colliders around me (will also hit myself)
            Collider2D[] hitCollider = Physics2D.OverlapCircleAll(this.transform.position, 1.0f, 1 << 8);

            //for every other collider found in the circle, kill them.
            foreach (Collider2D collidersFound in hitCollider)
            {
                if (collidersFound.gameObject != this.gameObject)
                {
                    //Null check
                    if (collidersFound.GetComponent<PlayerStatus>())
                    {
                        if (collidersFound.GetComponent<PlayerStatus>().IsStunned)
                        {
                            runningAnimation = true;
                            _rigidBody.velocity = Vector2.zero;
                            this.GetComponentInChildren<Animator>().SetBool("IsKilling", true);
                            collidersFound.GetComponent<PlayerStatus>().KillPlayer(this.GetComponent<PlayerStatus>());
                        }
                    }
                }
            }
        }
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
                    break;
                case 1:
                    BodyAnimator.SetBool("HoldingOneHandedGun", true);
                    BodyAnimator.SetBool("HoldingTwoHandedGun", false);
                    break;
                case 2:
                    BodyAnimator.SetBool("HoldingOneHandedGun", false);
                    BodyAnimator.SetBool("HoldingTwoHandedGun", true);
                    break;
                default:
                    break;
            }
        }
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _AmmoText = PlayerUIArray.Instance.playerElements[GetComponent<ControllerSetter>().m_playerNumber].m_AmmoText.GetComponent<Text>();
    }
}
