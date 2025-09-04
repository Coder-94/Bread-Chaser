using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<int, Data.MobStat> NMStatDict { get; private set; } = new Dictionary<int, Data.MobStat>();
    public Dictionary<int, Data.PlayerStat> PLStatDict { get; private set; } = new Dictionary<int, Data.PlayerStat>();
    public Dictionary<string, Data.GatchaCheck> GatchaDict { get; private set; } = new Dictionary<string, Data.GatchaCheck>();

    public Dictionary<string, Data.TestCheck> TestDict { get; private set; } = new Dictionary<string, Data.TestCheck>();
    public void Init()
    {
        NMStatDict = LoadJson<Data.NMStatData, int, Data.MobStat>("NMStatData").MakeDict();
        PLStatDict = LoadJson<Data.PLStatData, int, Data.PlayerStat>("PLStatData").MakeDict();
        //GatchaList = LoadJson<Data.OptionData, string, Data.GatchaCheck>("OptionData").gatchaCheck.MakeDict();

        OptionData loadedData = LoadJson<OptionData, string, GatchaCheck>("OptionData");
        ILoader<string, GatchaCheck> gatchaLoader = loadedData;
        ILoader<string, TestCheck> testLoader = loadedData;

        GatchaDict = gatchaLoader.MakeDict();
        TestDict = testLoader.MakeDict();
        
        Data.GatchaCheck stat = Managers.Data.GatchaDict["gatchaCheck"];
        Debug.Log(stat.choosingNum);
        Data.TestCheck stat2 = Managers.Data.TestDict["gatchaCheck"];
        Debug.Log(stat2.test2);
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
}
