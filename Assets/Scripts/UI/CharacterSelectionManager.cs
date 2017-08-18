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
    //Each controller will have a gameobject(their player);
    public Dictionary<XboxCtrlrInput.XboxController , GameObject> playerSelectedCharacter = new Dictionary<XboxCtrlrInput.XboxController , GameObject>();

    static CharacterSelectionManager mInstance = null;
    public bool m_bMovedToMainScene = false;
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
        SceneManager.sceneLoaded += OnSceneLoaded;
        Object[] temp = Resources.LoadAll("Characters" , typeof(GameObject));
        CharacterArray = new GameObject[temp.Length];
        for (int i = 0; i < CharacterArray.Length;  ++i)
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
        if (playerSelectedCharacter.Count > 1 || Application.isEditor)
        {
            //only load the scene if I still havnt moved to arena scene
            if (Input.GetButtonDown("Start") && !m_bMovedToMainScene)
            {
                SceneManager.LoadScene(2); //oh fuck.
            }
        }

        if (Application.isEditor && Input.GetKeyDown(KeyCode.R))
        {
            foreach(PlayerStatus player in GameManagerc.Instance.InGamePlayers)
            {
                Destroy(player.gameObject.gameObject , 1);
            }
            SceneManager.LoadScene(0);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //if my currnet scene is 1 (when loaded into the game arean and I havn't already moved in, spawn the players
        if (scene.buildIndex == 1 && !m_bMovedToMainScene)
        {
            for (int i = 0; i < playerSelectedCharacter.Count; ++i)
            {
                ControllerManager.Instance.FindSpawns();
                GameObject go = Instantiate(playerSelectedCharacter[XboxController.First + i] , ControllerManager.Instance.spawnPoints[i].position , Quaternion.identity , null);
                go.GetComponent<ControllerSetter>().SetController(PlayerIndex.One + i);
                go.GetComponent<ControllerSetter>().m_playerNumber = i;
                go.GetComponent<PlayerStatus>().spawnIndex = i;
                PlayerUIArray.Instance.playerElements[i].gameObject.SetActive(true);
                GameManagerc.Instance.AddPlayer(go.GetComponent<PlayerStatus>());
                DontDestroyOnLoad(go);
                go.SetActive(true);
                CameraControl.mInstance.m_Targets.Add(go.transform);
                m_bMovedToMainScene = true;
            }
        }
    }
}
