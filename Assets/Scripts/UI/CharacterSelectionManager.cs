using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionManager : MonoBehaviour
{
    public GameObject[] SelectableCharacters;
    
    public Dictionary<GameObject , bool> characterSelected;

    public static CharacterSelectionManager instance;
	// Use this for initialization
	void Start ()
    {
        //? instantiate the dictionary, and populate it with the selectable characters.
        characterSelected = new Dictionary<GameObject , bool>();
        foreach (GameObject character in SelectableCharacters)
        {
            characterSelected.Add(character , false);
        }
	}

    private void Awake()
    {
        //! check if there is already an instance
        if (instance)
        {
            Debug.LogError("There is already a character selection manager");
            return;
        }
        instance = this;
    }
    // Update is called once per frame
    void Update ()
    {
		
	}
}
