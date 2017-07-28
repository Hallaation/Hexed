using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    //generic timer class
    public Timer(float a_fTimeToWait)
    {
        mfTimeToWait = a_fTimeToWait;
    }

    private float mfTimeToWait = 0;
    private float mfTimer = 0; public void set(float a) { mfTimer = a; } public float get() { return mfTimer; }

    public bool Tick(float timeIncrement)
    {
        mfTimer += timeIncrement;
        if (mfTimer >= mfTimeToWait)
        {
            mfTimer = 0;
            return true;
        }
        return false;
    }

    public bool WaitForEnd(float a_fIncrement)
    {
        while (mfTimer <= mfTimeToWait)
        {
            mfTimer += a_fIncrement;
        }
        return true;
    }

}
