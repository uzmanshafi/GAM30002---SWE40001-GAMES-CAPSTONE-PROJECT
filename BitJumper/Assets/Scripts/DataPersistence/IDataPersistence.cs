using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Interface used to restrict the instantiation of the class and ensures that only one instance of the class exists
public interface IDataPersistence
{
    void LoadData(GameData data);   
    void SaveData(GameData data);
}
