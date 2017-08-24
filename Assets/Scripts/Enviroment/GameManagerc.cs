using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using XboxCtrlrInput;
using XInputDotNetPure;
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

//learn the definition of deathmatch please
public enum Gamemode_type
{
    LAST_MAN_STANDING_DEATHMATCH, //last person to stand earns a point, probably the default
    DEATHMATCH_POINTS, //killing a player will earn them a point, up to a certain point
    DEATHMATCH_TIMED, //kill as many players as you can in the allocated time
    CAPTURE_THE_FLAG, //unsued, probably not going to implement, put it in here for the lols;

}

//Used to store the game state
public class GameManagerc : MonoBehaviour
{

    //dictionary mapping XCI index with the XInputDotNet indexes
    Dictionary<XboxController , int> XboxControllerPlayerNumbers = new Dictionary<XboxController , int>
    {
        {XboxController.First,   0 },
        {XboxController.Second,  1 },
        {XboxController.Third,   2 },
        {XboxController.Fourth,  3 },
    };

    Timer waitForRoundEnd;
    //lets try a dictionary again
    //public List<int> PlayerWins = new List<int>();
    public Dictionary<PlayerStatus , int> PlayerWins = new Dictionary<PlayerStatus , int>();
    public List<PlayerStatus> InGamePlayers = new List<PlayerStatus>();
    GameObject WinningPlayer = null;

    public Gamemode_type m_gameMode = Gamemode_type.LAST_MAN_STANDING_DEATHMATCH;

    public int m_iPointsNeeded = 5;
    public float m_fTimedDeathMatchTime;

    static GameManagerc mInstance = null;
    Timer DeathmatchTimer;
    //lets keep the variables at the top shall we
    private GameObject FinishUIPanel;
    private bool m_bRoundOver;
    public GameObject MapToLoad;

    public Sprite PointSprite;
    public GameObject[] PointOrigins;
    private GameObject PointsPanel;

