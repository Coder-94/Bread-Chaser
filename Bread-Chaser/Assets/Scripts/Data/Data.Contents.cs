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
        public int grade;
        public int hp;
        public int atk;
    }

    [Serializable]
    public class StatData : ILoader<int, Stat>
    {
        public List<Stat> normalMobStat = new List<Stat>();

        public Dictionary<int, Stat> MakeDict()
        {
            Dictionary<int, Stat> dict = new Dictionary<int, Stat>();
            foreach (Stat stat in normalMobStat)
                dict.Add(stat.grade, stat);

            return dict;
        }
    }
    #endregion
}
