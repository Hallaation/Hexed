using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MakeScriptableObject
{

    [MenuItem("Assets/Create/PlayerColours")]
    public static void CreatePlayerColourAsset()
    {
        PlayerColours playercolours = ScriptableObject.CreateInstance<PlayerColours>();
        AssetDatabase.CreateAsset(playercolours, "Assets/Resources/PlayerColours.asset");
        AssetDatabase.SaveAssets();
    }

}
