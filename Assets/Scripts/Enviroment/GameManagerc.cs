using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public List<int> PlayerWins = new List<int>();
    public Gamemode_type m_gameMode = Gamemode_type.LAST_MAN_STANDING_DEATHMATCH;
    public int m_iPointsNeeded = 5;
    public float m_fTimedDeathMatchTime;

    static GameManagerc mInstance = null;
    Timer DeathmatchTimer;
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

    public List<PlayerStatus> InGamePlayers = new List<PlayerStatus>();
    GameObject WinningPlayer = null;
    private bool m_bRoundOver;
    // Use this for initialization
    void Start()
    {
        DeathmatchTimer = new Timer(m_fTimedDeathMatchTime);
        SceneManager.sceneLoaded += OnSceneLoaded;
        waitForRoundEnd = new Timer(3);
        mInstance = GameManagerc.Instance;
        m_bRoundOver = false;
        Physics.gravity = new Vector3(0, 0, 10);
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
                    RoundEndLastManStanding();
                    //RoundEndDeathMatchMaxPoints();
                    break;
                case Gamemode_type.DEATHMATCH_TIMED:
                    RoundEndLastManStanding();
                    //RoundEndDeathMatchTimed();
                    break;
                case Gamemode_type.CAPTURE_THE_FLAG:
                    RoundEndLastManStanding();
                    //lol
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
                    PlayerWins[i] += 1;
                    if (PlayerWins[i] >= m_iPointsNeeded)
                    {
                        Debug.LogError("Poinst required have been reached");

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
        //TODO round shouldn't be over until one of the players has reached the maximum points
        //TODO For Max points, guns should be respawning with new ammo, any guns with no ammo should be deleted after a while when they have no ammo (like duck game)
        //TODO Respawn players after they are killed (Like Smash bros.) no i-Frames though, they can be camped for all I care.
        m_bRoundOver = true;
        //TODO Sort players by score?
        //TODO Load Character select / win screen;
    }

    void RoundEndDeathMatchTimed()
    {
        //TODO similiar to max points (should change this to kills) 
        //TODO Penalty for death? 
        //TODO Round isn't over until the timer has reached 0.
        m_bRoundOver = true;
        //TODO Sort players by score?
        //TODO Load Character select / win screen;
    }

    void OnSceneLoaded(Scene scene , LoadSceneMode mode)
    {
        //look for a gamemanager, then delete it.
        Object[] items = FindObjectsOfType<GameManagerc>();
        for (int i = 0; i < items.Length; ++i)
        {
            if (items[i] != this)
            {
                Destroy(items[i]);
            }
        }
        Debug.Log("Scene load");
    }

    public void AddPlayer(PlayerStatus aPlayer)
    {
        InGamePlayers.Add(aPlayer);
        PlayerWins.Add(0);
    }
}

