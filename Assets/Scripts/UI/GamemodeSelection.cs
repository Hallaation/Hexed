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
    private EventSystem _eventSystem;
    //private Text _pointText;

    private int m_iPointWinIndex;
    private int m_iMapPickIndex;
    private int m_iGamemodeIndex;
    public PickType m_pickType = PickType.GAMEMODEPICK;
    private Gamemode_type m_GamemodeSelected = Gamemode_type.LAST_MAN_STANDING_DEATHMATCH;
    private bool ResetSticks = false;
    private GameObject[] GMSettingObjects;
    private bool[] StickMovement; // index 0 for left horizontal, index 1 for right horizontal
    Image _mapSprite;
    public int[] mPointsToWin =
    {
        5, 10, 15, 20
    };


    public Sprite[] mapSprites; //TODO in heavy construction should be done later
    // Use this for initialization
    void Start()
    {
        m_iPointWinIndex = 0;
        m_iMapPickIndex = 0;
        _button = GetComponentInChildren<Button>();

        _eventSystem = FindObjectOfType<EventSystem>();

        StickMovement = new bool[2] { false , false };
        if (m_pickType == PickType.MAPPICK)
            _mapSprite = GetComponentInChildren<Image>();
            
        //If I am a settings type of object, I will open the settings
        if (m_pickType == PickType.GAMEMODESETTINGS)
        {
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
            UIManager.instance.defaultPanel = this.transform.parent.gameObject;
            UIManager.instance.menuStatus.Push(UIManager.instance.defaultPanel);
            _button.onClick.AddListener(delegate () { UIManager.instance.OpenUIElement(GMSettingObjects[(int)GameManagerc.Instance.m_gameMode]); });
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
        CheckForStickReset();
        //everything needs to change fuck
        if (_eventSystem.currentSelectedGameObject.transform.parent == _button.transform.parent)
        {
            switch (m_pickType)
            {
                case PickType.GAMEMODEPICK:
                    GameManagerc.Instance.m_gameMode = Gamemode_type.LAST_MAN_STANDING_DEATHMATCH + m_iGamemodeIndex;
                    //check for right input
                    //If Dpad right, increment through the array
                    //If dpad left, decrement through the array
                    //wrap around if at the bounds of array
                    if ((DpadHorizontalTest() > 0) || StickMovement[1])
                    {
                        StickMovement[1] = false;
                        if (m_iGamemodeIndex == (int)Gamemode_type.CAPTURE_THE_FLAG)
                            m_iGamemodeIndex = 0;
                        else
                            m_iGamemodeIndex++;

                        m_GamemodeSelected = Gamemode_type.LAST_MAN_STANDING_DEATHMATCH + m_iGamemodeIndex;
                    }
                    else if ((DpadHorizontalTest() < 0) || StickMovement[0])
                    {
                        StickMovement[0] = false;
                        if (m_iGamemodeIndex == 0)
                            m_iGamemodeIndex = (int)Gamemode_type.CAPTURE_THE_FLAG;
                        else
                            m_iGamemodeIndex--;

                        m_GamemodeSelected = Gamemode_type.LAST_MAN_STANDING_DEATHMATCH + m_iGamemodeIndex;
                    }
                    break;
                case PickType.MAPPICK:
                    _mapSprite.sprite = mapSprites[m_iMapPickIndex];
                    if ((DpadHorizontalTest() > 0) || StickMovement[1])
                    {
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
                        StickMovement[0] = false;
                        Debug.Log("below 0");
                        if (m_iMapPickIndex == 0)
                        {
                            m_iMapPickIndex = mapSprites.Length - 1;
                        }
                        else
                        {
                            Debug.Log("Decrement");
                            m_iMapPickIndex--;
                        }

                    }
                    break;
            }
        }
        //update different according to pick type
        switch (m_pickType)
        {
            case PickType.GAMEMODEPICK:
                switch (m_GamemodeSelected)
                {
                    case Gamemode_type.LAST_MAN_STANDING_DEATHMATCH:
                        _button.GetComponentInChildren<Text>().text = "Gamemode: Last Man Standing DM";
                        break;
                    case Gamemode_type.DEATHMATCH_POINTS:
                        _button.GetComponentInChildren<Text>().text = "Gamemode: MaxKills Deathmatch";
                        break;
                    case Gamemode_type.DEATHMATCH_TIMED:
                        _button.GetComponentInChildren<Text>().text = "Gamemode: Timed Deathmatch";
                        break;
                    case Gamemode_type.CAPTURE_THE_FLAG:
                        _button.GetComponentInChildren<Text>().text = "Gamemode: Capture the flag";
                        break;
                    default:
                        break;
                }
                GameManagerc.Instance.m_iPointsNeeded = mPointsToWin[m_iPointWinIndex];
                break;
            case PickType.MAPPICK:
                break;
        }

        for (int i = 0; i < (int)PlayerIndex.Four; i++)
        {
            //if any of the controllers have an input, dont reset the sticks
            if (XCI.GetAxis(XboxAxis.LeftStickX , XboxController.First + i) != 0)
            {
                ResetSticks = false;
                Debug.Log("one of them isnt 0");
                i = int.MaxValue;
                break;
            }
            else
            {
                ResetSticks = true;
            }
        }

    }

    /// <summary>
    /// Checks for horizontal dpad from any controller, returns -1 for left, 1 for right, 0 for nothing
    /// </summary>
    /// <returns>-1 if Left, 1 if right, 0 if nothing</returns>
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
    public void EnterGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ButtonTest()
    {
        Debug.Log("Test button");
    }
}
