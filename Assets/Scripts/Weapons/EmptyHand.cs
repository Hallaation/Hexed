using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyHand : Weapon
{
    public AudioClip punchEffect;
    private AudioSource temp; //TODO change to a singleton audio manager later
    public float KnockBack = 2;
    public override void StartUp()
    {
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
            shotReady = false;

            Ray2D ray = new Ray2D(this.transform.position + this.transform.up * 1.2f , this.transform.up);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin , ray.direction , 0.3f , 1 << 8);
            if (hit)
            {
                if (hit.transform != this.transform)
                {
                    Debug.Log("Test");
                    temp.Play();

                    hit.transform.GetComponent<PlayerStatus>().TimesPunched++;
                    if (hit.transform.GetComponent<PlayerStatus>().TimesPunched >= 3)
                    {
                        hit.transform.GetComponent<PlayerStatus>().StunPlayer(this.transform.up * KnockBack * 10);
                        hit.transform.GetComponent<Move>().StatusApplied();
                    }

                    return true;
                }
            }
            Debug.DrawRay(ray.origin , ray.direction);
        }
        return false;
        
    }

}
