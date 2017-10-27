using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonTester : MonoBehaviour
{
    private static SingletonTester mInstance;

    public static SingletonTester Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = (SingletonTester)FindObjectOfType(typeof(SingletonTester));
                if (!mInstance)
                {
                    mInstance = (new GameObject("SingletonTester")).AddComponent<SingletonTester>();
                }
            }
            return mInstance;
        }
    }
    public List<MonoBehaviour> singletons = new List<MonoBehaviour>();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddSingleton(MonoBehaviour a_singleton)
    {
        if(!singletons.Contains(a_singleton))
        {
            singletons.Add(a_singleton);
        }
    }
}
