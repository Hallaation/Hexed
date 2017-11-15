using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    public float BloodTravelSpeed = 1;
    public float GrowthSpeed = 1.2f;
    public bool MaxSize;
    SpriteRenderer GlassSpriteRenderer;
    public GameObject[] BloodSprites;
    public bool BloodSpawner = false;
    // Use this for initialization
    void Start()
    {
        MaxSize = false;
        GlassSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (MaxSize == false && BloodSpawner == false )
        {
            if (transform.localScale.sqrMagnitude < 2.99)
            {
                transform.localScale += transform.localScale * (Time.deltaTime * GrowthSpeed);

            }
            else
                MaxSize = true;
        }
    }

    public void CreateBloodSplatter(Vector3 a_Position, Vector3 a_Velocity, Quaternion a_Rotation)
    {
        int RandomBlood = Random.Range(0, BloodSprites.Length);
        Vector3 NormalizedVelocity = a_Velocity.normalized;
        Vector3 AdjustedPosition = a_Position + (NormalizedVelocity);
        GameObject spawnedBlood = Instantiate(BloodSprites[RandomBlood], AdjustedPosition, a_Rotation);
        spawnedBlood.transform.localScale *= .2f;
        spawnedBlood.GetComponent<Rigidbody2D>().velocity = new Vector3(0,0,0);
        spawnedBlood.GetComponent<Rigidbody2D>().AddForce(a_Velocity.normalized * BloodTravelSpeed, ForceMode2D.Impulse);
    }
    void Reset()
    {
        Destroy(this);
    }
}
