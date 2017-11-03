using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[System.Serializable]
public class Gun : Weapon
{
    [Space]
    [Header("Empty Clip Audio")]
    public AudioClip m_EmptyClipAudio;
    [Range(0, 1)]
    public float EmptyVolume = 1.0f;

    [Space]
    [Header("Gun Specific")]

    private ParticleSystem EmptyClipPS;
    public bool m_bAutomaticGun;
    public bool m_bBurstFire;
    public bool m_b2Handed = false;
    public int m_iAmmo = 30;
    private int m_iMaxAmmo;
    public float m_fBulletImpactKnockback = 0.1f;
    public GameObject bullet;
    [SerializeField]
    private float m_fSpreadJitter = 1.0f;
    public float m_fSpreadIncrease = 0.1f;
    public float m_fMaxJitter = 1.0f;

    public float m_fBulletSpawnOffSet = -.1f; //? Don't put below 0.009?
    public float m_fFiringForce = 20.0f;
    // public bool m_bAutomatic;
    [Header("BurstFire Settings")]
    public int m_iBurstFire;
    public float m_fTimeBetweenBurstShots;

    private ParticleSystem MuzzelFlash;

    private Timer spreadTimer;
    private bool TickTimer = false;
    //overriding startup, all weapons should do this
    public override void StartUp()
    {
        if (transform.childCount > 3)
            MuzzelFlash = transform.GetChild(3).GetComponent<ParticleSystem>();

        //the max jitter is determined in the editor, this is to simply remember the maximum jitter desired
        //m_fMaxJitter = m_fSpreadJitter;
        m_fSpreadJitter = 0;
        m_iMaxAmmo = m_iAmmo;
        //set the spread jitter to 0
        //make a timer with a wait time of 0.2f, around the human average reaction speed
        spreadTimer = new Timer(0.2f);
        EmptyClipPS = this.transform.GetChild(this.transform.childCount - 1).GetComponent<ParticleSystem>();

    }

