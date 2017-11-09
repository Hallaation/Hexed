using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using XboxCtrlrInput;
using XInputDotNetPure;
using Kino;
using UnityEngine.Audio;
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
    HEAD_HUNTERS,
    //DEATHMATCH_POINTS, //killing a player will earn them a point, up to a certain point Currently broken and only semi implemented.

}

//Used to store the game state
public class GameManagerc : MonoBehaviour
{
    //dictionary mapping XCI index with the XInputDotNet indexes
    Dictionary<XboxController, int> XboxControllerPlayerNumbers = new Dictionary<XboxController, int>
    {
        {XboxController.First,   0 },
        {XboxController.Second,  1 },
        {XboxController.Third,   2 },
        {XboxController.Fourth,  3 },
    };

    Timer waitForRoundEnd;
    //lets try a dictionary again
    //public List<int> PlayerWins = new List<int>();
    public Dictionary<PlayerStatus, int> PlayerWins = new Dictionary<PlayerStatus, int>();
    public List<PlayerStatus> InGamePlayers = new List<PlayerStatus>();
    // GameObject WinningPlayer = null;

    public Gamemode_type m_gameMode = Gamemode_type.LAST_MAN_STANDING_DEATHMATCH;

    public int m_iPointsNeeded = 5;
    public float m_fTimeTillPoints = 2f;
    public float m_fTimeAfterPoints = 3.0f;
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
    private PlayerStatus m_WinningPlayer;
    private Animator InGameScreenAnimator;
    public Animator GetScreenAnimator() { return InGameScreenAnimator; }
    private bool mbFinishedShowingScores = false;
    public bool mbInstanceIsMe = false;
    public bool mbMapLoaded = false;
    private bool m_bFirstTimeLoading = true;
    [SerializeField]
    private bool m_bGamePaused = false;
    [SerializeField]
    private bool m_bRoundReady = false;
    private bool m_bDoGlitch = true;
    private bool m_bDoReadyKill = true;
    private bool m_bRematchPressed = false;
    public bool RoundReady { get { return m_bRoundReady; } }
    private ScreenTransition screenTransition;

    public bool Paused { get { return m_bGamePaused; } set { m_bGamePaused = value; } }

    public int m_iPointsIndex = 0;
    public bool m_bAllowPause = false;
    [HideInInspector]
    public bool m_bDoLogoTransition = true;
    GameObject[] PointXPositions;
    GameObject[] PointYPositions;

    public AudioMixer MasterAudioMixer;
    public AudioClip m_DingSound;
    private AudioSource m_AudioSource;
    //! Screen Glitch lerp values
    //Scan line, Vertical Lines, Horizontal Shake, Colour Drift.
    private Vector4 lerpValues = new Vector4(0.8f, 0.6f, 0.3f, 0.7f);
    private Vector4 CurrentGlitchValues = new Vector4();
    public List<RigidbodyPauser> _rbPausers;

    public AudioClip m_GlitchEffect;
    [HideInInspector]
    public PlayerStatus lastPlayerToEarnPoints = null;

