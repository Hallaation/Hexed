using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, Reset
{
    CircleCollider2D m_CircleCollider;
    Vector2 m_vVelocity;
    Vector3 PreviousVelocity;
    Quaternion StartRotation;
    float PreviousRotation;
    private bool m_bStopRayCasts = false;
    public Vector3 GetPreviousVelocity() { return PreviousVelocity; }
    public Vector2 Velocity { get { return m_vVelocity; } set { m_vVelocity = value; } }
    ParticleSystem ParticleSparks;
    ParticleSystem[] WallCollidedParticles;
    SpriteRenderer BulletSprite;
    Rigidbody2D m_rigidBody;
    Vector3 VChildPrevRotation;
    public bool CanHurtSelf = false;
    public bool CanHurtKnockedDown = false;
    [HideInInspector]
    public PlayerStatus bulletOwner;
    public PlayerStatus m_bShooter;
    [HideInInspector]
    public int m_iDamage;
    public bool m_bGiveIFrames = false;
    public float m_fBulletImpactKnockBack = 5;
    public TrailRenderer trail;
    public float m_fMaximumTime = 60;
    public bool m_bBouncyBullet = false;
    public int m_iMaxBounces = 3;
    private int m_iCurrentBounces;

    //public GameObject HitParticle;


    void Start()
    {
        BulletSprite = GetComponent<SpriteRenderer>();
        ParticleSparks = GetComponentInChildren<ParticleSystem>();
        WallCollidedParticles = transform.GetChild(0).GetComponentsInChildren<ParticleSystem>();

        m_CircleCollider = GetComponent<CircleCollider2D>();
        m_rigidBody = GetComponent<Rigidbody2D>();
        PreviousRotation = GetComponent<Rigidbody2D>().rotation;
        PreviousVelocity = GetComponent<Rigidbody2D>().velocity;
        StartRotation = transform.rotation;

        Vector2 dir = m_rigidBody.velocity;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.GetChild(0).rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        if (trail != null)
        {
            trail.sortingLayerName = "Defult";
            trail.sortingOrder = 8;
        }
        Destroy(this.gameObject, m_fMaximumTime);
    }
    // Update is called once per frame
    void Update()
    {

        PreviousVelocity = this.GetComponent<Rigidbody2D>().velocity;
        PreviousRotation = m_rigidBody.rotation;



        VChildPrevRotation = transform.localEulerAngles;
    }
    //? THIS is done. lol
    private void FixedUpdate()
    {
        if (!m_bStopRayCasts)
        {
            // Ray2D WallCheckRay = new Ray2D(transform.position, transform.right);
            //Raycast from me, to my right vector (because all the rotations are fucked) on the distance I'll travel for the next frame.
            //Only raycast against the player, wall, door and glass 
            RaycastHit2D RayHit = Physics2D.Raycast(this.transform.position, m_rigidBody.velocity.normalized, m_rigidBody.velocity.magnitude * Time.fixedDeltaTime * 2,
                (/*Player */ 1 << 8 | /*Shield*/ 1 << 9 | /* Wall */1 << 10 |/*Glass*/ 1 << 14 | /*Door*/ 1 << 11 | /*Dumpster*/ 1 << 23));
            Debug.DrawRay(this.transform.position, m_rigidBody.velocity.normalized * m_rigidBody.velocity.magnitude * Time.fixedDeltaTime * 2, Color.red, 0.5f);
            //Debug.Break();
            if (RayHit)
            {
                //If I hit a wall, glass or door, snap me to their location and turn me off
                if (RayHit.transform.gameObject.layer == LayerMask.NameToLayer("Wall") || RayHit.transform.gameObject.layer == LayerMask.NameToLayer("Door") || RayHit.transform.gameObject.layer == LayerMask.NameToLayer("Glass") || RayHit.transform.gameObject.layer == LayerMask.NameToLayer("Dumpster"))
                {
                    //If I find any hitbybullet interface, find all, then call its function
                    if (RayHit.transform.gameObject.GetComponent<IHitByBullet>() != null)
                    {
                        foreach (IHitByBullet item in RayHit.transform.gameObject.GetComponents<IHitByBullet>())
                        {
                            item.HitByBullet(m_rigidBody.velocity, RayHit.point);
                        }
                    }
                    GameManagerc.Instance._rbPausers.Remove(this.GetComponent<RigidbodyPauser>());
                    //StopBullet(RayHit);
                    if (m_bBouncyBullet)
                    {
                        ReflectBullet(RayHit);
                    }
                    else
                    {
                        StopBullet(RayHit);
                    }
                    //If a wall, play the particle

                    //bullet reflection
                    //this.transform.position += this.transform.up * 1.2f;
                    //this.transform.rotation = Quaternion.Inverse(this.transform.rotation);
                    //
                    //Vector3 velocity = this.GetComponent<Rigidbody2D>().velocity;
                    //this.GetComponent<Rigidbody2D>().velocity = Vector3.Reflect(velocity, RayHit.transform.up);
                    //this.GetComponent<Bullet>().bulletOwner = null;
                    if (!m_bBouncyBullet)
                        StartCoroutine(PlayParticle(RayHit.point));

                }
                else if (RayHit.transform.gameObject.layer != LayerMask.NameToLayer("Player"))
                {
                    //If I find any hitbybullet interface, find all, then call its function
                    if (RayHit.transform.gameObject.GetComponent<IHitByBullet>() != null)
                    {
                        foreach (IHitByBullet item in RayHit.transform.gameObject.GetComponents<IHitByBullet>())
                        {
                            item.HitByBullet(m_rigidBody.velocity, RayHit.point);
                        }
                    }
                    GameManagerc.Instance._rbPausers.Remove(this.GetComponent<RigidbodyPauser>());
                    m_bStopRayCasts = true;
                    StopBullet(RayHit);
                    Destroy(this.gameObject, 1);

                }
                else
                {
                    //If i hit A player (when all the other cases aren't met), check to see if the bullet doens't own to me
                    if ((!RayHit.transform.GetComponent<PlayerStatus>().IsStunned || CanHurtKnockedDown) && (RayHit.transform.GetComponent<PlayerStatus>() != bulletOwner || CanHurtSelf) && !RayHit.transform.GetComponent<PlayerStatus>().IsDead)
                    {
                        //If I find any hitbybullet interface, find all, then call its function
                        if (RayHit.transform.gameObject.GetComponent<IHitByBullet>() != null)
                        {
                            foreach (IHitByBullet item in RayHit.transform.gameObject.GetComponents<IHitByBullet>())
                            {
                                item.HitByBullet(m_rigidBody.velocity, RayHit.point);

                            }
                        }
                        m_rigidBody.position = RayHit.point; //Snap the bullet to the collided object
                        RayHit.transform.GetComponent<Rigidbody2D>().position += (Vector2)this.transform.right * m_fBulletImpactKnockBack;
                        PlayerStatus PlayerIHit = RayHit.transform.GetComponent<PlayerStatus>(); //Store the player I hit temporarily
                        RayHit.transform.GetComponent<Rigidbody2D>().AddForce(m_rigidBody.velocity);
                        PlayerIHit.HitPlayer(this, m_bGiveIFrames);
                        if (PlayerIHit.m_iHealth <= 0)
                        {
                            if (!PlayerIHit.IsDead && GameManagerc.Instance.m_gameMode == Gamemode_type.HEAD_HUNTERS)
                            {
                                //if (PlayerIHit != m_bShooter) //if the hit player doesn't equal my shooter
                                //{
                                Debug.Log("player hit");
                                m_bShooter.KilledAPlayer(PlayerIHit);
                                //}
                            }
                            PlayerIHit.IsDead = true;
                            GameManagerc.Instance.lastPlayerToEarnPoints = m_bShooter;
                            PlayerIHit.GetComponent<Rigidbody2D>().velocity = m_rigidBody.velocity * m_fBulletImpactKnockBack;
                            float angle = Mathf.Atan2(m_rigidBody.velocity.normalized.x, -m_rigidBody.velocity.normalized.y);
                            PlayerIHit.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
                        }

                        //RayHit.transform.GetComponent<Move>().StatusApplied();
                        m_bStopRayCasts = true;
                        Blood hitBlood = RayHit.transform.root.Find("Sprites").GetComponent<Blood>();
                        if (hitBlood)
                            RayHit.transform.root.Find("Sprites").GetComponent<Blood>().CreateBloodSplatter(RayHit.transform.root.position, PreviousVelocity, StartRotation);

                        StopBullet(RayHit);
                        Destroy(this.gameObject, 0.5f); //Destroy me beacuse I have no other purpose, 
                        //? Maybe change the bullets to a pool instead
                    }
                }
            }
        }
        //if (RayHit.distance < a) 
        //{
        //    m_rigidBody.isKinematic = true;

        //    transform.position = RayHit.point;
        //    StartCoroutine(PlayParticle(RayHit.point));
        //}
    }

    void StopBullet(RaycastHit2D RayHit)
    {
        m_bStopRayCasts = true;
        GameManagerc.Instance._rbPausers.Remove(this.GetComponent<RigidbodyPauser>());
        m_rigidBody.velocity = Vector2.zero;
        m_rigidBody.simulated = false;
        BulletSprite.enabled = false;
        transform.position = RayHit.point;
        transform.rotation = StartRotation;
    }

    public void ReflectBullet(RaycastHit2D a_RayHit)
    {
        Vector2 bulletVelocity = Vector2.zero;
        //m_bStopRayCasts = true;
        if (m_rigidBody)
        {
            bulletVelocity = m_rigidBody.velocity;
        }
        this.transform.position = a_RayHit.point;
        
        m_rigidBody.velocity = Vector2.Reflect(bulletVelocity, a_RayHit.normal);
        m_iCurrentBounces++;
        bulletOwner = null;
        if (m_iCurrentBounces > m_iMaxBounces)
        {
            StopBullet(a_RayHit);
        }
        StartCoroutine(PlayParticle(a_RayHit.point, a_RayHit, false));
    }

    IEnumerator PlayParticle(Collision2D hit)
    {
        if (ParticleSparks != null)
        {

            // transform.GetChild(0).localEulerAngles = new Vector3(VChildPrevRotation.x , VChildPrevRotation.y , VChildPrevRotation.z); // parent - 90z
            transform.position = new Vector3(hit.contacts[0].point.x, hit.contacts[0].point.y, 0);
            ParticleSparks.Play();

            //GameObject hitInstance = Instantiate(HitParticle, this.transform.position, Quaternion.identity) as GameObject;
            //hitInstance.transform.up = hit.transform.up;
            //hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        }
        yield return new WaitForSecondsRealtime(ParticleSparks.main.duration);
        Destroy(this.gameObject);
    }

    public void PlayParticles(Vector2 HitPoint)
    {
        WallCollidedParticles = transform.GetChild(0).GetComponentsInChildren<ParticleSystem>();
        StartCoroutine(PlayParticle(HitPoint));
    }
    /// <summary>
    /// Called whenever the raycast hits a wall
    /// </summary>
    /// <param name="HitPoint"></param>
    /// <returns></returns>
    IEnumerator PlayParticle(Vector2 HitPoint, bool destroyBullet = true)
    {
        float longestParticleDuration = 0;
        if (WallCollidedParticles.Length > 0)
        {
            // transform.GetChild(0).localEulerAngles = new Vector3(VChildPrevRotation.x , VChildPrevRotation.y , VChildPrevRotation.z);
            foreach (ParticleSystem particle in WallCollidedParticles)
            {
                particle.transform.position = HitPoint;
                if (particle.main.duration > longestParticleDuration)
                    longestParticleDuration = particle.main.duration;
                particle.Play();
            }
        }
        //Wait for the longest particle
        yield return new WaitForSecondsRealtime(longestParticleDuration);
        if (destroyBullet)
        {
            Destroy(this.gameObject);
        }
    }

    IEnumerator PlayParticle(Vector2 HitPoint, RaycastHit2D rayHit, bool destroyBullet = true)
    {
        float longestParticleDuration = 0;
        if (WallCollidedParticles.Length > 0)
        {
            // transform.GetChild(0).localEulerAngles = new Vector3(VChildPrevRotation.x , VChildPrevRotation.y , VChildPrevRotation.z);
            foreach (ParticleSystem particle in WallCollidedParticles)
            {
                particle.transform.position = HitPoint;
                float angle = Mathf.Atan2(-rayHit.normal.y, -rayHit.normal.x) * Mathf.Rad2Deg;
                particle.transform.rotation = Quaternion.Euler(angle, 270, 0);
                Debug.DrawRay(rayHit.point, rayHit.normal, Colors.BlanchedAlmond, 1);
                if (particle.main.duration > longestParticleDuration)
                    longestParticleDuration = particle.main.duration;
                particle.Play();
            }
        }
        //Wait for the longest particle
        yield return new WaitForSecondsRealtime(longestParticleDuration);
        if (destroyBullet)
        {
            Destroy(this.gameObject);
        }
    }

    //This is now all completely useless, 
    void OnCollisionEnter2D(Collision2D hit)
    {
        if (m_rigidBody.isKinematic == false)
            if (hit.collider.tag == "Shield")
            {
                this.GetComponent<Rigidbody2D>().velocity = Vector3.Reflect(transform.up + PreviousVelocity, hit.transform.up);

                return;
            }
        m_CircleCollider.enabled = false;
        m_rigidBody.rotation = PreviousRotation;
        //if I hit a wall, a door, some glass or a "height objecct" I will stop everything.
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Wall") || hit.transform.gameObject.layer == LayerMask.NameToLayer("Door") || hit.transform.gameObject.layer == LayerMask.NameToLayer("Glass") || hit.transform.gameObject.layer == LayerMask.NameToLayer("HeightObject"))
        {
            //m_rigidBody.velocity = Vector2.zero;
            //m_rigidBody.simulated = false;
            //BulletSprite.enabled = false;
            //
            //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Wall"))
            //    StartCoroutine(PlayParticle(hit));
            //else
            //    //print("lol");
            //    Destroy(this.gameObject);

            //TODO Play Spark effect

        }
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (!hit.transform.GetComponent<PlayerStatus>().IsStunned && hit.transform.GetComponent<PlayerStatus>() != bulletOwner)
            {
                //hit.transform.GetComponent<PlayerStatus>().m_iHealth -= m_iDamage;
                hit.transform.GetComponent<PlayerStatus>().HitPlayer(this, m_bGiveIFrames);
                if (hit.transform.GetComponent<PlayerStatus>().m_iHealth <= 0)
                {
                    hit.transform.GetComponent<PlayerStatus>().IsDead = true;
                }

                Destroy(this.gameObject);
            }
        }

    }

    void OnTriggerEnter2D()
    {

    }

    public void Reset()
    {
        Destroy(this);
    }
}
