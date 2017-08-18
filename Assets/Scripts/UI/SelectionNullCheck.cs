using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class SelectionNullCheck : MonoBehaviour
{
    private EventSystem _eventSystem;

    private GameObject oldSelected;

    // Use this for initialization
    void Start()
    {
        _eventSystem = FindObjectOfType<EventSystem>();
        if (!_eventSystem.currentSelectedGameObject)
        {
            _eventSystem.SetSelectedGameObject(FindObjectOfType<Button>().gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (_eventSystem.currentSelectedGameObject)
            oldSelected = _eventSystem.currentSelectedGameObject;
        else
            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(oldSelected);

    }

}
