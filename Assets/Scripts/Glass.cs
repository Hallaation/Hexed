using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour {
    public GameObject Shard1;
    public GameObject Shard2;
    public GameObject Shard3;
    public GameObject Shard4;
    public GameObject Shard5;
    public GameObject Shard6;
    public GameObject Shard7;
    public GameObject Shard8;
    public Sprite BrokenGlass;
    SpriteRenderer GlassSpriteRenderer;
    bool IsShattered = false;
    BoxCollider2D GlassCollider;
    // Use this for initialization
    void Start () {
        GlassSpriteRenderer = GetComponent<SpriteRenderer>();
        GlassCollider = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
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
       
        GameObject Shard1Object = Instantiate(Shard1, this.transform.position + new Vector3(.2f, 0, 0), this.transform.rotation);
        GameObject Shard2Object = Instantiate(Shard2, this.transform.position + new Vector3(-.2f, 0, 0), this.transform.rotation);
        GameObject Shard3Object = Instantiate(Shard3, this.transform.position, this.transform.rotation);
        GameObject Shard4Object = Instantiate(Shard4, this.transform.position + new Vector3(-.1f, 0, 0), this.transform.rotation);
        GameObject Shard5Object = Instantiate(Shard5, this.transform.position + new Vector3(-.5f, 0, 0), this.transform.rotation);
        GameObject Shard6Object = Instantiate(Shard6, this.transform.position + new Vector3(-.4f, 0, 0), this.transform.rotation);
        GameObject Shard7Object = Instantiate(Shard7, this.transform.position + new Vector3(.3f, 0, 0), this.transform.rotation);
        GameObject Shard8Object = Instantiate(Shard8, this.transform.position + new Vector3(.4f, 0, 0), this.transform.rotation);

        Shard1Object.GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Bullet>().GetPreviousVelocity() * .01f;
        Shard2Object.GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Bullet>().GetPreviousVelocity() * .08f;
        Shard3Object.GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Bullet>().GetPreviousVelocity() * .015f;
        Shard4Object.GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Bullet>().GetPreviousVelocity() * .1f;
        Shard5Object.GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Bullet>().GetPreviousVelocity() * .015f;
        Shard6Object.GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Bullet>().GetPreviousVelocity() * .092f;
        Shard7Object.GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Bullet>().GetPreviousVelocity() * .1f;
        Shard8Object.GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Bullet>().GetPreviousVelocity() * .075f;

        Shard1Object.GetComponent<Rigidbody2D>().angularVelocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity.x + hit.gameObject.GetComponent<Rigidbody2D>().velocity.y;
        Shard2Object.GetComponent<Rigidbody2D>().angularVelocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity.x + hit.gameObject.GetComponent<Rigidbody2D>().velocity.y;
        Shard3Object.GetComponent<Rigidbody2D>().angularVelocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity.x + hit.gameObject.GetComponent<Rigidbody2D>().velocity.y;
        Shard4Object.GetComponent<Rigidbody2D>().angularVelocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity.x + hit.gameObject.GetComponent<Rigidbody2D>().velocity.y;
    }
    void SpawnShards(Collider2D hit)
    {

        GameObject Shard1Object = Instantiate(Shard1, this.transform.position + new Vector3(.2f, .2f, 0), this.transform.rotation);
        GameObject Shard2Object = Instantiate(Shard2, this.transform.position + new Vector3(-.2f, 0, 0), this.transform.rotation);
        GameObject Shard3Object = Instantiate(Shard3, this.transform.position, this.transform.rotation);
        GameObject Shard4Object = Instantiate(Shard4, this.transform.position + new Vector3(-.1f, -.1f, 0), this.transform.rotation);
        GameObject Shard5Object = Instantiate(Shard5, this.transform.position + new Vector3(-.5f, -.1f, 0), this.transform.rotation);
        GameObject Shard6Object = Instantiate(Shard6, this.transform.position + new Vector3(-.4f, -.3f, 0), this.transform.rotation);
        GameObject Shard7Object = Instantiate(Shard7, this.transform.position + new Vector3(.3f, .5f, 0), this.transform.rotation);
        GameObject Shard8Object = Instantiate(Shard8, this.transform.position + new Vector3(.4f, .1f, 0), this.transform.rotation);

        Shard1Object.GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity * .01f;
        Shard2Object.GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity * .08f;
        Shard3Object.GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity * .015f;
        Shard4Object.GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity * .1f;
        Shard5Object.GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity * .015f;
        Shard6Object.GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity * .092f;
        Shard7Object.GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity * .1f;
        Shard8Object.GetComponent<Rigidbody2D>().velocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity * .075f;

        Shard1Object.GetComponent<Rigidbody2D>().angularVelocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity.x + hit.gameObject.GetComponent<Rigidbody2D>().velocity.y;
        Shard2Object.GetComponent<Rigidbody2D>().angularVelocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity.x + hit.gameObject.GetComponent<Rigidbody2D>().velocity.y;
        Shard3Object.GetComponent<Rigidbody2D>().angularVelocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity.x + hit.gameObject.GetComponent<Rigidbody2D>().velocity.y;
        Shard4Object.GetComponent<Rigidbody2D>().angularVelocity = hit.gameObject.GetComponent<Rigidbody2D>().velocity.x + hit.gameObject.GetComponent<Rigidbody2D>().velocity.y;
    }
}
