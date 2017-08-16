using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerc : MonoBehaviour
{
    Timer waitForRoundEnd;

    List<int> PlayerWins = new List<int>();



    static GameManagerc mInstance = null;
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
        SceneManager.sceneLoaded += OnSceneLoaded;
        waitForRoundEnd = new Timer(3);
        mInstance = GameManagerc.Instance;
        m_bRoundOver = false;
        foreach (PlayerStatus player in InGamePlayers)
        {
            PlayerWins.Add(0);
        }
            
    }


    // Update is called once per frame
    void Update()
    {
        if (InGamePlayers.Count > 1)
        {
            StartCoroutine(CheckForRoundEnd());
        }
        else
        {

        }
    }

    IEnumerator CheckForRoundEnd()
    {
        if (!m_bRoundOver)
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
                RoundEnd();
            }
        }
        else
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
        yield return null;
    }

    void RoundEnd()
    {
        m_bRoundOver = true;
        int i = 0;
        foreach (PlayerStatus player in InGamePlayers)
        {

            if (!player.IsDead)
            {
                PlayerWins[i] += 1;
            }
            ++i;
        }
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
    }
}
