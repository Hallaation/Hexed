using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SubStringReplace : ScriptableWizard
{

    public string NameToFind = "Column_002";
    public bool FindExactName = true;
    public GameObject NewType;
    public List<GameObject> oldObjects = new List<GameObject>();


    private int m_iCounter;
    private string foundLocations;
    [MenuItem("Custom/Replace Objects via Substring")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Replace GameObjects" , typeof(SubStringReplace) , "Replace" , "Search");
    }

    void OnWizardCreate()
    {
        if (EditorUtility.DisplayDialog("" , "Are you sure you wish to replace?" , "Yes replace them", "Nevermind"))
        {
            foreach (GameObject go in oldObjects)
            {
                GameObject newObject;
                newObject = PrefabUtility.InstantiatePrefab(NewType) as GameObject;
                newObject.transform.position = go.transform.position;
                newObject.transform.rotation = go.transform.rotation;
                newObject.transform.parent = go.transform.parent;
                newObject.name = go.name;
                newObject.tag = go.tag;
                DestroyImmediate(go);
            }
        }
    }

    void OnWizardOtherButton()
    {
        if (NameToFind != "")
        {
            m_iCounter = 0;
            oldObjects.Clear();
            foreach (GameObject goItem in GameObject.FindObjectsOfType(typeof(GameObject)))
            {
                if (!FindExactName)
                {
                    if (goItem.name.Contains(NameToFind))
                    {
                        ++m_iCounter;
                        oldObjects.Add(goItem);
                    }
                }
                else
                {
                    if (goItem.name == NameToFind)
                    {
                        ++m_iCounter;
                        oldObjects.Add(goItem);
                    }
                }
            }
            foreach (GameObject go in oldObjects)
            {
                if (go.transform.parent)
                {
                    foundLocations += go.transform.parent.name + "  ";
                }
                else
                {
                    foundLocations = "Root heirarchy";
                }
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
