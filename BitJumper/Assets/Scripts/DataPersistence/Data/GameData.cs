using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int deathCount;

    // Values within constructor are the default values
    public GameData()
    {
        this.deathCount = 0;
    }
}
