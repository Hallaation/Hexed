using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Weapon {
    bool PinPulled;
    PlayerStatus Owner;
    [Space]
    [Header("Grenade Specific")]
    public float TimeTillBoom;
    public GameObject BulletForSharpnel;
    public int TotalBulletShrapnel;
    Quaternion RandomAngle;
    bool BlownUp;
    public float m_fBulletImpactKnockback = .1f;
    public AudioClip AudioPinPull;
    public AudioClip AudioExplosion;
    Rigidbody2D MyRigidBody;
    public float m_fFiringForce = 1;
    public float GrowthRate = 1f;
    // Use this for initialization
    override public void StartUp() {
        RandomAngle = Quaternion.identity;
        PinPulled = false;
        BlownUp = false;
        MyRigidBody = transform.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        if (PinPulled == true && BlownUp == false) 
        {
            TimeTillBoom -= Time.deltaTime;
            if (TimeTillBoom <= .5f)
            {
                transform.localScale += (transform.localScale * GrowthRate * Time.deltaTime);
                if (TimeTillBoom <= 0)
                {
                    BOOM();
                }
            }
        }
        if (BlownUp == true && transform.localScale.x > 0)
        {
            transform.localScale -= (transform.localScale * GrowthRate * Time.deltaTime * 10);
        }
    }
    public override bool Attack(bool trigger)
    {
        if(PinPulled == false)
        {
            Color Red = new Color(255, 0, 0, 255);
            transform.GetChild(0).GetComponent<SpriteRenderer>().material.color = Color.red;
            Owner = transform.root.GetComponent<PlayerStatus>();
            m_AudioSource.clip = AudioPinPull;
            m_AudioSource.Play();
            PinPulled = true;
        }
        return true;
    }
    public void BOOM()
    {
        while(TotalBulletShrapnel > 0)
        {
            m_AudioSource.clip = AudioExplosion;
            m_AudioSource.Play();
            float randomfloat = Random.Range(0f, 360f);
            RandomAngle.eulerAngles = new Vector3(0, 0, randomfloat);

           GameObject FiredBullet = Instantiate(BulletForSharpnel, this.transform.position, RandomAngle);

            TotalBulletShrapnel--;
            Rigidbody2D BulletRigidBody = FiredBullet.GetComponent<Rigidbody2D>();
            BulletRigidBody.AddForce(FiredBullet.transform.up * m_fFiringForce, ForceMode2D.Impulse);
            Bullet bulletComponent = FiredBullet.GetComponent<Bullet>();
            bulletComponent.bulletOwner = GetComponentInParent<PlayerStatus>(); //copy stuff over
            bulletComponent.m_bShooter = GetComponentInParent<PlayerStatus>();
            bulletComponent.m_iDamage = this.m_iDamage;
            bulletComponent.m_bGiveIFrames = m_bGivePlayersIFrames;
            bulletComponent.m_fBulletImpactKnockBack = m_fBulletImpactKnockback;
            BlownUp = true;
        }
    }
}
