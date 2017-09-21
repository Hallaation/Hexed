using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffsetUi : MonoBehaviour {

	public float speedX = 0.1f;
	public float speedY = 0.1f;
	private float curX;
	private float curY;

	void Awake () 
	{
		curX = GetComponent<Image>().material.mainTextureOffset.x;
		curY = GetComponent<Image>().material.mainTextureOffset.y;
	

	}
	
	// Update is called once per frame
	void Update () 
	{
		curX += Time.deltaTime * speedX;
		curY += Time.deltaTime * speedY;
		GetComponent<Image>().material.SetTextureOffset ("_MainTex", new Vector2 (curX, curY));
	}
}
