using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //base weapon class
    private Sprite m_DefaultSprite;
    public Sprite m_HeldSprite = null;
    public float m_fTimeBetweenShots = 0.01f; 
    public float m_iDamage;

    public bool m_bGivePlayersIFrames = false; 
    public float ThrowHitKnockbackScale;

    [Space]
    [Header("ShadowRelated")]
    public float MaxShadow; //How high the weapon sprite will move up
    public float MinShadow; //How low the weapon sprite will move down
    public float ShadowGrowthSpeed; //how fast the weapon will move from the min/max
    [Space]
    protected Timer TimerBetweenFiring;
    protected bool shotReady = true;
    protected bool stunPlayer = true;
    Rigidbody2D _rigidbody;
    public bool m_bActive = true;
    //[HideInInspector]
    public GameObject previousOwner; //previous owner used to make sure when the weapon is thrown, it doesnt stun the thrower.
    public GameObject weaponThrower; //Used to store the thrower;
    private SpriteRenderer WeaponSpriteRenderer; //The sprite rendere of the weapon sprite
    private Transform weaponSpriteTransform; //The transform of the weapon's sprite 
    public bool m_bMoveWeaponSpriteUp; //A bool used to determine if the weapon sprite will move up or down

    [Space]
    [Header("Weapon Attack Audio")]
    public AudioClip m_AudioClip;
    public bool m_bRandomizePitch = true;
    [Range(0 , 1)]
    public float clipVolume = 1;

    [Space]
    [Header("Pickup Audio")] 
    public AudioClip PickupAudio;
    [Range(0 , 1)]
    public float pickupVolume = 1;

    [Space]
    [Header("Weapon Drop Audio")]
    public AudioClip DropAudioClip;
    [Range(0 , 1)]
    public float DropAudioVolume = 1;
    
    [Space]
    [Header("Weapon Throw Audio")]
    public AudioClip ThrowAudioClip;
    [Range(0 , 1)]
    public float ThrowAudioVolume = 1;

    [Space]
    [Header("Thrown hit Audio")]
    public AudioClip ThrowHitAudio;
    public bool m_bRandomizeThrowHitPitch = true;
    [Range(0 , 1)]
    public float ThrowHitAudioVolume = 1;
    private AudioSource hitPlayerAudioSource;
    protected bool m_bPlayedAudio = false; //A fallback incase audio doesn't work, mostly a major fallback for the melee guns.
    //[Range(-2, 2)]
    //public float clipPitch;
    protected AudioSource m_AudioSource;

    // Use this for initialization
    void Start()
    {
        m_AudioSource = this.gameObject.AddComponent<AudioSource>();
        m_AudioSource.outputAudioMixerGroup = (Resources.Load("AudioMixer/SFXAudio") as GameObject).GetComponent<AudioSource>().outputAudioMixerGroup;
        //m_AudioSource.outputAudioMixerGroup = (Resources.Load("AudioMixer/SFXAudio") as  AudioSource).outputAudioMixerGroup;
        m_AudioSource.playOnAwake = false;
        m_AudioSource.clip = m_AudioClip;
        m_AudioSource.volume = clipVolume;
        m_AudioSource.spatialBlend =  1;

        hitPlayerAudioSource = this.gameObject.AddComponent<AudioSource>();
        hitPlayerAudioSource.outputAudioMixerGroup = (Resources.Load("AudioMixer/SFXAudio") as GameObject).GetComponent<AudioSource>().outputAudioMixerGroup;
        //hitPlayerAudioSource.outputAudioMixerGroup = (Resources.Load("AudioMixer/SFXAudio") as  AudioSource).outputAudioMixerGroup;
        hitPlayerAudioSource.playOnAwake = false;
        hitPlayerAudioSource.spatialBlend =  1;
        //m_AudioSource = AudioManager.RequestAudioSource(m_AudioClip, clipVolume, clipPitch);
        _rigidbody = GetComponent<Rigidbody2D>();
        TimerBetweenFiring = new Timer(m_fTimeBetweenShots);
        if (this.transform.childCount > 2 && tag != "Player")
        {
            WeaponSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            //Debug.Log(WeaponSprite.sprite);
            m_DefaultSprite = WeaponSpriteRenderer.sprite;

            weaponSpriteTransform = transform.GetChild(0).GetComponent<Transform>();
        }
        StartUp();
    }

    // Update is called once per frame
    void Update()
    {
        if (!transform.parent) // If weapon is not held, Grow and shrink shadow.
        {
            #region
            m_bActive = true;
            if (WeaponSpriteRenderer)
            {
                if (WeaponSpriteRenderer.sprite != m_DefaultSprite)
                {
                    WeaponSpriteRenderer.sprite = m_DefaultSprite;
                }
            }
            //If my weapon has more than 2 (the shadow object is the third)

            if (this.transform.childCount > 2 && tag != "Player" && _rigidbody.velocity.magnitude < .1f)
            {

                //turn the sprite renderer on
                if (transform.GetChild(2).GetComponent<SpriteRenderer>().enabled == false)
                {
                    transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = true;
                    transform.GetChild(0).transform.localPosition = new Vector3(0, 0, 0);
                    m_bMoveWeaponSpriteUp = true;
                }
                //If i want to move the weapon sprite up.
                if (m_bMoveWeaponSpriteUp && _rigidbody.velocity.magnitude < .3f)
                {


                    //move the weapon sprite up by the amount specified by ShadowGrowthSpeed
                    weaponSpriteTransform.localPosition += new Vector3(0, 0, Time.deltaTime * ShadowGrowthSpeed);
                    //once I have reached the maximum allowed
                    if (weaponSpriteTransform.localPosition.z >= MaxShadow)
                    {
                        //Snap the weapon's sprite transform to the max location
                        weaponSpriteTransform.localPosition = new Vector3(weaponSpriteTransform.localPosition.x, weaponSpriteTransform.localPosition.y, MaxShadow); 
                         m_bMoveWeaponSpriteUp = false;
                        //set the bool to false so the weapon will move down instead
                    }
                }
                else
                {
                    //! everything above but the opposite
                    weaponSpriteTransform.localPosition -= new Vector3(0, 0, Time.deltaTime * ShadowGrowthSpeed);
                    if (weaponSpriteTransform.localPosition.z <= MinShadow)
                    {
                        weaponSpriteTransform.localPosition = new Vector3(weaponSpriteTransform.localPosition.x, weaponSpriteTransform.localPosition.y, MinShadow);
                        m_bMoveWeaponSpriteUp = true;
                    }
                }
            }
            #endregion
        }
        //If my transform has a parent (its being held by a player)
        else if (transform.parent)
        {
            if (m_HeldSprite != null)
            {
                WeaponSpriteRenderer.sprite = m_HeldSprite;
            }
            //find the shadow sprite renderer
            if (this.transform.childCount > 2 && tag != "Player")
            {
                //turn the shadow's sprite renderer off
                transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = false;
            }
        }


        //if the weapon is active
        if (m_bActive)
        {
            //do weapon things a virtual function, in case any weapons need to do anything extra
            DoWeaponThings();
            if (!GetComponent<Move>())
                GetComponentInChildren<Renderer>().material.color = Color.white;

        }
        else
        {
            if (!GetComponent<Move>())
                GetComponentInChildren<Renderer>().material.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

        }
        //wait for next shot, ticks the timer until it is ready for the next shot
        WaitForNextShot();

    }

    void WaitForNextShot()
    {
        //once the player has shot a bullet, the timer will start to tick until the desired time. Until the desired time hasn't reached, the player cannot shoot
        if (!shotReady)
        {
            if (TimerBetweenFiring.mfTimeToWait != m_fTimeBetweenShots)
            {
                TimerBetweenFiring = new Timer(m_fTimeBetweenShots);
            }
            if (TimerBetweenFiring.Tick(Time.deltaTime))
            {
                shotReady = true;
            }
        }
    }
    public void DropWeapon()
    {
        //dropping the weapon will turn the physics back on
        GetComponent<Rigidbody2D>().simulated = true;
        this.transform.SetParent(null);                                                
        GetComponent<Rigidbody2D>().angularVelocity = 50.0f;
        m_AudioSource.PlayOneShot(DropAudioClip , DropAudioVolume);
    }
    /// <summary>
    /// Takes in a velocity vector, this will determine how fast it will be thrown out of the player's hand. Low velocity means it can be just dropped.
    /// </summary>
    /// <param name="velocity"></param>
    public void ThrowWeapon(Vector2 velocity)
    {
       
        //turn the physics back on set its parent to null, and apply the velocity. apply an angular velocity for it to spin.
        GetComponent<Rigidbody2D>().simulated = true;
        this.transform.SetParent(null);
        GetComponent<Rigidbody2D>().AddForce(velocity, ForceMode2D.Impulse);
        if (velocity.magnitude > 50)
        {
            GetComponent<Rigidbody2D>().angularVelocity = 600.0f;
            m_AudioSource.PlayOneShot(ThrowAudioClip , ThrowAudioVolume);
        }
        else
        {
            GetComponent<Rigidbody2D>().angularVelocity = 100.0f;
            m_AudioSource.PlayOneShot(DropAudioClip , DropAudioVolume);
        }
        //GetComponent<Rigidbody2D>().angularVelocity = angularVelocity;

        StartCoroutine(SetFalseOwner());
    }
    //virtual functions
    public virtual bool Attack(bool trigger)
    {
        return false;

    }

    public virtual void DoWeaponThings() { }
    public virtual void StartUp() { }

    void OnTriggerEnter2D(Collider2D a_collider)
    {
        if (stunPlayer)
        {
            //if it enters a trigger (another player in this case") the hit player gets stunned. calls the status applied to drop their weapon.
            if (GetComponent<Rigidbody2D>().velocity.magnitude >= 10 && a_collider.tag == "Player" && a_collider.GetComponentInParent<PlayerStatus>().gameObject != weaponThrower)
            {

                if (a_collider.GetComponentInParent<PlayerStatus>().IsStunned == false)
                {
                    a_collider.GetComponentInParent<PlayerStatus>().StunPlayer(_rigidbody.velocity * ThrowHitKnockbackScale);
                    a_collider.GetComponentInParent<Move>().StatusApplied();
                    hitPlayerAudioSource.clip = ThrowHitAudio;
                    hitPlayerAudioSource.volume = ThrowHitAudioVolume;
                    hitPlayerAudioSource.pitch = (m_bRandomizeThrowHitPitch) ? Random.Range(0.9f , 1.1f) : 1;
                    hitPlayerAudioSource.Play();
                }
            }
        }
    }

    IEnumerator SetFalseOwner()
    {
        yield return new WaitForEndOfFrame();
        previousOwner = null;

    }

    public void PlayPickup()
    {
        m_AudioSource.PlayOneShot(PickupAudio , pickupVolume);
    }
}
