using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EmptyHand : Weapon
{
    public AudioClip punchEffect;
    private AudioSource temp; //TODO change to a singleton audio manager later
 
    public override void StartUp()
    {
        this.gameObject.name = "Player + " + GetComponent<ControllerSetter>().m_playerNumber;
        if (!this.GetComponent<AudioSource>())
        {
            temp = this.gameObject.AddComponent<AudioSource>();
        }
        else
        {
            temp = GetComponent<AudioSource>();
        }
        temp.clip = punchEffect;
        temp.volume = 0.5f;
        stunPlayer = false;
    }

    public override bool Attack(bool trigger)
    {
        if (shotReady)
        {
            
            
            BoxCollider2D PunchHitBox = transform.Find("Punch").GetComponent<BoxCollider2D>();
            
            if(PunchHitBox.IsTouchingLayers(1 << 8))
            {
                //Debug.Log("Punch");
               
                Collider2D[] Overlap = Physics2D.OverlapBoxAll(PunchHitBox.transform.position, PunchHitBox.size, 0, 1<<8);
                int TotalCollisions = Overlap.Length;
                for(int i = 0; i < TotalCollisions; ++i)
                {
                    //TODO add a raycast to check for wall
                    if(Overlap[i].transform.tag == "Player" && Overlap[i].transform != this.transform)
                    {
                        Overlap[i].transform.GetComponent<PlayerStatus>().TimesPunched++;
                        if(Overlap[i].transform.GetComponent<PlayerStatus>().TimesPunched >= 3)
                        {
                            Overlap[i].transform.GetComponent<PlayerStatus>().StunPlayer(transform.up * KnockBack);
                            Overlap[i].transform.GetComponent<PlayerStatus>().TimesPunched = 0;
                            Overlap[i].transform.GetComponent<Move>().StatusApplied();//GetComponent<Move>().StatusApplied();
                        }
                    }
                }
            }
            shotReady = false;
            RaycastHit2D[] results;


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
