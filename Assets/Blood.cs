using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    public float BloodTravelSpeed = 1;
    public float GrowthSpeed = 1.2f;
    public GameObject Blood1;
    public GameObject Blood2;
    public GameObject Blood3;
    public GameObject Blood4;
    public GameObject Blood5;
    public bool MaxSize;
    SpriteRenderer GlassSpriteRenderer;
    private GameObject[] BloodSprites;
    private Vector2 StoredVelocity;
    // Use this for initialization
    void Start()
    {
        MaxSize = false;
        GlassSpriteRenderer = GetComponent<SpriteRenderer>();
        BloodSprites = new GameObject[]
                {
               Blood1,
               Blood2,
               Blood3,
               Blood4,
               //Blood5,

                };
    }

    // Update is called once per frame
    void Update()
    {
        if (MaxSize == false) {
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
        int RandomBlood = (int)Random.Range(0, 3.999f);
        Vector3 NormalizedVelocity = a_Velocity.normalized;
        Vector3 AdjustedPosition = a_Position + (NormalizedVelocity);

        BloodSprites[RandomBlood] = Instantiate(BloodSprites[RandomBlood], AdjustedPosition, this.transform.rotation);
        BloodSprites[RandomBlood].transform.localScale = BloodSprites[RandomBlood].transform.localScale * .2f;
        BloodSprites[RandomBlood].GetComponent<Rigidbody2D>().velocity =  new Vector3(0,0,0);
        BloodSprites[RandomBlood].GetComponent<Rigidbody2D>().AddForce(a_Velocity.normalized * BloodTravelSpeed, ForceMode2D.Impulse);
    }
    void Reset()
    {
        Destroy(this);
    }
}
