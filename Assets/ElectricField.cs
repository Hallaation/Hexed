using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricField : MonoBehaviour {
   public float TimeTillConstrict;
    float CurrentTime;

    [SerializeField]
    enum Position { TOP, BOTTOM, RIGHT, LEFT }; 
    [SerializeField]
    private Position mPosition = Position.TOP;
    public float GrowthSpeed;
    Vector3 GrowthY;
    Vector3 GrowthX;
    // Use this for initialization
    void Start ()
    {
        CurrentTime = 0;
        GrowthX = new Vector3(GrowthSpeed, 0, 0);
        GrowthY = new Vector3(0, GrowthSpeed, 0);
        Debug.Log(mPosition);
	}
	
	// Update is called once per frame
	void Update ()
    {
        CurrentTime += Time.deltaTime;
        if(CurrentTime > TimeTillConstrict)
        {
            Constrict();
        }
	}

    void Constrict()
    {
        switch (mPosition)
        {
            case Position.TOP:
                this.transform.localScale += GrowthY * Time.deltaTime;
                break;
            case Position.RIGHT:
                this.transform.localScale += GrowthX * Time.deltaTime;
                break;
            default:
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Stunned" || other.tag == "Player")
        {
            if (other.transform.root.GetComponent<PlayerStatus>())
            {
                PlayerStatus PlayerKilledByWall = other.transform.root.GetComponent<PlayerStatus>();
                PlayerKilledByWall.KillPlayer(PlayerKilledByWall);
            }
        }
    }
    //private void OnCollisionEnter2D(Collision2D other)
    //{
    //    if (other.transform.tag == "Stunned" || other.transform.tag == "Player")
    //    {
    //        if (other.transform.root.GetComponent<PlayerStatus>())
    //        {
    //            PlayerStatus PlayerKilledByWall = other.transform.root.GetComponent<PlayerStatus>();
    //            PlayerKilledByWall.KillPlayer(PlayerKilledByWall);
    //        }
    //    }
    //}
}

