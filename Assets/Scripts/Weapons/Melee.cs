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
            if (m_bAttacking == true && other.tag == "Player" && other.transform.root != this.transform.root && other.transform.root.GetComponent<PlayerStatus>().m_bStunned == false)
            {
                if (BodyAnimator != null)
                {
                    RaycastHit2D hit = Physics2D.Raycast(this.transform.position, other.transform.position - transform.position.normalized, (this.transform.position - transform.parent.position).magnitude + 0.3f, 1 << LayerMask.NameToLayer("Wall"));
                    if (hit)
                    {

                    }
                    m_AudioSource.Play();
                    //other.transform.parent.GetComponentInParent<PlayerStatus>().HitPlayer(this, false);
                    other.transform.parent.GetComponentInParent<PlayerStatus>().StunPlayer(transform.right * ThrowHitKnockBack);        //! Uses transform right instead of transform up due to using the bats right rather then players up

                    Debug.Log("BatEnterStun");
                }



                if (other.GetComponent<IHitByMelee>() != null)
                {
                    foreach (IHitByMelee item in other.GetComponents<IHitByMelee>())
                    {
                        item.HitByMelee(this, null);
                    }
                }
            }
        }
        //

        else if (GetComponent<Rigidbody2D>().velocity.magnitude > 20)
        {
            other.transform.parent.GetComponentInParent<PlayerStatus>().StunPlayer(transform.right * ThrowHitKnockBack);        //! Uses transform right instead of transform up due to using the bats right rather then players up
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
            if (m_bAttacking == true && other.tag == "Player" && other.transform.root != this.transform.root && other.transform.root.GetComponent<PlayerStatus>().m_bStunned == false)
            {
                if (BodyAnimator != null)
                {
                    RaycastHit2D hit = Physics2D.Raycast(this.transform.position, other.transform.position - transform.position.normalized, (this.transform.position - transform.parent.position).magnitude + 0.3f, 1 << LayerMask.NameToLayer("Wall"));
                    if (hit)
                    {

                    }
                    other.transform.parent.GetComponentInParent<PlayerStatus>().HitPlayer(this, false);
                    // other.transform.parent.GetComponentInParent<PlayerStatus>().StunPlayer(transform.right * KnockBack);        //! Uses transform right instead of transform up due to using the bats right rather then players up

                    Debug.Log("BatEnterStun");
                }



                if (other.GetComponent<IHitByMelee>() != null)
                {
                    foreach (IHitByMelee item in other.GetComponents<IHitByMelee>())
                    {
                        item.HitByMelee(this, null);
                    }
                }
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