using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeImage : MonoBehaviour {

	public bool isUiImage = false;
	public Image uiImage;

	public bool fadeIn = false;
	public bool fadeOut = false;

	public bool resetUiImage = false;
	public float uiResetNum;

	public bool resetGameObjectImage = false;
	public float gameObjectResetNum;

	public float fadeTime = 1f;





	void Update ()
	{


		if (isUiImage == false)
		{
			if (fadeOut == true)
			{
				var material = GetComponent<Renderer> ().material;
				var color = material.color;
				fadeTime -= Time.deltaTime;
				material.color = new Color (color.r, color.g, color.b, color.a = fadeTime);
				if (fadeTime <= 0)
				{
					fadeTime = 0;
					fadeOut = false;
				}
			}
			if (fadeIn == true)
			{
				var material = GetComponent<Renderer> ().material;
				var color = material.color;
				fadeTime += Time.deltaTime;
				material.color = new Color (color.r, color.g, color.b, color.a = fadeTime);
				if (fadeTime >= 1)
				{
					fadeTime = 1;
					fadeIn = false;
				}
			}
			if (resetGameObjectImage == true)
			{
				var material = GetComponent<Renderer> ().material;
				var color = material.color;
				material.color = new Color (color.r, color.g, color.b, gameObjectResetNum);
				fadeTime = gameObjectResetNum;
				fadeOut = false;
				fadeIn = false;
				resetGameObjectImage = false;
			}
		}

		if (isUiImage == true)
		{
			uiImage = GetComponent<Image> ();

			if (fadeOut == true)
			{
				var tempColor = uiImage.color;
				tempColor.a = fadeTime -= Time.deltaTime / 2;
				uiImage.color = tempColor;  
				if (fadeTime <= 0)
				{
					fadeTime = 0;
					fadeOut = false;
				}
			}

			if (fadeIn == true)
			{
				var tempColor = uiImage.color;
				tempColor.a = fadeTime += Time.deltaTime / 2;
				uiImage.color = tempColor;  
				if (fadeTime >= 1)
				{
					fadeTime = 1;
					fadeIn = false;
				}
			}

			if (resetUiImage == true)
			{
				var tempColor = uiImage.color;
				tempColor.a = uiResetNum;
				fadeTime = uiResetNum;
				uiImage.color = tempColor; 
				resetUiImage = false;

			}
		}
	}
}