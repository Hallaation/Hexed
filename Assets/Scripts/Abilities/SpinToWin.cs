using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinToWin : BaseAbility
{

    private GameObject spinSprite;
    private float m_fRotation;
    public float RotationSpeed =  20.0f;
    public float MovementSpeedSlow = 10.0f;
    public override void AdditionalLogic()
    {

    }

    public override void Initialise()
    {
        m_fRotation = 0;
        //find the gameobject containing the sprite, and set its parent to be the player's sprite (pure laziness)
        spinSprite = this.transform.Find("SpinSprite").gameObject;
        spinSprite.transform.SetParent(GetComponent<Move>().playerSpirte.transform);
        spinSprite.SetActive(false);
    }

    public override void UseSpecialAbility(bool UsingAbility = false)
    {
        if (UsingAbility)
        {
            Debug.Log(m_fRotation);
            //should use an animation instead of hacking it like this, but whatever.
            GetComponent<Move>().playerSpirte.transform.rotation = Quaternion.Euler(new Vector3(0 , 0 , GetComponent<Move>().playerSpirte.transform.rotation.eulerAngles.z + RotationSpeed));
            //disable stick rotation and turn on the radius
            GetComponent<Move>().m_bStockStickRotation = true;
            spinSprite.SetActive(true);

            
        }
        else
        {
            //let the player rotate again, turn the spin radius indicator off and reset the rotation and allow the player to rotate with their sticks
            GetComponent<Move>().m_bStockStickRotation = false;
            spinSprite.SetActive(false);
            GetComponent<Move>().playerSpirte.transform.rotation = this.transform.rotation;
        }
    }
}
