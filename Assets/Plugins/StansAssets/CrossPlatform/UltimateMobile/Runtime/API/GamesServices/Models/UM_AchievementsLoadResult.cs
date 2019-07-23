using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Templates;


namespace SA.CrossPlatform.GameServices
{
    [Serializable]
    public class UM_AchievementsLoadResult : SA_Result
    {

        [SerializeField] List<UM_iAchievement> m_achievements = new List<UM_iAchievement>();

        public UM_AchievementsLoadResult(SA_Error error):base(error) {

        }

        public UM_AchievementsLoadResult(List<UM_iAchievement> achievements) {
            m_achievements = achievements;
        }


        public List<UM_iAchievement> Achievements {
            get {
                return m_achievements;
            }
        }
    }
}