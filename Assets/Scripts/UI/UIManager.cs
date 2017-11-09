using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;
using UnityEngine.Audio;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
    private UINavigation uiNavigationInstance;
    public bool m_bRemoveLastPanel;

    //Main menu stuff
    [Space]
    [Header("Main Menu Variables")]
    public bool m_bInMainMenu = false;
    private GameObject m_CharacterSelectionPanel;
    private GameObject m_SettingsPanel;
    private GameObject m_CreditsPanel;

    private Animator m_ButtonAnimator;
    private Animator m_bMenuAnimator;
    private bool m_bOpenedPanel;
    private bool m_bInSelect = false;
    private bool FixAnimator = false;
    private Timer shortTimer;
    public bool RemoveLastPanel { get { return m_bRemoveLastPanel; } set { m_bRemoveLastPanel = value; } }
    // Use this for initialization
    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

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
                    mInstance = (new GameObject("UIManager")).AddComponent<UIManager>();
                }
                //mInstance.gameObject.name = "Singleton container";
                DontDestroyOnLoad(mInstance.gameObject);

            }
            return mInstance;
        }
    }

    void Awake()
    {

#if !UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
#endif
        shortTimer = new Timer(0.2f);
        uiNavigationInstance = UINavigation.Instance;
        m_bMenuAnimator = FindObjectOfType<Canvas>().GetComponent<Animator>();
        //Add me to the singleton tester
        SingletonTester.Instance.AddSingleton(this);
        SceneManager.sceneLoaded += OnSceneLoad;

        m_SettingsPanel = GameObject.Find("Options_Panel");
        m_CreditsPanel = GameObject.Find("Credits_Panel"); //Currently null;
        m_CharacterSelectionPanel = GameObject.Find("Character_Selection");
        // m_SettingsPanel.SetActive((!m_SettingsPanel));

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
        if (CharacterSelectionManager.Instance)
        {
            if (m_bMenuAnimator)
            {
                CharacterSelectionManager.Instance.LetPlayersSelectCharacters = (m_bMenuAnimator.GetCurrentAnimatorStateInfo(0).IsName("Title_Section_Second_Static") && m_bInSelect);
                if (!m_bInSelect && m_bMenuAnimator.GetCurrentAnimatorStateInfo(0).IsName("Title_Section_Second_Static"))
                    m_bInSelect = shortTimer.Tick(Time.deltaTime);

                AnimatorStateInfo stateInfo = m_bMenuAnimator.GetCurrentAnimatorStateInfo(0);
                //If any of the animators doesn't match up with the peek of the stack 
                //this is disgusting
                if (menuStatus.Peek().name == "First_Panel" && !(stateInfo.IsName("Title_Section_First_Static") || stateInfo.IsName("Title_Section_MoveBackToFirstSection"))) 
                {
                    m_bMenuAnimator.SetTrigger(MenuTransitionBoolParameters["First_Panel"]);
                }
                else if (menuStatus.Peek().name == "Second_Panel" && !(stateInfo.IsName("Title_Section_Second_Static") || stateInfo.IsName("Title_Section_MoveBackToSecondSection"))) 
                {
                    m_bMenuAnimator.SetTrigger(MenuTransitionBoolParameters["Second_Panel"]);
                }
                else if (menuStatus.Peek().name == "Third_Panel" && !stateInfo.IsName("Title_Section_ThirdSectionStatic"))
                {
                    m_bMenuAnimator.SetTrigger(MenuTransitionBoolParameters["Third_Panel"]);
                }
            }
        }
        if (menuStatus.Peek().name == "First_Panel")
        {
            //m_bMenuAnimator.SetTrigger(MenuTransitionBoolParameters["First_Panel"]); 
            //Change the button animation according to what is selected.
            if (!_eventSystem.currentSelectedGameObject)
                _eventSystem.SetSelectedGameObject(menuStatus.Peek().GetComponent<DefaultButton>().defaultButton);
            if (!m_bOpenedPanel)
            {
                switch (_eventSystem.currentSelectedGameObject.name)
                {
                    case "Credits_Button":
                        _eventSystem.currentSelectedGameObject.GetComponent<Button>().onClick.AddListener(delegate { MenuOpenPanel(m_CreditsPanel, "IsCredits"); });
                        m_ButtonAnimator.SetTrigger("SelectedCredits");
                        break;
                    case "Settings_Button":
                        _eventSystem.currentSelectedGameObject.GetComponent<Button>().onClick.AddListener(delegate { MenuOpenPanel(m_SettingsPanel, "IsSettings"); });
                        m_ButtonAnimator.SetTrigger("SelectedSettings");
                        break;
                    case "Quit_Button":
                        m_ButtonAnimator.SetTrigger("SelectedQuit");
                        break;
                    case "VS_Button":
                        m_ButtonAnimator.SetTrigger("SelectedVS");
                        break;
                    default: break;
                }
            }

        }
        for (int i = 0; i < 4; ++i)
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
        //GetComponent<AudioSource>().outputAudioMixerGroup;
        //If the menu status only has 1 object in it, 
        foreach (AnimatorControllerParameter item in m_bMenuAnimator.parameters)
        {
            m_bMenuAnimator.SetBool(item.name, false);
        }
        Debug.Log("Main menu back");
        shortTimer.CurrentTime = 0;
        m_bInSelect = false;
        if (menuStatus.Count == 1 || CharacterSelectionManager.Instance.JoinedPlayers < 4)
        {
            //If there are joined players and the peek of the stack is the third panel, go back
            if (CharacterSelectionManager.Instance.JoinedPlayers < 4 && menuStatus.Peek().name == "Third_Panel")
            {
                menuStatus.Pop();
                m_bMenuAnimator.SetBool(MenuTransitionBoolParameters[menuStatus.Peek().name], true);
                SetCurrentSelected(null);
            }
            else if (menuStatus.Peek().name == "First_Panel")
            {
                CharacterSelectionManager.Instance.LetPlayersSelectCharacters = false;
            }
            return;
        }
        else if (m_bOpenedPanel) //only will run If a panel was open (Credits or Settings)
        {
            //If I find any dropboxes and they are open, hide them
            foreach (var dropdown in menuStatus.Peek().GetComponentsInChildren<Dropdown>())
            {
                if (dropdown.transform.childCount > 3)
                {
                    dropdown.Hide();
                    return;
                }
            }
            //depending on what panel is being closed, do the different animation thing.
            switch (menuStatus.Peek().name)
            {
                case "Options_Panel":
                    m_ButtonAnimator.SetBool("IsSettings", false);
                    m_ButtonAnimator.SetTrigger("SelectedSettings");
                    SettingsManager.Instance.SaveSettings();
                    break;
                case "Credits_Panel":
                    m_ButtonAnimator.SetTrigger("SelectedCredits");
                    m_ButtonAnimator.SetBool("IsCredits", false);
                    break;
            }
            //Save my changes here
            if (SettingsManager.Instance.UnsavedChanges)
            {
                SettingsManager.Instance.OnApplyButtonClick();
            }
            //set the first object on the stack to false
            //menuStatus.Peek().SetActive(false);
            menuStatus.Pop();
            m_ButtonAnimator = menuStatus.Peek().GetComponent<Animator>();
            DefaultButton temp = menuStatus.Peek().GetComponent<DefaultButton>();

            m_bMenuAnimator.SetBool(MenuTransitionBoolParameters[menuStatus.Peek().name], true);
            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(temp.defaultButton);
            m_bOpenedPanel = false;
        }
        else
        {
            menuStatus.Pop();
            //Swap the trigger to whatever the parameter is.
            m_bMenuAnimator.SetBool(MenuTransitionBoolParameters[menuStatus.Peek().name], true);
            m_ButtonAnimator = menuStatus.Peek().GetComponent<Animator>();
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
            foreach (AnimatorControllerParameter item in m_bMenuAnimator.parameters)
            {
                m_bMenuAnimator.SetBool(item.name, false);
            }

            if (m_SettingsPanel.activeSelf)
                m_SettingsPanel.SetActive(false);
            if (m_CreditsPanel)
                m_CreditsPanel.SetActive(false);
            menuStatus.Push(panelToMove);
            //swap the trigger to the corresponding parameter name
            m_bMenuAnimator.SetBool(MenuTransitionBoolParameters[panelToMove.name], true);
            if (panelToMove.GetComponent<Animator>())
                m_ButtonAnimator = panelToMove.GetComponent<Animator>();
            m_CharacterSelectionPanel.SetActive(true);
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
        //If loaded in main menu, run the main menu update
        if (m_bInMainMenu)
        {
            MainMenuUpdate();
            return;
        }

        //if the current selected is null
        if (menuStatus.Count > 0)
        {
            if (!GameObject.Find("EventSystem").GetComponent<EventSystem>().currentSelectedGameObject)
            {
                //set the selected
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
        for (int i = 0; i < 4; ++i)
        {
            if (XCI.GetButtonDown(XboxButton.B, XboxController.First + i))
            {
                Back();
            }

            //If I press start and the menu status (stack of UI elements) is nothing, push the default panel into the stack.
            if (XCI.GetButton(XboxButton.Start, XboxController.First + i) && menuStatus.Count <= 0)
            {
                if (GameManagerc.Instance.m_bAllowPause)
                {
                    if (!menuStatus.Contains(defaultPanel))
                    {
                        menuStatus.Push(defaultPanel);
                        defaultPanel.SetActive(true);
                        if (GameManagerc.Instance.PointsPanel)
                            GameManagerc.Instance.PointsPanel.SetActive(false);


                        GameManagerc.Instance.GetScreenAnimator().SetTrigger("ShowScreen");
                        GameManagerc.Instance.Paused = true;
                        foreach (RigidbodyPauser item in GameManagerc.Instance._rbPausers)
                        {
                            item.PauseRigidbody();
                        }
                        StartCoroutine(PauseGame()); //Pause the game

                        GameObject selectable = defaultPanel.GetComponentInChildren<DefaultButton>().gameObject;
                        defaultPanel.transform.GetChild(0).Find("MainMenu").GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { GameManagerc.Instance.GoToStart(); });
                        SetCurrentSelected(selectable);
                    }
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
        if (menuStatus.Count == 1 && !m_bRemoveLastPanel)
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


    public void MenuOpenPanel(GameObject PanelToOpen, string AnimationParameter = "")
    {
        if (!menuStatus.Contains(PanelToOpen))
        {
            //Do my animations
            switch (AnimationParameter)
            {
                case "IsSettings":
                    m_ButtonAnimator.ResetTrigger("SelectedSettings");
                    break;
                case "IsCredits":
                    m_ButtonAnimator.ResetTrigger("SelectedCredits");
                    break;
            }
            m_ButtonAnimator.SetBool(AnimationParameter, true);
            menuStatus.Push(PanelToOpen);
            //Wait for animations here
            StartCoroutine(WaitForAnimation(PanelToOpen, AnimationParameter));
        }
    }



    void DoLast(GameObject PanelToOpen, string AnimationParameter)
    {
        m_CharacterSelectionPanel.SetActive(false);
        m_CreditsPanel.SetActive(false);
        m_bMenuAnimator.SetTrigger(MenuTransitionBoolParameters["Second_Panel"]);
        m_bOpenedPanel = true;

        //Turn on the panel
        PanelToOpen.SetActive(true);

        //Turn on any children
        if (PanelToOpen.transform.childCount > 0)
        {
            for (int i = 0; i < PanelToOpen.transform.childCount; ++i)
            {
                PanelToOpen.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        //Add it to menustatus
        _eventSystem.SetSelectedGameObject(null);
        if (menuStatus.Peek().GetComponentInChildren<Button>()) //Find any button
        {
            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(menuStatus.Peek().GetComponentInChildren<Button>().gameObject);
        }
    }
    //whatever the element is, push it into the stack 
    public void OpenUIElement(GameObject ElementToOpen, bool openChildren = false)
    {


        if (!menuStatus.Contains(ElementToOpen))
        {
            if (menuStatus.Count > 0)
                menuStatus.Peek().SetActive(false);

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

            if (menuStatus.Peek().GetComponentInChildren<DefaultButton>())
            {
                _eventSystem.SetSelectedGameObject(null);
                _eventSystem.SetSelectedGameObject(menuStatus.Peek().GetComponentInChildren<DefaultButton>().gameObject);
                print(menuStatus.Peek().GetComponentInChildren<DefaultButton>().gameObject);
            }
            else if (menuStatus.Peek().GetComponentInChildren<Button>())
            {
                _eventSystem.SetSelectedGameObject(null);
                _eventSystem.SetSelectedGameObject(menuStatus.Peek().GetComponentInChildren<Button>().gameObject);
                print(menuStatus.Peek().GetComponentInChildren<Button>().gameObject);
                //defaultToReturnTo = menuStatus.Peek().GetComponentInChildren<Button>().gameObject;
            }
        }
    }

    public void UnpauseGame()
    {
        if (menuStatus.Count >= 1)
        {
            //menuStatus.Peek().SetActive(false);
            menuStatus.Pop();
            _eventSystem.SetSelectedGameObject(null);
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

        foreach (RigidbodyPauser item in GameManagerc.Instance._rbPausers)
        {
            item.UnpauseRigidbody();
        }
        SettingsManager.Instance.SaveSettings();
        GameManagerc.Instance.GetScreenAnimator().SetTrigger("RemoveScreen");

        Time.timeScale = 1;
        GameManagerc.Instance.Paused = false;
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene load");
        menuStatus.Clear();
        _eventSystem = FindObjectOfType<EventSystem>();
        //very specific scene, if in this scene, I want to remove the last panel
        if (scene.buildIndex == 1)
        {
            //defaultPanel = FindObjectOfType<DefaultPanel>().gameObject;
            //Button ResumeButton = defaultPanel.transform.GetChild(0).transform.Find("ResumeButton").GetChild(0).GetComponent<Button>();
            //ResumeButton.onClick.AddListener(delegate { UnpauseGame(); });
            //m_bRemoveLastPanel = false;
            //    defaultPanel.gameObject.SetActive(false);

        }

        else if (scene.buildIndex != 0) //otherwise don't allow the removal of the last panel;
        {
            //don't allow the last panel to be removed 
            m_bRemoveLastPanel = false;
        }
        else if (scene.buildIndex == 0) //else if I am in the main scene menu (assuming it is 0 as of right now)
        {
            m_SettingsPanel = GameObject.Find("Options_Panel");
            m_bInMainMenu = true;
            m_bMenuAnimator = FindObjectOfType<Canvas>().GetComponent<Animator>();
            if (!m_SettingsPanel)
            {
                m_SettingsPanel = GameObject.Find("Options_Panel");
            }

            m_CreditsPanel = GameObject.Find("Credits_Panel"); //Currently null;
            m_CharacterSelectionPanel = GameObject.Find("Character_Selection");
            _eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            Button vsButton = GameObject.Find("VS_Button").GetComponent<Button>();
            Button SettingsButton = GameObject.Find("Settings_Button").GetComponent<Button>();
            Button CreditsButton = GameObject.Find("Credits_Button").GetComponent<Button>();
            Button QuitBUtton = GameObject.Find("Quit_Button").GetComponent<Button>();
            vsButton.onClick.AddListener(delegate { MainMenuChangePanel(GameObject.Find("Second_Panel")); });
            SettingsButton.onClick.AddListener(delegate { MenuOpenPanel(m_SettingsPanel, "IsSettings"); });
            CreditsButton.onClick.AddListener(delegate { MenuOpenPanel(m_CreditsPanel, "IsCredits"); });
            QuitBUtton.onClick.AddListener(delegate { QuitGame(); });
            //find the first panel and push it to the stack
            menuStatus.Push(GameObject.Find("First_Panel"));
            m_ButtonAnimator = menuStatus.Peek().GetComponent<Animator>();
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

    public void SetDefaultPanel(GameObject a_DefaultPanel)
    {
        defaultPanel = a_DefaultPanel;
        Button ResumeButton = defaultPanel.transform.GetChild(0).transform.Find("ResumeButton").GetChild(0).GetComponent<Button>();
        ResumeButton.onClick.AddListener(delegate { UnpauseGame(); });
        m_bRemoveLastPanel = false;
    }


    public void TurnActive()
    {
        m_SettingsPanel.SetActive(true);
    }


    IEnumerator WaitForAnimation(GameObject PanelToOpen, string AnimationParameter = "")
    {
        yield return new WaitForSeconds(m_ButtonAnimator.GetCurrentAnimatorStateInfo(0).length - 1.3f /*+ m_ButtonAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime*/);
        DoLast(PanelToOpen, AnimationParameter);
    }

    IEnumerator PauseGame()
    {
        yield return new WaitForSeconds(1);
        Time.timeScale = 0;
    }


}
