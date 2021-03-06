﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScroll : MonoBehaviour
{

    public float m_fSpeed = 0.2f;
    private float _t = 0;
    private Vector3 OriginalPosition;
    private Vector3 DesiredPosition;
    [HideInInspector]
    public bool m_bInterpolateCredits;
    // Use this for initialization
    void Start()
    {
        OriginalPosition = this.GetComponent<RectTransform>().anchoredPosition;
        DesiredPosition = new Vector3(OriginalPosition.x, 180, OriginalPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bInterpolateCredits)
        {
            _t += Time.deltaTime * m_fSpeed;
            this.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(OriginalPosition, DesiredPosition, _t * m_fSpeed);

            if (this.GetComponent<RectTransform>().anchoredPosition.magnitude >= DesiredPosition.magnitude - 0.3f)
            {
                UIManager.Instance.MainMenuBack();
            }
        }
        else
        {
            _t = 0;
        }
    }
}
