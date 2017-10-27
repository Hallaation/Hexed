using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyPauser : MonoBehaviour
{
    Vector3 originalVelocity;
    float originalAngularVelocity;
    Rigidbody2D m_RigidBody;

    void Start()
    {
        originalVelocity = new Vector3();
        m_RigidBody = this.GetComponent<Rigidbody2D>();
    }

    public void PauseRigidbody()
    {
        if (!m_RigidBody)
        {
            m_RigidBody = GetComponent<Rigidbody2D>();
        }
        originalVelocity = m_RigidBody.velocity;
        originalAngularVelocity = m_RigidBody.angularVelocity;
        m_RigidBody.velocity = Vector3.zero;
        m_RigidBody.angularVelocity = 0;
        
    }

    public void UnpauseRigidbody()
    {
        m_RigidBody.velocity = originalVelocity;
        m_RigidBody.angularVelocity = originalAngularVelocity;
    }
}
