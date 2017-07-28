using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarPercentageTest : MonoBehaviour
{

    [Range(0 , 1)]
    public float percent;
    float oldPercent = 0;
    new Renderer renderer;

    void Awake()
    {
        renderer = GetComponent<Renderer>();
    }
    void Update()
    {
        if (oldPercent != percent)
        {
            renderer.material.SetTextureOffset("_MaskTex" , Vector2.right * (1 - percent * 2));
            oldPercent = percent;
        }
    }
}
