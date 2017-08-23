using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XInputDotNetPure;
using XboxCtrlrInput;

public class ControllerManager : MonoBehaviour
{
    public int AbilityToAdd = 0;
    bool addedAbility = false;
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
    Dictionary<PlayerIndex , bool> playerIdx = new Dictionary<PlayerIndex , bool>
    {
        {PlayerIndex.One, false },
        {PlayerIndex.Two, false },
        {PlayerIndex.Three, false },
        {PlayerIndex.Four, false },
    };

    //dictionary mapping XCI index with the XInputDotNet indexes
    Dictionary<PlayerIndex , XboxController> xboxControllers = new Dictionary<PlayerIndex , XboxController>
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

    List<GameObject> players = new List<GameObject>();
    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        mInstance = Instance;
        //GameObject spawnParent = GameObject.FindGameObjectWithTag("SpawnPoints");
        ////Find the spawn points
        ////Debug.LogError(spawnParent.transform.childCount);
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
        ref_cameraController = GameObject.FindObjectOfType<CameraControl>();
    }

    IEnumerator LookForPlayers()
    {
        // Debug.Log(nextPlayer);
        for (int i = 0; i < maxPlayers; i++)
        {
            //make a player index based off what i is
            PlayerIndex testIndex = (PlayerIndex)i;
            //get the gamepad state of the current index set before
            GamePadState testState = GamePad.GetState(testIndex);
            if (testState.IsConnected &&
                !playerIdx[testIndex] &&
                (XCI.GetButtonDown(XboxButton.Start , xboxControllers[testIndex]) || XCI.GetButtonDown(XboxButton.Back , xboxControllers[testIndex]))
                && DebugMode)
            //if the player of index i has pressed Start, and their controller is connected, and their controller has yet to be assgined
            {

                //assign a controller and spawn the player
                playerIdx[testIndex] = true;
                if (XCI.GetButtonDown(XboxButton.Start , xboxControllers[testIndex]))
                {
                    GameObject go2 = Instantiate(playerPrefab , spawnPoints[nextPlayer].position , Quaternion.identity , null);
                    go2.GetComponent<ControllerSetter>().SetController(testIndex);
                    go2.GetComponent<ControllerSetter>().m_playerNumber = i;
                    go2.GetComponent<PlayerStatus>().spawnIndex = nextPlayer;
                    PlayerUIArray.Instance.playerElements[i].gameObject.SetActive(true);
                    GameManagerc.Instance.AddPlayer(go2.GetComponent<PlayerStatus>());
                    DontDestroyOnLoad(go2);
                    // AddAbility(AbilityToAdd, go);
                    addedAbility = true;
                    go2.SetActive(true);
                    if (ref_cameraController)
                    {
                        //if the cameracontrol exists, add the instantiated player to a camera targets
                        ref_cameraController.m_Targets.Add(go2.transform);
                    }
                }
                else if (XCI.GetButtonDown(XboxButton.Back , xboxControllers[testIndex]))
                {
                    GameObject go = Instantiate(playerPrefab2 , spawnPoints[nextPlayer].position , Quaternion.identity , null);
                    go.GetComponent<ControllerSetter>().SetController(testIndex);
                    go.GetComponent<ControllerSetter>().m_playerNumber = i;
                    go.GetComponent<PlayerStatus>().spawnIndex = nextPlayer;
                    GameManagerc.Instance.AddPlayer(go.GetComponent<PlayerStatus>());
                    DontDestroyOnLoad(go);
                    // AddAbility(AbilityToAdd, go);
                    addedAbility = true;
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

        yield return new WaitForEndOfFrame();
    }
    void AddAbility(int abilityIndex , GameObject playerToAddAbility)
    {
        if (addedAbility)
        {
            abilityIndex = 1;
        }
        else
        {
            abilityIndex = 0;
        }

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
                temp1.m_TeleportForce = mine1.m_TeleportForce;
                break;

            default: break;

        }

    }
    void OnSceneLoaded(Scene scene , LoadSceneMode mode)
    {
        ControllerManager[] items = FindObjectsOfType<ControllerManager>() as ControllerManager[];

        for (int i = 0; i < items.Length; ++i)
        {
            if (items[i] != this)
            {
                //Destroy(items[i].gameObject);
            }
        }
        
        if (scene.buildIndex != 0)
        {
            GameObject spawnParent = GameObject.FindGameObjectWithTag("SpawnPoints");
            //Find the spawn points    ////    spawnPoints = new Transform[spawnParent.transform.childCount];
            if (spawnParent)
            {
                for (int i = 0; i < spawnParent.transform.childCount; ++i)
                {
                    spawnPoints[i] = spawnParent.transform.GetChild(i);
                }
            }
        }

        //look for the spawn points
    }

    public void FindSpawns()
    {
        GameObject spawnParent = GameObject.FindGameObjectWithTag("SpawnPoints");
        //Find the spawn points
        spawnPoints = new Transform[spawnParent.transform.childCount];
        for (int i = 0; i < spawnParent.transform.childCount; ++i)
        {
            spawnPoints[i] = spawnParent.transform.GetChild(i);
        }

    }
}
