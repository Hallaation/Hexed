using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//WTF Louis Nguyen, 28th July 2017;

public class PlayerUIElements : MonoBehaviour
{
    //TODO change these to the sprites and sprite masks later. 
    [HideInInspector]
    public GameObject m_ManaText;
    [HideInInspector]
    public GameObject m_HealthText;
    [HideInInspector]
    public GameObject m_AmmoText;
    [HideInInspector]
    public GameObject m_AbilityType;

    [HideInInspector]
    public GameObject m_manaBarMask;
    [HideInInspector]
    public GameObject m_HealthBarMask;
    [HideInInspector]
    public GameObject m_ScrollingIcon;

    public Material m_StaticObjectMaterial;
	// Use this for initialization
	void Awake()
    {
        //lets find my children and find the objects
        m_ManaText = transform.Find("OldObjects").Find("Mana").gameObject;
        m_HealthText = transform.Find("OldObjects").Find("Health").gameObject;
        m_AmmoText = transform.Find("OldObjects").Find("Ammo").gameObject;
        m_AbilityType = transform.Find("OldObjects").Find("AbilityType").gameObject;

        //m_manaBarMask = transform.Find("OldObjects").Find("ManaBar").Find("BarMask").gameObject;
        m_manaBarMask = transform.Find("StatusUI").Find("SpecialBar").Find("BarMask").gameObject;
        //m_HealthBarMask = transform.Find("OldObjects").Find("HealthBar").Find("BarMask").gameObject;

        m_HealthBarMask = transform.Find("StatusUI").Find("HealthBar").Find("BarMask").gameObject;
        m_ScrollingIcon = transform.Find("StatusUI").Find("HealthBar").Find("Scrolling_Icon").gameObject;

        //add the player ui array component to the parent object
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

