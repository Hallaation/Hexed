using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;
using XInputDotNetPure;
public class CharacterSelectionManager : MonoBehaviour
{
    public GameObject[] CharacterArray;

    public Dictionary<GameObject , bool> CharacterSelectionStatus;
    public int JoinedPlayers = 0;
    //Each controller will have a gameobject(their player);
    public Dictionary<XboxCtrlrInput.XboxController , GameObject> playerSelectedCharacter = new Dictionary<XboxCtrlrInput.XboxController , GameObject>();

    static CharacterSelectionManager mInstance = null;
    public bool m_bMovedToMainScene = false;

    public bool LetPlayersSelectCharacters = false;
    //lazy singleton if an instance of this doesn't exist, make one
    //Instance property 
    public static CharacterSelectionManager Instance
    {
        get
        {
            //if an instance doesnt exist
            if (mInstance == null)
            {
                //look for an instance
                mInstance = (CharacterSelectionManager)FindObjectOfType(typeof(CharacterSelectionManager));
                //if an instance wasn't found, make an instance
                if (mInstance == null)
                {
                    mInstance = (new GameObject("CharacterSelectionManager")).AddComponent<CharacterSelectionManager>();
                }
                //set to dont destroy on load
                DontDestroyOnLoad(mInstance.gameObject);
            }
            return mInstance;
        }
    }

    // Use this for initialization
    private void Awake()
    {
        SingletonTester.Instance.AddSingleton(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
        Object[] temp = Resources.LoadAll("Characters" , typeof(GameObject));
        CharacterArray = new GameObject[temp.Length];
        for (int i = 0; i < CharacterArray.Length; ++i)
        {
            CharacterArray[i] = temp[i] as GameObject;
        }
        //Application.targetFrameRate = 30;
        //! check if there is already an instance
        //? instantiate the dictionary, and populate it with the selectable characters.
        CharacterSelectionStatus = new Dictionary<GameObject , bool>();
        foreach (GameObject character in CharacterArray)
        {
            CharacterSelectionStatus.Add(character , false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (playerSelectedCharacter.Count > 1 /*|| Application.isEditor*/)
        {
            //only load the scene if I still havnt moved to arena scene
            if (Input.GetButtonDown("Start") && !m_bMovedToMainScene)
            {
                UIManager.Instance.MainMenuChangePanel(GameObject.Find("Third_Panel"));
                //SceneManager.LoadScene(2); //oh fuck.
            }
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Return))
            {
                UIManager.Instance.MainMenuChangePanel(GameObject.Find("Third_Panel"));
            }
#endif
        }

        if (Application.isEditor && Input.GetKeyDown(KeyCode.R))
        {
            foreach (PlayerStatus player in GameManagerc.Instance.InGamePlayers)
            {
                Destroy(player.gameObject.gameObject , 1);
            }
            SceneManager.LoadScene(0);
        }
    }

    public void LoadPlayers() //! SpawnPlayers, Spawn Players,
    {

        XboxController[] JoinedXboxControllers = new XboxController[playerSelectedCharacter.Count];
        int nextIndex = 0;
        for (int i = 0; i < 4; i++)
        {
            if (playerSelectedCharacter.ContainsKey(XboxController.First + i))
            {
                JoinedXboxControllers[nextIndex] = XboxController.First + i;
                nextIndex++;
            }
        }
        foreach (var item in playerSelectedCharacter)
        {
            Debug.Log(item.Key);
        }
        if (!m_bMovedToMainScene)
        {
            for (int i = 0; i < 4 - JoinedPlayers; ++i)
            {
                ControllerManager.Instance.FindSpawns();
                //Debug.Log(JoinedXboxControllers[i]);

                Vector3 spawnPosition = ControllerManager.Instance.spawnPoints[i].position; //Get the spawn position
                //Make the gameojbect and keep a reference scoped to the single loop

                GameObject go = Instantiate(playerSelectedCharacter[JoinedXboxControllers[i]] , spawnPosition , Quaternion.identity , null);
                //Set anything required for the player to work.
                go.GetComponent<ControllerSetter>().SetController(JoinedXboxControllers[i]);
                go.GetComponent<ControllerSetter>().m_playerNumber = (int)JoinedXboxControllers[i] - 1;
                go.GetComponent<PlayerStatus>().spawnIndex = i;
                //Find the Array for players
                PlayerUIArray.Instance.playerElements[i].gameObject.SetActive(true);
                //Make a reference in game manager
                GameManagerc.Instance.AddPlayer(go.GetComponent<PlayerStatus>());
                DontDestroyOnLoad(go);//turn it on
                go.SetActive(true);
                CameraControl.mInstance.m_Targets.Add(go.transform); //make a reference in camera control
                m_bMovedToMainScene = true;

            }
        }

    }
    void OnSceneLoaded(Scene scene , LoadSceneMode mode)
    {

        if (scene.buildIndex == 0)
        {
            playerSelectedCharacter.Clear();
            CharacterSelectionStatus.Clear();

            Object[] temp = Resources.LoadAll("Characters" , typeof(GameObject));
            CharacterArray = new GameObject[temp.Length];
            for (int i = 0; i < CharacterArray.Length; ++i)
            {
                CharacterArray[i] = temp[i] as GameObject;
            }
            //Application.targetFrameRate = 30;
            //! check if there is already an instance
            //? instantiate the dictionary, and populate it with the selectable characters.
            CharacterSelectionStatus = new Dictionary<GameObject , bool>();
            foreach (GameObject character in CharacterArray)
            {
                CharacterSelectionStatus.Add(character , false);
            }
        }
    }
}
