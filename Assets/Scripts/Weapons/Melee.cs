using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Weapon
{
    public AudioClip punchEffect;
    private AudioSource temp; //TODO change to a singleton audio manager later
    public float OnHitFlinchTime = .3f;

    public override void StartUp()
    {

    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override bool Attack(bool trigger)
    {
        if (shotReady)
        {

            BoxCollider2D MeleeHitBox = transform.Find("Punch").GetComponent<BoxCollider2D>();

            if (MeleeHitBox.IsTouchingLayers(1 << 8)) // If Punch Hitbox is touching PLayer layer.
            {
                //Debug.Log("Punch");

                Collider2D[] Overlap = Physics2D.OverlapBoxAll(MeleeHitBox.transform.position, MeleeHitBox.size, 0, 1 << 8); //An Overlap collider with the punch hitbox. There is probably a better way. 

                int TotalCollisions = Overlap.Length;
                for (int i = 0; i < TotalCollisions; ++i)
                {
                    if (Overlap[i].transform.tag == "Player" && Overlap[i].transform != this.transform) // If hit another player
                    {

                        bool HitPlayerFirst = true;
                        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, (MeleeHitBox.transform.position.y + MeleeHitBox.size.y), 1 << LayerMask.NameToLayer("Wall"));
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
                        if (HitPlayerFirst && Overlap[i].GetComponent<PlayerStatus>().IsStunned == false)// If wall check returns player first. Punch as normal.
                        {
                            PlayerStatus hitPlayer = Overlap[i].GetComponent<PlayerStatus>();
                            hitPlayer.StunPlayer(transform.up * KnockBack);



                            //Debug.Log("PunchedEnemy");

                        }
                    }
                }
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

}
