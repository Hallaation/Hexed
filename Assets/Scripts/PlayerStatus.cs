using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public int m_iHealth = 3; //health completely useless right now
    int m_iTimesPunched = 0;
    bool m_bDead = false;
    bool m_bStunned = false;
    public bool IsDead { get { return m_bDead; } set { m_bDead = value; } }
    public bool IsStunned { get { return m_bStunned; } set { m_bStunned = value; } }
    public int TimesPunched { get { return m_iTimesPunched; } set { m_iTimesPunched = value; } }

    public float m_fStunTime = 1;
    Timer stunTimer;
    Color _playerColour;

    [HideInInspector]
    public GameObject killMePrompt = null;
    [HideInInspector]
    public GameObject killMeArea = null;
    //if the player is dead, the renderer will change their colour to gray, and all physics simulation of the player's rigidbody will be turned off.
    void Start()
    {
        //initialize my timer and get the player's colour to return to.
        stunTimer = new Timer(m_fStunTime);
        _playerColour = GetComponent<Renderer>().material.color;
        killMePrompt.SetActive(false);
    }
    void Update()
    {

        //if im dead, set my colour to gray, turn of all physics simulations and exit the function
        if (m_bDead)
        {
            this.GetComponent<Renderer>().material.color = Color.grey;
            this.GetComponent<Rigidbody2D>().simulated = false;
            killMePrompt.SetActive(false);
            killMeArea.SetActive(false);
            return;
        }
        //if im stunned, make me cyan and show any kill prompts (X button and kill radius);
        if (m_bStunned)
        {
            killMeArea.SetActive(true);
            this.GetComponent<Renderer>().material.color = Color.cyan;
            if (stunTimer.Tick(Time.deltaTime))
            {
                m_bStunned = false;
            }
        }
        else
        {
            killMeArea.SetActive(false);
            killMePrompt.SetActive(false);
            this.GetComponent<Renderer>().material.color = _playerColour;
        }
        if (m_iTimesPunched >= 2)
        {
            StunPlayer();
            GetComponent<Move>().StatusApplied();
            m_iTimesPunched = 0;
        }

    }

    public void StunPlayer()
    {
        //stun the player called outside of class
        m_bStunned = true;
    }

    public void KillPlayer()
    {
        //kill the player, called outside of class (mostly used for downed kills)
        m_bDead = true;
    }
}
