using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using XboxCtrlrInput;
using XInputDotNetPure;
using UnityEngine.SceneManagement;
public enum PickType
{
    GAMEMODEPICK,
    GAMEMODESETTINGS,
    MAPPICK,
}

public class GamemodeSelection : MonoBehaviour
{
    private Button _button;
    private Text _buttonText;
    private EventSystem _eventSystem;
    //private Text _pointText;

    private int m_iPointWinIndex;
    private int m_iMapPickIndex;
    private int m_iGamemodeIndex;
    public PickType m_pickType = PickType.GAMEMODEPICK;
    private Gamemode_type m_GamemodeSelected = Gamemode_type.LAST_MAN_STANDING_DEATHMATCH;
    private bool ResetSticks = false;
    private bool[] StickMovement; // index 0 for left horizontal, index 1 for right horizontal
    private GameObject[] GMSettingObjects;
    private Animator m_animator;
    Image _mapSprite;
    public int[] mPointsToWin =
    {
        1, 3, 5, 10
    };


    public Sprite[] mapSprites;
    public GameObject[] MapObjects;
    public GameObject MapToLoad;

    Text _SettingsValue;
    // Use this for initialization
    void Start()
    {
        m_animator = GetComponentInChildren<Animator>();
        m_iPointWinIndex = 0;
        m_iMapPickIndex = 0;
        _button = GetComponentInChildren<Button>();
        _buttonText = GetComponentInChildren<Text>();
        _eventSystem = FindObjectOfType<EventSystem>();
        StickMovement = new bool[2] { false, false };

        if (m_pickType == PickType.MAPPICK)
        {
            _mapSprite = _button.transform.GetChild(0).GetComponent<Image>();
            //Load the maps and populate the arrays with them
            Object[] temp = Resources.LoadAll("Maps", typeof(GameObject));
            MapObjects = new GameObject[temp.Length];
            mapSprites = new Sprite[temp.Length];
            for (int i = 0; i < MapObjects.Length; i++)
            {
                MapObjects[i] = temp[i] as GameObject;
                mapSprites[i] = (temp[i] as GameObject).GetComponent<Map>().mapSelectionSprite;
            }
        }

        //If I am a settings type of object, I will open the settings
        if (m_pickType == PickType.GAMEMODESETTINGS)
        {
            _SettingsValue = GetComponentInChildren<Text>();

            /* old shit
            //find the objects, make the array then populate the array.
            GMSettingObjects = new GameObject[GameObject.Find("GameModeSettingsPanels").transform.childCount];
            for (int i = 0; i < GameObject.Find("GameModeSettingsPanels").transform.childCount; ++i)
            {
                GMSettingObjects[i] = GameObject.Find("GameModeSettingsPanels").transform.GetChild(i).gameObject;
            }

            //then turn them off
            foreach (GameObject item in GMSettingObjects)
            {
                item.SetActive(false);
            }
            //Debug.Log(UIManager.Instance);
            //Debug.Log(UIManager.Instance.defaultPanel);

            UIManager.Instance.defaultPanel = this.transform.parent.gameObject;
            UIManager.Instance.menuStatus.Push(UIManager.Instance.defaultPanel);
            _button.onClick.AddListener(delegate () { UIManager.Instance.OpenUIElement(GMSettingObjects[(int)GameManagerc.Instance.m_gameMode]); });
            */
        }



    }

