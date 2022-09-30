using AllosiusDevUtilities;
using AllosiusDevCore.DialogSystem;
using AllosiusDevCore.QuestSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllosiusDevCore
{
    public class GameManager : Singleton<GameManager>
    {
        #region Properties

        public QuestList QuestManager => questManager;

        #endregion

        #region UnityInspector

        [SerializeField] private QuestList questManager;

        #endregion
    }
}
