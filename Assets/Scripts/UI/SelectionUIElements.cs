using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XboxCtrlrInput;
using XInputDotNetPure;
/// <summary>
/// this script/class will control all the elements within the selection screen
/// Selectable characters will be obtained from a singleton
/// Only used for a single controller
/// </summary>
public class SelectionUIElements : MonoBehaviour
{
    public XboxController m_controller = XboxController.First;
    [Space]
    [Header("Interpolation Properties")]
    public Transform EndPosition; //end point only going to use the Y for this
    private Vector3 EndPoint;
    public float m_fLerpSpeed = 1.0f; //how fast this object will move towards the docking position;
    private Transform startPoint; //starting point (my spot in this case);
    private float StartTime;
    private float journeyLength;
    private bool DoLerp = false;
    private bool ReturnLerp = false;
    [Space]
    //Varaibles to do UI stuff
    private CharacterSelectionManager selectionManager;
    private bool m_bPlayerJoined = false;
    private bool m_bSelectedCharacter = false;
    private bool[] StickMovement; // index 0 for left horizontal, index 1 for right horizontal
    private bool ResetSticks = false;
    private GameObject m_PressAToJoinGO;
    private GameObject m_SelectedCharacterGO;
    private Image SelectedCharacterImage;
    private int m_iSelectedIndex = 0;
    // Use this for initialization
    void Awake()
    {
        //make a copy-ish of my transform
        GameObject temp = new GameObject("return point", typeof(RectTransform));
        temp.transform.SetParent(this.transform.parent);
        temp.transform.position = this.transform.position; 

        //set the starting point to the copied transform
        startPoint = temp.transform;

        //EndPoint = new Vector3(this.GetComponent<RectTransform>().localPosition.x , EndPosition.localPosition.y , 0);
        journeyLength = Vector3.Distance(startPoint.position , EndPosition.position);

        m_PressAToJoinGO = transform.Find("PressAToJoin").gameObject;
        m_SelectedCharacterGO = transform.Find("Character_Selected").gameObject;
        SelectedCharacterImage = m_SelectedCharacterGO.GetComponent<Image>();
        selectionManager = CharacterSelectionManager.Instance;


        // Debug.Log(CharacterSelectionManager.instance);
        StickMovement = new bool[2] { false , false };
    }
    IEnumerator GetSelectionManagerInstance()
    {
        while (!selectionManager)
        {
            selectionManager = CharacterSelectionManager.Instance;
        }
        yield return null;
    }
    // Update is called once per frame
    void Update()
    {
        
        if (ResetSticks)
        {
            if (XCI.GetAxis(XboxAxis.LeftStickX , m_controller) < 0)
            {
                StickMovement[0] = true;
                ResetSticks = false;
            }
            else if (XCI.GetAxis(XboxAxis.LeftStickX , m_controller) > 0)
            {
                StickMovement[1] = true;
                ResetSticks = false;
            }
        }

        //Check if the player has opted to join into the game.
        if (m_bPlayerJoined)
        {
            //Let the player press left and right to choose their character. Only if they havn't selected one yet.
            if (!m_bSelectedCharacter)
            {
                if (XCI.GetButtonDown(XboxButton.DPadLeft , m_controller) || XCI.GetAxis(XboxAxis.LeftStickX , m_controller) < 0)
                {
                    //change the index first then change things accordingly
                    if (StickMovement[0] || XCI.GetButtonDown(XboxButton.DPadLeft , m_controller))
                    {

                        StickMovement[0] = false;
                        m_iSelectedIndex--;
                        if (m_iSelectedIndex < 0)
                        {
                            m_iSelectedIndex = selectionManager.CharacterArray.Length - 1;
                        }
                    }
                }
                else if (XCI.GetButtonDown(XboxButton.DPadRight , m_controller) || XCI.GetAxis(XboxAxis.LeftStickX , m_controller) > 0)
                {
                    if (StickMovement[1] || XCI.GetButtonDown(XboxButton.DPadRight , m_controller))
                    {
                        Debug.Log("Right stick " + StickMovement[1]);
                        StickMovement[1] = false;
                        m_iSelectedIndex++;
                        if (m_iSelectedIndex > selectionManager.CharacterArray.Length - 1)
                        {
                            m_iSelectedIndex = 0;
                        }
                    }
                }
                else if (XCI.GetButtonDown(XboxButton.A , m_controller))
                {
                    //check to see if the character isn't selected
                    if (!selectionManager.CharacterSelectionStatus[selectionManager.CharacterArray[m_iSelectedIndex]])
                    {
                        //this is where the character is selected
                        StartTime = Time.time;
                        DoLerp = true;
                        ReturnLerp = false;
                        m_bSelectedCharacter = true;
                        selectionManager.playerSelectedCharacter.Add(m_controller , selectionManager.CharacterArray[m_iSelectedIndex]);
                        selectionManager.CharacterSelectionStatus[selectionManager.CharacterArray[m_iSelectedIndex]] = true; //set the status selected to true
                    }
                }
            }
            else if (XCI.GetButtonDown(XboxButton.B , m_controller) && selectionManager.CharacterSelectionStatus[selectionManager.CharacterArray[m_iSelectedIndex]])
            {
                //reset my selected status
                m_bSelectedCharacter = false;
                //remove me from the selected character list
                selectionManager.playerSelectedCharacter.Remove(m_controller);
                //reset the selection status
                selectionManager.CharacterSelectionStatus[selectionManager.CharacterArray[m_iSelectedIndex]] = false;
                StartTime = Time.time;
                DoLerp = false;
                ReturnLerp = true;
            }
            //! change the selected sprite near the end of the frame, after all the processing has been done.
            bool IsCharacterSelected = selectionManager.CharacterSelectionStatus[selectionManager.CharacterArray[m_iSelectedIndex]];
            SelectedCharacterImage.sprite = selectionManager.CharacterArray[m_iSelectedIndex].GetComponent<BaseAbility>().SelectionSprites[Convert.ToInt32(IsCharacterSelected)];
        }
        else //! if the player hasn't joined the game yet
        {
            //! scan for A input
            if (XCI.GetButton(XboxButton.A , m_controller))
            {
                m_bPlayerJoined = true;
                m_PressAToJoinGO.SetActive(false);
            }
        }
        //check for stick reset at the end of the frame
        ResetSticks = (XCI.GetAxis(XboxAxis.LeftStickX , m_controller) == 0);

        InterpolateToEndPoint(DoLerp, ReturnLerp);
    }// end of update
    void InterpolateToEndPoint(bool a_bDoLerp, bool a_bReturnLerp)
    {
        //if the boolean coming in is DoLerp, do the regular
        if (a_bDoLerp)
        {
            journeyLength = Vector3.Distance(startPoint.position , EndPosition.position);
            float distCovered = (Time.time - StartTime) * m_fLerpSpeed;
            float fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(startPoint.position, new Vector3(GetComponent<RectTransform>().position.x , EndPosition.position.y , 0) , fracJourney);
            //Debug.Log("starting point " + startPoint.position.position);
            //Debug.Log("ending point " + new Vector3(GetComponent<RectTransform>().position.x , EndPosition.position.y , 0));
        }
        //if I don't want to lerp
        
        else if (a_bReturnLerp)
        {
            journeyLength = Vector3.Distance(EndPosition.position , startPoint.position);
            float distCovered = (Time.time - StartTime) * m_fLerpSpeed;
            float fracJourney = distCovered / journeyLength;
            //from the end to the start.
            transform.position = Vector3.Lerp(new Vector3(GetComponent<RectTransform>().position.x , EndPosition.position.y , 0) , startPoint.position , fracJourney);
            //Debug.Log("reversed starting point " + new Vector3(GetComponent<RectTransform>().position.x , EndPosition.position.y , 0));
            //Debug.Log("reversed ending point " + startPoint.position.position);
        }
        
    }
}
