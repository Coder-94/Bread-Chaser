using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    #region Stat
    [Serializable]
    public class Stat
    {
        public string stage;
        public int hp;
        public int atk;
    }

    [Serializable]
    public class StatData : ILoader<string, Stat>
    {
        public List<Stat> normalMobStat = new List<Stat>();

        public Dictionary<string, Stat> MakeDict()
        {
            Dictionary<string, Stat> dict = new Dictionary<string, Stat>();
            foreach (Stat stat in normalMobStat)
                dict.Add(stat.stage, stat);

            return dict;
        }
    }
    #endregion
}
