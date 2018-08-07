using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StartWhenReady : MonoBehaviour
{
    public Sprite[] PressStartSprites;

    public Image m_SpriteRenderer;
	// Use this for initialization
	void Start ()
    {
        m_SpriteRenderer = this.GetComponent<Image>();
	}
	
}
