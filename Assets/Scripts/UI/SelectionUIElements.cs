using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XboxCtrlrInput;
using XInputDotNetPure;
/// <summary>
/// this script/class will control all the elements within the selection screen
/// Selectable characters will be obtained from a singleton
/// </summary>
public class SelectionUIElements : MonoBehaviour
{
    private CharacterSelectionManager selectionManager;
    public XboxController m_controller = XboxController.First;
    private bool m_bPlayerJoined = false;
    
    // Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {

        if (m_bPlayerJoined)
        {
            //! let them choose different characters
        }
        else
        {
            //! scan for A input
        }
	}
}