    public override bool Attack(bool trigger)
    {
        //player will only be allowed to attack once a shot is ready AND they have ammo. This is used to determine the time to wait in between shots.
        if (shotReady && m_iAmmo != 0 && m_bActive && (trigger == true || m_bAutomaticGun == true))
        {
            //Set the timer to start ticking, whenever the attack is called, reset the time to 0 so it will continue to tick and incease the spread jitter;
            spreadTimer.CurrentTime = 0;
            TickTimer = true;

            //If the burstfire variable is greater than 1 do a burst fire
            if (m_iBurstFire > 1)
            {
                StartCoroutine(BurstFire());
                return true;
            }
            //otherwise check if there is enough ammo and fire a bullet
            else if (m_iAmmo > 0)
            {
                FireBullet();
                return true;
            }
        }
        else if (m_iAmmo == 0 && shotReady && (trigger == true || m_bAutomaticGun == true))
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.clip = m_EmptyClipAudio;
                m_AudioSource.volume = clipVolume;
            }
            m_AudioSource.PlayOneShot(m_EmptyClipAudio, clipVolume);
            shotReady = false;
            EmptyClipPS.Play();
            //TODO Add an empty chamber sound effect
            //TODO Add gun click sound effect here.
        }
        return false;
    }

    IEnumerator BurstFire()
    {
        int i = m_iBurstFire;

        TimerBetweenFiring.CurrentTime = 0;
        shotReady = false;

        //while there is enough ammo and the gun is being held, fire bullets
        while (i > 0 && m_iAmmo > 0 && transform.parent != null)
        {
            FireBullet();
            i--;
            TimerBetweenFiring.CurrentTime = 0;
            //wait for seconds of specified time to loop again
            yield return new WaitForSeconds(m_fTimeBetweenBurstShots);

        }
        //set the timer to 0, resetting shotready
        TimerBetweenFiring.CurrentTime = 0;
        yield return null;
    }


    void FireBullet()
    {
        //Get the owner and set the vibration
        Move owner;
        if ((owner = this.transform.root.GetComponent<Move>()))
        {
            owner.vibrationValue.x = 0.8f;
            owner.vibrationValue.y = 0.8f;
        }
        //Whenever fire bullet mis called, Make the bullet prefab, get the damage from the player that is holding this gun
        GameObject FiredBullet = Instantiate(bullet, this.transform.parent.position + this.transform.parent.up * m_fBulletSpawnOffSet, this.transform.rotation);
        if(transform.Find("MuzzelFlash"))
        {
            transform.Find("MuzzelFlash").GetChild(0).GetComponent<ParticleSystem>().Play();

        }



        Bullet bulletComponent = FiredBullet.GetComponent<Bullet>();
        bulletComponent.bulletOwner = GetComponentInParent<PlayerStatus>(); //copy stuff over
        bulletComponent.m_iDamage = this.m_iDamage;
        bulletComponent.m_bGiveIFrames = m_bGivePlayersIFrames;
        bulletComponent.m_fBulletImpactKnockBack = m_fBulletImpactKnockback;

        //Make a quaternion on the forward vector to determine the bullet spread jitter and set the bullet's rotation to the jitter
        FiredBullet.transform.rotation = this.transform.parent.rotation * Quaternion.Euler(Vector3.forward * m_fSpreadJitter * Random.Range(-1.0f, 1.0f));
        //apply an initial force to the bullet's rigidbody based on what direction the bullet is facing,
        Rigidbody2D FiredBulletRigidBody = FiredBullet.GetComponent<Rigidbody2D>();
        FiredBulletRigidBody.AddForce(FiredBullet.transform.up * m_fFiringForce, ForceMode2D.Impulse);
        GameManagerc.Instance._rbPausers.Add(FiredBullet.AddComponent<RigidbodyPauser>());

        Ray2D ray = new Ray2D(this.transform.parent.position, FiredBullet.transform.up);
        Debug.DrawRay(ray.origin, ray.direction * m_fBulletSpawnOffSet * 1.5f, Colors.Azure, 5);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, m_fBulletSpawnOffSet * 1.5f, LayerMask.GetMask("Wall", "Door", "Glass"));
        if (hit)
        {
            foreach (var item in hit.transform.GetComponents<IHitByBullet>())
            {
                item.HitByBullet(FiredBulletRigidBody.velocity, hit.point);
            }
            FiredBullet.transform.position = hit.point;
            FiredBulletRigidBody.velocity = Vector2.zero;
            bulletComponent.PlayParticles(hit.point);
        }

        //! Based on the bullet's velocity vector, get a rotation from it and change the bullets rotation to represent the velocity vector;
        Vector2 dir = FiredBulletRigidBody.velocity;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        FiredBullet.GetComponent<Transform>().rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        //!If I do have a Muzzel flash particle, play them.
        if (MuzzelFlash != null)
            MuzzelFlash.Play();
        shotReady = false; //set shot ready to false to enable the timer to tick.
        //Copy.CopyComponent(m_AudioSource, FiredBullet);
        m_AudioSource.pitch = (m_bRandomizePitch) ? Random.Range(0.9f, 1.1f) : 1;

        m_AudioSource.Play();
        //AudioSource bulletSource = FiredBullet.GetComponent<AudioSource>();
        //bulletSource.clip = m_AudioClip;
        //bulletSource.volume = clipVolume;
        //bulletSource.pitch = Random.Range(0.3f, 0.9f);
        //bulletSource.Play();
        //m_AudioSource.PlayOneShot(m_AudioClip, clipVolume);

        --m_iAmmo; //deduct the ammo
    }

    //anything the weapon should so specifically should be put here
    public override void DoWeaponThings()
    {
        //! DO WEAPON THINGS
        if (TickTimer)
        {
            //whenever the timer is started.
            //if the timer is ticking, increase the spread jitter. I
            if (!spreadTimer.Tick(Time.deltaTime))
            {
                //if the current jitter is below the max jitter, continue to increase
                if (m_fSpreadJitter < m_fMaxJitter)
                {
                    m_fSpreadJitter += m_fSpreadIncrease * Time.deltaTime;
                }
                else
                {
                    //otherwise once the max is reached, set it to the max
                    m_fSpreadJitter = m_fMaxJitter;
                }
            }
            else
            {
                //whenever the timer has finished, jtiter is set to 0 and the timer will stop ticking until the trigger has been pressed again.
                m_fSpreadJitter = 0;
                TickTimer = false;
            }
        }
    }


}