    public static GameManagerc Instance
    {
        get
        {
            //if an instance doesnt exist
            if (mInstance == null)
            {
                //look for an instance
                mInstance = (GameManagerc)FindObjectOfType(typeof(GameManagerc));
                //if an instance wasn't found, make an instance
                if (mInstance == null)
                {
                    mInstance = (new GameObject("GameManager")).AddComponent<GameManagerc>();
                }
                //set to dont destroy on load
                DontDestroyOnLoad(mInstance.gameObject);
            }
            return mInstance;
        }
    }
    // Use this for initialization
    void Start()
    {
        //Find the 

        DeathmatchTimer = new Timer(m_fTimedDeathMatchTime);
        SceneManager.sceneLoaded += OnSceneLoaded;
        waitForRoundEnd = new Timer(3);
        mInstance = GameManagerc.Instance;
        if (mInstance.gameObject != this.gameObject)
        {
            Destroy(this.gameObject);
        }
        m_bRoundOver = false;
        Physics.gravity = new Vector3(0 , 0 , 10); //why
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            //tempPoint++;
            //GameObject go = new GameObject("Point", typeof(RectTransform));
            //go.AddComponent<CanvasRenderer>();
            //go.AddComponent<Image>().sprite = test;

            //go.transform.SetParent(AddPointSpot.transform);
            //go.transform.position += new Vector3(AddPointSpot.transform.position.x + 40 * tempPoint , AddPointSpot.transform.position.y);
            //go.transform.localScale = new Vector3(1,1,1);

        }
        if (InGamePlayers.Count > 1)
        {
            StartCoroutine(CheckForRoundEnd());
        }

    }

    IEnumerator CheckForRoundEnd()
    {
        if (!m_bRoundOver)
        {
            //Depending on the game mode, different logic will happen, a very basic state maching
            switch (m_gameMode)
            {
                case Gamemode_type.LAST_MAN_STANDING_DEATHMATCH:
                    RoundEndLastManStanding();
                    break;
                case Gamemode_type.DEATHMATCH_POINTS:
                    RoundEndDeathMatchMaxPoints();
                    break;
                case Gamemode_type.DEATHMATCH_TIMED:
                    RoundEndDeathMatchTimed();
                    break;
                case Gamemode_type.CAPTURE_THE_FLAG:
                    //RoundEndLastManStanding();
                    m_bRoundOver = true;
                    break;
                default:
                    break;
            }
        }
        else
        {
            //once the round is over, reset players
            if (waitForRoundEnd.Tick(Time.deltaTime))
            {
                //reload scene
                foreach (PlayerStatus players in InGamePlayers)
                {
                    players.ResetPlayer();
                }
                //InGamePlayers = new List<PlayerStatus>(); this is wrong, the players should be reset not deleted the controller manager should not be reset as well
                m_bRoundOver = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                //End of round logic goes here.
            }
        }
        yield return null;
    }

    /// <summary>
    /// Points are awarded if a player is the last man standing
    /// </summary>
    void RoundEndLastManStanding()
    {
        //Find the amount of dead players
        int DeadCount = 0;
        foreach (PlayerStatus player in InGamePlayers)
        {
            if (player.IsDead)
            {
                DeadCount++;
            }
        }

        //If there is only 1 alive
        if (DeadCount >= InGamePlayers.Count - 1)
        {
            //the round is now over
            //look for the one player that is 
            m_bRoundOver = true;
            int i = 0;
            foreach (PlayerStatus player in InGamePlayers)
            {

                if (!player.IsDead)
                {
                    //increase the winning player's point by 1
                    PlayerWins[player] += 1;
                    //TODO Point tallying screen goes here.
                    //! HERE
                    PointsPanel.SetActive(true);
                    for (int k = 0; k < PlayerWins[player]; k++)
                    { 
                        GameObject go = new GameObject("Point" , typeof(RectTransform));
                        go.AddComponent<CanvasRenderer>();
                        go.AddComponent<Image>().sprite = PointSprite;
                        go.transform.SetParent(PointOrigins[XboxControllerPlayerNumbers[player.GetComponent<ControllerSetter>().mXboxController]].transform);
                        //go.transform.position += new Vector3(AddPointSpot.transform.position.x + 40 * tempPoint , AddPointSpot.transform.position.y);
                        Vector3 AddPointSpot = PointOrigins[XboxControllerPlayerNumbers[player.GetComponent<ControllerSetter>().mXboxController]].transform.position;
                        go.transform.position += new Vector3(AddPointSpot.x + 40 * k , AddPointSpot.y);
                    }
                   // Debug.Break();
                    //If player has reached the points required to win
                    if (PlayerWins[player] >= m_iPointsNeeded)
                    {
                        //Set the time scale to 0 (essentially pausing the game-ish)
                        Debug.LogError("Points required have been reached");
                        Time.timeScale = 0;
                        //open the finish panel, UI manager will set all the children to true, thus rendering them
                        UIManager.Instance.OpenUIElement(FinishUIPanel , true);
                        UIManager.Instance.RemoveLastPanel = false;
                        //Reset the event managers current selected object to the rematch button
                        FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
                        FindObjectOfType<EventSystem>().SetSelectedGameObject(FinishUIPanel.transform.Find("Rematch").gameObject);

                        //TODO Load Character select / win screen;
                        //TODO Sort players by score?
                    }
                }
                ++i; //probably unused, keeping it here in case it is actually used.
            }
        }
    }
    void RoundEndDeathMatchMaxPoints()
    {
        foreach (PlayerStatus player in InGamePlayers)
        {
            if (player.IsDead)
            {
                player.ResetPlayer();
            }

            //If player has reached the points required to win
            if (PlayerWins[player] >= m_iPointsNeeded)
            {
                //Set the time scale to 0 (essentially pausing the game-ish)
                Debug.LogError("Points required have been reached");
                Time.timeScale = 0;
                //open the finish panel, UI manager will set all the children to true, thus rendering them
                UIManager.Instance.OpenUIElement(FinishUIPanel , true);
                UIManager.Instance.RemoveLastPanel = false;
                //Reset the event managers current selected object to the rematch button
                FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
                FindObjectOfType<EventSystem>().SetSelectedGameObject(FinishUIPanel.transform.Find("Rematch").gameObject);

                //TODO Load Character select / win screen;
                //TODO Sort players by score?
            }
        }
        //RoundEndLastManStanding();
        //TODO round shouldn't be over until one of the players has reached the maximum points
        //TODO For Max points, guns should be respawning with new ammo, any guns with no ammo should be deleted after a while when they have no ammo (like duck game)
        //TODO Respawn players after they are killed (Like Smash bros.) no i-Frames though, they can be camped for all I care.
        //m_bRoundOver = true;
        //TODO Sort players by score?
        //TODO Load Character select / win screen;
    }

    void RoundEndDeathMatchTimed()
    {
        RoundEndLastManStanding();
        //TODO similiar to max points (should change this to kills) 
        //TODO Penalty for death? 
        //TODO Round isn't over until the timer has reached 0.
        //m_bRoundOver = true;
        //TODO Sort players by score?
        //TODO Load Character select / win screen;
    }

    void OnSceneLoaded(Scene scene , LoadSceneMode mode)
    {
        //look for a gamemanager, then delete it.
        //Object[] items = FindObjectsOfType<GameManagerc>();
        //for (int i = 0; i < items.Length; ++i)
        //{
        //    if (items[i] != this)
        //    {
        //        //Destroy(items[i]);
        //    }
        //}

        Debug.Log("----------------------------------------------------------------------\n GameManager loaded in ControllerTest\n----------------------------------------------------------------------------");
        //If I found the finished game panel
        if (MapToLoad)
        {
            GameObject go = Instantiate(MapToLoad);
            go.transform.position = Vector3.zero;
            go.transform.DetachChildren();
        }
        if (GameObject.Find("FinishedGamePanel"))
        {
            //Set the UI panel reference to that object
            FinishUIPanel = GameObject.Find("FinishedGamePanel");
            //set the object buttons delegates
            FinishUIPanel.transform.Find("Rematch").GetComponent<Button>().onClick.AddListener(delegate { Rematch(); });
            FinishUIPanel.transform.Find("Main Menu").GetComponent<Button>().onClick.AddListener(delegate { GoToStart(); });

            //turn the children off (so this object can still be found if needed be);
            for (int i = 0; i < FinishUIPanel.transform.childCount; ++i)
            {
                FinishUIPanel.transform.GetChild(i).gameObject.SetActive(false);
            }

        }
        //Find the points panel and populate the array.
        PointsPanel = GameObject.Find("PointsPanel");
        GameObject temp = GameObject.Find("PointsPanel");
        PointOrigins = new GameObject[temp.transform.childCount];
        for (int i = 0; i < temp.transform.childCount; i++)
        {
            PointOrigins[i] = temp.transform.GetChild(i).transform.Find("PointsOrigin").gameObject;
            PointSprite = temp.transform.GetChild(i).transform.Find("PointsOrigin").GetComponent<Image>().sprite;
        }

    }

    /// <summary>
    /// Adds a player to the game, used to check if they're dead or not to determine if the round is over or not.
    /// </summary>
    /// <param name="aPlayer"></param>
    public void AddPlayer(PlayerStatus aPlayer)
    {
        InGamePlayers.Add(aPlayer);
        PlayerWins.Add(aPlayer , 0);
    }


    public void Rematch()
    {
        //reset the time scale
        Time.timeScale = 1;
        //The round is not over
        m_bRoundOver = false;
        //reset every player
        foreach (KeyValuePair<PlayerStatus , int> item in PlayerWins)
        {
            item.Key.ResetPlayer();
        }
        //Reset the players' points
        foreach (var item in InGamePlayers)
        {
            PlayerWins[item] = 0;
        }
        WinningPlayer = null; // still unsued
        m_bRoundOver = false;
        //reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// used to reutn to the character selection screen
    /// </summary>
    public void GoToStart()
    {
        //clear the players list
        for (int i = 0; i < InGamePlayers.Count; i++)
        {
            InGamePlayers[i].Clear();
            InGamePlayers[i].gameObject.SetActive(false);
        }


        //turn off all the singletons
        UIManager.Instance.gameObject.SetActive(false);
        ControllerManager.Instance.gameObject.SetActive(false);
        CharacterSelectionManager.Instance.gameObject.SetActive(false);
        PlayerUIArray.Instance.gameObject.SetActive(false);

        //Destroy the singleton objects
        Destroy(PlayerUIArray.Instance.gameObject);
        Destroy(UIManager.Instance.gameObject);
        Destroy(ControllerManager.Instance.gameObject);
        Destroy(CharacterSelectionManager.Instance.gameObject);
        Destroy(this.gameObject);
        //reset the time scale
        Time.timeScale = 1;
        //? Don't know why but the scene loads before all the logic even happens
        //WTF scene begings to load even though line hasnt been called. what is happening. 
        SceneManager.LoadScene(0);
    }
    IEnumerator waitForSeconds()
    {
        yield return new WaitForSecondsRealtime(1);
    }

}

