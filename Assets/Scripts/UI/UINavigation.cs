using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XboxCtrlrInput;
using XInputDotNetPure;
public class UINavigation : MonoBehaviour
{
    EventSystem _EventSystem;

    private static UINavigation mInstance;

    public static UINavigation Instance
    {
        get
        {
            if (!mInstance)
            {
                mInstance = (UINavigation)FindObjectOfType(typeof(UINavigation));

                if (!mInstance)
                {
                    mInstance = (new GameObject("UINavigation")).AddComponent<UINavigation>();
                }
                DontDestroyOnLoad(mInstance.gameObject);
            }
            return mInstance;
        }
    }

    // Use this for initialization
    void Start()
    {
        SingletonTester.Instance.AddSingleton(this);

        _EventSystem = FindObjectOfType<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        //only look for objects if the event system is selecting something
        //if (_EventSystem.currentSelectedGameObject)
        //{
        //    //Instead of searching, all of these navigation will have to be set in the inspector/editor itself. 
        //    //For every directional button, get the button's navigation on up/down/left/right and set the event system's selected object to these.
        //}

        for (int i = 0; i < XCI.GetNumPluggedCtrlrs(); i++)
        {
            if (XCI.GetButtonDown(XboxButton.DPadDown, XboxController.First + i))
            {
                Debug.Log("Dpad down");
            }
            if (XCI.GetButtonDown(XboxButton.DPadUp, XboxController.First + i))
            {
                Debug.Log("Dpad up");
            }
            if (XCI.GetButtonDown(XboxButton.DPadLeft, XboxController.First + i))
            {
                Debug.Log("Dpad left");
            }
            if (XCI.GetButtonDown(XboxButton.DPadRight, XboxController.First + i))
            {
                Debug.Log("Dpad right");
            }
        }
    }
}
