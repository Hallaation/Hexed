using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    CircleCollider2D Hit;
    Vector2 m_vVelocity;
    Vector3 PreviousVelocity;
    public Vector3 GetPreviousVelocity() { return PreviousVelocity; }
    public Vector2 Velocity { get { return m_vVelocity; }  set { m_vVelocity = value; } }
    ParticleSystem ParticleSparks;
    SpriteRenderer BulletSprite;
    Rigidbody2D m_rigidBody;
    Vector3 VChildPrevRotation;
    [HideInInspector]
    public PlayerStatus bulletOwner;
    [HideInInspector]
    public float m_iDamage;
    public bool m_bGiveIFrames = false;
    //public GameObject HitParticle;

  
    void Start()
    {
        BulletSprite = GetComponent<SpriteRenderer>();
        ParticleSparks = GetComponentInChildren<ParticleSystem>();
        Hit = GetComponent<CircleCollider2D>();
        m_rigidBody = GetComponent<Rigidbody2D>();

        PreviousVelocity = GetComponent<Rigidbody2D>().velocity;
       // Destroy(this.gameObject , 5);
    }
	// Update is called once per frame
	void Update ()
    {

        //raycasts in front for collision check
        //Ray2D ray = new Ray2D(this.transform.position, -this.transform.up);
        //Debug.DrawRay(ray.origin, ray.direction, Color.red);

        //RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, .3f, (1 << 8 | 1 << 9 | 1 << 10 | 1 << 11));
        //int ColliderCase = 0;
        ////TODO Maybe make this a switch but that takes effort
        //if (hit.collider)
        //{
        //    if (hit.collider.tag == "Shield")
        //    {
        //        hit.collider.GetComponentInParent<ShieldAbility>().TakeBullet(this.gameObject, hit);
        //        return;
        //    }
        //    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
        //    {
        //        if (!hit.transform.GetComponent<PlayerStatus>().IsStunned && hit.transform.GetComponent<PlayerStatus>() != bulletOwner)
        //        {
        //            Debug.Log("Hit player");
        //            //Debug.Log("Raycast hit player");
        //            hit.transform.GetComponent<PlayerStatus>().m_iHealth -= m_iDamage; //TODO Get damage from parent which should be the weapon.
        //            if (hit.transform.GetComponent<PlayerStatus>().m_iHealth <= 0)
        //            {
        //                hit.transform.GetComponent<PlayerStatus>().IsDead = true;
        //            }
        //            hit.transform.GetComponent<Move>().StatusApplied();
        //            Destroy(this.gameObject);
        //        }
        //    }

        //}
        PreviousVelocity = this.GetComponent<Rigidbody2D>().velocity;
        Vector2 dir = m_rigidBody.velocity;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


      
        VChildPrevRotation = transform.localEulerAngles;
    }

    IEnumerator PlayParticle(Collision2D hit)
    {
        Debug.Log("spark");
        if (ParticleSparks != null)
        {
            transform.GetChild(0).localEulerAngles = new Vector3(VChildPrevRotation.x,VChildPrevRotation.y,VChildPrevRotation.z - 90); // parent - 90z
            transform.position = new Vector3(hit.contacts[0].point.x,hit.contacts[0].point.y,0);
            ParticleSparks.Play();

            //GameObject hitInstance = Instantiate(HitParticle, this.transform.position, Quaternion.identity) as GameObject;
            //hitInstance.transform.up = hit.transform.up;
            //hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        }
        yield return new WaitForSeconds(.4f);
        Destroy(this.gameObject);
    }

    void OnCollisionEnter2D(Collision2D hit)
    {
        if (hit.collider.tag == "Shield")
        {
            this.GetComponent<Rigidbody2D>().velocity = Vector3.Reflect(transform.up + PreviousVelocity, hit.transform.up);

            return;
        }
        //if I hit a wall, a door, some glass or a "height objecct" I will stop everything.
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Wall") || hit.transform.gameObject.layer == LayerMask.NameToLayer("Door") || hit.transform.gameObject.layer == LayerMask.NameToLayer("Glass") || hit.transform.gameObject.layer == LayerMask.NameToLayer("HeightObject"))
        {          
            m_rigidBody.velocity = Vector2.zero;
            m_rigidBody.simulated = false;
            BulletSprite.enabled = false;
            Hit.enabled = false;
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Wall"))
                StartCoroutine(PlayParticle(hit));
            else
                Destroy(this.gameObject);

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

}
