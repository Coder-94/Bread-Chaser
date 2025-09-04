using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    #region Stat
    [Serializable]
    public class MobStat
    {
        public int          id;
        public float        hp;
        public float        atk;
        public int          atkSpeed;
    }

    [Serializable]
    public class PlayerStat
    {
        public int          id;
        public int          level;
        public float        hp;
        public float        atk;
        public float        moveSpeed;
        public float        atkSpeed;
    }

    [Serializable]
    public class GatchaCheck
    {
        public string optionName;
        public int choosingNum;
        public int reRollChance;
    }
    #endregion

    #region StatDict

    [Serializable]
    public class NMStatData : ILoader<int, MobStat>
    {
        public List<MobStat> mobStat = new List<MobStat>();

        public Dictionary<int, MobStat> MakeDict()
        {
            Dictionary<int, MobStat> dict = new Dictionary<int, MobStat>();
            foreach (MobStat stat in mobStat)
                dict.Add(stat.id, stat);

            return dict;
        }
    }

    [Serializable]
    public class PLStatData : ILoader<int, PlayerStat>
    {
        public List<PlayerStat> playerStat = new List<PlayerStat>();

        public Dictionary<int, PlayerStat> MakeDict()
        {
            Dictionary<int, PlayerStat> dict = new Dictionary<int, PlayerStat>();
            foreach (PlayerStat stat in playerStat)
                dict.Add(stat.id, stat);

            return dict;
        }
    }

    [Serializable]
    public class OptionData : ILoader<string, GatchaCheck>, ILoader<string, TestCheck>
    {
        public List<GatchaCheck> gatchaCheck = new List<GatchaCheck>();
        //
        Dictionary<string, GatchaCheck> ILoader<string, GatchaCheck>.MakeDict()
        {
            Dictionary<string, GatchaCheck> dict = new Dictionary<string, GatchaCheck>();
            foreach (GatchaCheck gatcha in gatchaCheck)
                dict.Add(gatcha.optionName, gatcha);

            return dict;
        }

        public List<TestCheck> testCheck = new List<TestCheck>();

        Dictionary<string, TestCheck> ILoader<string, TestCheck>.MakeDict()
        {
            Dictionary<string, TestCheck> dict = new Dictionary<string, TestCheck>();
            foreach (TestCheck test in testCheck)
                dict.Add(test.optionName, test);

            return dict;
        }
    }

    [Serializable]
    public class TestCheck
    {
        public string optionName;
        public int test;
        public int test2;
    }
    #endregion
}
