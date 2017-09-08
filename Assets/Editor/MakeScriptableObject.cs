using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MakeScriptableObject
{

    [MenuItem("Assets/Create/Database")]
    public static void CreatePlayerColorAsset()
    {
        Database database = ScriptableObject.CreateInstance<Database>();
        AssetDatabase.CreateAsset(database, "Assets/Resources/Database.asset");
        AssetDatabase.SaveAssets();
    }

}
