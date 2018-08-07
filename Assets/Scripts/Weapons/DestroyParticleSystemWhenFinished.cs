using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticleSystemWhenFinished : MonoBehaviour {

    ParticleSystem ps;
	void Awake ()
    {
        ps = GetComponent<ParticleSystem>();
	}
	
	void Update ()
    {
        //if (ps)
        //{
        //    if (!ps.IsAlive())
        //    {
        //        Destroy(gameObject);
        //    }
        //}
	}
}
