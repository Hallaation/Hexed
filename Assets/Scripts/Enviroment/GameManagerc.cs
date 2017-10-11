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
    //DEATHMATCH_POINTS, //killing a player will earn them a point, up to a certain point Currently broken and only semi implemented.

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
    // GameObject WinningPlayer = null;

    public Gamemode_type m_gameMode = Gamemode_type.LAST_MAN_STANDING_DEATHMATCH;

    public int m_iPointsNeeded = 5;
    // public float m_fTimedDeathMatchTime;

    static GameManagerc mInstance = null;
    // Timer DeathmatchTimer;
    //lets keep the variables at the top shall we
    private GameObject FinishUIPanel;
    private bool m_bRoundOver;
    public GameObject MapToLoad;
    public bool InstanceCreated;
    public Sprite PointSprite;
    public GameObject[] PointContainers;
    public GameObject PointsPanel;
    public GameObject MenuPanel;
    private Animator InGameScreenAnimator;
    public Animator GetScreenAnimator() { return InGameScreenAnimator; }
    private bool mbFinishedShowingScores = false;
    public bool mbInstanceIsMe = false;
    public bool mbMapLoaded = false;
    private bool m_bGamePaused = false;
    public bool Paused { get { return m_bGamePaused; } set { m_bGamePaused = value; } }

    public int m_iPointsIndex = 0;
    GameObject[] PointXPositions;
    GameObject[] PointYPositions;
    //Lazy singleton
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
    void Awake()
    {
        SingletonTester.Instance.AddSingleton(this);
        InstanceCreated = true;
        //Find the 
        DontDestroyOnLoad(this.gameObject);
        //DeathmatchTimer = new Timer(m_fTimedDeathMatchTime);
        waitForRoundEnd = new Timer(3);
        mInstance = GameManagerc.Instance;
        if (mInstance.gameObject != this.gameObject)
        {
            Destroy(this.gameObject);
        }
        else
        {
            mbInstanceIsMe = true;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        //Debug.Log(mInstance.gameObject);
        m_bRoundOver = false;
        Physics.gravity = new Vector3(0 , 0 , 10); //why
    }


    // Update is called once per frame
    void Update()
    {
        if (!Paused)
        {
            Time.timeScale = 1;
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.G))
            {
                GoToStart();
                //Rematch();
                //MapToLoad = null;
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                Rematch();
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                KillPlayer1();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                //Stun all players
                int MaxPlayers = InGamePlayers.Count;
                for (int i = 0; i < MaxPlayers; i++)
                {
                    //InGamePlayers[i].KillPlayer(InGamePlayers[InGamePlayers.Count - 1]);
                    InGamePlayers[i].StunPlayer(Vector3.zero);
                }
            }
#endif
            if (InGamePlayers.Count > 1)
            {
                StartCoroutine(CheckForRoundEnd());
            }
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
                    CheckPlayersPoints();
                    break;
                //case Gamemode_type.DEATHMATCH_POINTS:
                //    RoundEndDeathMatchMaxPoints();
                //    CheckPlayersPoints();
                //    break;
                //case Gamemode_type.DEATHMATCH_TIMED:
                //    RoundEndDeathMatchTimed();
                //    break;
                //case Gamemode_type.CAPTURE_THE_FLAG:
                //    //RoundEndLastManStanding();
                //    m_bRoundOver = true;
                //    break;
                default:
                    break;
            }
        }
        else
        {
            //once the round is over, reset players
            //If the scores has been shown
            if (mbFinishedShowingScores)
            {
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
        }
        yield return null;
    }

    void KillPlayer1()
    {
        int MaxPlayers = InGamePlayers.Count;
        for (int i = 0; i < MaxPlayers - 1; i++)
        {
            InGamePlayers[i].KillPlayer(InGamePlayers[InGamePlayers.Count - 1]);
        }
        //InGamePlayers[0].KillPlayer(InGamePlayers[1]);
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
                    StartCoroutine(AddPointsToPanel(player));
                    // Debug.Break();
                }
                ++i; //probably unused, keeping it here in case it is actually used.
            }
            CheckPlayersPoints();
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
                //Debug.LogError("Points required have been reached");
                Time.timeScale = 0;
                //open the finish panel, UI manager will set all the children to true, thus rendering them
                UIManager.Instance.OpenUIElement(FinishUIPanel , true);
                UIManager.Instance.RemoveLastPanel = false;
                //Reset the event managers current selected object to the rematch button
                //FindObjectOfType<EventSystem>().SetSelectedGameObject(FinishUIPanel.transform.Find("Rematch").gameObject);

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

    void CheckPlayersPoints()
    {
        //If player has reached the points required to win
        //And I havn't shown the finished panel yet, show it, set the show panel to true so this doesnt run again.
        if (mbFinishedShowingScores)
        {
            foreach (PlayerStatus player in InGamePlayers)
            {
                if (PlayerWins[player] >= m_iPointsNeeded)
                {
                    //Set the time scale to 0 (essentially pausing the game-ish)
                    //Debug.LogError("Points required have been reached");
                    Time.timeScale = 0;
                    //open the finish panel, UI manager will set all the children to true, thus rendering them
                    UIManager.Instance.OpenUIElement(FinishUIPanel , true);
                    UIManager.Instance.RemoveLastPanel = false;
                    //Reset the event managers current selected object to the rematch button
                    if (FindObjectOfType<EventSystem>().currentSelectedGameObject == null)
                    {
                        FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
                        FindObjectOfType<EventSystem>().SetSelectedGameObject(FinishUIPanel.transform.Find("Main Menu").gameObject);
                        //mbFinishedPanelShown = true;
                    }
                    //TODO Load Character select / win screen;
                    //TODO Sort players by score?
                }
            }
        }
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
        //oh fukc
        //check if the instance is this game object

        //If the map to load isnt null, load it
        if (scene.buildIndex == 1)
        {
            UINavigation LoadInstance = UINavigation.Instance;
 
            if (MapToLoad)
            {
                GameObject go = Instantiate(MapToLoad);
                go.transform.position = Vector3.zero;
                go.transform.DetachChildren();
                mInstance.mbMapLoaded = true;
                ControllerManager.Instance.FindSpawns();
                CharacterSelectionManager.Instance.LoadPlayers();

                //Debug.Log("Loaded map and players");
            }
            //If I found the finished game panel
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
            MenuPanel = GameObject.Find("PausePanel");
            //MenuPanel.SetActive(true);
            //Find the points panel and populate the array.
            PointsPanel = GameObject.Find("PointsPanel");
            InGameScreenAnimator = PointsPanel.GetComponentInParent<Animator>();

            PointXPositions = new GameObject[GameObject.Find("PointXPositions").transform.childCount];
            for (int i = 0; i < PointXPositions.Length; i++)
            {
                PointXPositions[i] = GameObject.Find("PointXPositions").transform.GetChild(i).gameObject;
            }

            //Populate the Y positions 
            PointYPositions = new GameObject[GameObject.Find("PointYPositions").transform.childCount];
            for (int i = 0; i < PointYPositions.Length; i++)
            {
                PointYPositions[i] = GameObject.Find("PointYPositions").transform.GetChild(i).gameObject;
            }

            //Populate the array
            PointContainers = new GameObject[PointsPanel.transform.childCount];
            GameObject[] ActivePanels = new GameObject[4 - CharacterSelectionManager.Instance.JoinedPlayers];
            int ActivePanelIndex = 0;

            //Load player portraits.
            for (int i = 0; i < PointContainers.Length; i++)
            {
                if ()
                PointContainers[i].transform.GetChild(PointContainers[i].transform.childCount).GetChild(0).GetComponent<Image>().sprite = null;
            }

            //Move the point containers depending on how many points are required.
            for (int i = 0; i < PointsPanel.transform.childCount; i++)
            {
                PointContainers[i] = PointsPanel.transform.GetChild(i).gameObject;
                Vector3 temp = PointContainers[i].transform.position;
                //Get the last object in container (portrait)
                PointContainers[i].transform.position = new Vector3(PointXPositions[m_iPointsIndex].transform.position.x , temp.y , temp.z);

                //For every object after the points neeeded, turn them off since their not required.
                for (int j = m_iPointsNeeded; j < PointContainers[i].transform.childCount - 1; j++)
                {
                    PointContainers[i].transform.GetChild(j).gameObject.SetActive(false);
                }

                PointContainers[i].SetActive(false);
            }

            //can also be used for the amount of players in the scene.
            XboxController[] JoinedXboxControllers = new XboxController[CharacterSelectionManager.Instance.playerSelectedCharacter.Count];
            int nextIndex = 0;
            
            for (int i = 0; i < 4; i++)
            {
                if (CharacterSelectionManager.Instance.playerSelectedCharacter.ContainsKey(XboxController.First + i))
                {
                    JoinedXboxControllers[nextIndex] = XboxController.First + i;
                    nextIndex++;
                }
            }

            //Turn every player's UI on.
            foreach (var item in JoinedXboxControllers)
            {
                PointContainers[(int)item - 1].SetActive(true);
                ActivePanels[ActivePanelIndex] = PointContainers[(int)item - 1];
                ActivePanelIndex++;
            }

            for (int i = 0; i < 4 - CharacterSelectionManager.Instance.JoinedPlayers; i++)
            {
                Vector3 temp = ActivePanels[i].transform.position;
                ActivePanels[i].transform.position = new Vector3(temp.x , PointYPositions[4 - CharacterSelectionManager.Instance.JoinedPlayers - 2].transform.GetChild(i).position.y , temp.z);
            }

            foreach (var Player in InGamePlayers) //For every player in the game.
            {
                int iPlayerIndex = XboxControllerPlayerNumbers[Player.GetComponent<ControllerSetter>().mXboxController];
                for (int j = 0; j < PlayerWins[Player]; j++) //For every point the player has.
                {
                    if (PlayerWins[Player] > 0)
                    {
                        Image temp = PointContainers[iPlayerIndex].transform.GetChild(j).GetComponent<Image>();
                        PointContainers[iPlayerIndex].transform.GetChild(j).GetComponent<Image>().color = Color.blue;
                    }
                }
            }

            //PointsPanel.SetActive(false);
            //mInstance.mbLoadedIntoGame = true;
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
       // Debug.Log("Start of rematch");
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
        // WinningPlayer = null; // still unsued
        m_bRoundOver = false;
        mbFinishedShowingScores = false;
        //mbFinishedPanelShown = false;
        //reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //Debug.Log("End of rematch after button");
    }

    /// <summary>
    /// used to reutn to the character selection screen
    /// </summary>
    public void GoToStart()
    {
        Time.timeScale = 1;
        //clear the players list
        for (int i = 0; i < InGamePlayers.Count; i++)
        {
            InGamePlayers[i].Clear();
            InGamePlayers[i].gameObject.SetActive(false);
            Destroy(InGamePlayers[i].gameObject , 1);
        }
        InGamePlayers = new List<PlayerStatus>();

        //Quite literally a deconstrucotr
        PlayerWins.Clear();
        PlayerWins = new Dictionary<PlayerStatus , int>();
        InGamePlayers.Clear();
        InGamePlayers = new List<PlayerStatus>();
        m_gameMode = Gamemode_type.LAST_MAN_STANDING_DEATHMATCH;
        m_iPointsNeeded = 5;
        mInstance = null;
        FinishUIPanel = null;
        m_bRoundOver = false;
        MapToLoad = null;
        PointSprite = null;
        PointContainers = null;
        PointsPanel = null;
        InGameScreenAnimator = null;
        mbFinishedShowingScores = false;
        MenuPanel.SetActive(true);
        //mbLoadedIntoGame = false;
        mbInstanceIsMe = false;
        //mbFinishedPanelShown = false;
        mbMapLoaded = false;

        UIManager.Instance.gameObject.SetActive(false);
        ControllerManager.Instance.gameObject.SetActive(false);
        CharacterSelectionManager.Instance.gameObject.SetActive(false);
        //PlayerUIArray.Instance.gameObject.SetActive(false);

        UINavigation.Instance.gameObject.SetActive(false);
        //Destroy the singleton objects

        ///Destroy(PlayerUIArray.Instance.gameObject);
        Destroy(UIManager.Instance.gameObject);
        Destroy(ControllerManager.Instance.gameObject);
        Destroy(CharacterSelectionManager.Instance.gameObject);
        Destroy(UINavigation.Instance.gameObject);
        Destroy(this.gameObject);
        SceneManager.LoadScene(0);
        //StartCoroutine(WaitForSeconds(0.2f));
    }

    IEnumerator WaitForSeconds(float time)
    {
        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene(0);
    }

    IEnumerator AddPointsToPanel(PlayerStatus player)
    {
        PointsPanel.SetActive(true);
        MenuPanel.SetActive(false);
        InGameScreenAnimator.SetTrigger("ShowScreen");
        mbFinishedShowingScores = false;
        GameObject go = new GameObject("Point" , typeof(RectTransform));
        go.AddComponent<CanvasRenderer>();
        go.AddComponent<Image>().sprite = PointSprite;

        yield return new WaitForSeconds(1);

        int PlayerIndex = XboxControllerPlayerNumbers[player.GetComponent<ControllerSetter>().mXboxController];
        PointContainers[PlayerIndex].transform.GetChild(PlayerWins[player] - 1).GetComponent<Image>().color = Color.blue;

        yield return new WaitForSeconds(2);
        mbFinishedShowingScores = true;
        InGameScreenAnimator.SetTrigger("RemoveScreen");
        //PointsPanel.SetActive(false);

    }


}

