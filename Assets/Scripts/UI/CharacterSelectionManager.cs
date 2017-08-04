using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionManager : MonoBehaviour
{
    public GameObject[] CharacterArray;

    public Dictionary<GameObject , bool> CharacterSelectionStatus;

    public Dictionary<XboxCtrlrInput.XboxController , GameObject> playerSelectedCharacter = new Dictionary<XboxCtrlrInput.XboxController , GameObject>();

    static CharacterSelectionManager mInstance = null;

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


}
