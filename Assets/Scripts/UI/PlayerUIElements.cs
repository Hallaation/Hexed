using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//WTF Louis Nguyen, 28th July 2017;

public class PlayerUIElements : MonoBehaviour
{
    //TODO change these to the sprites and sprite masks later. 
    [HideInInspector]
    public GameObject m_AmmoText;

    [HideInInspector]
    public GameObject m_manaBarMask;
    [HideInInspector]
    public GameObject m_HealthBarMask;
    [HideInInspector]
    public GameObject m_healthScrolllingIcon;
    [HideInInspector]
    public GameObject m_SpecialScrollingIcon;

    public Material m_StaticObjectMaterial;
    public Material m_UIOutlineMaterial;
	// Use this for initialization
	void Awake()
    {
        m_AmmoText = transform.Find("StatusUI").Find("Ammo").gameObject;


        //m_manaBarMask = transform.Find("OldObjects").Find("ManaBar").Find("BarMask").gameObject;
        m_manaBarMask = transform.Find("StatusUI").Find("SpecialBar").Find("BarMask").gameObject;
        //m_HealthBarMask = transform.Find("OldObjects").Find("HealthBar").Find("BarMask").gameObject;

        m_HealthBarMask = transform.Find("StatusUI").Find("HealthBar").Find("BarMask").gameObject;
        m_healthScrolllingIcon = transform.Find("StatusUI").Find("HealthBar").Find("Scrolling_Icon").gameObject;
        m_SpecialScrollingIcon = transform.Find("StatusUI").Find("SpecialBar").Find("Scrolling_Icon").gameObject;
        //add the player ui array component to the parent object
        if (!this.GetComponentInParent<PlayerUIArray>())
        {
            this.transform.parent.gameObject.AddComponent<PlayerUIArray>();
        }
        else
        {
            PlayerUIArray.instance.UpdateArray();
        }
        
	}

    private void Update()
    {
        m_UIOutlineMaterial.SetColor("_Color" , m_StaticObjectMaterial.color);
    }
}

//! a container class used for other scripts, used to find any of the UI required for the player(s)
public class PlayerUIArray : MonoBehaviour
{
    [HideInInspector]
    public PlayerUIElements[] playerElements;

    public static PlayerUIArray instance;
    void Awake()
    {
        if (instance) //check to see if it already exists
        {
            Debug.LogError("More than one");
            return;
        }
        instance = this;
        playerElements = this.GetComponentsInChildren<PlayerUIElements>();
    }

    public void UpdateArray()
    {
        playerElements = this.GetComponentsInChildren<PlayerUIElements>();
    }
}
