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
    public int m_iGlassDamage = 1;
    private int PunchKBForce = 50000;
    private Animator m_BodyAnimator;
    private bool m_bAttacking;
    public int m_PunchesToKnockOut = 2;
    private int previousAnimatorState;
    private int InitialState;
    private Move _moveClass;
    private PlayerStatus m_LastPlayerPunched;

    public Animator BodyAnimator { get { return m_BodyAnimator; } set { m_BodyAnimator = value; } }

    public override void StartUp()
    {
        //this.gameObject.name = "Player + " + GetComponent<ControllerSetter>().m_playerNumber;
        BodyAnimator = this.transform.Find("Sprites").GetChild(0).GetComponent<Animator>();
        previousAnimatorState = BodyAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        InitialState = previousAnimatorState;
        BodyAnimator.SetBool("Moving", true);
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

            //        temp.Play();

            //        hit.transform.GetComponent<PlayerStatus>().TimesPunched++;
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
                    // Debug.DrawRay(PunchHitBox.transform.position, PunchHitBox.transform.up * PunchHitBox.size.magnitude, Colors.Azure, 4);
                    //m_AudioSource.Play();
                    #region Punching player logic
                    if (PunchHitBox.IsTouchingLayers(1 << 8)) // If Punch Hitbox is touching PLayer layer.
                    {

                        Collider2D[] Overlap = Physics2D.OverlapBoxAll(PunchHitBox.transform.position, PunchHitBox.size, 0, LayerMask.GetMask("Player", "Glass")); //An Overlap collider with the punch hitbox. There is probably a better way. 
                        int TotalCollisions = Overlap.Length;
                        for (int i = 0; i < TotalCollisions; ++i)
                        {
                            if (Overlap[i].transform.tag == "Player" && Overlap[i].transform != this.transform) // If hit another player
                            {
                                bool HitPlayerFirst = true;
                                RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, (PunchHitBox.transform.position.y + PunchHitBox.size.y), LayerMask.GetMask("Wall", "Glass"));
                                if (hit.collider != null) // If hit a wall, check to see it hits the wall before the player if it does, stop the check here.
                                {
                                    float DistanceToPlayer = Vector3.Distance(transform.position, Overlap[i].transform.position);
                                    float DistanceToWall = Vector3.Distance(transform.position, hit.point);
                                    if (DistanceToWall < DistanceToPlayer) // If Hit wall first.
                                    {
                                        HitPlayerFirst = false;
  
                                    }
                              }
                                if (HitPlayerFirst && Overlap[i].GetComponentInParent<PlayerStatus>().IsStunned == false)// If wall check returns player first. Punch as normal.
                                {
                                    PlayerStatus hitPlayer = Overlap[i].GetComponentInParent<PlayerStatus>();
                                    if (hitPlayer != this.GetComponent<PlayerStatus>() && !m_LastPlayerPunched)
                                    {
                                        hitPlayer.TimesPunched++;
                                        m_LastPlayerPunched = hitPlayer;
                                        hitPlayer.MiniStun(this.transform.up * (KnockBack * 1.5f), PunchFlinchTime);
                                        float tempPitch = (m_bRandomizeHitPitch) ? Random.Range(0.9f, 1.1f) : 1;
                                        hitPlayer.GetComponent<IHitByMelee>().HitByMelee(this, audioClip, m_clipVolume, tempPitch);
   
                                        if (hitPlayer.TimesPunched >= m_PunchesToKnockOut)
                                        {
                                            hitPlayer.StunPlayer(transform.up * KnockBack);
                                            hitPlayer.TimesPunched = 0;
                                            hitPlayer.GetComponent<Move>().StatusApplied();//GetComponent<Move>().StatusApplied();
                            
                                        }
                                        else
                                        { hitPlayer.GetComponent<Rigidbody2D>().AddForce(this.transform.up * PunchKBForce); }
                                    }
                                }
                            }

                        }
                    }
                    #endregion
                    #region GlassHittingLogic
                    else if (PunchHitBox.IsTouchingLayers(LayerMask.GetMask("Glass")))
                    {
                        //Do a raycast to determine which one I hit.
                        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, ((this.transform.position - PunchHitBox.transform.position).magnitude + PunchHitBox.size.magnitude * 0.3f), LayerMask.GetMask("Glass"));
                        Debug.DrawRay(transform.position, this.transform.up * ((this.transform.position - PunchHitBox.transform.position).magnitude + PunchHitBox.size.magnitude * 0.3f), Colors.Bisque, 1);
                      //  RaycastHit2D hit = Physics2D.BoxCast(this., PunchHitBox.size, PunchHitBox.transform.rotation.eulerAngles.z, this.transform.up);
                        if (hit)
                        {
                            if (hit.transform.GetComponent<Glass>())
                            {
                                hit.transform.GetComponent<Glass>().HitGlass(this.transform.up * KnockBack, hit.point, m_iGlassDamage);
                            }

                        }
                    }
                    #endregion
                }

                previousAnimatorState = BodyAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash; //Store this frames new animation state's hash
                m_LastPlayerPunched = null;
            }
        }

    }
}
