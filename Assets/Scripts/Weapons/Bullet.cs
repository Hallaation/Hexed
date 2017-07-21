using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Vector2 m_vVelocity;
    public Vector2 Velocity { get { return m_vVelocity; }  set { m_vVelocity = value; } }

    [HideInInspector]
    public PlayerStatus bulletOwner;
    void Start()
    {
       // Destroy(this.gameObject , 5);
    }
	// Update is called once per frame
	void Update ()
    {
        //raycasts in front for collision check
        Ray2D ray = new Ray2D(this.transform.position , -this.transform.right);
        Debug.DrawRay(ray.origin , ray.direction, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(ray.origin , ray.direction , 0.5f , 1 << 8);
        if (hit.collider)
        {
            if (!hit.transform.GetComponent<PlayerStatus>().IsStunned && hit.transform.GetComponent<PlayerStatus>() != bulletOwner)
            {
                Debug.Log("Raycast hit player");
                hit.transform.GetComponent<PlayerStatus>().IsDead = true;
                hit.transform.GetComponent<Move>().StatusApplied();
                Destroy(this.gameObject);
            }
        }
	}
}
