using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour, IHitByBullet, IHitByMelee
{
    //? What is an array
    public GameObject Shard1;
    public GameObject Shard2;
    public GameObject Shard3;
    public GameObject Shard4;
    public GameObject Shard5;
    public GameObject Shard6;
    public GameObject Shard7;
    public GameObject Shard8;

    //[Space]
    //[Header("Glass Audio")]
    //public AudioClip m_BreakingClip;
    //public float clipAudio;

    //[Space]
    private GameObject[] Shards;
    //private AudioSource m_audioSource;

    public Sprite BrokenGlass;
    SpriteRenderer GlassSpriteRenderer;
    bool IsShattered = false;
    BoxCollider2D GlassCollider;
    // Use this for initialization
    void Start()
    {
        //if (this.GetComponent<AudioSource>())
        //{
        //    m_audioSource = this.gameObject.AddComponent<AudioSource>();
        //}
        //else
        //    m_audioSource = this.GetComponent<AudioSource>();

        GlassSpriteRenderer = GetComponent<SpriteRenderer>();
        GlassCollider = GetComponent<BoxCollider2D>();
        Shards = new GameObject[]
            {
                Shard1,
                Shard2,
                Shard3,
                Shard4,
                Shard5,
                Shard6,
                Shard7,
                Shard8,
            };
    }

    // Update is called once per frame

    void OnCollisionEnter2D(Collision2D hit)                    //! when u throw bat
    {

        if (hit.transform.tag == "2hMelee")
        {

            Shatter();
            SpawnShards(hit);
            
          //  this.GetComponent<HitByMeleeAction>().HitByMelee(hit.transform.GetComponentInParent<Melee>(), null);
        }
        else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Bullet") && IsShattered == false)
        {

            Shatter();
            SpawnShards(hit);
            Destroy(hit.gameObject);
        }
    }

    //For Melee weapon collisions
    private void OnTriggerEnter2D(Collider2D hit)
    {
        if ((hit.transform.parent.tag == "2hMelee" || hit.transform.parent.tag == "1hMelee" || hit.transform.gameObject.layer == LayerMask.NameToLayer("Bullet")) && IsShattered == false)
        {
            if (hit.transform.parent.GetComponent<Melee>().m_bAttacking == true)
            {
                Shatter();
                SpawnShards(hit);
              //  this.GetComponent<HitByMeleeAction>().HitByMelee(null, null);
            }
        }
        if ((hit.transform.root.tag == "2hMelee" || hit.transform.root.tag == "1hMelee") && IsShattered == false)
            if (hit.transform.root.GetComponent<Melee>()) // Null Check
                if (hit.transform.root.GetComponent<Melee>().m_bAttacking == true)
                {
                    Shatter();
                    SpawnShards(hit);
                    this.GetComponent<HitByMeleeAction>().HitByMelee(null, null);
                }
    }

    void Shatter()
    {
        GlassSpriteRenderer.sprite = BrokenGlass;
        GlassCollider.enabled = false;
        IsShattered = true;
        GlassSpriteRenderer.sortingOrder = -10;
        //m_audioSource.PlayOneShot(m_BreakingClip);
    }

    void SpawnShards(Collision2D hit)
    {
        //make a temporary array to hold all the shard objects
        GameObject[] shardObjects = new GameObject[Shards.Length];
        //for every shard, instantiate them and set their rotation and velocity.
        //Their velocity will be based on the velocity of the bullet hitting them
        //for shards indexes 4 and below, their angular velocity will be changed to the bullet's velocity magnitude.
        //The shards will be instantiated along the object's up vector, the range will be UnityEngine.Randomed so they spawn in UnityEngine.Random locations of the glass piece

        for (int i = 0; i < Shards.Length; i++)
        {
            shardObjects[i] = Instantiate(Shards[i], this.transform.position + this.transform.up * UnityEngine.Random.Range(-1f, 0f), this.transform.rotation);
            shardObjects[i].transform.rotation = Quaternion.Euler(Vector3.forward * UnityEngine.Random.Range(40, 180));
            //shardObjects[i].GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity * UnityEngine.Random.Range(0.03f , .1f);
            if (hit.gameObject.GetComponent<Bullet>())
                shardObjects[i].GetComponent<Rigidbody2D>().AddForce(hit.gameObject.GetComponent<Bullet>().GetPreviousVelocity() * UnityEngine.Random.Range(0.03f, .1f), ForceMode2D.Impulse);
            if (hit.transform.GetComponent<Melee>())
                shardObjects[i].GetComponent<Rigidbody2D>().AddForce(hit.gameObject.GetComponent<Melee>().GetPreviousVelocity() * UnityEngine.Random.Range(.03f, .1f), ForceMode2D.Impulse);
            if (i < 4)
            {
                shardObjects[i].GetComponent<Rigidbody2D>().angularVelocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
            }
        }

    }
    void SpawnShards(Collider2D hit)
    {
      //  Debug.Log("Does this ever get called");
        //make a temporary array to hold all the shard objects
        GameObject[] shardObjects = new GameObject[Shards.Length];
        //for every shard, instantiate them and set their rotation and velocity.
        //Their velocity will be based on the velocity of the bullet hitting them
        //for shards indexes 4 and below, their angular velocity will be changed to the bullet's velocity magnitude.
        //The shards will be instantiated along the object's up vector, the range will be UnityEngine.Randomed so they spawn in UnityEngine.Random locations of the glass piece
        for (int i = 0; i < Shards.Length; i++)
        {

            shardObjects[i] = Instantiate(Shards[i] , this.transform.position + this.transform.up * UnityEngine.Random.Range(-1f, 0f), this.transform.rotation);
            shardObjects[i].transform.rotation = Quaternion.Euler(Vector3.forward * UnityEngine.Random.Range(40, 180));
            //shardObjects[i].GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity * UnityEngine.Random.Range(0.03f , .1f);
            if (hit.transform.tag == "2hMelee")
            {
                shardObjects[i].GetComponent<Rigidbody2D>().AddForce(hit.GetComponentInParent<Melee>().KnockBack * hit.transform.right * UnityEngine.Random.Range(4.5f, 15f), ForceMode2D.Impulse);
            }
            else
            {
                shardObjects[i].GetComponent<Rigidbody2D>().AddForce(hit.transform.root.GetComponent<Rigidbody2D>().velocity * UnityEngine.Random.Range(0.3f, 1f), ForceMode2D.Impulse);
            }
            if (i < 4)
            {
                shardObjects[i].GetComponent<Rigidbody2D>().angularVelocity = hit.transform.root.GetComponent<Rigidbody2D>().velocity.magnitude;
            }
        }

    }


    public void HitByBullet(Vector3 a_Vecocity, Vector3 HitPoint)
    {
        Shatter();

        //make a temporary array to hold all the shard objects
        GameObject[] shardObjects = new GameObject[Shards.Length];
        //for every shard, instantiate them and set their rotation and velocity.
        //Their velocity will be based on the velocity of the bullet hitting them
        //for shards indexes 4 and below, their angular velocity will be changed to the bullet's velocity magnitude.
        //The shards will be instantiated along the object's up vector, the range will be UnityEngine.Randomed so they spawn in UnityEngine.Random locations of the glass piece
        for (int i = 0; i < Shards.Length; i++)
        {
            
            shardObjects[i] = Instantiate(Shards[i], this.transform.position + this.transform.up * UnityEngine.Random.Range(-1f, 0f), this.transform.rotation);
            shardObjects[i].transform.rotation = Quaternion.Euler(Vector3.forward * UnityEngine.Random.Range(40, 180));
            //shardObjects[i].GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity * UnityEngine.Random.Range(0.03f , .1f);
            shardObjects[i].GetComponent<Rigidbody2D>().AddForce(a_Vecocity * UnityEngine.Random.Range(0.03f, .1f), ForceMode2D.Impulse);
            if (i < 4)
            {
                shardObjects[i].GetComponent<Rigidbody2D>().angularVelocity = a_Vecocity.magnitude;
            }
        }
    }

    public void HitByMelee(Weapon meleeWeapon, AudioClip soundEffect, float Volume, float Pitch)
    {
        /*
        Shatter();

        //make a temporary array to hold all the shard objects
        GameObject[] shardObjects = new GameObject[Shards.Length];
        //for every shard, instantiate them and set their rotation and velocity.
        //Their velocity will be based on the velocity of the bullet hitting them
        //for shards indexes 4 and below, their angular velocity will be changed to the bullet's velocity magnitude.
        //The shards will be instantiated along the object's up vector, the range will be UnityEngine.Randomed so they spawn in UnityEngine.Random locations of the glass piece
        for (int i = 0; i < Shards.Length; i++)
        {
            shardObjects[i] = Instantiate(Shard1, this.transform.position + this.transform.up * UnityEngine.Random.Range(-1f, 0f), this.transform.rotation);
            shardObjects[i].transform.rotation = Quaternion.Euler(Vector3.forward * UnityEngine.Random.Range(40, 180));

            if (meleeWeapon.transform.parent)
            {
                shardObjects[i].GetComponent<Rigidbody2D>().AddForce(this.transform.up * meleeWeapon.KnockBack * UnityEngine.Random.Range(0.03f, .1f), ForceMode2D.Impulse);
                if (i < 4)
                {
                    shardObjects[i].GetComponent<Rigidbody2D>().angularVelocity = meleeWeapon.KnockBack;
                }
            }
            else
            {
                shardObjects[i].GetComponent<Rigidbody2D>().AddForce(meleeWeapon.gameObject.GetComponent<Melee>().GetPreviousVelocity() * UnityEngine.Random.Range(.03f, .1f), ForceMode2D.Impulse);
                if (i < 4)
                {
                    shardObjects[i].GetComponent<Rigidbody2D>().angularVelocity = meleeWeapon.gameObject.GetComponent<Melee>().GetPreviousVelocity().magnitude;
                }
            }
                //shardObjects[i].GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity * UnityEngine.Random.Range(0.03f , .1f);
       
        
        }*/
    }


}
