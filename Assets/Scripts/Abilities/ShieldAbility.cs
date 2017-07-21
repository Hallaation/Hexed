using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class ShieldAbility : BaseAbility
{
    [Space]
    [Header("Shield Variables")]
    public float testVariable;
	// Use this for initialization
	void Start ()
    {
		
	}
	
    public override void UseSpecialAbility()
    {
        //TODO Shield logic goes here.
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
