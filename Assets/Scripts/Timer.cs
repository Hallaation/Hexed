using System;
using System.Collections;
using System.Collections.Generic;

public class Timer
{
    //generic timer class
    /// <summary>
    /// Constructor, Time to wait to determine how long the timer will last
    /// </summary>
    /// <param name="a_fTimeToWait"></param>
    public Timer(ref float a_fTimeToWait)
    {
        mfTimeToWait = a_fTimeToWait;
    }

    public Timer(float a_fTimeToWait)
    {
        mfTimeToWait = a_fTimeToWait;
    }
    //Time the timer is going to tick for
    public float mfTimeToWait = 0;
    private float mfTimer = 0;


    public float CurrentTime { get { return mfTimer; } set { mfTimer = value; } }

    public void SetTimer(float SetTime) { mfTimer = SetTime; }

    /// <summary>
    /// Increments the timer based on the time increment argument (usually deltatime)
    /// Returns true or false. True if the timer has finished, false if it still hasnt reached the max
    /// </summary>
    /// <param name="timeIncrement"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Using a while loop will pause the current thread until the timer has ended
    /// </summary>
    /// <param name="a_fIncrement"></param>
    /// <returns></returns>
    public bool WaitForEnd(float a_fIncrement)
    {
        while (mfTimer <= mfTimeToWait)
        {
            mfTimer += a_fIncrement;
        }
        return true;
    }

}
