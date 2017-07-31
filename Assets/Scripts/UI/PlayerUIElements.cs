using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//WTF Louis Nguyen, 28th July 2017;

public class PlayerUIElements : MonoBehaviour
{
    //TODO change these to the sprites and sprite masks later. 
    public GameObject m_ManaText; 
    public GameObject m_HealthText;
    public GameObject m_AmmoText;
    public GameObject m_AbilityType;
    public GameObject m_manaBarMask;
    public GameObject m_HealthBarMask;
	// Use this for initialization
	void Awake()
    {
        //lets find my children and find the objects
        m_ManaText = transform.Find("Mana").gameObject;
        m_HealthText = transform.Find("Health").gameObject;
        m_AmmoText = transform.Find("Ammo").gameObject;
        m_AbilityType = transform.Find("AbilityType").gameObject;

        //find the mana bar
        m_manaBarMask = transform.Find("ManaBar").Find("BarMask").gameObject;
        m_HealthBarMask = transform.Find("HealthBar").Find("BarMask").gameObject;
        if (!this.GetComponentInParent<PlayerUIArray>())
        {
            this.transform.parent.gameObject.AddComponent<PlayerUIArray>();
        }
        else
        {
            GetComponentInParent<PlayerUIArray>().UpdateArray();
        }
	}
	
}

//! a container class used for other scripts, used to find any of the UI required for the player(s)
public class PlayerUIArray : MonoBehaviour
{
    [HideInInspector]
    public PlayerUIElements[] playerElements;

    void Awake()
    {
        playerElements = this.GetComponentsInChildren<PlayerUIElements>();

    }

    public void UpdateArray()
    {
        playerElements = this.GetComponentsInChildren<PlayerUIElements>();
    }
}

