using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using XInputDotNetPure;

public class ControllerTest : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            if (XCI.GetButtonDown(XboxButton.DPadLeft , XboxController.First + i))
            {
                Debug.Log("DpadLeft on 4th controller");
            }
        }
    }
 
}
