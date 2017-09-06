using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MakeScriptableObject
{

    [MenuItem("Assets/Create/PlayerColors")]
    public static void CreatePlayerColorAsset()
    {
        PlayerColors playerColors = ScriptableObject.CreateInstance<PlayerColors>();
        AssetDatabase.CreateAsset(playerColors, "Assets/Resources/PlayerColors.asset");
        AssetDatabase.SaveAssets();
    }

}
