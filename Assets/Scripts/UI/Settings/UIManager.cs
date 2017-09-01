﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;

public class UIManager : MonoBehaviour
{
    Dictionary<string, string> MenuTransitionBoolParameters = new Dictionary<string, string>
    {
        {"First_Panel", "MoveToFirstPanel" },
        {"Second_Panel", "MoveToSecondPanel" },
        {"Third_Panel", "MoveToThirdPanel" }
    };
    //CS LUL
    public static UIManager mInstance;

    public Stack<GameObject> menuStatus;
    public GameObject defaultPanel;
    private GameObject selected;
    //only used if all else fails
    private GameObject defaultToReturnTo;
    private EventSystem _eventSystem;
    private Animator m_bMenuAnimator;
    private UINavigation uiNavigationInstance;
    public bool m_bRemoveLastPanel;
    public bool m_bInMainMenu = false;
    public bool RemoveLastPanel { get { return m_bRemoveLastPanel; } set { m_bRemoveLastPanel = value; } }
    // Use this for initialization


    public static UIManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                //If I already exist, make the instance that
                mInstance = (UIManager)FindObjectOfType(typeof(UIManager));

                if (mInstance == null)
                {
                    //if not found, make an object and attach me to it
                    mInstance = (new GameObject("ControllerManager")).AddComponent<UIManager>();
                }
                mInstance.gameObject.name = "Singleton container";
                DontDestroyOnLoad(mInstance.gameObject);

            }
            return mInstance;
        }
    }

    void Awake()
    {
        uiNavigationInstance = UINavigation.Instance;
        m_bMenuAnimator = FindObjectOfType<Canvas>().GetComponent<Animator>();
        //Add me to the singleton tester
        SingletonTester.Instance.AddSingleton(this);
        SceneManager.sceneLoaded += OnSceneLoad;

        //instance = FindObjectOfType<UIManager>();


        _eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        selected = _eventSystem.currentSelectedGameObject;
        menuStatus = new Stack<GameObject>();

        if (Instance.gameObject != this.gameObject)
        {
            Destroy(this.gameObject);
        }
    }

    void MainMenuUpdate()
    {
        CharacterSelectionManager.Instance.LetPlayersSelectCharacters = m_bMenuAnimator.GetCurrentAnimatorStateInfo(0).IsName("Title_Section_Second_Static");
        if (menuStatus.Peek().name == "First_Panel")
        {
            //Change the button animation according to what is selected.
            switch (_eventSystem.currentSelectedGameObject.name)
            {
                case "Credits_Button": break;
                case "Settings_Button": break;
                case "Quit_Button": break;
                case "VS_Button": break;
                default: break;
            }
        }

        for (int i = 0; i < XCI.GetNumPluggedCtrlrs(); ++i)
        {
            if (XCI.GetButtonDown(XboxButton.B, XboxController.First + i))
            {
                //Scan for every plugged in controller B button
                MainMenuBack();
            }
        }

    }

    public void MainMenuBack()
    {
        //If the menu status only has 1 object in it, dont do anything
        if (menuStatus.Count == 1 || CharacterSelectionManager.Instance.JoinedPlayers > 0)
        {
            return;
        }
        else
        {
            menuStatus.Pop();
            //Swap the trigger to whatever the parameter is.
            m_bMenuAnimator.SetTrigger(MenuTransitionBoolParameters[menuStatus.Peek().name]);
            DefaultButton temp = menuStatus.Peek().GetComponent<DefaultButton>();
            if (temp)
            {
                //Set the default button
                SetCurrentSelected(temp.defaultButton);
            }
        }
    }

    public void MainMenuChangePanel(GameObject panelToMove)
    {
        if (!menuStatus.Contains(panelToMove))
        {
            menuStatus.Push(panelToMove);
            //swap the trigger to the corresponding parameter name
            m_bMenuAnimator.SetTrigger(MenuTransitionBoolParameters[panelToMove.name]);
            //Find the default button, if none found, set to null
            DefaultButton temp = panelToMove.GetComponent<DefaultButton>();
            if (temp)
            {
                //Set the default button
                SetCurrentSelected(temp.defaultButton);
            }
            else
            {
                SetCurrentSelected(null);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {

        if (m_bInMainMenu)
        {
            MainMenuUpdate();
            return;
        }
        Debug.Log("I should never reach here");
        //if the current selected is null
        if (menuStatus.Count > 0)
        {
            if (!GameObject.Find("EventSystem").GetComponent<EventSystem>().currentSelectedGameObject)
            {
                //set the selected
                Debug.Log("Currently selecting null");
                //when all else fails, return to a default
                if (!selected)
                {
                    selected = defaultToReturnTo;
                }
                _eventSystem.SetSelectedGameObject(selected);
            }
            else
            {
                selected = _eventSystem.currentSelectedGameObject;
            }
        }
        //If I press B return to the previous UI thing.
        for (int i = 0; i < (int)XInputDotNetPure.PlayerIndex.Four; ++i)
        {
            if (XCI.GetButtonDown(XboxButton.B, XboxController.First + i))
            {
                Back();
            }

            //If I press start and the menu status (stack of UI elements) is nothing, push the default panel into the stack.
            if (XCI.GetButton(XboxButton.Start, XboxController.First + i) && menuStatus.Count <= 0)
            {
                if (!menuStatus.Contains(defaultPanel))
                {
                    menuStatus.Push(defaultPanel);
                    defaultPanel.SetActive(true);
                    _eventSystem.SetSelectedGameObject(null);
                    _eventSystem.SetSelectedGameObject(defaultPanel.GetComponentInChildren<Button>().gameObject);
                }
            }

        }
    }

    public void Back()
    {
        //set the current menu (whatever it is), turn it off then pop it off the stack.
        //If I don't want to remove the default panel and the top of the stack is the default,  exit the function
        //if (!RemoveDefaultPanel && menuStatus.Peek() == defaultPanel) 
        //    return;

        //if the menu status count is 1 and I don't want to remove the last panel, exit out of the function
        if (menuStatus.Count == 1 && m_bRemoveLastPanel)
        {
            return;
        }

        if (menuStatus.Count >= 1)
        {
            menuStatus.Peek().SetActive(false);
            menuStatus.Pop();
            if (menuStatus.Count >= 1)
            {
                menuStatus.Peek().SetActive(true);

                //find defaults if there are any and set it to my selected.
                if (menuStatus.Peek().transform.Find("DefaultButton"))
                {
                    _eventSystem.SetSelectedGameObject(null);
                    _eventSystem.SetSelectedGameObject(menuStatus.Peek().GetComponentInChildren<Button>().gameObject);
                    defaultToReturnTo = menuStatus.Peek().GetComponentInChildren<Button>().gameObject;
                }
            }
        }
    }


    //whatever the element is, push it into the stack 
    public void OpenUIElement(GameObject ElementToOpen, bool openChildren = false)
    {

        //Debug.Log(current);
        if (!menuStatus.Contains(ElementToOpen))
        {
            if (menuStatus.Count > 0)
                menuStatus.Peek().SetActive(false);
            //Debug.Log(ElementToOpen);
            //Debug.Break();
            ElementToOpen.SetActive(true);
            if (ElementToOpen.transform.childCount > 0 && openChildren)
            {
                for (int i = 0; i < ElementToOpen.transform.childCount; ++i)
                {
                    ElementToOpen.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
            menuStatus.Push(ElementToOpen.gameObject);
            _eventSystem.SetSelectedGameObject(null);

            //find defaults if there are any and set it to my selected.
            //Debug.Log();
            if (menuStatus.Peek().GetComponentInChildren<Button>())
            {
                _eventSystem.SetSelectedGameObject(null);
                _eventSystem.SetSelectedGameObject(menuStatus.Peek().GetComponentInChildren<Button>().gameObject);
                defaultToReturnTo = menuStatus.Peek().GetComponentInChildren<Button>().gameObject;
            }
        }

    }



    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        menuStatus.Clear();
        //very specific scene, if in this scene, I want to remove the last panel
        if (scene.buildIndex == 2)
        {
            m_bRemoveLastPanel = true;
        }

        else if (scene.buildIndex != 0) //otherwise don't allow the removal of the last panel;
        {
            //don't allow the last panel to be removed 
            m_bRemoveLastPanel = false;
        }
        else if (scene.buildIndex == 0) //else if I am in the main scene menu (assuming it is 0 as of right now)
        {
            m_bInMainMenu = true;
            //find the first panel and push it to the stack
            menuStatus.Push(GameObject.Find("First_Panel"));
        }
        else
        {
            m_bRemoveLastPanel = true;
        }

    }

    void SetCurrentSelected(GameObject a_selected)
    {
        _eventSystem.SetSelectedGameObject(null);
        _eventSystem.SetSelectedGameObject(a_selected);
    }

}
