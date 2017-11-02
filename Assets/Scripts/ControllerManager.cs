using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XInputDotNetPure;
using XboxCtrlrInput;

public class ControllerManager : MonoBehaviour
{
    static ControllerManager mInstance = null;

    public bool DebugMode = false;
    //lazy singleton
    public static ControllerManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                //If I already exist, make the instance that
                mInstance = (ControllerManager)FindObjectOfType(typeof(ControllerManager));

                if (mInstance == null)
                {
                    //if not found, make an object and attach me to it
                    mInstance = (new GameObject("ControllerManager")).AddComponent<ControllerManager>();
                }

                DontDestroyOnLoad(mInstance.gameObject);

            }
            return mInstance;
        }
    }

    //dictionary used to determine if the controller index has been assigned yet.
    Dictionary<PlayerIndex, bool> playerIdx = new Dictionary<PlayerIndex, bool>
    {
        {PlayerIndex.One, false },
        {PlayerIndex.Two, false },
        {PlayerIndex.Three, false },
        {PlayerIndex.Four, false },
    };

    //dictionary mapping XCI index with the XInputDotNet indexes
    Dictionary<PlayerIndex, XboxController> xboxControllers = new Dictionary<PlayerIndex, XboxController>
    {
        {PlayerIndex.One, XboxController.First },
        {PlayerIndex.Two, XboxController.Second },
        {PlayerIndex.Three, XboxController.Third },
        {PlayerIndex.Four, XboxController.Fourth },
    };

    //4 players

    public bool assignControllers = true;
    public CameraControl ref_cameraController;
    public int maxPlayers = 4;
    int nextPlayer = 0;
    public GameObject playerPrefab;
    public GameObject playerPrefab2;
    public Transform[] spawnPoints;

    // List<GameObject> players = new List<GameObject>();
    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        ref_cameraController = GameObject.FindObjectOfType<CameraControl>();
        //smInstance = Instance;
        //GameObject spawnParent = GameObject.FindGameObjectWithTag("SpawnPoints");
        ////Find the spawn points

        //spawnPoints = new Transform[spawnParent.transform.childCount];
        //for (int i = 0; i < spawnParent.transform.childCount; ++i)
        //{
        //    spawnPoints[i] = spawnParent.transform.GetChild(i);
        //}
        //   playerPrefab = Resources.Load("Characters/SmokePlayer") as GameObject;
        if (DebugMode)
        {
            FindSpawns();
        }
    }


    void Update()
    {
        StartCoroutine(LookForPlayers());
    }

    IEnumerator LookForPlayers()
    {

        if (DebugMode)
        {
            for (int i = 0; i < maxPlayers; i++)
            {
                //make a player index based off what i is
                PlayerIndex testIndex = (PlayerIndex)i;
                //get the gamepad state of the current index set before
                GamePadState testState = GamePad.GetState(testIndex);
                if (testState.IsConnected &&
                    !playerIdx[testIndex] &&
                    (XCI.GetButtonDown(XboxButton.Start, xboxControllers[testIndex]) || XCI.GetButtonDown(XboxButton.Back, xboxControllers[testIndex])))
                //if the player of index i has pressed Start, and their controller is connected, and their controller has yet to be assgined
                {

                    //assign a controller and spawn the player
                    playerIdx[testIndex] = true;
                    if (XCI.GetButtonDown(XboxButton.Start, xboxControllers[testIndex]))
                    {
                        GameObject go2 = Instantiate(playerPrefab, spawnPoints[nextPlayer].position, Quaternion.identity, null);
                        go2.SetActive(true);
                        go2.GetComponent<ControllerSetter>().SetController(testIndex);
                        go2.GetComponent<ControllerSetter>().m_playerNumber = i;
                        go2.GetComponent<PlayerStatus>().spawnIndex = nextPlayer;
                        //PlayerUIArray.Instance.playerElements[i].gameObject.SetActive(true);
                        GameManagerc.Instance.AddPlayer(go2.GetComponent<PlayerStatus>());
                        DontDestroyOnLoad(go2);
                        go2.SetActive(true);
                        if (ref_cameraController)
                        {
                            //if the cameracontrol exists, add the instantiated player to a camera targets
                            ref_cameraController.m_Targets.Add(go2.transform);
                        }
                    }
                    else if (XCI.GetButtonDown(XboxButton.Back, xboxControllers[testIndex]))
                    {
                        GameObject go = Instantiate(playerPrefab2, spawnPoints[nextPlayer].position, Quaternion.identity, null);
                        go.SetActive(true);
                        go.GetComponent<ControllerSetter>().SetController(testIndex);
                        go.GetComponent<ControllerSetter>().m_playerNumber = i;
                        go.GetComponent<PlayerStatus>().spawnIndex = nextPlayer;
                        GameManagerc.Instance.AddPlayer(go.GetComponent<PlayerStatus>());
                        DontDestroyOnLoad(go);
                        go.SetActive(true);
                        if (ref_cameraController)
                        {
                            //if the cameracontrol exists, add the instantiated player to a camera targets
                            ref_cameraController.m_Targets.Add(go.transform);
                        }
                    }

                    nextPlayer++; //increment to determine next spawn point
                    break;
                }
            }
        }
        yield return new WaitForEndOfFrame();
    }
    void AddAbility(int abilityIndex, GameObject playerToAddAbility)
    {
        if (playerToAddAbility.GetComponent<BaseAbility>())
        {
            Destroy(playerToAddAbility.GetComponent<BaseAbility>());
        }

        switch (abilityIndex)
        {
            case 0:
                {
                    ShieldAbility temp = playerToAddAbility.AddComponent<ShieldAbility>();
                    ShieldAbility mine = GetComponent<ShieldAbility>();
                    temp.ManaCost = mine.ManaCost;
                    temp.RepeatedUsage = mine.RepeatedUsage;
                    temp.repeatedManaCost = mine.repeatedManaCost;
                    temp.PassiveManaRegeneration = mine.PassiveManaRegeneration;
                    temp.m_fMinimumManaRequired = mine.m_fMinimumManaRequired;
                    temp.m_fMovementSpeedSlowDown = mine.m_fMovementSpeedSlowDown;
                }
                break;
            case 1:
                Teleport temp1 = playerToAddAbility.AddComponent<Teleport>();
                Teleport mine1 = GetComponent<Teleport>();
                temp1.ManaCost = mine1.ManaCost;
                temp1.repeatedManaCost = mine1.repeatedManaCost;
                temp1.PassiveManaRegeneration = mine1.PassiveManaRegeneration;
                temp1.m_fMinimumManaRequired = mine1.m_fMinimumManaRequired;
                temp1.m_fMovementSpeedSlowDown = mine1.m_fMovementSpeedSlowDown;
                temp1.m_DashSpeed = mine1.m_DashSpeed;
                temp1.m_DurationOfDash = mine1.m_DurationOfDash;
                //temp1.m_TeleportForce = mine1.m_TeleportForce;
                break;

            default: break;

        }

    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ControllerManager[] items = FindObjectsOfType<ControllerManager>() as ControllerManager[];

        for (int i = 0; i < items.Length; ++i)
        {
            if (items[i] != this)
            {
                //Destroy(items[i].gameObject);
            }
        }

        //if (scene.buildIndex != 0)
        //{
        //    FindSpawns();
        //}

        //look for the spawn points
    }

    public void FindSpawns()
    {

        int NumofChildren = 0;
        int PlayerCountIndex = 0;
        GameObject spawnParent = GameObject.FindGameObjectWithTag("SpawnPoints");
        //Find the spawn points
        //Figure out which spawn points to get
        switch (4 - CharacterSelectionManager.Instance.JoinedPlayers)
        {
            case 2:
                spawnPoints = new Transform[spawnParent.transform.GetChild(0).childCount];
                NumofChildren = 2;
                PlayerCountIndex = 0;
                break;
            case 3:
                spawnPoints = new Transform[spawnParent.transform.GetChild(1).childCount];
                NumofChildren = 3;
                PlayerCountIndex = 1;
                break;
            case 4:
                spawnPoints = new Transform[spawnParent.transform.GetChild(2).childCount];
                NumofChildren = 4;
                PlayerCountIndex = 2;
                break;
            default:
                break;
        }

        //Get the spawn points depdendent on the number of players
        for (int i = 0; i < NumofChildren; ++i)
        {
            //print(spawnParent.transform.GetChild(PlayerCountIndex));
            //print(spawnParent.transform.GetChild(PlayerCountIndex).GetChild(i));
            spawnPoints[i] = spawnParent.transform.GetChild(PlayerCountIndex).GetChild(i);
        }

        //Scramble array
        int ScrambleAmount = 50;
        for (int i = 0; i < ScrambleAmount; i++)
        {
            for (int j = 0; j < spawnPoints.Length; j++)
            {
                int randomIndex = Random.Range(0, spawnPoints.Length);
                while (randomIndex == j)
                {
                    randomIndex = Random.Range(0, spawnPoints.Length);
                }
                Vector3 tempPosition = spawnPoints[j].transform.position;
                spawnPoints[j].position = spawnPoints[randomIndex].position;
                spawnPoints[randomIndex].position = tempPosition;
            }
        }
    }
}
