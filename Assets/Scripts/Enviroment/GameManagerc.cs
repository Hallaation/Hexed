using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

public class GameManagerc : MonoBehaviour
{
    Timer waitForRoundEnd;
    //lets try a dictionary again
    //public List<int> PlayerWins = new List<int>();
    public Dictionary<PlayerStatus , int> PlayerWins = new Dictionary<PlayerStatus , int>();
    public Gamemode_type m_gameMode = Gamemode_type.LAST_MAN_STANDING_DEATHMATCH;
    public int m_iPointsNeeded = 5;
    public float m_fTimedDeathMatchTime;

    static GameManagerc mInstance = null;
    Timer DeathmatchTimer;
    //lets keep the variables at the top shall we
    public List<PlayerStatus> InGamePlayers = new List<PlayerStatus>();
    GameObject WinningPlayer = null;
    private GameObject FinishUIPanel;
    private bool m_bRoundOver;
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
        DeathmatchTimer = new Timer(m_fTimedDeathMatchTime);
        SceneManager.sceneLoaded += OnSceneLoaded;
        waitForRoundEnd = new Timer(3);
        mInstance = GameManagerc.Instance;
        m_bRoundOver = false;
        Physics.gravity = new Vector3(0 , 0 , 10);
    }


    // Update is called once per frame
    void Update()
    {
        if (InGamePlayers.Count > 1)
        {
            StartCoroutine(CheckForRoundEnd());
        }

    }

    IEnumerator CheckForRoundEnd()
    {
        if (!m_bRoundOver)
        {
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

    void RoundEndLastManStanding()
    {
        int DeadCount = 0;
        foreach (PlayerStatus player in InGamePlayers)
        {
            if (player.IsDead)
            {
                DeadCount++;
            }
        }
        if (DeadCount >= InGamePlayers.Count - 1)
        {
            m_bRoundOver = true;
            int i = 0;
            foreach (PlayerStatus player in InGamePlayers)
            {

                if (!player.IsDead)
                {
                    PlayerWins[player] += 1;
                    if (PlayerWins[player] >= m_iPointsNeeded)
                    {
                        Debug.LogError("Poinst required have been reached");
                        Time.timeScale = 0;
                        UIManager.Instance.OpenUIElement(FinishUIPanel, true);
                        UIManager.Instance.RemoveLastPanel = false;
                        FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
                        FindObjectOfType<EventSystem>().SetSelectedGameObject(FinishUIPanel.transform.Find("Rematch").gameObject);

                        
                        //this one line breaks things ResetPoints();
                        //TODO Load Character select / win screen;
                        //TODO Sort players by score?
                    }
                }
                ++i;
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
        Debug.Log("Scene load");
        if (GameObject.Find("FinishedGamePanel"))
        {
            Debug.ClearDeveloperConsole();
            FinishUIPanel = GameObject.Find("FinishedGamePanel");
            FinishUIPanel.transform.Find("Rematch").GetComponent<Button>().onClick.AddListener(delegate { Rematch(); });
            FinishUIPanel.transform.Find("Main Menu").GetComponent<Button>().onClick.AddListener(delegate { GoToStart(); });
            
            for (int i = 0; i < FinishUIPanel.transform.childCount; ++i)
            {
                FinishUIPanel.transform.GetChild(i).gameObject.SetActive(false);
            }

        }

    }

    public void AddPlayer(PlayerStatus aPlayer)
    {
        InGamePlayers.Add(aPlayer);
        PlayerWins.Add(aPlayer , 0);
    }


    public void Rematch()
    {
        Time.timeScale = 1;
        m_bRoundOver = false;
        foreach (KeyValuePair<PlayerStatus , int> item in PlayerWins)
        {
            item.Key.ResetPlayer();
        }
        foreach (var item in InGamePlayers)
        {
            PlayerWins[item] = 0;
        }
        WinningPlayer = null;
        m_bRoundOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void GoToStart()
    {
        for (int i = 0; i < InGamePlayers.Count; i++)
        {
            InGamePlayers[i].Clear();
            InGamePlayers[i].gameObject.SetActive(false);
        }

        //Debug.Break();
        //empty the list
        //InGamePlayers.Clear();
        //turn off all the singletons so they no longer update, then destroy them
        //StartCoroutine(waitForSeconds());
        UIManager.Instance.gameObject.SetActive(false);
        ControllerManager.Instance.gameObject.SetActive(false);
        CharacterSelectionManager.Instance.gameObject.SetActive(false);
        PlayerUIArray.Instance.gameObject.SetActive(false);

        Destroy(PlayerUIArray.Instance.gameObject);
        Destroy(UIManager.Instance.gameObject);
        Destroy(ControllerManager.Instance.gameObject);
        Destroy(CharacterSelectionManager.Instance.gameObject);
        Destroy(this.gameObject);
        Time.timeScale = 1;
        //WTF scene begings to load even though line hasnt been called. what is happening. 
        SceneManager.LoadScene(0);
    }
    IEnumerator waitForSeconds()
    {
        yield return new WaitForSecondsRealtime(1);
    }

}

