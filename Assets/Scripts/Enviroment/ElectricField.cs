using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricField : MonoBehaviour {
   //public float TimeTillConstrict;
    float CurrentTime;
    bool EnabledConstriction = false; public void SetEnabledConstriction(bool a_bool) { EnabledConstriction = a_bool; }
    bool PreventConstriction = false; public bool GetPreventConstriction() {return PreventConstriction; } public void SetPreventConstriction(bool a_bool) { PreventConstriction = a_bool; }
     bool Kill;
    [SerializeField]
    enum Position { TOP, BOTTOM, RIGHT, LEFT }; 
    [SerializeField]
    private Position AxisToMoveOn = Position.TOP;
   float GrowthSpeed;
    Vector3 GrowthY; public void SetGrowthY(Vector3 Vector3Y) { GrowthY = Vector3Y ; }
    Vector3 GrowthX; public void SetGrowthX(Vector3 Vector3X) { GrowthX = Vector3X; }

    // Use this for initialization
    void Start ()
    {
        CurrentTime = 0;
      
        Debug.Log(AxisToMoveOn);
        GrowthSpeed = transform.parent.GetComponent<ElectricManager>().GrowthSpeed;
        Kill = transform.parent.GetComponent<ElectricManager>().Kill;
        GrowthX = new Vector3(GrowthSpeed, 0, 0);
        GrowthY = new Vector3(0, GrowthSpeed, 0);
    }

   

	// Update is called once per frame
	void Update ()
    {
        if (!PreventConstriction)
        {
            //CurrentTime += Time.deltaTime;
            //if (CurrentTime > TimeTillConstrict)
            //{

                if (EnabledConstriction == true)
                {
                if (Kill)
                    ConstrictScale(); // Constrict but increasing the scale of the field
                else
                    ConstrictPosition();  // Constrict by Drawing the position of the field closer to the center.
                }
            //}
        }
	}

    void ConstrictScale()
    {
        switch (AxisToMoveOn)
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
        switch (AxisToMoveOn)
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
        if (Vector3.Distance(transform.position,  transform.parent.transform.position) <= 1 )
        {
            PreventConstriction = true;
            transform.parent.GetComponent<ElectricManager>().SetWallsDisabled();
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

