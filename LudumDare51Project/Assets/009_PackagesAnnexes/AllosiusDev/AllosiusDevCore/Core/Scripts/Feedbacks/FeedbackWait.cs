using AllosiusDevCore;
using System;
using System.Collections;
using UnityEngine;

namespace AllosiusDevCore
{
    [Serializable]
    public class FeedbackWait : BaseFeedback
    {
        public float waitTime;

        public override IEnumerator Execute(FeedbacksReader _owner)
        {
            if (IsActive && _owner.activeEffects)
            {
                Debug.Log("Wait");
                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}
