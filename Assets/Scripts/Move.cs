using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XboxCtrlrInput;
using XInputDotNetPure;
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
    
    public GameObject weaponMount;
    public GameObject fistObject;
    public float movementSpeed = 10.0f;
    public float throwingForce = 100.0f;
    float StoredMoveSpeed;
    public bool m_b2DMode = true;
    EmptyHand defaultWeapon;
    [HideInInspector]
    public Vector2 vibrationValue;
    [HideInInspector]
    public GameObject playerSpirte;
    float MoveDelayTimer;
    public float StartMoveDelay = 3;
    // Use this for initialization
    void Start()
    {
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
        // Delay
        MoveDelayTimer = 0;
        StoredMoveSpeed = movementSpeed;
       StartCoroutine( DelayMovement());
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
                /*
                //! an ammo text changing for UI, move this to another function then change to sprites/masking later
                if (heldWeapon)
                {
                    if (heldWeapon.GetComponent<Gun>())
                    {
                   //     _AmmoText.text = heldWeapon.GetComponent<Gun>().m_iAmmo.ToString();
                    }
                    else
                    {
                     //   _AmmoText.text = "Infinite Ammo";
                    }
                }
                else
                {
              //      _AmmoText.text = "you punch";
                }
                */
            }
            else //otherwise set the iskilling to false so it can return the animation to idle
            {
                runningAnimation = false;
             //   _rigidBody.velocity = Vector2.zero;
                GetComponentInChildren<Animator>().SetBool("IsKilling" , false);
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
    bool TriggerReleaseCheck()
    {
        if (XCI.GetAxis(XboxAxis.RightTrigger , m_controller.mXboxController) > 0)
        {
            return m_bTriggerReleased;
        }
        else
            m_bTriggerReleased = true;
        return m_bTriggerReleased;
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
            Vector3 gravity = new Vector3(0 , -9.8f , 0);
            _characterController.Move(gravity * (1 - Time.fixedDeltaTime * 0.5f));
        }
    }

    void Special()
    {

        if (XCI.GetAxis(XboxAxis.LeftTrigger , m_controller.mXboxController) > 0)
        {
            GetComponent<BaseAbility>().UseSpecialAbility(true);
        }
        else if (XCI.GetAxis(XboxAxis.LeftTrigger , m_controller.mXboxController) <= 0)
        {
            GetComponent<BaseAbility>().UseSpecialAbility(false);
        }
    }
    Vector3 CheckDeadZone(Vector3 controllerInput , float deadzone)
    {
        //if any of the numbers are below a certain deadzone, they get zeroed.
        Vector3 temp = controllerInput;
        if (Mathf.Abs(temp.x) <= deadzone)
        {
            temp.x = 0;
        }
        if (Mathf.Abs(temp.y) <= deadzone)
        {
            temp.y = 0;
        }
        if (Mathf.Abs(temp.z) <= deadzone)
        {
            temp.z = 0;
        }
        return temp;
    }
    void CalculateMovement()
    {

        Vector3 movement = Vector3.zero;
        //get our raw input
        //moves on X,Y when in 2D mode
        //otherwise moves on Z,X in 3D
        //condition ? true : false
        movement = (!m_b2DMode) ? new Vector3(XCI.GetAxis(XboxAxis.LeftStickX , m_controller.mXboxController) , 0 , XCI.GetAxis(XboxAxis.LeftStickY , m_controller.mXboxController)) : new Vector3(XCI.GetAxis(XboxAxis.LeftStickX , m_controller.mXboxController) , XCI.GetAxis(XboxAxis.LeftStickY , m_controller.mXboxController));

        Vector3 vrotation = Vector3.zero;
        if (!m_bStopStickRotation)
        {
            vrotation = new Vector2(GamePad.GetState(m_controller.mPlayerIndex).ThumbSticks.Right.X , GamePad.GetState(m_controller.mPlayerIndex).ThumbSticks.Right.Y);
        }
        //if im not getting any input from the right stick, make my rotation from the left stick instead
        //if rotation is none and stick rotation is allowed
        if (vrotation == Vector3.zero && !m_bStopStickRotation)
        {
            //turn off the crosshair
            crosshair.SetActive(false);
            vrotation = new Vector2(GamePad.GetState(m_controller.mPlayerIndex).ThumbSticks.Left.X , GamePad.GetState(m_controller.mPlayerIndex).ThumbSticks.Left.Y);
        }
        else
        {
            //turn on crosshair if im stopping
            crosshair.SetActive(!m_bStopStickRotation);
        }

        //check the deadzone for rotation
        vrotation = CheckDeadZone(vrotation , 0.1f);

        if (vrotation.magnitude != 0)
        {
            //ternary operator asking if 2dmode, does rotation based on which mode
            //if 2D, the rotation twists around the Z axis
            //otherwise 3D, the rotation twists around Y axis;
            this.transform.rotation = (!m_b2DMode) ? Quaternion.Euler(0 , Mathf.Atan2(vrotation.x , vrotation.y) * Mathf.Rad2Deg , 0) : this.transform.rotation = Quaternion.Euler(0 , 0 , Mathf.Atan2(-vrotation.x , vrotation.y) * Mathf.Rad2Deg);
        }

        if (!_characterController && !m_status.IsStunned)
        {
            //this.transform.position += movement * movementSpeed * Time.deltaTime;
            //_rigidBody.AddForce(movement * movementSpeed * Time.deltaTime , ForceMode2D.Impulse);
            _rigidBody.velocity = movement * movementSpeed;
        }
        else
        {
            _characterController.Move(movement * movementSpeed * Time.deltaTime);
        }
    }

    public void ThrowMyWeapon(Vector2 movement , Vector2 throwDirection , bool tossWeapon)
    {
        if (heldWeapon && heldWeapon.GetComponent<Weapon>().m_bActive)
        {
            //throw the weapon away
            if (/*movement.magnitude == 0 ||*/ !tossWeapon)
            {
                //drop the weapon. magic number 2.
                heldWeapon.GetComponent<Weapon>().throwWeapon(throwDirection * 2);
                heldWeapon.GetComponent<Weapon>().previousOwner = this.gameObject;
                m_bHoldingWeapon = false;
                heldWeapon = null;

            }
            else
            {
                //toss it away with force
                heldWeapon.GetComponent<Weapon>().throwWeapon(throwDirection * throwingForce);
                heldWeapon.GetComponent<Weapon>().previousOwner = this.gameObject;
                m_bHoldingWeapon = false;
                heldWeapon = null;
            }
        }
    }
    void CheckForPickup()
    {
        //both the LB and B button will be used to pickup weapons, pressing these again will determine how the weapon being held will be thrown away.
        //Pressing B button will do a small throw and land just in front of where the player threw it away.
        if (XCI.GetButtonDown(XboxButton.B , m_controller.mXboxController))
        {
            Vector2 movement = new Vector3(XCI.GetAxis(XboxAxis.LeftStickX , m_controller.mXboxController) , XCI.GetAxis(XboxAxis.LeftStickY , m_controller.mXboxController));
            Vector2 throwDirection = new Vector2(this.transform.up.x , this.transform.up.y);

            PickUpWeaponCheck(movement , throwDirection , false);
        }
        //pressing LB will throw the weapon away at a higher velocity, essentially making a projectile, this throw will be used to stun players.
        if (XCI.GetButtonDown(XboxButton.LeftBumper , m_controller.mXboxController))
        {
            Vector2 movement = new Vector3(XCI.GetAxis(XboxAxis.LeftStickX , m_controller.mXboxController) , XCI.GetAxis(XboxAxis.LeftStickY , m_controller.mXboxController));
            Vector2 throwDirection = new Vector2(this.transform.up.x , this.transform.up.y);

            PickUpWeaponCheck(movement , throwDirection , true);
        }
    }


    bool PickUpWeaponCheck(Vector2 stickMovement , Vector2 throwingDirection , bool tossWeapon)
    {

        Collider2D hitCollider = Physics2D.OverlapCircle(this.transform.position , 1.0f , (1 << 12));

        //If there is a weapon being held, the weapon will be thrown away.
        if (heldWeapon)
        {
            ThrowMyWeapon(stickMovement , throwingDirection , tossWeapon);
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
        hitCollider.gameObject.transform.parent.SetParent(this.transform);
        hitCollider.gameObject.transform.parent.position = weaponMount.transform.position; //set position to the weapon mount spot
        hitCollider.gameObject.transform.parent.rotation = weaponMount.transform.rotation; //set its rotation
        Rigidbody2D weaponRigidBody = hitCollider.transform.parent.GetComponent<Rigidbody2D>(); //find its rigidbody in its parent
        weaponRigidBody.simulated = false; //turn off any of its simulation
        weaponRigidBody.velocity = Vector2.zero; //set any velocity to nothing
        weaponRigidBody.angularVelocity = 0.0f; //set any angular velocity to nothing

        m_bHoldingWeapon = true;
        vibrationValue.y = 0.5f; //vibrate controller for haptic feedback
    }
    void Attack(bool TriggerCheck)
    {
        //attacks with weapon in hand, if no weapon, they do a melee punch instead.
        if (XCI.GetAxis(XboxAxis.RightTrigger , m_controller.mXboxController) > 0)
        {
            if (m_bHoldingWeapon)
            {
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
                //currently doesnt actually do melee attacks. using controller vibration for testing purposes
            }
        }
    }

    public void StatusApplied()
    {
        //called outside
        //whenever a status is applied to player (stunned / killed) they drop their weapon
        if (heldWeapon)
        {
            ThrowMyWeapon(Vector2.zero , this.transform.up , false);
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
        if (XCI.GetButtonDown(XboxButton.X , m_controller.mXboxController))
        {
            //look for any colliders around me (will also hit myself)
            Collider2D[] hitCollider = Physics2D.OverlapCircleAll(this.transform.position , 1.0f , 1 << 8);

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
                            this.GetComponentInChildren<Animator>().SetBool("IsKilling" , true);
                            collidersFound.GetComponent<PlayerStatus>().KillPlayer();
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
}
