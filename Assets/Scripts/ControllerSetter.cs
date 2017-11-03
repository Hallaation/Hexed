using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using XInputDotNetPure;

public class ControllerSetter : MonoBehaviour
{


    public XboxController mXboxController;
    public PlayerIndex mPlayerIndex;
    public int m_playerNumber;

    public void SetController(PlayerIndex a_playerIndex)
    {
        //sets my controller ID, for input management.
        mPlayerIndex = a_playerIndex;
        switch (a_playerIndex)
        {
            case PlayerIndex.One:
                mXboxController = XboxController.First;
                break;
            case PlayerIndex.Two:
                mXboxController = XboxController.Second;
                break;
            case PlayerIndex.Three:
                mXboxController = XboxController.Third;
                break;
            case PlayerIndex.Four:
                mXboxController = XboxController.Fourth;
                break;
        }
    }

    public void SetController(XboxController xboxController)
    {
        mXboxController = xboxController; 
    }

    void Update()
    {
        switch (mXboxController)
        {
            case XboxController.First:
                mPlayerIndex = PlayerIndex.One;
                break;
            case XboxController.Second:
                mPlayerIndex = PlayerIndex.Two;
                break;
            case XboxController.Third:
                mPlayerIndex = PlayerIndex.Three;
                break;
            case XboxController.Fourth:
                mPlayerIndex = PlayerIndex.Four;
                break;
            default:
                break;
        }
    }
}


