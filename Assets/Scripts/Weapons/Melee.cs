using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Weapon
{
    [Space]
    [Header("MeleeVariables")]
    public float OnHitFlinchTime = .3f;
    public bool Attacking;
    public bool m_b2Handed;
    public float DurationOfAttack;
    private Animator BodyAnimator; public void SetAnimator(Animator AnimatorToSet) { BodyAnimator = AnimatorToSet; }
    [Space]

    [Header("Melee Hit Audio")]
    public AudioClip hitAudioClip;
    public bool m_bRandomizeHitAudio = true;
    [Range(0 , 1)]
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
            if (Attacking == true && other.tag == "Player" && other.transform.root != this.transform.root && other.transform.root.GetComponent<PlayerStatus>().m_bStunned == false)
            {
                if (BodyAnimator != null)
                {
                    RaycastHit2D hit = Physics2D.Raycast(this.transform.position, other.transform.position - transform.position.normalized, (this.transform.position - transform.parent.position).magnitude + 0.3f, 1 << LayerMask.NameToLayer("Wall"));
                    if (hit)
                    {

                    }
                    //other.transform.parent.GetComponentInParent<PlayerStatus>().HitPlayer(this, false);
                    other.transform.parent.GetComponentInParent<PlayerStatus>().StunPlayer(transform.right * KnockBack);        //! Uses transform right instead of transform up due to using the bats right rather then players up

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
            other.transform.parent.GetComponentInParent<PlayerStatus>().StunPlayer(transform.right * KnockBack);        //! Uses transform right instead of transform up due to using the bats right rather then players up
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
            if (Attacking == true && other.tag == "Player" && other.transform.root != this.transform.root && other.transform.root.GetComponent<PlayerStatus>().m_bStunned == false)
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

        if (shotReady)
        {
            
            //BoxCollider2D MeleeHitBox = transform.parent.Find("Punch").GetComponent<BoxCollider2D>();
            if (m_bActive)
            {
                
                shotReady = false;
                return true;
            }

            //if (MeleeHitBox.IsTouchingLayers(1 << 8)) // If Punch Hitbox is touching PLayer layer.
            //{

            //    StartCoroutine(AttackDuration());

            //    //Debug.Log("Punch");

            //    Collider2D[] Overlap = Physics2D.OverlapBoxAll(MeleeHitBox.transform.position, MeleeHitBox.size, MeleeHitBox.transform.eulerAngles.z, 1 << 8); //An Overlap collider with the punch hitbox. There is probably a better way. 

            //int TotalCollisions = Overlap.Length;
            //for (int i = 0; i < TotalCollisions; ++i)
            //{
            //    if (Overlap[i].transform.tag == "Player" && Overlap[i].transform != this.transform) // If hit another player
            //    {

            //        bool HitPlayerFirst = true;
            //        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, (MeleeHitBox.transform.position.y + MeleeHitBox.size.y), 1 << LayerMask.NameToLayer("Wall"));
            //        if (hit.collider != null) // If hit a wall, check to see it hits the wall before the player if it does, stop the check here.
            //        {
            //            float DistanceToPlayer = Vector3.Distance(transform.position, Overlap[i].transform.position);
            //            float DistanceToWall = Vector3.Distance(transform.position, hit.point);
            //            if (DistanceToWall < DistanceToPlayer) // If Hit wall first.
            //            {
            //                HitPlayerFirst = false;
            //                //Debug.Log("HitWallFirst");
            //            }
            //            //Debug.Log("HitPlayerFirst");
            //        }
            //        if (HitPlayerFirst && Overlap[i].transform.parent)// If wall check returns player first. Punch as normal.
            //        {
            //            if (Overlap[i].transform.parent.GetComponentInParent<PlayerStatus>().IsStunned == false)
            //            {
            //                PlayerStatus hitPlayer = Overlap[i].transform.parent.GetComponentInParent<PlayerStatus>();
            //                hitPlayer.StunPlayer(transform.up * KnockBack);
            //            }
            //        }
            //        }
            //    }

        }
        
        return false;
    }

    //  RaycastHit2D[] results;


    //Vector3 center = PunchHitBox.transform.postion + item.center;
    //Vector3 radius = Item.radius;

    //Collider[] allOverlappingColliders = Physics.OverlapSphere(center, radius);


    //Ray2D ray = new Ray2D(this.transform.position, this.transform.up);
    //RaycastHit2D hit = Physics2D.Raycast(ray.origin , ray.direction,  1.5f, 1 << 8);
    //if (hit)
    //{
    //    if (hit.transform != this.transform)
    //    {
    //        Debug.Log("Test");
    //        temp.Play();

    //        hit.transform.GetComponent<PlayerStatus>().TimesPunched++;
    //        Debug.Log(hit.transform.GetComponent<PlayerStatus>().TimesPunched);
    //        if (hit.transform.GetComponent<PlayerStatus>().TimesPunched >= 3)
    //        {
    //            hit.transform.GetComponent<PlayerStatus>().StunPlayer(this.transform.up * KnockBack * 10);
    //            hit.transform.GetComponent<Move>().StatusApplied();
    //        }

    //        return true;
    //    }
    //}

    //Debug.DrawRay(ray.origin , ray.direction);



    public override void DoWeaponThings()
    {
        PreviousVelocity = GetComponent<Rigidbody2D>().velocity;
        if (BodyAnimator)
            if (BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("TwoHandedMeleeRightAttack")
                || BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("TwoHandedMeleeLeftAttack")
                || BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("OneHandedMeleeAttack"))
            {
                Attacking = true;
            }
        else
            {
                Attacking = false;
            }
    }
   

    IEnumerator AttackDuration()
    {

        yield return new WaitForSeconds(DurationOfAttack);
        Attacking = false;
        yield return null;
    }
}