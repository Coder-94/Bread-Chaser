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
        public int          atk;
        public int          atkSpeed;
    }

    [Serializable]
    public class PlayerStat
    {
        public int          level;
        public float        hp;
        public int          atk;
        public float        moveSpeed;
        public float        atkSpeed;
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
                dict.Add(stat.level, stat);

            return dict;
        }
    }
    #endregion
}
