using AllosiusDevCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllosiusDevCore
{
    [Serializable]
    public class FeedbackInstantiateObject : BaseFeedback
    {
        public GameObject objectToInstantiate;
        public Vector3 objectPositionOffset;

        public override IEnumerator Execute(FeedbacksReader _owner)
        {
            if (IsActive && _owner.activeEffects)
            {
                GameObject _feedbackInstantiate = GameObject.Instantiate(objectToInstantiate, _owner.transform.position + objectPositionOffset, Quaternion.identity);
            }


            return base.Execute(_owner);
        }
    }
}
