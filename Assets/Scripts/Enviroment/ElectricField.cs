using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricField : MonoBehaviour {
   public float TimeTillConstrict;
    float CurrentTime;
    public bool Kill;
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
            if (Kill)
                ConstrictScale();
            else
                ConstrictPosition();
        }
	}

    void ConstrictScale()
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
    void ConstrictPosition()
    {
        switch (mPosition)
        {
            case Position.TOP:
                this.transform.localPosition += GrowthY * Time.deltaTime;
                break;
            case Position.RIGHT:
                this.transform.localPosition += GrowthX * Time.deltaTime;
                break;
            case Position.LEFT:
                this.transform.localPosition -= GrowthX * Time.deltaTime;
                break;
            case Position.BOTTOM:
                this.transform.localPosition -= GrowthY * Time.deltaTime;
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

