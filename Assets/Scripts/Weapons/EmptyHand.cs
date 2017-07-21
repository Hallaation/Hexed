using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyHand : Weapon
{

    public override void StartUp()
    {
        stunPlayer = false;
    }
    public override bool Attack()
    {
        if (shotReady)
        {
            shotReady = false;

            Ray2D ray = new Ray2D(this.transform.position + this.transform.up * 1.2f , this.transform.up);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin , ray.direction , 0.3f , 1 << 8);
            if (hit)
            {
                //hit.transform.GetComponent<PlayerStatus>().StunPlayer();
                hit.transform.GetComponent<PlayerStatus>().TimesPunched++;
                //hit.transform.GetComponent<Move>().StatusApplied();
                return true;
            }
            Debug.DrawRay(ray.origin , ray.direction);
        }
        return false;
        
    }

}
