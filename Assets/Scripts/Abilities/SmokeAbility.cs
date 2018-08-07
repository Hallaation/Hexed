using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using XInputDotNetPure;
public class SmokeAbility : BaseAbility
{
    private bool ButtonReset = true;

    [Header("Smoke varaibles")]
    //private float m_fMinimumRadius;
    public float m_fMaximumRadius;

    public GameObject SmokeBombPrefab;

    public override void Initialise()
    {
        RegenMana = true;
    }

    public override void AdditionalLogic()
    {

    }

    public override void UseSpecialAbility(bool UsingAbility = false)
    {
        if (currentMana >= ManaCost && ButtonReset && UsingAbility)
        {
            StartCoroutine(RunSpecialAbility());
            ButtonReset = false;
        }
        
        if (XCI.GetAxis(XboxAxis.LeftTrigger, GetComponent<ControllerSetter>().mXboxController) == 0)
        {
            ButtonReset = true;
        }
    }

    IEnumerator RunSpecialAbility()
    {
        GameObject go = Instantiate(SmokeBombPrefab, this.transform.position, Quaternion.identity, null);
        go.GetComponent<ParticleSystem>().Play();
        GameObject.Destroy(go , go.GetComponent<ParticleSystem>().main.duration + go.GetComponent<ParticleSystem>().main.startLifetimeMultiplier + 2);
        currentMana -= ManaCost;
        yield return null;
    }
}
