using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using XboxCtrlrInput;
using XInputDotNetPure;
using UnityEngine.SceneManagement;


public class GamemodeSettings : MonoBehaviour
{

    public float[] mPointsToWin;
    private int PointWinIndex;

    private bool ResetSticks = false;
    private bool[] StickMovement; // index 0 for left horizontal, index 1 for right horizontal

    Text _SettingsValue;
    GameManagerc m_ManagerInstance;
    // Use this for initialization
    void Start()
    {
        _SettingsValue = GetComponentInChildren<Text>();
        StickMovement = new bool[2] { false , false };
        m_ManagerInstance = GameManagerc.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if ((DpadHorizontalTest() > 0) || StickMovement[1])
        {
            StickMovement[1] = false;
            if (PointWinIndex == mPointsToWin.Length - 1)
            {
                PointWinIndex = 0;
            }
            else
            {
                PointWinIndex++;
            }

        }
        else if ((DpadHorizontalTest() < 0) || StickMovement[0])
        {
            StickMovement[0] = false;
            Debug.Log("below 0");
            if (PointWinIndex == 0)
            {
                PointWinIndex = mPointsToWin.Length - 1;
            }
            else
            {
                Debug.Log("Decrement");
                PointWinIndex--;
            }

        }
        _SettingsValue.text = mPointsToWin[PointWinIndex].ToString("0");
        m_ManagerInstance.m_iPointsNeeded = (int)mPointsToWin[PointWinIndex];
    }


    int DpadHorizontalTest()
    {

        for (int i = 0; i < (int)PlayerIndex.Four; ++i)
        {
            if (XCI.GetButtonDown(XboxButton.DPadRight , XboxController.First + i))
            {
                return 1;
            }
            else if (XCI.GetButtonDown(XboxButton.DPadLeft , XboxController.First + i))
            {
                return -1;
            }

        }
        return 0;
    }

    void CheckForStickReset()
    {
        //check each controller
        for (int i = 0; i < (int)PlayerIndex.Four; ++i)
        {
            if (ResetSticks)
            {
                if (XCI.GetAxis(XboxAxis.LeftStickX , XboxController.First + i) < 0)
                {
                    StickMovement[0] = true;
                    ResetSticks = false;
                }
                else if (XCI.GetAxis(XboxAxis.LeftStickX , XboxController.First + i) > 0)
                {
                    StickMovement[1] = true;
                    ResetSticks = false;
                }
            }

        }
    }
}
