using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //base weapon class
    public float m_fTimeBetweenShots = 0.01f;
    public float m_iDamage;

    protected Timer TimerBetweenFiring;
    protected bool shotReady = true;

    protected bool stunPlayer = true;
    [HideInInspector]
    public GameObject previousOwner; //previous owner used to make sure when the weapon is thrown, it doesnt stun the thrower.

    // Use this for initialization
    void Start ()
    {
        TimerBetweenFiring = new Timer(m_fTimeBetweenShots);
        StartUp();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //do weapon things a virtual function, in case any weapons need to do anything extra
        DoWeaponThings();
        //wait for next shot, ticks the timer until it is ready for the next shot
        waitForNextShot();
    }

    void waitForNextShot()
    {
        //once the player has shot a bullet, the timer will start to tick until the desired time. Until the desired time hasn't reached, the player cannot shoot
        if (!shotReady)
        {
            if (TimerBetweenFiring.Tick(Time.deltaTime))
            {
                shotReady = true;
            }
        }
    }
    public void dropWeapon()
    {
        //dropping the weapon will turn the physics back on
        GetComponent<Rigidbody2D>().simulated = true;
        this.transform.SetParent(null);
        GetComponent<Rigidbody2D>().angularVelocity = 50.0f;
    }
    /// <summary>
    /// Takes in a velocity vector, this will determine how fast it will be thrown out of the player's hand. Low velocity means it can be just dropped.
    /// </summary>
    /// <param name="velocity"></param>
    public void throwWeapon(Vector2 velocity)
    {
        //turn the physics back on set its parent to null, and apply the velocity. apply an angular velocity for it to spin.
        GetComponent<Rigidbody2D>().simulated = true;
        this.transform.SetParent(null);
        GetComponent<Rigidbody2D>().AddForce(velocity, ForceMode2D.Impulse);
        GetComponent<Rigidbody2D>().angularVelocity = 600.0f;
    }
    //virtual functions
    public virtual bool Attack() { return false; }
    public virtual void DoWeaponThings() { }
    public virtual void StartUp() { }

    void OnTriggerEnter2D(Collider2D a_collider)
    {
        if (stunPlayer)
        {
            //if it enters a trigger (another player in this case") the hit player gets stunned. calls the status applied to drop their weapon.
            if (GetComponent<Rigidbody2D>().velocity.magnitude >= 10 && a_collider.tag == "Player" && a_collider.gameObject != previousOwner)
            {
                a_collider.GetComponent<PlayerStatus>().StunPlayer();
                a_collider.GetComponent<Move>().StatusApplied();
            }
        }
    }
}
