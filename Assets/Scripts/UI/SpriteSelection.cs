using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSelection : MonoBehaviour
{

    public Sprite[] m_Sprites;
    private Image m_SRenderer;

    public float lowerLimit = 0;
    public float upperlimit = 1;
    // Use this for initialization
    void Start()
    {
        m_SRenderer = this.GetComponent<Image>();
        StartCoroutine(ChangeSprites());
    }


    IEnumerator ChangeSprites()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(lowerLimit, upperlimit));
            if (m_Sprites.Length > 0)
            {
                m_SRenderer.sprite = m_Sprites[Random.Range(0, m_Sprites.Length)];
            }
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            if (m_Sprites.Length > 0)
            {
                m_SRenderer.sprite = m_Sprites[0];
            }
            yield return null;
        }
    }

}
