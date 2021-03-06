﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour, IHitByBullet, Reset
{
    public bool KeepBreaking = false;
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
    Sprite SolidGlass;
    public Sprite BrokenGlass;
    SpriteRenderer GlassSpriteRenderer;
    bool IsShattered = false;
    BoxCollider2D GlassCollider;
    int StoredSortingLayer;
    int m_iHealth = 2;
    public Sprite m_CrackedImage = null;
    public AudioClip m_crackingSound = null;
    private AudioSource m_audioSource;
    // Use this for initialization
    void Start()
    {
        m_audioSource = this.GetComponent<AudioSource>();
        SolidGlass = GetComponent<Sprite>();
        StoredSortingLayer = GetComponent<SpriteRenderer>().sortingOrder;
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
        if (hit.gameObject.layer == LayerMask.NameToLayer("Pickup"))
        {
            if (hit.transform.tag == "1hMelee" || hit.transform.tag == "2hMelee" && !GetComponent<Grenade>())
            {
                if (hit.transform.GetComponent<Rigidbody2D>().velocity.magnitude > 4 || hit.transform.GetComponent<Melee>().m_bAttacking == true)
                {
                    Shatter();
                    SpawnShards(hit);
                    GetComponent<HitByMeleeAction>().PlaySound();
                    
                    //  this.GetComponent<HitByMeleeAction>().HitByMelee(hit.transform.GetComponentInParent<Melee>(), null);
                }
            }
            else
            {
                if (hit.transform.GetComponent<Rigidbody2D>().velocity.magnitude > 4)
                {
                    Shatter();
                    SpawnShards(hit);
                    GetComponent<HitByMeleeAction>().PlaySound();
                    //  this.GetComponent<HitByMeleeAction>().HitByMelee(hit.transform.GetComponentInParent<Melee>(), null);
                }
            }

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
            if(hit.transform.parent.GetComponent<Melee>())
            if (hit.transform.parent.GetComponent<Melee>().m_bAttacking == true)
            {
                if (hit.transform.parent.GetComponent<Melee>())
                {
                    Shatter();
                    SpawnShards(hit);
                        GetComponent<HitByMeleeAction>().PlaySound();
                    }
                //  this.GetComponent<HitByMeleeAction>().HitByMelee(null, null);
            }
        }
        if ((hit.transform.root.tag == "2hMelee" || hit.transform.root.tag == "1hMelee") && IsShattered == false)
            if (hit.transform.root.GetComponent<Melee>()) // Null Check
                if (hit.transform.root.GetComponent<Melee>().m_bAttacking == true)
                {
                    Shatter();
                    SpawnShards(hit);
                    GetComponent<HitByMeleeAction>().PlaySound();
                }
    }

    public void HitGlass(Vector3 velocity, Vector3 hitPoint, int a_iDamage = 0)
    {
        m_iHealth -= a_iDamage;
        if (m_iHealth <= 0)
        {
            HitByBullet(velocity, hitPoint);
            foreach (IHitByMelee item in this.GetComponents<IHitByMelee>())
            {
                item.HitByMelee(null, null);
            }
        }
        if (m_iHealth != 0)
        {
            m_audioSource.clip = m_crackingSound;
            m_audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            m_audioSource.Play();
            this.transform.GetComponent<SpriteRenderer>().sprite = m_CrackedImage;
        }
    }

    void Shatter()
    {
        if (!KeepBreaking)
        {
            GlassSpriteRenderer.sprite = BrokenGlass;
            GlassCollider.enabled = false;
            IsShattered = true;
            GlassSpriteRenderer.sortingOrder = -10;

            m_audioSource.clip = m_crackingSound;
            m_audioSource.Play();
        }
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
            shardObjects[i] = Instantiate(Shards[i], this.transform.position + this.transform.up * UnityEngine.Random.Range(-1f, 0f), this.transform.rotation, transform);

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
        //make a temporary array to hold all the shard objects
        GameObject[] shardObjects = new GameObject[Shards.Length];
        //for every shard, instantiate them and set their rotation and velocity.
        //Their velocity will be based on the velocity of the bullet hitting them
        //for shards indexes 4 and below, their angular velocity will be changed to the bullet's velocity magnitude.
        //The shards will be instantiated along the object's up vector, the range will be UnityEngine.Randomed so they spawn in UnityEngine.Random locations of the glass piece
        for (int i = 0; i < Shards.Length; i++)
        {

            shardObjects[i] = Instantiate(Shards[i], this.transform.position + this.transform.up * UnityEngine.Random.Range(-1f, 0f), this.transform.rotation, transform);
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

            shardObjects[i] = Instantiate(Shards[i], this.transform.position + this.transform.up * UnityEngine.Random.Range(-1f, 0f), this.transform.rotation, transform);
            shardObjects[i].transform.rotation = Quaternion.Euler(Vector3.forward * UnityEngine.Random.Range(40, 180));
            //shardObjects[i].GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity * UnityEngine.Random.Range(0.03f , .1f);
            shardObjects[i].GetComponent<Rigidbody2D>().AddForce(a_Vecocity * UnityEngine.Random.Range(0.03f, .1f), ForceMode2D.Impulse);
            if (i < 4)
            {
                shardObjects[i].GetComponent<Rigidbody2D>().angularVelocity = a_Vecocity.magnitude;
            }
        }
    }



    public void Reset()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        IsShattered = false;
        GlassCollider.enabled = true;
        GlassSpriteRenderer.sortingOrder = StoredSortingLayer;
        GlassSpriteRenderer.sprite = SolidGlass;
    }
}
