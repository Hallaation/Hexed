using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class BaseAbility : MonoBehaviour
{

    //abilities are going to cost mana.
    private float currentMana = 0.0f;
    public float m_fMaximumMana = 100;
    public float PassiveManaRegeneration = 1.0f;

    public float ManaCost = 10.0f;
    public bool RepeatedUsage = false;
    public float repeatedManaCost = 0.5f;
    public float m_fMinimumManaRequired = 50;
    //TODO if the shield is charged up to 100% of mana, the shield can deflect bullets, knock over players when run over with it
    //TODO otherwise all the shield is block bullets and knock over players when run over with it.
    
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public virtual void UseSpecialAbility() { } 
}

//editor script, shows/ hide the repeated mana cost in case the ability is something that of a shield.
[CustomEditor(typeof(BaseAbility))]
public class AbilityEditor : Editor
{

    override public void OnInspectorGUI()
    {
        
        var AbilityScript = target as BaseAbility;

        AbilityScript.m_fMaximumMana = EditorGUILayout.FloatField("Maximum Mana: " , AbilityScript.m_fMaximumMana);
        AbilityScript.PassiveManaRegeneration = EditorGUILayout.FloatField("Mana regen / tick: " , AbilityScript.PassiveManaRegeneration);
        AbilityScript.ManaCost = EditorGUILayout.FloatField("Ability mana cost: " , AbilityScript.ManaCost);


        AbilityScript.RepeatedUsage = GUILayout.Toggle(AbilityScript.RepeatedUsage , "Constant spell usage");
        

        if (AbilityScript.RepeatedUsage)
        {
            AbilityScript.repeatedManaCost = EditorGUILayout.FloatField("Repeated Mana cost: " , AbilityScript.repeatedManaCost);
            AbilityScript.m_fMinimumManaRequired = EditorGUILayout.FloatField("Min mana required: " , AbilityScript.m_fMinimumManaRequired);
        }
        
    }

}