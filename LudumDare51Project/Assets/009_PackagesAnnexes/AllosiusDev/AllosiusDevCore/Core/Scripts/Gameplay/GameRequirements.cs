using AllosiusDevCore.QuestSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllosiusDevCore
{
    [System.Serializable]
    public class GameRequirements
    {
        #region UnityInspector

        [SerializeField] public List<Requirement> requirementsList = new List<Requirement>();

        #endregion

        #region Behaviour

        public bool ExecuteGameRequirements()
        {
            Debug.Log("Execute Game Requirements");

            if (requirementsList.Count > 0)
            {
                foreach (var item in requirementsList)
                {
                    if (item.requirementType == RequirementType.HasQuest)
                    {
                        if (item.CheckHasQuest(GameManager.Instance.QuestManager) == false)
                        {
                            //Debug.Log("false");
                            return false;
                        }
                    }
                    else if (item.requirementType == RequirementType.QuestState)
                    {
                        if (item.CheckQuestState(GameManager.Instance.QuestManager) == false)
                        {
                            //Debug.Log("false");
                            return false;
                        }
                    }

                }
            }

            Debug.Log("true");
            return true;
        }

        #endregion
    }

}

namespace AllosiusDevCore
{
    [System.Serializable]
    public class Requirement
    {
        #region UnityInspector

        public RequirementType requirementType;

        [Space]
        [Header("Has Quest Properties")]
        [SerializeField] public QuestData questToCheck;
        [SerializeField] public bool questCheckedValueWanted;
        [SerializeField] public StateQuestWanted questToCheckState;

        [Space]
        [Header("Quest State Properties")]
        [SerializeField] public QuestData questAssociatedToCheck;
        [SerializeField] public QuestStepData questStepToCheck;
        [SerializeField] public bool questStepCheckedValueWanted;
        [SerializeField] public StateQuestWanted questStepToCheckState;


        #endregion

        #region Behaviour

        public bool CheckHasQuest(QuestList questList)
        {
            bool hasQuest = questList.HasQuest(questToCheck, questToCheckState);
            if (hasQuest == questCheckedValueWanted)
            {
                //Debug.Log("true");
                return true;
            }
            //Debug.Log("false");
            return false;
        }

        public bool CheckQuestState(QuestList questList)
        {
            bool hasQuestStep = questList.HasQuestStep(questAssociatedToCheck, questStepToCheck, questStepToCheckState);
            if (hasQuestStep == questStepCheckedValueWanted)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}

namespace AllosiusDevCore
{
    public enum RequirementType
    {
        HasQuest,
        QuestState,
        HasItem,
        HasRequiredRythmeGameRank,
    }
}

namespace AllosiusDevCore
{
    public enum StateQuestWanted
    {
        None,
        Uncompleted,
        Completed,
    }
}