    private GameObject ReadyKillContainer;
    private Color pointsOriginalColour;
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
                    mInstance.gameObject.AddComponent<MusicFader>();
                    mInstance.GetComponent<MusicFader>().FadeSpeed = 5;
                }
                //set to dont destroy on load
                DontDestroyOnLoad(mInstance.gameObject);
            }

            return mInstance;
        }
    }
    IEnumerator SetInitalAudio()
    {
        yield return new WaitForSeconds(1);
        MasterAudioMixer = AudioManager.RequestMixerGroup(SourceType.MASTER).audioMixer;
        if (SettingsManager.Instance)
            MasterAudioMixer.SetFloat("Music", SettingsManager.Instance.musicVolumeSlider.value);
        yield return null;
    }

    // Use this for initialization
    void Awake()
    {
        m_DingSound = Resources.Load("Audio/SFX/ding-sound-effect") as AudioClip;
        m_GlitchEffect = Resources.Load("Audio/SFX/glitch-sound-effect") as AudioClip;
        m_GlitchEffect.LoadAudioData();
        m_AudioSource = this.GetComponent<AudioSource>();
        _rbPausers = new List<RigidbodyPauser>();


        if (!m_AudioSource)
        {
            m_AudioSource = this.gameObject.AddComponent<AudioSource>();
            m_AudioSource.clip = m_DingSound;
            m_AudioSource.outputAudioMixerGroup = AudioManager.RequestMixerGroup(SourceType.SFX);
        }

        StartCoroutine(SetInitalAudio());


        SingletonTester.Instance.AddSingleton(this);
        InstanceCreated = true;
        //Find the 
        DontDestroyOnLoad(this.gameObject);
        //DeathmatchTimer = new Timer(m_fTimedDeathMatchTime);
        waitForRoundEnd = new Timer(m_fTimeAfterPoints);
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

        m_bRoundOver = false;
        Physics.gravity = new Vector3(0, 0, 10); //why
        m_bRoundReady = true;
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

            if (Input.GetKeyDown(KeyCode.O))
            {
                StartCoroutine(InterpolateGlitch(false));
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                StartCoroutine(InterpolateGlitch(true));
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
                    mbFinishedShowingScores = false;
                    break;
                case Gamemode_type.HEAD_HUNTERS:
                    RoundEndHeadHunters();
                    CheckPlayersPoints();
                    mbFinishedShowingScores = false;
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
                //if (waitForRoundEnd.Tick(Time.deltaTime))
                //{
                //Don't think I need to scramble the spawns here, I'll do it anyway
                //reload scene
                ControllerManager.Instance.FindSpawns();
                foreach (PlayerStatus players in InGamePlayers)
                {
                    players.ResetPlayer();
                }
                m_bRoundOver = false;
                if (FindObjectOfType<ScreenTransition>())
                    FindObjectOfType<ScreenTransition>().OpenDoor();

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //reloads the scene.
                                                                                  //TODO instead of reloading scene, just reset E V E R Y T H I N G in the scene.
                                                                                  //End of round logic goes here.
                                                                                  //} 
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
            foreach (PlayerStatus player in InGamePlayers)
            {
                if (!player.IsDead)
                {
                    player.mIEarnedPoints++;
                    lastPlayerToEarnPoints = player;
                    //increase the winning player's point by 1
                    Debug.Log("add all");
                    StartCoroutine(AddToAllPoints());
                }
            }
            mbFinishedShowingScores = true;
            m_bRoundOver = true;
        }
    }

    void RoundEndHeadHunters()
    {
        int DeadCount = 0;
        foreach (PlayerStatus player in InGamePlayers)
        {
            if (player.IsDead)
            {
                DeadCount++;
            }
        }
        if (DeadCount >= InGamePlayers.Count - 1 || m_WinningPlayer)
        {
            if (!m_WinningPlayer)
            {
                StartCoroutine(AddToAllPoints());
            }
            mbFinishedShowingScores = true;
            m_bRoundOver = true;
        }
    }

    void CheckPlayersPoints()
    {
        //If player has reached the points required to win
        //And I havn't shown the finished panel yet, show it, set the show panel to true so this doesnt run again.
        if (lastPlayerToEarnPoints) //null check
        {
            if (!m_WinningPlayer) //if there isn't a winner
            {
                if (PlayerWins[lastPlayerToEarnPoints] + lastPlayerToEarnPoints.mIEarnedPoints >= m_iPointsNeeded) //if the last player to earn a point has won
                {
                    m_WinningPlayer = lastPlayerToEarnPoints; //set my winning player
                    m_bRoundOver = true;
                    StartCoroutine(AddToAllPoints());
                    //StartCoroutine(AddToAllPoints());
                }
            }
        }

        if (mbFinishedShowingScores)
        {
            //foreach (PlayerStatus player in InGamePlayers)
            //{
            if (PlayerWins[lastPlayerToEarnPoints] >= m_iPointsNeeded)
            {
                m_WinningPlayer = lastPlayerToEarnPoints;
                //Set the time scale to 0 (essentially pausing the game-ish)
                //Time.timeScale = 0;
                //open the finish panel, UI manager will set all the children to true, thus rendering them
                //#finish panel, 

                m_bDoGlitch = false;
                InGameScreenAnimator.SetTrigger("ShowScreen");
                PointsPanel.SetActive(false);
                MenuPanel.SetActive(false);
                UIManager.Instance.OpenUIElement(FinishUIPanel, true);
                GameObject startscreen;
                for (int i = 0; i < (startscreen = GameObject.Find("StartScreen")).transform.childCount; i++)
                {
                    startscreen.transform.GetChild(i).GetComponent<Image>().enabled = false;
                }
                m_bDoReadyKill = false;
                //TODO Player portraits
                FinishUIPanel.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = m_WinningPlayer.GetComponent<BaseAbility>().m_CharacterPortrait;
                FinishUIPanel.transform.GetChild(2).GetChild(1).GetComponent<Image>().color = m_WinningPlayer.GetComponent<PlayerStatus>()._playerColor;
                //  m_WinningPlayer = m_WinningPlayer;
                if (FindObjectOfType<ScreenTransition>())
                    FindObjectOfType<ScreenTransition>().OpenDoor();

                UIManager.Instance.RemoveLastPanel = false;
                GameManagerc.Instance.Paused = true;
                //Reset the event managers current selected object to the rematch button
                if (FindObjectOfType<EventSystem>().currentSelectedGameObject == null)
                {
                    FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
                    FindObjectOfType<EventSystem>().SetSelectedGameObject(FinishUIPanel.transform.Find("Main Menu").gameObject);
                    //mbFinishedPanelShown = true;
                }
            }
            //}
        }
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)

    {
        //oh fukc
        //check if the instance is this game object

        //If the map to load isnt null, load it
        if (scene.buildIndex == 1)
        {
            m_bAllowPause = true;
            UINavigation LoadInstance = UINavigation.Instance;
            GetComponent<MusicFader>().FadeIn();
            if (!m_bFirstTimeLoading) //if this isn't the first time loading into the scene
            {
                if (FindObjectOfType<ScreenTransition>())
                {
                    screenTransition = FindObjectOfType<ScreenTransition>();
                    for (int i = 0; i < screenTransition.transform.childCount; i++)
                    {
                        screenTransition.transform.GetChild(i).GetComponent<Image>().enabled = false;
                    }
                }
                if (m_bDoGlitch)
                {
                    StartCoroutine(InterpolateGlitch(true));
                }
            }
            else
            {
                m_bFirstTimeLoading = false;
            }

            if (MapToLoad)
            {
                GameObject go = Instantiate(MapToLoad);
                go.transform.position = Vector3.zero;
                go.transform.DetachChildren();
                mInstance.mbMapLoaded = true;
                ControllerManager.Instance.FindSpawns();
                CharacterSelectionManager.Instance.LoadPlayers();


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
            UIManager.Instance.SetDefaultPanel(MenuPanel);
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

            #region Point panel moving etc.
            //Move the point containers depending on how many points are required.
            for (int i = 0; i < PointsPanel.transform.childCount; i++)
            {
                PointContainers[i] = PointsPanel.transform.GetChild(i).gameObject;
                Vector3 temp = PointContainers[i].transform.position;
                //Get the last object in container (portrait)
                PointContainers[i].transform.position = new Vector3(PointXPositions[m_iPointsIndex].transform.position.x, temp.y, temp.z);

                //For every object after the points neeeded, turn them off since their not required.
                for (int j = m_iPointsNeeded; j < PointContainers[i].transform.childCount - 1; j++)
                {
                    PointContainers[i].transform.GetChild(j).gameObject.SetActive(false);
                }
                //Turn off the containers so they don't show up.
                PointContainers[i].SetActive(false);
            }

            //Load player portraits.
            foreach (var item in CharacterSelectionManager.Instance.playerSelectedCharacter)
            {

                int playerNumber = (int)item.Key - 1;

                if (item.Value.GetComponent<BaseAbility>().m_CharacterPortrait) //If the character has a portrait
                {
                    //Get the last object in point containers (the portrait container), get the first child (the "fill"- what is consitency) and change its sprite to the character's (item.value) sprite found in the base ability.
                    PointContainers[playerNumber].transform.GetChild(PointContainers[playerNumber].transform.childCount - 1).GetChild(0).GetComponent<Image>().sprite = item.Value.GetComponent<BaseAbility>().m_CharacterPortrait;
                    Image portraitOutline = PointContainers[playerNumber].transform.GetChild(PointContainers[playerNumber].transform.childCount - 1).GetChild(1).GetComponent<Image>();
                    portraitOutline.material.SetColor("_Color", Color.white);
                    Move tempMove = item.Value.GetComponent<Move>();

                    Database ColorDatabase = Resources.Load("Database") as Database;

                    Dictionary<string, Color> colorDictionary = new Dictionary<string, Color>();
                    for (int i = 0; i < ColorDatabase.colors.Length; i++)
                    {
                        if (!colorDictionary.ContainsKey(ColorDatabase.colors[i].PlayerType))
                            colorDictionary.Add(ColorDatabase.colors[i].PlayerType, ColorDatabase.colors[i].playerColor);
                    }
                    portraitOutline.color = colorDictionary[tempMove.ColorDatabaseKey];
                }
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
                ActivePanels[i].transform.position = new Vector3(temp.x, PointYPositions[4 - CharacterSelectionManager.Instance.JoinedPlayers - 2].transform.GetChild(i).position.y, temp.z);
            }

            foreach (var Player in InGamePlayers) //For every player in the game.
            {
                int iPlayerIndex = XboxControllerPlayerNumbers[Player.GetComponent<ControllerSetter>().mXboxController];
                for (int j = 0; j < PlayerWins[Player]; j++) //For every point the player has.
                {
                    if (PlayerWins[Player] > 0)
                    {
                        Image temp = PointContainers[iPlayerIndex].transform.GetChild(j).GetComponent<Image>();
                        pointsOriginalColour = PointContainers[iPlayerIndex].transform.GetChild(j).GetComponent<Image>().color;
                        PointContainers[iPlayerIndex].transform.GetChild(j).GetComponent<Image>().color = Color.blue;
                    }
                }
            }
            #endregion

            PointsPanel.SetActive(false);
            ReadyKillContainer = GameObject.Find("StartScreen");
            GameObject KillAudio = ReadyKillContainer.transform.GetChild(0).gameObject;
            GameObject GetReady = ReadyKillContainer.transform.GetChild(1).gameObject;

            //if (m_bShowReadyFight)

            if (m_bDoReadyKill)
            {
                StartCoroutine(ReadyKill(ReadyKillContainer));
            }
            //find weapons and add shit to them
            _rbPausers.Clear();
            _rbPausers = new List<RigidbodyPauser>();
            foreach (Rigidbody2D item in FindObjectsOfType<Rigidbody2D>())
            {
                if (item.GetComponentInParent<Weapon>())
                {
                    if (!item.GetComponent<RigidbodyPauser>())
                    {
                        RigidbodyPauser rbp = item.gameObject.AddComponent<RigidbodyPauser>();
                        if (!_rbPausers.Contains(rbp))
                        {
                            _rbPausers.Add(rbp);
                        }
                    }
                    else
                    {
                        RigidbodyPauser rbp = item.gameObject.GetComponent<RigidbodyPauser>();
                        if (!_rbPausers.Contains(rbp))
                        {
                            _rbPausers.Add(rbp);
                        }
                    }

                }
            }
            //m_bRoundReady = true;
            m_bDoLogoTransition = false;
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
        PlayerWins.Add(aPlayer, 0);
    }


    public void Rematch()
    {
        //reset the time scale
        GameManagerc.Instance.Paused = false;
        m_bRematchPressed = true;
        //The round is not over
        m_bRoundOver = false;
        //reset every player
        ControllerManager.Instance.FindSpawns();
        foreach (KeyValuePair<PlayerStatus, int> item in PlayerWins)
        {
            item.Key.ResetPlayer();
        }
        //Reset the players' points
        foreach (var item in InGamePlayers)
        {
            PlayerWins[item] = 0;
        }
        lastPlayerToEarnPoints.mIEarnedPoints = 0;
        PlayerWins[lastPlayerToEarnPoints] = 0;
        lastPlayerToEarnPoints = null;
        m_bRoundOver = false;
        mbFinishedShowingScores = false;
        //mbFinishedPanelShown = false;
        //reload the current scene
        m_bDoReadyKill = true;
        m_bDoGlitch = true;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        FinishUIPanel.SetActive(false);
        m_bRoundReady = false;
        m_WinningPlayer = null;
        //InGameScreenAnimator.SetTrigger("RemoveScreen");
        StartCoroutine(ReadyKill(ReadyKillContainer));

        //Go through each point and turn them back to white.
        foreach (var item in PointContainers) //for every point container
        {
            for (int i = 0; i < item.transform.childCount - 2; i++)
            {
                item.transform.GetChild(i).GetComponent<Image>().color = pointsOriginalColour;
            }
        }

    }

    /// <summary>
    /// used to reutn to the character selection screen
    /// </summary>
    public void GoToStart()
    {
        Time.timeScale = 1;
        if (!screenTransition)
        {
            screenTransition = FindObjectOfType<ScreenTransition>();
        }
        for (int i = 0; i < screenTransition.transform.childCount; i++)
        {
            screenTransition.transform.GetChild(i).GetComponent<Image>().enabled = true;
        }
        if (FindObjectOfType<ScreenTransition>())
            FindObjectOfType<ScreenTransition>().CloseDoor();
        CameraControl.mInstance.enabled = false;
        Destroy(CameraControl.mInstance);
        //CameraControl.mInstance.m_Targets.Clear();
        //clear the players list
        for (int i = 0; i < InGamePlayers.Count; i++)
        {
            InGamePlayers[i].Clear();
            InGamePlayers[i].gameObject.SetActive(false);
            Destroy(InGamePlayers[i].gameObject, 1);
        }
        InGamePlayers = new List<PlayerStatus>();

        //Quite literally a deconstrucotr
        PlayerWins.Clear();
        PlayerWins = new Dictionary<PlayerStatus, int>();
        InGamePlayers.Clear();
        InGamePlayers = new List<PlayerStatus>();
        m_gameMode = Gamemode_type.LAST_MAN_STANDING_DEATHMATCH;
        //FindObjectOfType<ScreenTransition>().CloseDoor();
        mInstance = null;
        FinishUIPanel = null;
        m_bRoundOver = false;
        MapToLoad = null;
        PointSprite = null;
        PointContainers = null;
        PointsPanel = null;
        InGameScreenAnimator = null;
        mbFinishedShowingScores = false;
        //  MenuPanel.SetActive(true);
        //mbLoadedIntoGame = false;
        mbInstanceIsMe = false;
        //mbFinishedPanelShown = false;
        mbMapLoaded = false;
        m_bGamePaused = false;
        m_bFirstTimeLoading = true;
        m_bDoReadyKill = true;
        m_bDoGlitch = true;
        UIManager.Instance.gameObject.SetActive(false);
        ControllerManager.Instance.gameObject.SetActive(false);
        CharacterSelectionManager.Instance.gameObject.SetActive(false);

        //PlayerUIArray.Instance.gameObject.SetActive(false);

        UINavigation.Instance.gameObject.SetActive(false);
        //Destroy the singleton objects

        ///Destroy(PlayerUIArray.Instance.gameObject);
        Destroy(GameAudioPicker.Instance);
        Destroy(UINavigation.Instance);
        Destroy(CharacterSelectionManager.Instance);
        Destroy(ControllerManager.Instance);
        Destroy(UIManager.Instance);
        Destroy(GameAudioPicker.Instance.gameObject);
        GetComponent<MusicFader>().FadeOut();
        StartCoroutine(ReturnToMenu());
        //Destroy(GameManagerc.Instance);
        //Destroy(this.gameObject);
    }

    IEnumerator ReturnToMenu()
    {

        yield return new WaitForSeconds(2);

        GameAudioPicker.Instance.gameObject.SetActive(false);
        Destroy(GameAudioPicker.Instance);
        Destroy(GameAudioPicker.Instance.gameObject);
        SceneManager.LoadScene(0);
        StartCoroutine(GetComponent<MusicFader>().MusicFadeIn());
        yield return null;
    }
    IEnumerator WaitForSeconds(float time)
    {
        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene(0);
    }

    IEnumerator AddPointsToPanel(PlayerStatus player, int PointGain = 0)
    {
        m_bAllowPause = false;
        yield return new WaitForSeconds(m_fTimeTillPoints);
        PointsPanel.SetActive(true);
        MenuPanel.SetActive(false);
        InGameScreenAnimator.SetTrigger("ShowScreen");
        mbFinishedShowingScores = false;
        GameObject go = new GameObject("Point", typeof(RectTransform));
        go.AddComponent<CanvasRenderer>();
        go.AddComponent<Image>().sprite = PointSprite;

        yield return new WaitForSeconds(1);

        int PlayerIndex = XboxControllerPlayerNumbers[player.GetComponent<ControllerSetter>().mXboxController]; //get the index of player
                                                                                                                //for the point container that the player owns, 

        //starting from the current player's points, turn everything after that to blue.
        for (int i = PlayerWins[player]; i < PlayerWins[player] + PointGain; i++)
        {
            Debug.Log(i);
            PointContainers[PlayerIndex].transform.GetChild(i).GetComponent<Image>().color = Color.blue;
        }
        Debug.Log(PlayerWins[player]);
        PlayerWins[player] += PointGain; //increase the player's points
        if (PlayerWins[player] >= m_iPointsNeeded)
        {
            m_bDoReadyKill = false;
        }
        player.mIEarnedPoints = 0; //set to 0 after the points are added.
        //TODO play ding.
        m_AudioSource.Play();
        yield return new WaitForSeconds(2);
        InGameScreenAnimator.SetTrigger("RemoveScreen");
        StartCoroutine(InterpolateGlitch(false));
        //if (FindObjectOfType<ScreenTransition>())
        //    FindObjectOfType<ScreenTransition>().CloseDoor();
        //Start interpolation.
        m_bAllowPause = true;
        //PointsPanel.SetActive(false);
    }

    IEnumerator AddToAllPoints()
    {

        m_bAllowPause = false;
        yield return new WaitForSeconds(m_fTimeTillPoints);
        PointsPanel.SetActive(true);
        MenuPanel.SetActive(false);
        InGameScreenAnimator.SetTrigger("ShowScreen");
        mbFinishedShowingScores = false;
        GameObject go = new GameObject("Point", typeof(RectTransform));
        go.AddComponent<CanvasRenderer>();
        go.AddComponent<Image>().sprite = PointSprite;

        yield return new WaitForSeconds(1);
        foreach (PlayerStatus player in InGamePlayers)
        {


            int PlayerIndex = XboxControllerPlayerNumbers[player.GetComponent<ControllerSetter>().mXboxController]; //get the index of player
                                                                                                                    //for the point container that the player owns
                                                                                                                    //starting from the current player's points, turn everything after that to blue.
            for (int i = PlayerWins[player]; i < PlayerWins[player] + player.mIEarnedPoints; i++)
            {
                if (player.mIEarnedPoints > 0)
                {
                    yield return new WaitForSeconds(0.5f);
                }
                PointContainers[PlayerIndex].transform.GetChild(i).GetComponent<Image>().color = Color.blue;
                m_AudioSource.Play();
            }
            PlayerWins[player] += player.mIEarnedPoints; //increase the player's points
            if (PlayerWins[player] >= m_iPointsNeeded)
            {
                m_bDoReadyKill = false;
            }
            player.mIEarnedPoints = 0; //set to 0 after the points are added.
        }
        //TODO play ding.
        yield return new WaitForSeconds(2);
        InGameScreenAnimator.SetTrigger("RemoveScreen");
        StartCoroutine(InterpolateGlitch(false));
        //if (FindObjectOfType<ScreenTransition>())
        //    FindObjectOfType<ScreenTransition>().CloseDoor();
        //Start interpolation.
        m_bAllowPause = true;
        //PointsPanel.SetActive(false);
        yield return null;
    }
    IEnumerator InterpolateGlitch(bool Reverse)
    {
        AnalogGlitch glitch = FindObjectOfType<AnalogGlitch>();
        var t = 0.0f;
        float maxTime = 1;
        m_AudioSource.clip = m_GlitchEffect;
        m_AudioSource.loop = true;
        m_AudioSource.Play();
        while (t < maxTime)
        {
            //Scan line, Vertical Lines, Horizontal Shake, Colour Drift.
            if (!Reverse) //Go into glitch
            {
                t += Time.deltaTime / maxTime;
                CurrentGlitchValues = Vector4.Lerp(Vector4.zero, lerpValues, t);
                glitch.scanLineJitter = CurrentGlitchValues.x;
                glitch.verticalJump = CurrentGlitchValues.y;
                glitch.horizontalShake = CurrentGlitchValues.z;
                glitch.colorDrift = CurrentGlitchValues.w;
                yield return null;
            }
            else //get out of glitch
            {
                t += Time.deltaTime / maxTime;
                CurrentGlitchValues = Vector4.Lerp(lerpValues, Vector4.zero, t);

                glitch.scanLineJitter = CurrentGlitchValues.x;
                glitch.verticalJump = CurrentGlitchValues.y;
                glitch.horizontalShake = CurrentGlitchValues.z;
                glitch.colorDrift = CurrentGlitchValues.w;
                yield return null;
            }

        } //end of while

        mbFinishedShowingScores = true;
        if (Reverse)
        {
            m_AudioSource.clip = m_DingSound;
            m_AudioSource.loop = false;
        }

        yield return null;
    }

    IEnumerator ReadyKill(GameObject ReadyFightContainer)
    {

        GameObject Kill = ReadyFightContainer.transform.GetChild(0).gameObject;
        GameObject getReady = ReadyFightContainer.transform.GetChild(1).gameObject;

        Image KillImage = Kill.GetComponent<Image>();
        Image GetReadyImage = getReady.GetComponent<Image>();

        m_bRoundReady = false;
        MenuPanel.SetActive(false);
        ScreenTransition transition = FindObjectOfType<ScreenTransition>();
        m_bAllowPause = false;
        if (transition)
        {
            Kill.GetComponent<Image>().enabled = false;
            getReady.GetComponent<Image>().enabled = false;
            if (!m_bRematchPressed)
                InGameScreenAnimator.SetTrigger("ShowScreen");
            while (!transition.DoorOpened) { yield return null; } //while the door hasn't opened yet.
            yield return new WaitForSeconds(0.5f);
            //if (m_bDoGlitch)
            //{
            //Turn kill off
            KillImage.enabled = false;
            //turn get ready on
            GetReadyImage.enabled = true;
            //play the get ready audio
            getReady.GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(2);
            //Turn get ready off
            GetReadyImage.enabled = false;
            //Turn kill on
            KillImage.enabled = true;
            //play kill audio
            Kill.GetComponent<AudioSource>().Play();
            //remove screen
            yield return new WaitForSeconds(1.5f);
            //im sorry
            InGameScreenAnimator.SetTrigger("RemoveScreen");
            m_bRematchPressed = false;
            m_bRoundReady = true;
            KillImage.enabled = false;
            m_bAllowPause = true;
            //ready to play
            //}
        }
        yield return null;
    }


}

