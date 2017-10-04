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
    public GameObject m_ScoreText;

    [HideInInspector]
    public GameObject m_manaBarMask;
    [HideInInspector]
    public GameObject m_HealthBarMask;
    [HideInInspector]
    public GameObject m_HealthBarContainer;
    [HideInInspector]
    public GameObject m_healthScrolllingIcon;
    [HideInInspector]
    public GameObject m_SpecialScrollingIcon;

    public List<GameObject> m_objects;
    public Material m_StaticObjectMaterial;
    public Material m_UIOutlineMaterial;
    // Use this for initialization
    void Awake()
    {
        //m_AmmoText = transform.Find("StatusUI").Find("Ammo").gameObject;

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
            PlayerUIArray.Instance.UpdateArray();
        }
        m_objects = new List<GameObject>();
        for (int i = 0; i < transform.Find("StatusUI").childCount; i++)
        {
            m_objects.Add(transform.Find("StatusUI").GetChild(i).gameObject);
        }
        foreach (var item in m_objects)
        {
            item.SetActive(false);
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
    //[HideInInspector]
    public PlayerUIElements[] playerElements;

    private static PlayerUIArray mInstance;

    public static PlayerUIArray Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = (PlayerUIArray)FindObjectOfType(typeof(PlayerUIArray));
            }
            return mInstance;
        }

    }
    void Awake()
    {
       // mInstance = Instance;
        playerElements = this.GetComponentsInChildren<PlayerUIElements>();
        //Debug.Log(Instance);

        UpdateArray();
        //foreach (PlayerUIElements UIContainer in playerElements)
        //{
        //    UIContainer.gameObject.SetActive(false);
        //}

    }

    public void UpdateArray()
    {
        playerElements = this.GetComponentsInChildren<PlayerUIElements>();
    }



}
