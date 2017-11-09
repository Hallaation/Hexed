using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class SliderSelected : MonoBehaviour
{
    private Slider m_Slider;
    private EventSystem _EventSystem;
    private Image m_HandleSurround;
    // Use this for initialization
    void Awake()
    {
        _EventSystem = FindObjectOfType<EventSystem>();
        m_Slider = this.GetComponent<Slider>();
        m_HandleSurround = this.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        m_HandleSurround.enabled = (_EventSystem.currentSelectedGameObject == this.gameObject);
    }
}
