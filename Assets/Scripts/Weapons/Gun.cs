using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gun : Weapon
{
    public int m_iAmmo = 30;
    public GameObject bullet;
    [SerializeField]
    public float m_fSpreadJitter = 1.0f;
    public float m_fBulletSpawnOffSet = 2.0f;
    public float m_fFiringForce = 20.0f;
    public bool m_bAutomatic;

    private float TimeSinceLastShot;
    //overriding startup, all weapons should do this
    public override void StartUp()
    {
        
    }

    public override bool Attack()
    {

        //player will only be allowed to attack once a shot is ready AND they have ammo. This is used to determine the time to wait in between shots.
        if (shotReady && m_iAmmo != 0 && m_bActive ) 
        {
            GameObject FiredBullet = Instantiate(bullet , this.transform.parent.position + this.transform.parent.up * m_fBulletSpawnOffSet , this.transform.rotation);
            FiredBullet.GetComponent<Bullet>().bulletOwner = GetComponentInParent<PlayerStatus>();
            FiredBullet.GetComponent<Bullet>().m_iDamage = this.m_iDamage;
            FiredBullet.transform.rotation = this.transform.parent.rotation * Quaternion.Euler(Vector3.forward * m_fSpreadJitter * Random.Range(-1.0f , 1.0f));
            FiredBullet.GetComponent<Rigidbody2D>().AddForce(FiredBullet.transform.up * m_fFiringForce , ForceMode2D.Impulse);
            shotReady = false;
            --m_iAmmo;
            return true;
        }
        else if (m_iAmmo == 0)
        {
            //TODO Add gun click sound effect here.
        }
        return false;
    }

    //anything the weapon should so specifically should be put here
    public override void DoWeaponThings()
    {
        //? DO WEAPON THINGS
    }

}
