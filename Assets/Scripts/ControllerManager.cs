using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
using XboxCtrlrInput;
public class ControllerManager : MonoBehaviour
{

    public int AbilityToAdd = 0;
    bool addedAbility = false;
    //dictionary used to determine if the controller index has been assigned yet.
    Dictionary<PlayerIndex , bool> playerIdx = new Dictionary<PlayerIndex , bool>
    {
        {PlayerIndex.One, false },
        {PlayerIndex.Two, false },
        {PlayerIndex.Three, false },
        {PlayerIndex.Four, false },
    };

    //dictionary mapping XCI index with the XInputDotNet indexes
    Dictionary<PlayerIndex , XboxController> xboxControllers = new Dictionary<PlayerIndex , XboxController>
    {
        {PlayerIndex.One, XboxController.First },
        {PlayerIndex.Two, XboxController.Second },
        {PlayerIndex.Three, XboxController.Third },
        {PlayerIndex.Four, XboxController.Fourth },
    };


    //4 players

    public bool assignControllers = true;
    public CameraControl ref_cameraController;
    int maxPlayers = 4;
    int nextPlayer = 0;
    public GameObject playerPrefab;
    public Transform[] spawnPoints;

    void Update()
    {
       // Debug.Log(nextPlayer);
        for (int i = 0; i < 4; i++)
        {
            //make a player index based off what i is
            PlayerIndex testIndex = (PlayerIndex)i;
            //get the gamepad state of the current index set before
            GamePadState testState = GamePad.GetState(testIndex);
            if (testState.IsConnected &&
                !playerIdx[testIndex] &&
                XCI.GetButtonDown(XboxButton.Start , xboxControllers[testIndex]))
                //if the player of index i has pressed Start, and their controller is connected, and their controller has yet to be assgined
            {
                //assign a controller and spawn the player
                playerIdx[testIndex] = true;
                GameObject go = Instantiate(playerPrefab , spawnPoints[nextPlayer].position , Quaternion.identity , null);
                go.GetComponent<ControllerSetter>().SetController(testIndex);
                go.GetComponent<ControllerSetter>().m_playerNumber = i;
                AddAbility(AbilityToAdd, go);
                addedAbility = true;
                go.SetActive(true);
                if (ref_cameraController)
                {
                    //if the cameracontrol exists, add the instantiated player to a camera targets
                    ref_cameraController.m_Targets.Add(go.transform);
                }
                nextPlayer++; //increment to determine next spawn point
                break;
            }
        }
    }

    void AddAbility(int abilityIndex, GameObject playerToAddAbility)
    {
        if (addedAbility)
        {
            abilityIndex = 1;
        }
        else
        {
            abilityIndex = 0;
        }

        if (playerToAddAbility.GetComponent<BaseAbility>())
        {
            Destroy(playerToAddAbility.GetComponent<BaseAbility>());
        }

        switch (abilityIndex)
        {
            case 0:
                {
                    ShieldAbility temp = playerToAddAbility.AddComponent<ShieldAbility>();
                    ShieldAbility mine = GetComponent<ShieldAbility>();
                    temp.ManaCost = mine.ManaCost;
                    temp.RepeatedUsage = mine.RepeatedUsage;
                    temp.repeatedManaCost = mine.repeatedManaCost;
                    temp.PassiveManaRegeneration = mine.PassiveManaRegeneration;
                    temp.m_fMinimumManaRequired = mine.m_fMinimumManaRequired;
                    temp.m_fMovementSpeedSlowDown = mine.m_fMovementSpeedSlowDown;
                }
                break;
            case 1:
                Teleport temp1 = playerToAddAbility.AddComponent<Teleport>();
                Teleport mine1 = GetComponent<Teleport>();
                temp1.ManaCost = mine1.ManaCost;
                temp1.repeatedManaCost = mine1.repeatedManaCost;
                temp1.PassiveManaRegeneration = mine1.PassiveManaRegeneration;
                temp1.m_fMinimumManaRequired = mine1.m_fMinimumManaRequired;
                temp1.m_fMovementSpeedSlowDown = mine1.m_fMovementSpeedSlowDown;
                temp1.m_TeleportForce = mine1.m_TeleportForce;
                break;

            default: break;

        }
        
    }
}
