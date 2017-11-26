using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricManager : MonoBehaviour {
    ElectricField[] FieldArray;
    float Timer;
    int WallToMove = 4;
    bool BeginConstriction = false;
    int WallsDisabled = 0; public void SetWallsDisabled(int a_int) { WallsDisabled = a_int;  } public void SetWallsDisabled() { WallsDisabled++; }   

    public int TimeTillConstriction;
    public bool ConstrictSeperately = true;
    public float TimeBetweenConstrictions;
    
    // Use this for initialization
    void Start () {
         FieldArray = new ElectricField[4];
        for(int i = 0; i < FieldArray.Length; ++i)
        {
            FieldArray[i] = transform.GetChild(i).GetComponent<ElectricField>();
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (WallsDisabled != 4 && ConstrictSeperately)
        {
            Timer += Time.deltaTime;
            if (Timer > TimeTillConstriction)
            {
                BeginConstriction = true;
            }
            if (BeginConstriction)
            {
                if (Timer > TimeBetweenConstrictions)
                {
                    Constrict();
                    Timer = 0;
                }
            }
        }
        else if(!ConstrictSeperately)
        {
            Timer += Time.deltaTime;
            if (Timer > TimeTillConstriction)
            {
                ConstrictAll();
                Timer = -9999;
            }

        }
	}


    void Constrict()
    {
        if (WallToMove != 4) // Disable movement on last wall.
        {
            FieldArray[WallToMove].GetComponent<ElectricField>().SetEnabledConstriction(false);
        }
        bool WallSelected = false;
        while (WallSelected == false)
        {
            WallToMove = Random.Range(0, 4);
            if (!FieldArray[WallToMove].GetComponent<ElectricField>().GetPreventConstriction())
            {
                FieldArray[WallToMove].GetComponent<ElectricField>().SetEnabledConstriction(true);
                WallSelected = true;
            }
        }
    }
    void ConstrictAll()
    {
        for (int i = 0; i < FieldArray.Length; ++i)
        {
            FieldArray[i].GetComponent<ElectricField>().SetEnabledConstriction(true);
        }
    }

}
