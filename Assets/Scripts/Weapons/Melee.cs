using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Weapon
{
    [Space]
    [Header("MeleeVariables")]
    public float m_fHitFlinchTime = .3f;
    public bool m_bAttacking;
    public bool m_b2Handed;
    public bool ReverseAnimation;

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
            if(m_bAttacking == true && other.tag == "Wall")
            {
                float AnimationTime = BodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                BodyAnimator.SetFloat("Speed", -.5f);
                if (BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("TwoHandedMeleeLeftAttack"))
                {
                    BodyAnimator.Play("TwoHandedMeleeLeftAttack", 0, AnimationTime);
                    ReverseAnimation = true;
                }
                else if(BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("TwoHandedMeleeRightAttack"))
                {
                    BodyAnimator.Play("TwoHandedMeleeRightAttack", 0, AnimationTime);
                    ReverseAnimation = true;
                }
                else
                {
                    BodyAnimator.Play("OneHandedMeleeAttack", 0, AnimationTime);
                    ReverseAnimation = true;
                }
            }
           else if (m_bAttacking == true && other.tag == "Player" && other.transform.root != this.transform.root && other.transform.root.GetComponent<PlayerStatus>().m_bStunned == false)
            {
                if (BodyAnimator != null)
                {
                    RaycastHit2D hit = Physics2D.Raycast(this.transform.position, other.transform.position - transform.position.normalized, (this.transform.position - transform.parent.position).magnitude + 0.3f, 1 << LayerMask.NameToLayer("Wall"));
                    if (hit)
                    {

                    }
                    else
                    {
                        m_AudioSource.Play();
                        //other.transform.parent.GetComponentInParent<PlayerStatus>().HitPlayer(this, false);
                        other.transform.parent.GetComponentInParent<PlayerStatus>().StunPlayer(transform.right * KnockBack);        //! Uses transform right instead of transform up due to using the bats right rather then players up

                        Debug.Log("BatEnterStun");
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
                    other.transform.root.GetComponentInParent<PlayerStatus>().StunPlayer(transform.right * KnockBack);        //! Uses transform right instead of transform up due to using the bats right rather then players up
                }
            }
            //Find every hit by melee interface and call its function
            foreach (IHitByMelee item in other.GetComponents<IHitByMelee>())
            {
                item.HitByMelee(this, null);
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        //If this transform's root is the player and the collided root isn't this weapon's root
        if (this.transform.root.tag == "Player" && other.transform.root != this.transform.root)
        {
            if (m_bAttacking == true && other.tag == "Wall")
            {
                float AnimationTime = BodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                BodyAnimator.SetFloat("Speed", -.5f);
                if (BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("TwoHandedMeleeLeftAttack"))
                {
                    BodyAnimator.Play("TwoHandedMeleeLeftAttack", 0, AnimationTime);
                    ReverseAnimation = true;
                }
                else if (BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("TwoHandedMeleeRightAttack"))
                {
                    BodyAnimator.Play("TwoHandedMeleeRightAttack", 0, AnimationTime);
                    ReverseAnimation = true;
                }
                else
                {
                    BodyAnimator.Play("OneHandedMeleeAttack", 0, AnimationTime);
                }
            }
            //check if attacking and check if its a valid hit (on another player and the other player isn't stunned)
           else if (m_bAttacking == true && other.tag == "Player" && other.transform.root != this.transform.root && other.transform.root.GetComponent<PlayerStatus>().m_bStunned == false)
            {
                if (BodyAnimator != null) //Null check on body animator
                {
                    //Do raycast that does nothing else
                    RaycastHit2D hit = Physics2D.Raycast(this.transform.position, other.transform.position - transform.position.normalized, (this.transform.position - transform.parent.position).magnitude + 0.3f, 1 << LayerMask.NameToLayer("Wall"));
                    if (hit)
                    {

                    }
                    else
                    {
                        other.transform.parent.GetComponentInParent<PlayerStatus>().HitPlayer(this, false); //hit player
                       // other.transform.parent.GetComponentInParent<PlayerStatus>().StunPlayer(transform.right * KnockBack);        //! Uses transform right instead of transform up due to using the bats right rather then players up
                        Debug.Log("BatEnterStun");
                    }
                }
                //Find every hitbymelee interface and call its function.
                if (other.GetComponent<IHitByMelee>() != null)
                {
                    foreach (IHitByMelee item in other.GetComponents<IHitByMelee>())
                    {
                        item.HitByMelee(this, null);
                    }
                }
            }
        }
        else if (GetComponent<Rigidbody2D>().velocity.magnitude > 10) //Velocity check on my RigidBody
        {
            if (other.transform.root.GetComponentInParent<PlayerStatus>()) //If the other(hit object) has a playerstatus
            {
                if (GetComponent<Rigidbody2D>().velocity.magnitude >= 10 && other.tag == "Player" && other.GetComponentInParent<PlayerStatus>().gameObject != weaponThrower) 
                    //check to see if the velocity is still valid and the collided object isn't the thower
                {
                    //Stun the player
                    other.transform.root.GetComponentInParent<PlayerStatus>().StunPlayer(transform.right * KnockBack);        //! Uses transform right instead of transform up due to using the bats right rather then players up
                }
            }
            //Find every hitbymelee interface and call its function.
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



    public override void DoWeaponThings()
    {
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
            m_AudioSource.Play();
            //reset audio
        }
        else if (!m_bAttacking)
        {
            m_bPlayedAudio = false;
        }
       
    }

    

}