using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Weapon {
    bool PinPulled;
    float TimeTillBoom;
    public Bullet BulletForSharpnel;
    public int TotalBulletShrapnel;
    Vector3 RandomAngle;
    
    // Use this for initialization
    override public void StartUp() {
        RandomAngle.Set(0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if(PinPulled == true)
        {
            TimeTillBoom -= Time.deltaTime;
            if(TimeTillBoom <= 0)
            {
                
            }
        }
	}
    public override bool Attack(bool trigger)
    {
        if(PinPulled == false)
        {
            PinPulled = true;
        }
        return true;
    }
    public void BOOM()
    {
        while(TotalBulletShrapnel > 0)
        {
            RandomAngle.Set(0,0, Random.Range(0f, 360f));
            
            Instantiate<Bullet>(BulletForSharpnel, this.transform.position, RandomAngle.);
            TotalBulletShrapnel--;
        }
    }
}
