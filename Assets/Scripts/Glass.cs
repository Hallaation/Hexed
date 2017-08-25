using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour
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

    private GameObject[] Shards;

    public Sprite BrokenGlass;
    SpriteRenderer GlassSpriteRenderer;
    bool IsShattered = false;
    BoxCollider2D GlassCollider;
    // Use this for initialization
    void Start()
    {
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

    void OnCollisionEnter2D(Collision2D hit)
    {
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Bullet") && IsShattered == false)
        {
            Shatter();
            SpawnShards(hit);
            Destroy(hit.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Bullet") && IsShattered == false)
        {
            Shatter();
            SpawnShards(hit);
        }
    }
    void Shatter()
    {
        GlassSpriteRenderer.sprite = BrokenGlass;
        GlassCollider.enabled = false;
        IsShattered = true;

    }

    void SpawnShards(Collision2D hit)
    {
        //make a temporary array to hold all the shard objects
        GameObject[] shardObjects = new GameObject[Shards.Length];
        //for every shard, instantiate them and set their rotation and velocity.
        //Their velocity will be based on the velocity of the bullet hitting them
        //for shards indexes 4 and below, their angular velocity will be changed to the bullet's velocity magnitude.
        //The shards will be instantiated along the object's up vector, the range will be randomed so they spawn in random locations of the glass piece
        
        for (int i = 0; i < Shards.Length; i++)
        {
            shardObjects[i] = Instantiate(Shard1 , this.transform.position + this.transform.up * Random.Range(-1f , 0f) , this.transform.rotation);
            shardObjects[i].transform.rotation = Quaternion.Euler(Vector3.forward * Random.Range(40 , 180));
            //shardObjects[i].GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity * Random.Range(0.03f , .1f);

            shardObjects[i].GetComponent<Rigidbody2D>().AddForce(hit.gameObject.GetComponent<Bullet>().GetPreviousVelocity() * Random.Range(0.03f , .1f), ForceMode2D.Impulse);
            if (i < 4)
            {
                shardObjects[i].GetComponent<Rigidbody2D>().angularVelocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
            }
        }

    }
    void SpawnShards(Collider2D hit)
    {
        Debug.Log("Does this ever get called");
        //make a temporary array to hold all the shard objects
        GameObject[] shardObjects = new GameObject[Shards.Length];
        //for every shard, instantiate them and set their rotation and velocity.
        //Their velocity will be based on the velocity of the bullet hitting them
        //for shards indexes 4 and below, their angular velocity will be changed to the bullet's velocity magnitude.
        //The shards will be instantiated along the object's up vector, the range will be randomed so they spawn in random locations of the glass piece
        for (int i = 0; i < Shards.Length; i++)
        {
            shardObjects[i] = Instantiate(Shard1 , this.transform.position + this.transform.up * Random.Range(-1f , 0f) , this.transform.rotation);
            shardObjects[i].transform.rotation = Quaternion.Euler(Vector3.forward * Random.Range(40 , 180));
            //shardObjects[i].GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity * Random.Range(0.03f , .1f);
            shardObjects[i].GetComponent<Rigidbody2D>().AddForce(hit.gameObject.GetComponent<Rigidbody2D>().velocity * Random.Range(0.03f , .1f) , ForceMode2D.Impulse);
            if (i < 4)
            {
                shardObjects[i].GetComponent<Rigidbody2D>().angularVelocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
            }
        }

    }
}
