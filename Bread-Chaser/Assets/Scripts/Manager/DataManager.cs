using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}
public class DataManager
{
    public Dictionary<int, Data.MobStat> NMStatDict { get; private set; } = new Dictionary<int, Data.MobStat>();
    public Dictionary<int, Data.PlayerStat> PLStatDict { get; private set; } = new Dictionary<int, Data.PlayerStat>();

    public void Init()
    {
        NMStatDict = LoadJson<Data.NMStatData, int, Data.MobStat>("NMStatData").MakeDict();
        PLStatDict = LoadJson<Data.PLStatData, int, Data.PlayerStat>("PLStatData").MakeDict();
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
}
