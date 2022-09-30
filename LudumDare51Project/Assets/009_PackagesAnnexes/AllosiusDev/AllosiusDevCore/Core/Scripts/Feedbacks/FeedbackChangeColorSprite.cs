using AllosiusDevCore;
using System;
using System.Collections;
using UnityEngine;

namespace AllosiusDevCore
{
    [Serializable]
    public class FeedbackChangeColorSprite : BaseFeedback
    {
        public Color newSpriteColor;

        public override IEnumerator Execute(FeedbacksReader _owner)
        {
            if (IsActive && _owner.activeEffects)
            {
                if (_owner.SpriteRenderer != null)
                {
                    Debug.Log("ChangeColor");
                    _owner.SpriteRenderer.color = newSpriteColor;
                }
                else if (_owner.GetComponent<SpriteRenderer>() != null)
                {
                    _owner.GetComponent<SpriteRenderer>().color = newSpriteColor;
                }
                else
                {
                    Debug.LogWarning("SpriteRenderer is null");
                }
            }
            return base.Execute(_owner);
        }
    }
}
