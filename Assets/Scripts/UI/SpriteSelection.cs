using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSelection : MonoBehaviour {

    public Sprite[] m_Sprites;
    private Image m_SRenderer;
	// Use this for initialization
	void Start ()
    {
        m_SRenderer = this.GetComponent<Image>();	
        if (m_Sprites.Length > 0)
        {
            m_SRenderer.sprite = m_Sprites[Random.Range(0, m_Sprites.Length)];
        }
	}

}
