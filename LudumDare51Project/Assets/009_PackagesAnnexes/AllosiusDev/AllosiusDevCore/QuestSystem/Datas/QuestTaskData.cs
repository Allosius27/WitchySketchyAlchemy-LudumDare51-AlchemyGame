using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllosiusDevCore.QuestSystem
{
    [CreateAssetMenu(fileName = "New QuestTask", menuName = "AllosiusDev/Quests/Quest Task")]
    public class QuestTaskData : ScriptableObject
    {
        #region Properties
        public enum TASK_TYPE
        {
            talkToNPC,
        }

        #endregion

        #region UnityInspector

        public string _name;

        public string descriptionTranslateKey;

        public int taskValue = 1;

        #endregion

        #region Functions

        public void updateThis(QuestTaskData newData)
        {
            _name = newData._name;
            descriptionTranslateKey = newData.descriptionTranslateKey;
            taskValue = newData.taskValue;
        }

        #endregion
    }
}