using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
                //DontDestroyOnLoad(mInstance.gameObject);
            }
            return mInstance;
        }
    }


    // Use this for initialization
    void Awake()
    {
        //SingletonTester.Instance.AddSingleton(this);
        SceneManager.sceneLoaded += OnSceneLoad;
        //_EventSystem = FindObjectOfType<EventSystem>();

    }
    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        _EventSystem = FindObjectOfType<EventSystem>();
    }
    // Update is called once per frame
    void Update()
    {
        #region Update

        //only look for objects if the event system is selecting something
        //if (_EventSystem.currentSelectedGameObject)
        //{
        //    //Instead of searching, all of these navigation will have to be set in the inspector/editor itself. 
        //    //For every directional button, get the button's navigation on up/down/left/right and set the event system's selected object to these.
        //}
        if (_EventSystem != null)
        {
            if (_EventSystem.currentSelectedGameObject)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (XCI.GetButtonDown(XboxButton.DPadDown, XboxController.First + i) )
                    {
                        if (_EventSystem.currentSelectedGameObject.GetComponent<Selectable>().navigation.selectOnDown != null)
                        {
                            GameObject select = _EventSystem.currentSelectedGameObject.GetComponent<Selectable>().navigation.selectOnDown.gameObject;
                            _EventSystem.SetSelectedGameObject(null);
                            _EventSystem.SetSelectedGameObject(select);
                        }
                    }
                    if (XCI.GetButtonDown(XboxButton.DPadUp, XboxController.First + i))
                    {
                        if (_EventSystem.currentSelectedGameObject.GetComponent<Selectable>().navigation.selectOnUp != null)
                        {
                            //DpadMovement[0, 0] = false;   //useless
                            //temp.CurrentTime = 0;         //useless
                            GameObject select = _EventSystem.currentSelectedGameObject.GetComponent<Selectable>().navigation.selectOnUp.gameObject;
                            _EventSystem.SetSelectedGameObject(null);
                            _EventSystem.SetSelectedGameObject(select);
                        }
                    }
                    if (XCI.GetButtonDown(XboxButton.DPadLeft, XboxController.First + i))
                    {
                        //Slider case
                        if (_EventSystem.currentSelectedGameObject.GetComponent<Slider>())
                        {
                            Slider sld = _EventSystem.currentSelectedGameObject.GetComponent<Slider>();
                            //_EventSystem.currentSelectedGameObject.GetComponent<Slider>().value -= DecrementValue;
                            sld.normalizedValue -= 0.1f;
                        }

                        if (_EventSystem.currentSelectedGameObject.GetComponent<Selectable>().navigation.selectOnLeft != null)

                        {
                            GameObject select = _EventSystem.currentSelectedGameObject.GetComponent<Selectable>().navigation.selectOnLeft.gameObject;
                            _EventSystem.SetSelectedGameObject(null);
                            _EventSystem.SetSelectedGameObject(select);
                        }
                    }
                    if (XCI.GetButtonDown(XboxButton.DPadRight, XboxController.First + i))
                    {
                        //Slider Case
                        if (_EventSystem.currentSelectedGameObject.GetComponent<Slider>())
                        {
                            Slider sld = _EventSystem.currentSelectedGameObject.GetComponent<Slider>();
             
                            sld.normalizedValue += 0.1f;
                        }

                        if (_EventSystem.currentSelectedGameObject.GetComponent<Selectable>().navigation.selectOnRight != null)
                        {
                            GameObject select = _EventSystem.currentSelectedGameObject.GetComponent<Selectable>().navigation.selectOnRight.gameObject;
                            _EventSystem.SetSelectedGameObject(null);
                            _EventSystem.SetSelectedGameObject(select);
                        }
                    }
                }
            }
        }
        else
        {
            _EventSystem = GameObject.FindObjectOfType<EventSystem>();
        }

        #endregion
    }
}
