using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllosiusDevCore.QuestSystem
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "AllosiusDev/Quests/Quest")]
    public class QuestData : ScriptableObject
    {
        #region UnityInspector

        public string _nameTranslateKey;

        public List<QuestStepData> questSteps = new List<QuestStepData>();

        #endregion

        #region Functions

        public bool HasQuestStep(QuestStepData objectiveRef)
        {
            if (questSteps.Contains(objectiveRef))
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
