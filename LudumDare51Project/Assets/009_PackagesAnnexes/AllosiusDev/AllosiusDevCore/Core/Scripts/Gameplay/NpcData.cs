using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllosiusDevCore

{
	[CreateAssetMenu(fileName = "New NpcData", menuName = "AllosiusDev/NpcData")]
    public class NpcData : ScriptableObject
    {
        #region UnityInspector

        public string nameNpc;

        #endregion
    }
}
