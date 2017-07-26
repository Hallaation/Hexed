using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


//editor script, shows/ hide the repeated mana cost in case the ability is something that of a shield.
[CustomEditor(typeof(BaseAbility) , true)]
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
            AbilityScript.m_fMovementSpeedSlowDown = EditorGUILayout.FloatField("Movement Speed Slowdown" , AbilityScript.m_fMovementSpeedSlowDown);
        }

    }

}

[CustomEditor(typeof(ShieldAbility))]
public class ShieldEditor : AbilityEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var TargetScript = target as ShieldAbility;
        if (TargetScript.RepeatedUsage)
        {
            TargetScript.testVariable = EditorGUILayout.FloatField("Test: " , TargetScript.testVariable);
        }
    }
}
