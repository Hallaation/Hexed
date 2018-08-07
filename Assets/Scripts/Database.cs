using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : ScriptableObject
{
    public PlayerColours[] colors;

}



[System.Serializable]
public class PlayerColours
{
    public string PlayerType;
    public Color playerColor;
}

