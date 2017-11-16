using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Weapon
{
    bool PinPulled;
    PlayerStatus Owner;
    [Space]
    [Header("Grenade Specific")]
    public float TimeTillBoom;
    float TimeTillSpriteChange;
    public GameObject BulletForSharpnel;
    public int TotalBulletShrapnel;
    Quaternion RandomAngle;
    bool BlownUp;
    public float m_fBulletImpactKnockback = .1f;
    public AudioClip AudioPinPull;
    public AudioClip AudioExplosion;
    public AudioClip AudioTickSound;
    Rigidbody2D MyRigidBody;
    public float m_fFiringForce = 1;
    public float GrowthRate = 1f;
    public Sprite PinSprite;
    public Sprite PinPulledSprite;
    public Sprite TickSprite;
    public Sprite tick2Sprite;
    Sprite[] SpriteArray = new Sprite[4];
    Sprite LastSprite;
    
    // Use this for initialization
    override public void StartUp()
    {
        LastSprite = PinSprite;
        RandomAngle = Quaternion.identity;
        PinPulled = false;
        BlownUp = false;
        MyRigidBody = transform.GetComponent<Rigidbody2D>();

        SpriteArray[3] = PinSprite;
        SpriteArray[2] = PinPulledSprite;
        SpriteArray[1] = TickSprite;
        SpriteArray[0] = tick2Sprite;
    }

    // Update is called once per frame
    public override void DoWeaponThings()
    {
        
        if (PinPulled == true && BlownUp == false)
        {
            if (TimeTillBoom >= 0)
            {
                SetWeaponSpriteRenderer(SpriteArray[(int)TimeTillBoom]);
                if(LastSprite != SpriteArray[(int)TimeTillBoom])
                {
                    LastSprite = SpriteArray[(int)TimeTillBoom];
                    m_AudioSource.clip = AudioTickSound;
                    m_AudioSource.Play();
                }
            }
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
        if (PinPulled == false)
        {
            Color Red = new Color(255, 0, 0, 255);
            transform.GetChild(0).GetComponent<SpriteRenderer>().material.color = Color.red;
            Owner = transform.root.GetComponent<PlayerStatus>();
            m_AudioSource.clip = AudioPinPull;
            m_AudioSource.Play();
            PinPulled = true;
        }
        else
        {
      //      this.GetComponent<Move>().SetThrowWeaponOverRide(true);
        }
            return true;
    }
    public void BOOM()
    {
        while (TotalBulletShrapnel > 0)
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
            bulletComponent.bulletOwner = Owner;
            bulletComponent.m_bShooter = Owner; 
            bulletComponent.m_iDamage = this.m_iDamage;
            bulletComponent.m_bGiveIFrames = m_bGivePlayersIFrames;
            bulletComponent.m_fBulletImpactKnockBack = m_fBulletImpactKnockback;
            BlownUp = true;
        }
    }
}
