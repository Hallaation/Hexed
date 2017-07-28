using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using XboxCtrlrInput;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Stack<GameObject> menuStatus;
    public GameObject defaultPanel;
    private GameObject selected;
    //only used if all else fails
    private GameObject defaultToReturnTo;
    private EventSystem _eventSystem;
    // Use this for initialization
    void Start()
    {

    }

    void Awake()
    {
        if (instance != null) //check if singleton has been initiated
        {
            Debug.LogError("More than one audiomanager");
            return;
        }
        instance = this;
        _eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        selected = _eventSystem.currentSelectedGameObject;
        menuStatus = new Stack<GameObject>();

    }

    // Update is called once per frame
    void Update()
    {
        //if the current selected is null
        if (!GameObject.Find("EventSystem").GetComponent<EventSystem>().currentSelectedGameObject)
        {
            //set the selected
            Debug.Log("Currently selecting null");
            //when all else fails, return to a default
            if (!selected)
            {
                selected = defaultToReturnTo;
            }
            _eventSystem.SetSelectedGameObject(selected);
        }
        else
        {
            selected = _eventSystem.currentSelectedGameObject;
        }

        if (XCI.GetButtonDown(XboxButton.B))
        {
            Back();
        }

        if (XCI.GetButton(XboxButton.Start) && menuStatus.Count <= 0)
        {
            if (!menuStatus.Contains(defaultPanel))
            {
                menuStatus.Push(defaultPanel);
                defaultPanel.SetActive(true);
                _eventSystem.SetSelectedGameObject(null);
                _eventSystem.SetSelectedGameObject(defaultPanel.transform.Find("DefaultButton").gameObject);
            }
        }
    }

    public void Back()
    {
        //set the current menu (whatever it is), turn it off then pop it off the stack.
        if (menuStatus.Count >= 1)
        {
            menuStatus.Peek().SetActive(false);
            menuStatus.Pop();
            if (menuStatus.Count >= 1)
            {
                menuStatus.Peek().SetActive(true);

                //find defaults if there are any and set it to my selected.
                if (menuStatus.Peek().transform.Find("DefaultButton"))
                {
                    _eventSystem.SetSelectedGameObject(null);
                    _eventSystem.SetSelectedGameObject(menuStatus.Peek().transform.Find("DefaultButton").gameObject);
                    defaultToReturnTo = menuStatus.Peek().transform.Find("DefaultButton").gameObject;
                }
            }
        }
    }

    public void Test(GameObject current)
    {
        //Debug.Log(current);
        if (!menuStatus.Contains(current))
        {
            menuStatus.Peek().SetActive(false);
            current.SetActive(true);
            menuStatus.Push(current.gameObject);
            _eventSystem.SetSelectedGameObject(null);

            //find defaults if there are any and set it to my selected.
            //Debug.Log();
            if (menuStatus.Peek().GetComponentInChildren<Button>())
            {
                _eventSystem.SetSelectedGameObject(null);
                _eventSystem.SetSelectedGameObject(menuStatus.Peek().GetComponentInChildren<Button>().gameObject);
                defaultToReturnTo = menuStatus.Peek().GetComponentInChildren<Button>().gameObject;
            }
        }

    }
}
