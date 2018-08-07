using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Weapon
{
    [Space]
    [Header("MeleeVariables")]

    public bool m_Stun = true;
    public float m_fHitFlinchTime = .3f;
    public bool m_bAttacking;
    public bool m_b2Handed;
    bool ReverseAnimation;
    bool ThrownHit = false;
    public int ThrownDamage = 1;
    private Animator BodyAnimator;
    public void SetAnimator(Animator AnimatorToSet) { BodyAnimator = AnimatorToSet; }

    [Space]

    [Header("Melee Hit Audio")]
    public AudioClip hitAudioClip;
    public bool m_bRandomizeHitAudio = true;
    [Range(0, 1)]
    public float hitAudioVolume = 1;
    Vector3 PreviousVelocity = Vector3.one; public Vector3 GetPreviousVelocity() { return PreviousVelocity; }

    public override void StartUp()
    {

    }
    // Use this for initialization


    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other)
    {
        if (this.transform.root.tag == "Player" && other.transform.root != this.transform.root)
        {
            if (m_bAttacking == true && other.tag == "Wall")
            {
                #region
                //float AnimationTime = BodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                //BodyAnimator.SetFloat("Speed", -.5f);
                //if (BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("TwoHandedMeleeLeftAttack"))
                //{
                //    BodyAnimator.Play("TwoHandedMeleeLeftAttack", 0, AnimationTime);
                //    ReverseAnimation = true;
                //}
                //else if(BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("TwoHandedMeleeRightAttack"))
                //{
                //    BodyAnimator.Play("TwoHandedMeleeRightAttack", 0, AnimationTime);
                //    ReverseAnimation = true;
                //}
                //else
                //{
                //    BodyAnimator.Play("OneHandedMeleeAttack", 0, AnimationTime);
                //    ReverseAnimation = true;
                //}
                #endregion
            }
            else if (m_bAttacking == true && other.tag == "Player" && other.transform.root != this.transform.root && other.transform.root.GetComponent<PlayerStatus>().m_bStunned == false)
            {
                if (BodyAnimator != null)
                {
                    float length = (this.transform.position - other.transform.position).magnitude;
                    Vector3 Direction = (other.transform.position - transform.position).normalized;
                    RaycastHit2D hit = Physics2D.Raycast(this.transform.position, Direction, length + 2, 1 << LayerMask.NameToLayer("Wall"));
                    if (hit)
                    {
                        Vector2 Position = new Vector2(transform.position.x, transform.position.y);
                        Vector2 OtherPosition = new Vector2(other.transform.position.x, other.transform.position.y);
                        if ((hit.point - Position).magnitude < (OtherPosition - Position).magnitude) // if RayHit Wall is closer then the other player.
                        {
                        }
                        else
                        {
                            HitPlayerStuff(other, false);
                        }
                    }
                    else
                    {
                        HitPlayerStuff(other, false);
                        //other.transform.parent.GetComponentInParent<PlayerStatus>().HitPlayer(this, false);
                        //! Uses transform right instead of transform up due to using the bats right rather then players 

                    }
                }
                //Find every hitbymelee interface and call its function
                if (other.GetComponent<IHitByMelee>() != null)
                {
                    foreach (IHitByMelee item in other.GetComponents<IHitByMelee>())
                    {
                        item.HitByMelee(this, null);
                    }
                }
            }
        }
        //Velocity check
        else if (GetComponent<Rigidbody2D>().velocity.magnitude > 10)
        {
            //Check if the other has a playerstatus to ensure it is a player
            if (other.transform.root.GetComponentInParent<PlayerStatus>())
            {
                //Check my RB velocity's magnitude, if hit another player and the other player isn't the weapon thrower
                if (GetComponent<Rigidbody2D>().velocity.magnitude >= 10 && other.tag == "Player" && other.GetComponentInParent<PlayerStatus>().gameObject != weaponThrower)
                {

                    //stun the player
                    if (!other.GetComponentInParent<PlayerStatus>().IsStunned)
                    {
                        HitPlayerStuff(other, true);      //! Uses transform right instead of transform up due to using the bats right rather then players 
                        //Play the hit audio
                        hitPlayerAudioSource.clip = ThrowHitAudio;
                        hitPlayerAudioSource.volume = ThrowHitAudioVolume;
                        hitPlayerAudioSource.pitch = (m_bRandomizeThrowHitPitch) ? Random.Range(0.9f, 1.1f) : 1;
                        hitPlayerAudioSource.Play();
                    }
                }
            }
            //Find every hit by melee interface and call its function
            foreach (IHitByMelee item in other.GetComponents<IHitByMelee>())
            {
                item.HitByMelee(this, null);
            }
        }
        if (other.GetComponent<IHitByMelee>() != null && m_bAttacking)
        {
            foreach (IHitByMelee item in other.GetComponents<IHitByMelee>())
            {
                item.HitByMelee(this, null);
            }
        }

    }


    void OnTriggerStay2D(Collider2D other)
    {
        if (this.transform.root.tag == "Player" && other.transform.root != this.transform.root)
        {
            if (m_bAttacking == true && other.tag == "Wall")
            {
                //float AnimationTime = BodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                //BodyAnimator.SetFloat("Speed", -.5f);
                //if (BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("TwoHandedMeleeLeftAttack"))
                //{
                //    BodyAnimator.Play("TwoHandedMeleeLeftAttack", 0, AnimationTime);
                //    ReverseAnimation = true;
                //}
                //else if(BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("TwoHandedMeleeRightAttack"))
                //{
                //    BodyAnimator.Play("TwoHandedMeleeRightAttack", 0, AnimationTime);
                //    ReverseAnimation = true;
                //}
                //else
                //{
                //    BodyAnimator.Play("OneHandedMeleeAttack", 0, AnimationTime);
                //    ReverseAnimation = true;
                //}
            }
            else if (m_bAttacking == true && other.tag == "Player" && other.transform.root != this.transform.root && other.transform.root.GetComponent<PlayerStatus>().m_bStunned == false)
            {
                if (BodyAnimator != null)
                {
                    float length = (this.transform.position - other.transform.position).magnitude;
                    Vector3 Direction = (other.transform.position - transform.position).normalized;
                    RaycastHit2D hit = Physics2D.Raycast(this.transform.position, Direction, length + 2, 1 << LayerMask.NameToLayer("Wall"));
                    if (hit)
                    {

                        Vector2 Position = new Vector2(transform.position.x, transform.position.y);
                        Vector2 OtherPosition = new Vector2(other.transform.position.x, other.transform.position.y);
                        if ((hit.point - Position).magnitude < (OtherPosition - Position).magnitude) // if RayHit Wall is closer then the other player.
                        {
                        }
                        else
                        {
                            HitPlayerStuff(other, false);
                        }
                    }
                    else
                    {
                        HitPlayerStuff(other, false);
                        //other.transform.parent.GetComponentInParent<PlayerStatus>().HitPlayer(this, false);
                        //! Uses transform right instead of transform up due to using the bats right rather then players up
                    }
                }
                //Find every hitbymelee interface and call its function
                if (other.GetComponent<IHitByMelee>() != null)
                {
                    foreach (IHitByMelee item in other.GetComponents<IHitByMelee>())
                    {
                        item.HitByMelee(this, null);
                    }
                }
            }
        }

        //Velocity check
        else if (GetComponent<Rigidbody2D>().velocity.magnitude > 10)
        {
            //Check if the other has a playerstatus to ensure it is a player
            if (other.transform.root.GetComponentInParent<PlayerStatus>())
            {
                PlayerStatus hitPlayer;
                //Check my RB velocity's magnitude, if hit another player and the other player isn't the weapon thrower
                if (GetComponent<Rigidbody2D>().velocity.magnitude >= 10 && other.tag == "Player" && (hitPlayer = other.GetComponentInParent<PlayerStatus>()).gameObject != weaponThrower)
                {
                    //stun the player
                    if (!hitPlayer.IsStunned)
                    {
                        HitPlayerStuff(other, true);        //! Uses transform right instead of transform up due to using the bats right rather then players up
                    }
                }
            }
            //Find every hit by melee interface and call its function
            foreach (IHitByMelee item in other.GetComponents<IHitByMelee>())
            {
                item.HitByMelee(this, null);
            }
        }
    }

    public override bool Attack(bool trigger)
    {
        //All of this does nothing.
        if (shotReady)
        {
            //BoxCollider2D MeleeHitBox = transform.parent.Find("Punch").GetComponent<BoxCollider2D>();
            if (m_bActive)
            {
                shotReady = false;
                return true;
            }
        }
        return false;
    }


    void HitPlayerStuff(Collider2D other, bool thrown) //called whenever a player is hit
    {
        PlayerStatus OtherPlayerStatus = other.transform.root.GetComponent<PlayerStatus>();
        if (m_Stun == true)
        {
            OtherPlayerStatus.StunPlayer(transform.right * KnockBack);
        }


        if (thrown)
        {
            if (ThrownHit == false)
            {
                OtherPlayerStatus.HitPlayer(ThrownDamage, this.transform.root.GetComponent<PlayerStatus>(), false);
                ThrownHit = true;
                transform.GetComponent<Rigidbody2D>().velocity = -PreviousVelocity * .2f;

            }
        }
        else if (m_iDamage > 0)
        {
            OtherPlayerStatus.HitPlayer(m_iDamage, this.transform.root.GetComponent<PlayerStatus>(), false);
        }
        if (other.transform.root.GetComponent<PlayerStatus>().m_iHealth <= .1f)
        {
            OtherPlayerStatus.KillPlayer(this.transform.root.GetComponent<PlayerStatus>());
            OtherPlayerStatus.transform.rotation = Quaternion.Euler(0,0, Mathf.Atan2(this.transform.root.up.x, -this.transform.root.up.y) * Mathf.Rad2Deg);
        }
        //Play hit by melee sound effect.
        if (OtherPlayerStatus.GetComponentInChildren<IHitByMelee>() != null)
        {
            foreach (IHitByMelee item in OtherPlayerStatus.GetComponentsInChildren<IHitByMelee>())
            {
                item.HitByMelee(this, null);
            }
        }
        // m_AudioSource.Play();


    }
    public override void DoWeaponThings()
    {
        if (GetComponent<Rigidbody2D>().velocity.magnitude < 10)
        {
            ThrownHit = false;
        }
        PreviousVelocity = GetComponent<Rigidbody2D>().velocity;
        if (BodyAnimator)
            if (BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("TwoHandedMeleeRightAttack")
                || BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("TwoHandedMeleeLeftAttack")
                || BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("OneHandedMeleeAttack"))
            {
                m_bAttacking = true;

            }
            else
            {
                m_bAttacking = false;
            }
        if (transform.root.tag == "Player")
        {
            if (ReverseAnimation == true)
            {
                if (BodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < .1f)
                {
                    BodyAnimator.SetBool("ReverseAnimator", true);
                    BodyAnimator.SetFloat("Speed", 1);
                    ReverseAnimation = false;
                }
            }
            else if (BodyAnimator != null)
            {

                BodyAnimator.SetBool("ReverseAnimator", false);
            }
        }
        else
        {
            ReverseAnimation = false;
        }
        if (m_bAttacking && !m_bPlayedAudio)
        {
            m_bPlayedAudio = true;
            m_AudioSource.pitch = (m_bRandomizePitch) ? Random.Range(0.9f, 1.1f) : 1;
            m_AudioSource.Play();
            //reset audio
        }
        else if (!m_bAttacking)
        {
            m_bPlayedAudio = false;
        }

    }

    public override void Reset()
    {
        base.Reset();
    }
}
    