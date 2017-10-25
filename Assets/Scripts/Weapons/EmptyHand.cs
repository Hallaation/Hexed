using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EmptyHand : Weapon
{
    [Space]
    [Header("Melee Impact Sound")]
    public AudioClip audioClip;
    public bool m_bRandomizeHitPitch = true;
    [Range(0, 1)]
    public float m_clipVolume = 1;

    public float PunchFlinchTime = .3f;
    private int PunchKBForce = 50000;
    private Animator m_BodyAnimator;
    private bool m_bAttacking;
    public int m_PunchesToKnockOut = 2;
    private int previousAnimatorState;
    private int InitialState;
    private Move _moveClass;
    public Animator BodyAnimator { get { return m_BodyAnimator; } set { m_BodyAnimator = value; } }

    public override void StartUp()
    {
        //this.gameObject.name = "Player + " + GetComponent<ControllerSetter>().m_playerNumber;
        BodyAnimator = this.transform.Find("Sprites").GetChild(0).GetComponent<Animator>();
        previousAnimatorState = BodyAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        InitialState = previousAnimatorState;
        BodyAnimator.SetBool("Moving", true);
        //Debug.Log(BodyAnimator.GetCurrentAnimatorStateInfo(0));
        stunPlayer = false;
        _moveClass = this.GetComponent<Move>();
    }

    public override bool Attack(bool trigger)
    {
        if (shotReady)
        {
            {
            }
            shotReady = false;
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
        }

        return false;

    }


    public override void DoWeaponThings()
    {
        if (!_moveClass.heldWeapon)
        {
            if (BodyAnimator) //null check
            {
                //Compare the 2 animations hashes, if they aren't the same, do the punch thing, also check if the hash isn't the initial state's hash
                if (BodyAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash != previousAnimatorState && BodyAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash != InitialState && (BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Unarmed_LeftPunch") || BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Unarmed_RightPunch")))
                {
                    m_AudioSource.Play();
                    BoxCollider2D PunchHitBox = transform.Find("Punch").GetComponent<BoxCollider2D>();
                    Debug.DrawRay(PunchHitBox.transform.position, PunchHitBox.transform.up * PunchHitBox.size.magnitude, Colors.Azure, 4);
                    //m_AudioSource.Play();
                    if (PunchHitBox.IsTouchingLayers(1 << 8)) // If Punch Hitbox is touching PLayer layer.
                    {
                        //Debug.Log("Punch");

                        Collider2D[] Overlap = Physics2D.OverlapBoxAll(PunchHitBox.transform.position, PunchHitBox.size, 0, 1 << 8); //An Overlap collider with the punch hitbox. There is probably a better way. 
                        int TotalCollisions = Overlap.Length;
                        for (int i = 0; i < TotalCollisions; ++i)
                        {
                            if (Overlap[i].transform.tag == "Player" && Overlap[i].transform != this.transform) // If hit another player
                            {

                                bool HitPlayerFirst = true;
                                RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, (PunchHitBox.transform.position.y + PunchHitBox.size.y), 1 << LayerMask.NameToLayer("Wall"));
                                if (hit.collider != null) // If hit a wall, check to see it hits the wall before the player if it does, stop the check here.
                                {
                                    float DistanceToPlayer = Vector3.Distance(transform.position, Overlap[i].transform.position);
                                    float DistanceToWall = Vector3.Distance(transform.position, hit.point);
                                    if (DistanceToWall < DistanceToPlayer) // If Hit wall first.
                                    {
                                        HitPlayerFirst = false;
                                        //Debug.Log("HitWallFirst");
                                    }
                                    //Debug.Log("HitPlayerFirst");
                                }
                                if (HitPlayerFirst && Overlap[i].GetComponentInParent<PlayerStatus>().IsStunned == false)// If wall check returns player first. Punch as normal.
                                {
                                    PlayerStatus hitPlayer = Overlap[i].GetComponentInParent<PlayerStatus>();
                                    if (hitPlayer != this.GetComponent<PlayerStatus>())
                                    {
                                        hitPlayer.TimesPunched++;
                                        hitPlayer.MiniStun(this.transform.up * (KnockBack * 1.5f), PunchFlinchTime);
                                        float tempPitch = (m_bRandomizeHitPitch) ? Random.Range(0.9f, 1.1f) : 1;
                                        hitPlayer.GetComponent<IHitByMelee>().HitByMelee(this, audioClip, m_clipVolume, tempPitch);
                                        //Debug.Log("PunchedEnemy");
                                        if (hitPlayer.TimesPunched >= m_PunchesToKnockOut)
                                        {
                                            hitPlayer.StunPlayer(transform.up * KnockBack);
                                            hitPlayer.TimesPunched = 0;
                                            hitPlayer.GetComponent<Move>().StatusApplied();//GetComponent<Move>().StatusApplied();
                                                                                           //Debug.Log("StunnedEnemy");
                                        }
                                        else
                                        { hitPlayer.GetComponent<Rigidbody2D>().AddForce(this.transform.up * PunchKBForce); }
                                    }
                                }
                            }
                        }
                    }
                }
                previousAnimatorState = BodyAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash; //Store this frames new animation state's hash
            }
        }

    }
}
