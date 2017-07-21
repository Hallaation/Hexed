using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gun : Weapon
{
    public GameObject bullet;
    [SerializeField]
    public float m_fSpreadJitter = 1.0f;
    public float m_fBulletSpawnOffSet = 2.0f;
    public float m_fFiringForce = 20.0f;
    //overriding startup, all weapons should do this
    public override void StartUp()
    {
        
    }

    public override bool Attack()
    {
        //player will only be allowed to attack once a shot is ready. This is used to determine the time to wait in between shots.
        if (shotReady)
        {
            GameObject go = Instantiate(bullet , this.transform.parent.position + this.transform.parent.up * m_fBulletSpawnOffSet , this.transform.rotation);
            go.GetComponent<Bullet>().bulletOwner = GetComponentInParent<PlayerStatus>();
            go.transform.rotation = this.transform.parent.rotation * Quaternion.Euler(Vector3.forward * m_fSpreadJitter * Random.Range(-1.0f , 1.0f));
            go.GetComponent<Rigidbody2D>().AddForce(go.transform.up * m_fFiringForce , ForceMode2D.Impulse);
            shotReady = false;
            return true;
        }
        return false;
    }

    //anything the weapon should so specifically should be put here
    public override void DoWeaponThings()
    {
        //? DO WEAPON THINGS
    }

}
