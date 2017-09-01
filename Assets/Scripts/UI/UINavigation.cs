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
        if (_EventSystem.currentSelectedGameObject)
        {
            for (int i = 0; i < XCI.GetNumPluggedCtrlrs(); i++)
            {
                if (XCI.GetButtonDown(XboxButton.DPadDown, XboxController.First + i))
                {
                    if (_EventSystem.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnDown != null)
                    //Debug.Log("left is null");
                    {
                        GameObject select = _EventSystem.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnDown.gameObject;
                        _EventSystem.SetSelectedGameObject(null);
                        _EventSystem.SetSelectedGameObject(select);
                    }
                }
                if (XCI.GetButtonDown(XboxButton.DPadUp, XboxController.First + i))
                {
                    if (_EventSystem.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnUp != null)
                    //Debug.Log("left is null");
                    {
                        GameObject select = _EventSystem.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnUp.gameObject;
                        _EventSystem.SetSelectedGameObject(null);
                        _EventSystem.SetSelectedGameObject(select);
                    }
                }
                if (XCI.GetButtonDown(XboxButton.DPadLeft, XboxController.First + i))
                {
                    if (_EventSystem.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnLeft != null)
                    //Debug.Log("left is null");
                    {
                        GameObject select = _EventSystem.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnLeft.gameObject;
                        _EventSystem.SetSelectedGameObject(null);
                        _EventSystem.SetSelectedGameObject(select);
                    }
                }
                if (XCI.GetButtonDown(XboxButton.DPadRight, XboxController.First + i))
                {
                    if (_EventSystem.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnRight != null)
                    //Debug.Log("left is null");
                    {
                        GameObject select = _EventSystem.currentSelectedGameObject.GetComponent<Button>().navigation.selectOnRight.gameObject;
                        _EventSystem.SetSelectedGameObject(null);
                        _EventSystem.SetSelectedGameObject(select);
                    }
                }
            }
        }
    }
}
