using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    CircleCollider2D Hit;
    Vector2 m_vVelocity;
    Vector3 PreviousVelocity;
    public Vector2 Velocity { get { return m_vVelocity; }  set { m_vVelocity = value; } }

    [HideInInspector]
    public PlayerStatus bulletOwner;
    [HideInInspector]
    public float m_iDamage;
    void Start()
    {
        Hit = GetComponent<CircleCollider2D>();
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
    }

    void OnCollisionEnter2D(Collision2D hit)
    {
        if (hit.collider.tag == "Shield")
        {
            this.GetComponent<Rigidbody2D>().velocity = Vector3.Reflect(transform.up + PreviousVelocity, hit.transform.up);

            return;
        }
        if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Wall") || hit.transform.gameObject.layer == LayerMask.NameToLayer("Door"))
        {
            //TODO Play Spark effect
            Destroy(this.gameObject);
        }
        Debug.Log(hit.transform.gameObject.layer);
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (!hit.transform.GetComponent<PlayerStatus>().IsStunned && hit.transform.GetComponent<PlayerStatus>() != bulletOwner)
            {
                Debug.Log("Hit player");
                //Debug.Log("Raycast hit player");
                hit.transform.GetComponent<PlayerStatus>().m_iHealth -= m_iDamage; //TODO Get damage from parent which should be the weapon.
                if (hit.transform.GetComponent<PlayerStatus>().m_iHealth <= 0)
                {
                    hit.transform.GetComponent<PlayerStatus>().IsDead = true;
                }
                hit.transform.GetComponent<Move>().StatusApplied();
                Destroy(this.gameObject);
            }
        }
    }

    void OnTriggerEnter2D()
    {
       
    }
}
