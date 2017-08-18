using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class PrefabApply : ScriptableWizard
{
    public string NameToFind = "Column_002";
    public List<GameObject> objectList = new List<GameObject>();


    private int m_iCounter;
    private string foundLocations;
    [MenuItem("Custom/Apply prefab script")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Replace GameObjects" , typeof(PrefabApply) , "Apply" , "Search");
    }

    void OnWizardCreate()
    {
        foreach (GameObject go in objectList)
        {
            PrefabUtility.ReplacePrefab(go,PrefabUtility.GetPrefabParent(go),ReplacePrefabOptions.ConnectToPrefab);
        }
    }


    void OnWizardOtherButton()
    {
        if (NameToFind != "")
        {
            m_iCounter = 0;
            objectList.Clear();
            foreach (GameObject goItem in GameObject.FindObjectsOfType(typeof(GameObject)))
            {
                if (goItem.name.Contains(NameToFind))
                {
                    ++m_iCounter;
                    objectList.Add(goItem);
                }
            }
            foreach (GameObject go in objectList)
            {
                foundLocations += go.name;
            }
            EditorUtility.DisplayDialog("Items found" , "I found " + m_iCounter + " of " + NameToFind + " Heres where they are: " , "OK");
            EditorUtility.DisplayDialog("Locations" , foundLocations , "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("ERROR" , "Please enter something into the NameToFind field" , "OK");
        }
    }
}