    //?
    //? F R O M 
    //? T H E
    //? G H A S T L Y
    //? E Y R I E S
    //? I
    //? C A N
    //? S E E
    //? T O
    //? T H E
    //? E N D S
    //? O F
    //? T H E
    //? W O R LD
    //? A N D
    //? F R O M 
    //? T H I S
    //? V A N T A G E
    //? P O I NT
    //? I
    //? D E C L A R E 
    //? W I T H
    //? U T T E R
    //? C E R T A I N T Y
    //? T H A T
    //? T H I S
    //? O N E
    //? I S
    //? I N
    //? T H E
    //? B A G
    //? .
    //?
    // Update is called once per frame
    void Update()
    {
        //if the top of the stack is the peek of this transform's parent's parent
        if (UIManager.Instance.menuStatus.Peek() == this.transform.parent.parent.gameObject)
        {
            CheckForStickReset();
            //if the event systems currently selected object is my assigned buttons parent, do the things according to my type.
            if (_eventSystem.currentSelectedGameObject.transform.parent == _button.transform.parent)
            {
                switch (m_pickType)
                {
                    case PickType.GAMEMODEPICK:
                        #region
                        /*
                        GameManagerc.Instance.m_gameMode = Gamemode_type.LAST_MAN_STANDING_DEATHMATCH + m_iGamemodeIndex;
                        //check for right input
                        //If Dpad right, increment through the array
                        //If dpad left, decrement through the array
                        //wrap around if at the bounds of array
                        if ((DpadHorizontalTest() > 0) || StickMovement[1])
                        {
                            StickMovement[1] = false;
                            if (m_iGamemodeIndex == (int)Gamemode_type.DEATHMATCH_POINTS)
                                m_iGamemodeIndex = 0;
                            else
                                m_iGamemodeIndex++;

                            m_GamemodeSelected = Gamemode_type.LAST_MAN_STANDING_DEATHMATCH + m_iGamemodeIndex;
                        }
                        else if ((DpadHorizontalTest() < 0) || StickMovement[0])
                        {
                            StickMovement[0] = false;
                            if (m_iGamemodeIndex == 0)
                                m_iGamemodeIndex = (int)Gamemode_type.DEATHMATCH_POINTS;
                            else
                                m_iGamemodeIndex--;

                            m_GamemodeSelected = Gamemode_type.LAST_MAN_STANDING_DEATHMATCH + m_iGamemodeIndex;
                        }
                        
                    */
                        break;
                    #endregion
                    case PickType.MAPPICK:
                        #region
                        {
                            _mapSprite.sprite = mapSprites[m_iMapPickIndex];
                            GameManagerc.Instance.MapToLoad = MapObjects[m_iMapPickIndex];
                            if ((DpadHorizontalTest() > 0) || StickMovement[1])
                            {
                                m_animator.SetTrigger("ButtonChangeRight");
                                StickMovement[1] = false;
                                if (m_iMapPickIndex == mapSprites.Length - 1)
                                {
                                    m_iMapPickIndex = 0;
                                }
                                else
                                {
                                    m_iMapPickIndex++;
                                }

                            }
                            else if ((DpadHorizontalTest() < 0) || StickMovement[0])
                            {
                                m_animator.SetTrigger("ButtonChangeLeft");
                                StickMovement[0] = false;

                                if (m_iMapPickIndex == 0)
                                {
                                    m_iMapPickIndex = mapSprites.Length - 1;
                                }
                                else
                                {
                                    //   Debug.Log("Decrement");
                                    m_iMapPickIndex--;
                                }

                            }

                        }
                        break;
                    #endregion
                    case PickType.GAMEMODESETTINGS:
                        #region
                        if ((DpadHorizontalTest() > 0) || StickMovement[1])
                        {
                            m_animator.SetTrigger("ButtonChangeRight");
                            StickMovement[1] = false;
                            if (m_iPointWinIndex == mPointsToWin.Length - 1)
                            {
                                m_iPointWinIndex = 0;
                            }
                            else
                            {
                                m_iPointWinIndex++;
                            }

                        }
                        else if ((DpadHorizontalTest() < 0) || StickMovement[0])
                        {
                            m_animator.SetTrigger("ButtonChangeLeft");
                            StickMovement[0] = false;

                            if (m_iPointWinIndex == 0)
                            {
                                m_iPointWinIndex = mPointsToWin.Length - 1;
                            }
                            else
                            {
                                // Debug.Log("Decrement");
                                m_iPointWinIndex--;
                            }

                        }
                        _SettingsValue.text = mPointsToWin[m_iPointWinIndex].ToString("0");
                        GameManagerc.Instance.m_iPointsNeeded = (int)mPointsToWin[m_iPointWinIndex];
                        GameManagerc.Instance.m_iPointsIndex = m_iPointWinIndex;
                        #endregion
                        break;
                }

            }
            //update button sprite according to pick type
            switch (m_pickType)
            {
                case PickType.GAMEMODEPICK:
                    switch (m_GamemodeSelected)
                    {
                        case Gamemode_type.LAST_MAN_STANDING_DEATHMATCH:
                            _buttonText.text = "Gamemode: Last Man Standing DM";
                            break;
                        //case Gamemode_type.DEATHMATCH_POINTS:
                        //    _buttonText.text = "Gamemode: MaxKills Deathmatch";
                        //    break;
                        //case Gamemode_type.DEATHMATCH_TIMED:
                        //    _button.GetComponentInChildren<Text>().text = "Gamemode: Timed Deathmatch";
                        //    break;
                        //case Gamemode_type.CAPTURE_THE_FLAG:
                        //    _button.GetComponentInChildren<Text>().text = "Gamemode: Capture the flag";
                        //    break;
                        default:
                            break;
                    }
                    //GameManagerc.Instance.m_iPointsNeeded = mPointsToWin[m_iPointWinIndex];
                    break;
                case PickType.MAPPICK:
                    _mapSprite.sprite = mapSprites[m_iMapPickIndex];
                    break;
            }

            for (int i = 0; i < (int)PlayerIndex.Four; i++)
            {
                //if any of the controllers have an input, dont reset the sticks
                if (XCI.GetAxis(XboxAxis.LeftStickX, XboxController.First + i) != 0)
                {
                    ResetSticks = false;
                    //Debug.Log("one of them isnt 0");
                    i = int.MaxValue;
                    break;
                }
                else
                {
                    ResetSticks = true;
                }
            }

        }
    }
    /// <summary>
    /// Checks for horizontal dpad from any controller, returns -1 for left, 1 for right, 0 for nothing
    /// </summary>
    /// <returns>-1 if Left, 1 if right, 0 if nothing</returns>
    int DpadHorizontalTest()
    {

        for (int i = 0; i < 4; ++i)
        {
            if (XCI.GetButtonDown(XboxButton.DPadRight, XboxController.First + i))
            {
                return 1;
            }
            else if (XCI.GetButtonDown(XboxButton.DPadLeft, XboxController.First + i))
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
            Vector2 StickInput = new Vector2(XCI.GetAxisRaw(XboxAxis.LeftStickX, XboxController.First + i), 0);
            //Debug.Log(StickInput);
            StickInput = CheckDeadZone(StickInput, 0.12f);
            if (ResetSticks)
            {
                if (StickInput.x < 0)
                {
                    StickMovement[0] = true;
                    ResetSticks = false;
                }
                else if (StickInput.x > 0)
                {
                    StickMovement[1] = true;
                    ResetSticks = false;
                }
            }

        }
    }


    Vector2 CheckDeadZone(Vector2 controllerInput, float deadzone)
    {
        Vector2 temp = controllerInput;
        //if any of the numbers are below a certain deadzone, they get zeroed.
        if (temp.magnitude < deadzone)
        {
            temp = Vector2.zero;
        }

        return temp;
    }


    public void EnterGame()
    {
        ScreenTransition transitionScrene = FindObjectOfType<ScreenTransition>();
        transitionScrene.m_Animator.SetTrigger("CloseDoor");
        StartCoroutine(WaitforTransition());
        //if (transitionScrene.m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Transition_Closed"))
        //{
        //    transitionScrene.transform.SetParent(null);
        //    DontDestroyOnLoad(transitionScrene.gameObject);
        //    UIManager.Instance.m_bInMainMenu = false;
        //    SceneManager.LoadScene(1);
        //}
    }

    IEnumerator WaitforTransition()
    {
        ScreenTransition transitionScrene = FindObjectOfType<ScreenTransition>();
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(1);
        UIManager.Instance.m_bInMainMenu = false;
        yield return null;
    }
    public void ButtonTest()
    {
        //Debug.Log("Test button");
    }
}
