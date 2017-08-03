using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairClamp : MonoBehaviour {
    
    private float m_fRotation = 45;


    void FixedUpdate()
    {
        this.transform.rotation = Quaternion.Euler(new Vector3(0 , 0 , m_fRotation));
    }
}
