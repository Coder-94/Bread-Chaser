using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    #region Stat
    [Serializable]
    public class NormalMobStat
    {
        public string       stage;
        public int          hp;
        public int          atk;
        public int          atkSpeed;
    }

    [Serializable]
    public class PlayerStat
    {
        public int          level;
        public int          hp;
        public int          atk;
        public float        moveSpeed;
        public float        atkSpeed;
    }
    #endregion

    #region StatDict

    [Serializable]
    public class NMStatData : ILoader<string, NormalMobStat>
    {
        public List<NormalMobStat> normalMobStat = new List<NormalMobStat>();

        public Dictionary<string, NormalMobStat> MakeDict()
        {
            Dictionary<string, NormalMobStat> dict = new Dictionary<string, NormalMobStat>();
            foreach (NormalMobStat stat in normalMobStat)
                dict.Add(stat.stage, stat);

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
                dict.Add(stat.level, stat);

            return dict;
        }
    }
    #endregion
}
